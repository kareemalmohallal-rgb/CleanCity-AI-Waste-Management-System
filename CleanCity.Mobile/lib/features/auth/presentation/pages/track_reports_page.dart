import 'dart:io';

import 'package:cleancityapp/features/auth/domain/entities/report_entity.dart';
import 'package:cleancityapp/features/auth/presentation/bloc/reports/reports_bloc.dart';
import 'package:cleancityapp/features/auth/presentation/pages/report_execution_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart';

class TrackReportsPage extends StatefulWidget {
  const TrackReportsPage({super.key});

  @override
  State<TrackReportsPage> createState() => _TrackReportsPageState();
}

class _TrackReportsPageState extends State<TrackReportsPage> {
  final TextEditingController _searchController = TextEditingController();
  final TextEditingController _rejectReasonController = TextEditingController();

  String _currentFilter = 'all';

  @override
  void initState() {
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      context.read<ReportsBloc>().add(const LoadReports());
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    _rejectReasonController.dispose();
    super.dispose();
  }

  Future<void> _refresh() async {
    if (!mounted) return;
    context.read<ReportsBloc>().add(const LoadReports());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text(
          'تتبع بلاغاتي',
          style: TextStyle(
            fontSize: 20,
            fontWeight: FontWeight.bold,
            color: Colors.white,
          ),
        ),
        centerTitle: true,
        backgroundColor: const Color.fromARGB(255, 87, 211, 157),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Colors.white),
          onPressed: () {
            FocusScope.of(context).unfocus();
            Navigator.pop(context);
          },
        ),
        elevation: 0,
        shape: const RoundedRectangleBorder(
          borderRadius: BorderRadius.only(
            bottomLeft: Radius.circular(16),
            bottomRight: Radius.circular(16),
          ),
        ),
      ),
      body: RefreshIndicator(
        onRefresh: _refresh,
        child: BlocConsumer<ReportsBloc, ReportsState>(
          listener: (context, state) {
            if (state.error != null && state.error!.trim().isNotEmpty) {
              ScaffoldMessenger.of(
                context,
              ).showSnackBar(SnackBar(content: Text(state.error!)));
            }
          },
          builder: (context, state) {
            final filtered = _applyFiltersOnEntities(state.reports);

            return Column(
              children: [
                Padding(
                  padding: const EdgeInsets.all(16.0),
                  child: _buildSearchBox(),
                ),
                Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 16.0),
                  child: SizedBox(
                    height: 48,
                    child: ListView(
                      scrollDirection: Axis.horizontal,
                      children: [
                        _buildFilterButton('الكل', 'all'),
                        const SizedBox(width: 8),
                        _buildFilterButton('الجديدة', 'new'),
                        const SizedBox(width: 8),
                        _buildFilterButton('قيد التنفيذ', 'in_progress'),
                        const SizedBox(width: 8),
                        _buildFilterButton('مكتملة', 'completed'),
                      ],
                    ),
                  ),
                ),
                const SizedBox(height: 8),
                Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 16.0),
                  child: Row(
                    children: [
                      Text(
                        'عدد البلاغات: ${filtered.length}',
                        style: const TextStyle(
                          color: Color(0xFF7F8C8D),
                          fontSize: 14,
                        ),
                      ),
                      const Spacer(),
                      if (state.actionLoading)
                        const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        ),
                    ],
                  ),
                ),
                const SizedBox(height: 8),
                Expanded(
                  child: state.loading
                      ? const Center(child: CircularProgressIndicator())
                      : filtered.isEmpty
                      ? _buildEmpty()
                      : ListView.builder(
                          padding: const EdgeInsets.all(16),
                          itemCount: filtered.length,
                          itemBuilder: (context, index) {
                            final e = filtered[index];
                            return _buildReportCard(e, state.actionLoading);
                          },
                        ),
                ),
              ],
            );
          },
        ),
      ),
    );
  }

  Widget _buildSearchBox() {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha:0.05),
            blurRadius: 8,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: TextField(
        controller: _searchController,
        onChanged: (_) => setState(() {}),
        decoration: InputDecoration(
          hintText: 'بحث عن بلاغ...',
          hintStyle: const TextStyle(color: Color(0xFF7F8C8D)),
          border: InputBorder.none,
          contentPadding: const EdgeInsets.symmetric(
            horizontal: 16,
            vertical: 14,
          ),
          prefixIcon: const Icon(Icons.search, color: Color(0xFF7F8C8D)),
          suffixIcon: _searchController.text.isNotEmpty
              ? IconButton(
                  icon: const Icon(Icons.clear, color: Color(0xFF7F8C8D)),
                  onPressed: () {
                    _searchController.clear();
                    setState(() {});
                  },
                )
              : null,
        ),
      ),
    );
  }

  Widget _buildEmpty() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(Icons.report_problem, size: 64, color: Colors.grey[400]),
          const SizedBox(height: 16),
          const Text(
            'لا توجد بلاغات',
            style: TextStyle(fontSize: 18, color: Color(0xFF7F8C8D)),
          ),
        ],
      ),
    );
  }

  List<ReportEntity> _applyFiltersOnEntities(List<ReportEntity> all) {
    final query = _searchController.text.trim().toLowerCase();

    return all.where((r) {
      final id = r.id.toLowerCase();
      final reportStatus = r.reportStatus.toLowerCase();
      final assignmentStatus = r.assignmentStatus.toLowerCase();
      final uiBucket = r.uiBucket.toLowerCase();
      final desc = r.description.toLowerCase();
      final location = r.locationText.toLowerCase();

      final matchesSearch =
          query.isEmpty ||
          id.contains(query) ||
          reportStatus.contains(query) ||
          assignmentStatus.contains(query) ||
          uiBucket.contains(query) ||
          desc.contains(query) ||
          location.contains(query);

      if (!matchesSearch) return false;

      if (_currentFilter == 'all') return true;
      return uiBucket == _currentFilter;
    }).toList();
  }

  Widget _buildFilterButton(String text, String filter) {
    final isActive = _currentFilter == filter;

    return ElevatedButton(
      onPressed: () => setState(() => _currentFilter = filter),
      style: ElevatedButton.styleFrom(
        backgroundColor: isActive ? const Color(0xFF3498DB) : Colors.white,
        foregroundColor: isActive ? Colors.white : const Color(0xFF3498DB),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(25),
          side: const BorderSide(color: Color(0xFF3498DB)),
        ),
        padding: const EdgeInsets.symmetric(horizontal: 16),
        elevation: 0,
      ),
      child: Text(
        text,
        style: TextStyle(
          fontWeight: FontWeight.w500,
          color: isActive ? Colors.white : const Color(0xFF3498DB),
        ),
      ),
    );
  }

  Widget _buildReportCard(ReportEntity e, bool actionLoading) {
    final (statusText, statusColor) = _statusUi(e.uiBucket);
    final timeText = DateFormat('yyyy-MM-dd HH:mm').format(e.createdAt);

    final showAssignmentActions = e.uiBucket == 'new';
    final isCompleted = e.uiBucket == 'completed';

    return GestureDetector(
      onTap: () => _navigateToReportExecutionPage(e),
      child: Container(
        margin: const EdgeInsets.only(bottom: 12),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(12),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha:0.05),
              blurRadius: 8,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.end, // العرض من اليمين
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      'بلاغ #${e.id}',
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: Color(0xFF2C3E50),
                      ),
                      overflow: TextOverflow.ellipsis,
                      textAlign: TextAlign.right,
                    ),
                  ),
                  Text(
                    timeText,
                    style: const TextStyle(
                      fontSize: 12,
                      color: Color(0xFF7F8C8D),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 10),
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 4,
                ),
                decoration: BoxDecoration(
                  color: statusColor.withValues(alpha:0.1),
                  borderRadius: BorderRadius.circular(20),
                ),
                child: Text(
                  statusText,
                  style: TextStyle(
                    color: statusColor,
                    fontSize: 12,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
              const SizedBox(height: 12),
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _buildThumb(e.imagePath),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment:
                          CrossAxisAlignment.end, // العرض من اليمين
                      children: [
                        Text(
                          e.locationText,
                          style: const TextStyle(
                            fontSize: 13,
                            color: Color(0xFF34495E),
                          ),
                          textAlign: TextAlign.right,
                        ),
                        const SizedBox(height: 8),
                        Text(
                          e.description.isEmpty ? '—' : e.description,
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                          style: const TextStyle(
                            fontSize: 13,
                            color: Color(0xFF7F8C8D),
                          ),
                          textAlign: TextAlign.right,
                        ),
                      ],
                    ),
                  ),
                ],
              ),
              if (showAssignmentActions) ...[
                const SizedBox(height: 14),
                Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    ElevatedButton(
                      onPressed: actionLoading
                          ? null
                          : () => _showRejectDialog(e),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFFE74C3C),
                        foregroundColor: Colors.white,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(8),
                        ),
                        padding: const EdgeInsets.symmetric(
                          horizontal: 20,
                          vertical: 10,
                        ),
                      ),
                      child: const Text('رفض الإسناد'),
                    ),
                    const SizedBox(width: 12),
                    ElevatedButton(
                      onPressed: actionLoading
                          ? null
                          : () => _acceptAssignment(e),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFF2ECC71),
                        foregroundColor: Colors.white,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(8),
                        ),
                        padding: const EdgeInsets.symmetric(
                          horizontal: 20,
                          vertical: 10,
                        ),
                      ),
                      child: const Text('قبول الإسناد'),
                    ),
                  ],
                ),
              ],
              if (isCompleted) ...[
                const SizedBox(height: 10),
                const Divider(),
                const Text(
                  'تم إغلاق البلاغ',
                  style: TextStyle(
                    color: Color(0xFF2ECC71),
                    fontWeight: FontWeight.bold,
                  ),
                  textAlign: TextAlign.right,
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }

  (String, Color) _statusUi(String uiBucket) {
    switch (uiBucket) {
      case 'new':
        return ('جديدة', const Color(0xFFE74C3C));
      case 'in_progress':
        return ('قيد التنفيذ', const Color(0xFFF39C12));
      case 'under_review':
        return ('قيد المراجعة', const Color(0xFF9B59B6));
      case 'completed':
        return ('مكتملة', const Color(0xFF2ECC71));
      case 'rejected':
        return ('مرفوضة', const Color(0xFF7F8C8D));
      default:
        return ('غير معروفة', const Color(0xFF95A5A6));
    }
  }

  Widget _buildThumb(String? imagePath) {
    final p = (imagePath ?? '').trim();

    if (p.isEmpty) {
      return Container(
        width: 80,
        height: 80,
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(8),
          color: Colors.grey[200],
        ),
        child: Icon(Icons.image, color: Colors.grey[500]),
      );
    }

    if (p.startsWith('http://') || p.startsWith('https://')) {
      return ClipRRect(
        borderRadius: BorderRadius.circular(8),
        child: Image.network(
          p,
          width: 80,
          height: 80,
          fit: BoxFit.cover,
          errorBuilder: (_, error, __) => _thumbFallback(),
          loadingBuilder: (context, child, progress) {
            if (progress == null) return child;
            return const SizedBox(
              width: 80,
              height: 80,
              child: Center(child: CircularProgressIndicator()),
            );
          },
        ),
      );
    }

    final isAsset = p.startsWith('assets/') || p.startsWith('images/');
    if (isAsset) {
      return Container(
        width: 80,
        height: 80,
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(8),
          image: DecorationImage(image: AssetImage(p), fit: BoxFit.cover),
        ),
      );
    }

    if (File(p).existsSync()) {
      return Container(
        width: 80,
        height: 80,
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(8),
          image: DecorationImage(image: FileImage(File(p)), fit: BoxFit.cover),
        ),
      );
    }

    return _thumbFallback();
  }

  Widget _thumbFallback() {
    return Container(
      width: 80,
      height: 80,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(8),
        color: Colors.grey[200],
      ),
      child: Icon(Icons.broken_image, color: Colors.grey[500]),
    );
  }

  void _navigateToReportExecutionPage(ReportEntity e) {
    FocusScope.of(context).unfocus();
    Navigator.push(
      context,
      MaterialPageRoute(builder: (_) => ReportExecutionPage(report: e)),
    );
  }

  void _acceptAssignment(ReportEntity report) {
    if (!mounted) return;
    context.read<ReportsBloc>().add(AcceptAssignedReport(reportId: report.id));
  }

  Future<void> _showRejectDialog(ReportEntity report) async {
    _rejectReasonController.clear();

    final result = await showDialog<String?>(
      context: context,
      builder: (dialogContext) {
        return AlertDialog(
          title: const Text('رفض الإسناد'),
          content: TextField(
            controller: _rejectReasonController,
            maxLines: 3,
            decoration: const InputDecoration(
              hintText: 'اكتب سبب الرفض (اختياري)',
              border: OutlineInputBorder(),
            ),
          ),
          actions: [
            TextButton(
              onPressed: () {
                FocusScope.of(dialogContext).unfocus();
                Navigator.of(dialogContext).pop(null);
              },
              child: const Text('إلغاء'),
            ),
            ElevatedButton(
              onPressed: () {
                FocusScope.of(dialogContext).unfocus();
                final text = _rejectReasonController.text.trim();
                Navigator.of(dialogContext).pop(text.isEmpty ? '' : text);
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color(0xFFE74C3C),
                foregroundColor: Colors.white,
              ),
              child: const Text('تأكيد الرفض'),
            ),
          ],
        );
      },
    );

    if (!mounted) return;
    if (result == null) return;

    context.read<ReportsBloc>().add(
      RejectAssignedReport(
        reportId: report.id,
        rejectionReason: result.isEmpty ? null : result,
      ),
    );
  }
}
