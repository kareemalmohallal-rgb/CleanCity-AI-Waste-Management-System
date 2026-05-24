import 'package:cleancityapp/core/network/api_factory.dart';
import 'package:cleancityapp/features/auth/data/models/sewage_api.dart';
import 'package:cleancityapp/features/auth/data/models/sewage_service_provider_dto.dart';
import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';

class SewageReportPage extends StatefulWidget {
  const SewageReportPage({super.key});

  @override
  State<SewageReportPage> createState() => _SewageReportPageState();
}

class _SewageReportPageState extends State<SewageReportPage> {
  final sewageApi = SewageApi(ApiFactory.api);

  bool _loading = true;
  String? _error;
  List<SewageServiceProviderDto> _providers = [];

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });

    try {
      final items = await sewageApi.getAllProviders();

      setState(() {
        _providers = items; // ✅ بدون فلترة isActive
        _loading = false;
      });
    } catch (e) {
      setState(() {
        _error = e.toString();
        _loading = false;
      });
    }
  }

  void _callPhoneNumber(String phoneNumber) async {
    final Uri url = Uri(scheme: 'tel', path: phoneNumber);

    if (await canLaunchUrl(url)) {
      await launchUrl(url, mode: LaunchMode.externalApplication);
    } else {
      if (!mounted) return;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('تعذر الاتصال بالرقم')));
    }
  }

  @override
  Widget build(BuildContext context) {
    return Directionality(
      textDirection: TextDirection.rtl,
      child: Scaffold(
        backgroundColor: const Color(0xFFF8F9FA),
        appBar: AppBar(
          title: const Text(
            'الشفاطات',
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
            onPressed: () => Navigator.pop(context),
          ),
          elevation: 4,
          shape: const RoundedRectangleBorder(
            borderRadius: BorderRadius.only(
              bottomLeft: Radius.circular(16),
              bottomRight: Radius.circular(16),
            ),
          ),
        ),
        body: RefreshIndicator(
          onRefresh: _load,
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // عنوان الصفحة
                Container(
                  margin: const EdgeInsets.only(bottom: 20),
                  padding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 12,
                  ),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(12),
                    boxShadow: [
                      BoxShadow(
                        color: Colors.grey.withValues(alpha:0.1),
                        spreadRadius: 2,
                        blurRadius: 8,
                        offset: const Offset(0, 2),
                      ),
                    ],
                  ),
                  child: const Row(
                    children: [
                      Icon(Icons.info_outline, color: Color(0xFF00695C)),
                      SizedBox(width: 12),
                      Expanded(
                        child: Text(
                          'قائمة شفاطات الصرف الصحي المتاحة في النظام',
                          style: TextStyle(
                            fontSize: 14,
                            color: Color(0xFF555555),
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),

                const Padding(
                  padding: EdgeInsets.only(bottom: 12),
                  child: Text(
                    'قائمة الشفاطات المتاحة',
                    style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFF333333),
                    ),
                  ),
                ),

                Expanded(child: _buildBody()),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildBody() {
    if (_loading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_error != null) {
      return ListView(
        children: [
          const SizedBox(height: 80),
          const Icon(Icons.wifi_off, size: 48, color: Colors.grey),
          const SizedBox(height: 12),
          Center(
            child: Text(
              'تعذر تحميل البيانات\n$_error',
              textAlign: TextAlign.center,
              style: const TextStyle(color: Colors.red),
            ),
          ),
          const SizedBox(height: 12),
          Center(
            child: ElevatedButton(
              onPressed: _load,
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color(0xFF00695C),
                foregroundColor: Colors.white,
              ),
              child: const Text('إعادة المحاولة'),
            ),
          ),
        ],
      );
    }

    if (_providers.isEmpty) {
      return ListView(
        children: const [
          SizedBox(height: 80),
          Icon(Icons.inbox_outlined, size: 48, color: Colors.grey),
          SizedBox(height: 12),
          Center(
            child: Text(
              'لا توجد شفاطات متاحة حالياً',
              style: TextStyle(color: Colors.grey),
            ),
          ),
        ],
      );
    }

    return ListView.builder(
      itemCount: _providers.length,
      itemBuilder: (context, index) => _buildTruckCard(_providers[index]),
    );
  }

  Widget _buildTruckCard(SewageServiceProviderDto truck) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: Colors.grey.withValues(alpha:0.1),
            spreadRadius: 2,
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          borderRadius: BorderRadius.circular(12),
          onTap: () => _showTruckDetails(context, truck),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  width: 60,
                  height: 60,
                  decoration: BoxDecoration(
                    color: const Color(0xFFE0F2F1),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: const Icon(
                    Icons.local_shipping,
                    color: Color(0xFF00695C),
                    size: 32,
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      _buildDetailRow('الاسم', truck.ownerName, Icons.person),
                      _buildDetailRow('الهاتف', truck.phoneNumber, Icons.phone),
                      _buildDetailRow(
                        'سعة الشفاط',
                        truck.size,
                        Icons.inventory,
                      ),
                      _buildDetailRow(
                        'سعر الشفط',
                        '${truck.price.toStringAsFixed(0)} ريال',
                        Icons.attach_money,
                      ),
                      const SizedBox(height: 8),
                      Align(
                        alignment: Alignment.centerRight,
                        child: ElevatedButton.icon(
                          onPressed: () => _callPhoneNumber(truck.phoneNumber),
                          icon: const Icon(Icons.call, size: 18),
                          label: const Text('طلب الخدمة'),
                          style: ElevatedButton.styleFrom(
                            backgroundColor: const Color(0xFF00695C),
                            foregroundColor: Colors.white,
                            padding: const EdgeInsets.symmetric(
                              horizontal: 12,
                              vertical: 8,
                            ),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(8),
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildDetailRow(
    String label,
    String value,
    IconData icon, {
    Color color = const Color(0xFF555555),
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        children: [
          Icon(icon, color: color, size: 20),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              label,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.bold,
                color: Color(0xFF333333),
              ),
            ),
          ),
          Text(
            value,
            style: TextStyle(
              fontSize: 14,
              color: color == const Color(0xFF555555)
                  ? const Color(0xFF333333)
                  : color,
            ),
          ),
        ],
      ),
    );
  }

  void _showTruckDetails(BuildContext context, SewageServiceProviderDto truck) {
    showModalBottomSheet(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (context) {
        return Directionality(
          textDirection: TextDirection.rtl,
          child: Container(
            padding: const EdgeInsets.all(24),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Center(
                  child: Container(
                    width: 60,
                    height: 4,
                    decoration: BoxDecoration(
                      color: Colors.grey[300],
                      borderRadius: BorderRadius.circular(2),
                    ),
                  ),
                ),
                const SizedBox(height: 20),
                const Text(
                  'تفاصيل الشفاط',
                  style: TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF333333),
                  ),
                ),
                const SizedBox(height: 20),
                _buildDetailRow('الاسم', truck.ownerName, Icons.person),
                _buildDetailRow('الهاتف', truck.phoneNumber, Icons.phone),
                _buildDetailRow(
                  'سعة الشفاط',
                  '${truck.size} لتر',
                  Icons.inventory,
                ),
                _buildDetailRow(
                  'سعر الشفط',
                  '${truck.price} ريال',
                  Icons.attach_money,
                ),
                const SizedBox(height: 20),
                Align(
                  alignment: Alignment.centerRight,
                  child: ElevatedButton.icon(
                    onPressed: () {
                      Navigator.pop(context);
                      _callPhoneNumber(truck.phoneNumber);
                    },
                    icon: const Icon(Icons.call, size: 18),
                    label: const Text('طلب الخدمة الآن'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: const Color(0xFF00695C),
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(
                        horizontal: 12,
                        vertical: 14,
                      ),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                    ),
                  ),
                ),
                const SizedBox(height: 12),
              ],
            ),
          ),
        );
      },
    );
  }
}
