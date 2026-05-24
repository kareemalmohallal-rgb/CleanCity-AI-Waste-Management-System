import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:cleancityapp/features/auth/domain/entities/report_entity.dart';
import 'package:cleancityapp/features/auth/presentation/bloc/reports/reports_bloc.dart';

class ReportExecutionPage extends StatefulWidget {
  final ReportEntity report;
  const ReportExecutionPage({super.key, required this.report});
  @override
  State<ReportExecutionPage> createState() => _ReportExecutionPageState();
}

class _ReportExecutionPageState extends State<ReportExecutionPage> {
  int? _selectedFinalStatus;

  final TextEditingController _notesController = TextEditingController();
  final List<_ExecutionStatusOption> _statusOptions = const [
    _ExecutionStatusOption(
      apiValue: 3,
      uiKey: 'completed',
      label: 'تم التنفيذ بنجاح',
    ),
    _ExecutionStatusOption(
      apiValue: 2,
      uiKey: 'under_review',
      label: 'بلاغ كاذب / يحتاج مراجعة',
    ),
  ];

  bool _didSubmit = false;

  @override
  void initState() {
    super.initState();
    _selectedFinalStatus = null;
  }

  @override
  void dispose() {
    _notesController.dispose();
    super.dispose();
  }

  String get _destinationQuery => widget.report.locationText.trim();

  Uri _buildGoogleMapsDirectionsUri(String destination) {
    return Uri.parse(
      'https://www.google.com/maps/dir/?api=1&destination=${Uri.encodeComponent(destination)}&travelmode=driving',
    );
  }

  Future<void> _startNavigation() async {
    if (_destinationQuery.isEmpty) {
      _showError('عنوان البلاغ غير متوفر');
      return;
    }

    final url = _buildGoogleMapsDirectionsUri(_destinationQuery);

    try {
      final launched = await launchUrl(
        url,
        mode: LaunchMode.externalApplication,
      );

      if (!launched && mounted) {
        _showError('لا يمكن فتح المسار على خرائط جوجل');
      }
    } catch (_) {
      if (!mounted) return;
      _showError('حدث خطأ أثناء محاولة فتح التوجيه على الخرائط');
    }
  }

  void _submitExecution() {
    if (widget.report.reportStatus != 'in_progress') {
      _showError('لا يمكن إنهاء هذا البلاغ لأنه ليس في حالة قيد التنفيذ.');
      return;
    }

    if (_selectedFinalStatus == null) {
      _showError('الرجاء تحديد النتيجة النهائية للبلاغ');
      return;
    }

    _didSubmit = true;

    context.read<ReportsBloc>().add(
      CompleteDriverExecution(
        reportId: widget.report.id,
        finalStatus: _selectedFinalStatus!,
      ),
    );
  }

  void _showError(String message) {
    if (!mounted) return;

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red,
        duration: const Duration(seconds: 3),
      ),
    );
  }

  void _showSuccessDialog() {
    showDialog(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text(
          'تم إنهاء البلاغ',
          textAlign: TextAlign.center,
          style: TextStyle(color: Color(0xFF29F14D)),
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(Icons.check_circle, color: Color(0xFF29F14D), size: 60),
            const SizedBox(height: 16),
            const Text(
              'تم تحديث حالة البلاغ بنجاح',
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            Text(
              'النتيجة: ${_selectedStatusLabel()}',
              textAlign: TextAlign.center,
              style: const TextStyle(fontSize: 14, color: Colors.grey),
            ),
          ],
        ),
        actions: [
          Center(
            child: ElevatedButton(
              onPressed: () {
                Navigator.pop(dialogContext);
                Navigator.pop(context);
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color(0xFF29F14D),
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(
                  horizontal: 32,
                  vertical: 12,
                ),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              child: const Text('حسناً'),
            ),
          ),
        ],
      ),
    );
  }

  String _selectedStatusLabel() {
    final option = _statusOptions
        .where((o) => o.apiValue == _selectedFinalStatus)
        .firstOrNull;
    return option?.label ?? '—';
  }

  String _reportStatusLabel(String value) {
    switch (value) {
      case 'received':
        return 'جديدة';
      case 'in_progress':
        return 'قيد التنفيذ';
      case 'under_review':
        return 'قيد المراجعة';
      case 'completed':
        return 'مكتملة';
      case 'rejected':
        return 'مرفوضة';
      default:
        return 'غير معروفة';
    }
  }

  @override
  Widget build(BuildContext context) {
    final actionLoading = context.select(
      (ReportsBloc b) => b.state.actionLoading,
    );

    return Directionality(
      textDirection: TextDirection.rtl,
      child: Scaffold(
        appBar: AppBar(
          title: const Text('تنفيذ البلاغ'),
          centerTitle: true,
          backgroundColor: const Color.fromARGB(255, 87, 211, 157),
          elevation: 2,
          shadowColor: Colors.grey[300],
          leading: IconButton(
            icon: const Icon(Icons.arrow_back),
            onPressed: () => Navigator.pop(context),
          ),
        ),
        body: BlocListener<ReportsBloc, ReportsState>(
          listener: (context, state) {
            if (_didSubmit &&
                !state.actionLoading &&
                (state.error == null || state.error!.isEmpty)) {
              _didSubmit = false;
              _showSuccessDialog();
            }

            if (state.error != null && state.error!.isNotEmpty) {
              _didSubmit = false;
              _showError(state.error!);
            }
          },
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(20),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildReportInfo(),
                const SizedBox(height: 20),
                _buildMapSection(),
                const SizedBox(height: 30),
                _buildCurrentStatusInfo(),
                const SizedBox(height: 20),
                _buildExecutionResultSection(),
                const SizedBox(height: 40),
                _buildSubmitButton(actionLoading),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildReportInfo() {
    return Container(
      width: double.infinity, // بعرض الشاشة
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.grey[50],
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.grey[300]!),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'البلاغ ${widget.report.id}',
            style: const TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
              color: Colors.black87,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            widget.report.description.isEmpty ? '—' : widget.report.description,
            style: const TextStyle(fontSize: 16, color: Colors.black87),
          ),
          const SizedBox(height: 4),
          Text(
            'الموقع: ${widget.report.locationText}',
            style: const TextStyle(fontSize: 14, color: Colors.grey),
          ),
        ],
      ),
    );
  }

  Widget _buildMapSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'خريطة الموقع',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: Colors.black87,
          ),
        ),
        const SizedBox(height: 10),
        SizedBox(
          width: double.infinity,
          height: 56,
          child: ElevatedButton.icon(
            onPressed: _startNavigation,
            icon: const Icon(Icons.directions),
            label: const Text('تحديد الوجهة', style: TextStyle(fontSize: 18)),
            style: ElevatedButton.styleFrom(
              backgroundColor: const Color(0xFF2196F3),
              foregroundColor: Colors.white,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              elevation: 3,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildCurrentStatusInfo() {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: const Color(0xFFE3F2FD),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: const Color(0xFFBBDEFB)),
      ),
      child: Row(
        children: [
          const Icon(Icons.info_outline, color: Color(0xFF1976D2)),
          const SizedBox(width: 10),
          Expanded(
            child: Text(
              'الحالة الحالية للبلاغ: ${_reportStatusLabel(widget.report.reportStatus)}',
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: Color(0xFF0D47A1),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildExecutionResultSection() {
    final canComplete = widget.report.reportStatus == 'in_progress';

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'نتيجة تنفيذ البلاغ',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: Colors.black87,
          ),
        ),
        const SizedBox(height: 10),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 12),
          decoration: BoxDecoration(
            color: canComplete ? Colors.grey[50] : Colors.grey[200],
            borderRadius: BorderRadius.circular(12),
            border: Border.all(color: Colors.grey[300]!),
          ),
          child: DropdownButton<int>(
            value: _selectedFinalStatus,
            hint: const Text('حدد النتيجة النهائية'),
            isExpanded: true,
            underline: const SizedBox(),
            icon: const Icon(Icons.arrow_drop_down),
            style: const TextStyle(fontSize: 16, color: Colors.black87),
            items: _statusOptions
                .map(
                  (o) => DropdownMenuItem<int>(
                    value: o.apiValue,
                    child: Text(o.label),
                  ),
                )
                .toList(),
            onChanged: canComplete
                ? (newValue) => setState(() => _selectedFinalStatus = newValue)
                : null,
          ),
        ),
        if (!canComplete) ...[
          const SizedBox(height: 8),
          const Text(
            'لا يمكن إنهاء البلاغ إلا إذا كانت حالته الحالية "قيد التنفيذ".',
            style: TextStyle(
              fontSize: 13,
              color: Colors.red,
              fontWeight: FontWeight.w500,
            ),
          ),
        ],
      ],
    );
  }

  Widget _buildSubmitButton(bool loading) {
    final canComplete = widget.report.reportStatus == 'in_progress';

    return SizedBox(
      width: double.infinity,
      height: 56,
      child: ElevatedButton(
        onPressed: (!canComplete || loading) ? null : _submitExecution,
        style: ElevatedButton.styleFrom(
          backgroundColor: const Color(0xFF29F14D),
          foregroundColor: Colors.white,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          elevation: 3,
        ),
        child: loading
            ? const Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  SizedBox(
                    width: 20,
                    height: 20,
                    child: CircularProgressIndicator(
                      color: Colors.white,
                      strokeWidth: 2,
                    ),
                  ),
                  SizedBox(width: 10),
                  Text('جاري الإرسال...'),
                ],
              )
            : const Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.send, size: 20),
                  SizedBox(width: 10),
                  Text(
                    'إنهاء التنفيذ',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                ],
              ),
      ),
    );
  }
}

class _ExecutionStatusOption {
  final int apiValue;
  final String uiKey;
  final String label;

  const _ExecutionStatusOption({
    required this.apiValue,
    required this.uiKey,
    required this.label,
  });
}

extension _FirstOrNullExt<T> on Iterable<T> {
  T? get firstOrNull => isEmpty ? null : first;
}
