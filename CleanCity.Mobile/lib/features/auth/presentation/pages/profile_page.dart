import 'package:cleancityapp/core/network/api_factory.dart';
import 'package:cleancityapp/features/auth/presentation/pages/login_page.dart';
import 'package:cleancityapp/features/settings/data/datasources/profile_remote_data_source.dart';
import 'package:flutter/material.dart';

import '../../../auth/data/datasources/auth_local_data_source.dart';
import '../../data/models/driver_model.dart';

class ProfilePage extends StatefulWidget {
  const ProfilePage({super.key});

  @override
  State<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
  DriverModel? _driver;
  bool _isLoadingProfile = true;
  String? _profileError;

  late final AuthLocalDataSource _authLocal;
  late final ProfileRemoteDataSource _profileRemote;

  @override
  void initState() {
    super.initState();
    _authLocal = AuthLocalDataSource();
    _profileRemote = ProfileRemoteDataSourceImpl(ApiFactory.api);
    _loadDriverProfile();
  }

  Future<void> _loadDriverProfile() async {
    setState(() {
      _isLoadingProfile = true;
      _profileError = null;
    });

    try {
      final driverId = await _authLocal.getDriverId();

      if (driverId == null) {
        throw Exception(
          'لا يوجد DriverId للمستخدم الحالي. تأكد أن تسجيل الدخول يرجع DriverId ويتم حفظه محليًا.',
        );
      }

      final driver = await _profileRemote.getDriverById(driverId);

      if (!mounted) return;
      setState(() {
        _driver = driver;
        _isLoadingProfile = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _profileError = e.toString();
        _isLoadingProfile = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final profileName = _driver?.fullName ?? '—';
    final profilePhone = _driver?.phoneNumber ?? '—';
    final isDriverActive = _driver?.isActive ?? false;

    return Directionality(
      textDirection: TextDirection.rtl,
      child: Scaffold(
        backgroundColor: const Color(0xFFF5F7FA),
        appBar: AppBar(
          title: const Text(
            'الملف الشخصي',
            style: TextStyle(
              fontSize: 20,
              fontWeight: FontWeight.bold,
              color: Colors.white,
            ),
          ),
          centerTitle: true,
          backgroundColor: const Color.fromARGB(255, 87, 211, 157),
          elevation: 0,
          shape: const RoundedRectangleBorder(
            borderRadius: BorderRadius.only(
              bottomLeft: Radius.circular(16),
              bottomRight: Radius.circular(16),
            ),
          ),
          leading: IconButton(
            icon: const Icon(Icons.arrow_back, color: Colors.white),
            onPressed: () => Navigator.pop(context),
          ),
        ),
        body: RefreshIndicator(
          onRefresh: _loadDriverProfile,
          child: SingleChildScrollView(
            physics: const AlwaysScrollableScrollPhysics(),
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  // قسم معلومات المستخدم
                  Container(
                    padding: const EdgeInsets.all(24),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(16),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.grey.withValues(alpha: 0.1),
                          blurRadius: 8,
                          offset: const Offset(0, 4),
                        ),
                      ],
                    ),
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        // صورة المستخدم
                        Container(
                          width: 100,
                          height: 100,
                          decoration: BoxDecoration(
                            shape: BoxShape.circle,
                            color: const Color(0xFF3498DB),
                            border: Border.all(
                              color: const Color(
                                0xFF3498DB,
                              ).withValues(alpha: 0.2),
                              width: 4,
                            ),
                          ),
                          child: const Icon(
                            Icons.person,
                            size: 50,
                            color: Colors.white,
                          ),
                        ),
                        const SizedBox(height: 16),
                        // حالة التحميل / الخطأ الخاصة بالبروفايل
                        if (_isLoadingProfile) ...[
                          const CircularProgressIndicator(),
                          const SizedBox(height: 12),
                          const Text(
                            'جاري تحميل بيانات السائق...',
                            style: TextStyle(color: Color(0xFF7F8C8D)),
                          ),
                          const SizedBox(height: 12),
                        ] else if (_profileError != null) ...[
                          Text(
                            'تعذر تحميل البيانات',
                            style: TextStyle(
                              color: Colors.red.shade400,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          const SizedBox(height: 8),
                          Text(
                            _profileError!,
                            textAlign: TextAlign.center,
                            style: const TextStyle(
                              fontSize: 12,
                              color: Color(0xFF7F8C8D),
                            ),
                          ),
                          const SizedBox(height: 12),
                          ElevatedButton.icon(
                            onPressed: _loadDriverProfile,
                            icon: const Icon(
                              Icons.refresh,
                              color: Colors.white,
                            ),
                            label: const Text(
                              'إعادة المحاولة',
                              style: TextStyle(color: Colors.white),
                            ),
                            style: ElevatedButton.styleFrom(
                              backgroundColor: const Color(0xFF3498DB),
                            ),
                          ),
                          const SizedBox(height: 12),
                        ],
                        // اسم المستخدم من API
                        Text(
                          profileName,
                          style: const TextStyle(
                            fontSize: 24,
                            fontWeight: FontWeight.bold,
                            color: Color(0xFF2C3E50),
                          ),
                        ),
                        const SizedBox(height: 8),
                        // رقم الهاتف
                        Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            const Icon(
                              Icons.phone,
                              size: 18,
                              color: Color(0xFF7F8C8D),
                            ),
                            const SizedBox(width: 8),
                            Text(
                              profilePhone,
                              style: const TextStyle(
                                fontSize: 18,
                                color: Color(0xFF7F8C8D),
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 8),
                        // حالة السائق نشط/غير نشط
                        Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 6,
                          ),
                          decoration: BoxDecoration(
                            color: isDriverActive
                                ? Colors.green.withValues(alpha: 0.12)
                                : Colors.green.withValues(alpha: 0.3),
                            borderRadius: BorderRadius.circular(20),
                            border: Border.all(
                              color: isDriverActive
                                  ? Colors.green.withValues(alpha: 0.3)
                                  : Colors.red.withValues(alpha: 0.3),
                            ),
                          ),
                          child: Text(
                            isDriverActive ? 'السائق نشط' : 'السائق غير نشط',
                            style: TextStyle(
                              color: isDriverActive ? Colors.green : Colors.red,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ),
                        const SizedBox(height: 16),
                        // زر تعديل الملف
                        SizedBox(
                          width: double.infinity,
                          child: OutlinedButton.icon(
                            onPressed: () {
                              Navigator.push(
                                context,
                                MaterialPageRoute(
                                  builder: (context) => const LoginPage(),
                                ),
                              );
                            },
                            style: OutlinedButton.styleFrom(
                              padding: const EdgeInsets.symmetric(vertical: 12),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(8),
                              ),
                              side: const BorderSide(color: Color(0xFF3498DB)),
                            ),
                            icon: const Icon(
                              Icons.edit,
                              color: Color(0xFF3498DB),
                            ),
                            label: const Text(
                              'تعديل الملف',
                              style: TextStyle(color: Color(0xFF3498DB)),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 16),
                  // معلومات إضافية
                  if (_driver != null)
                    Container(
                      padding: const EdgeInsets.all(16),
                      margin: const EdgeInsets.only(bottom: 16),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(16),
                        boxShadow: [
                          BoxShadow(
                            color: Colors.grey.withValues(alpha: 0.1),
                            blurRadius: 8,
                            offset: const Offset(0, 4),
                          ),
                        ],
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            'بيانات السائق من النظام',
                            style: TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                              color: Color(0xFF2C3E50),
                            ),
                          ),
                          const SizedBox(height: 12),
                          _buildInfoRow('رقم السائق', _driver!.id.toString()),
                          _buildInfoRow(
                            'رقم الرخصة',
                            _driver!.licenseNumber?.isNotEmpty == true
                                ? _driver!.licenseNumber!
                                : 'غير متوفر',
                          ),
                          _buildInfoRow(
                            'رقم المنطقة',
                            _driver!.areaId?.toString() ?? 'غير محدد',
                          ),
                          _buildInfoRow(
                            'رقم الشاحتة',
                            _driver!.truckId?.toString() ?? 'غير محدد',
                          ),
                        ],
                      ),
                    ),
                  // زر تسجيل الخروج
                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton.icon(
                      onPressed: () {
                        _showLogoutConfirmationDialog();
                      },
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFFE74C3C),
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                        elevation: 0,
                      ),
                      icon: const Icon(Icons.logout, color: Colors.white),
                      label: const Text(
                        'تسجيل خروج',
                        style: TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: Colors.white,
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(height: 8),
                  const Text(
                    'الإصدار 1.0.0',
                    textAlign: TextAlign.center,
                    style: TextStyle(color: Color(0xFF95A5A6)),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildInfoRow(String title, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          Expanded(
            flex: 3,
            child: Text(
              title,
              style: const TextStyle(
                fontWeight: FontWeight.w600,
                color: Color(0xFF7F8C8D),
              ),
            ),
          ),
          Expanded(
            flex: 4,
            child: Text(
              value,
              textAlign: TextAlign.end,
              style: const TextStyle(
                color: Color(0xFF2C3E50),
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
        ],
      ),
    );
  }

  void _showLogoutConfirmationDialog() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('تسجيل الخروج'),
        content: const Text('هل أنت متأكد من تسجيل الخروج؟'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text(
              'إلغاء',
              style: TextStyle(color: Color(0xFF2C3E50)),
            ),
          ),
          ElevatedButton(
            onPressed: () async {
              // ✅ احفظ الـ Navigator قبل أي async
              final navigator = Navigator.of(context);
              Navigator.pop(context);

              await _authLocal.clear();

              navigator.pushAndRemoveUntil(
                MaterialPageRoute(builder: (_) => const LoginPage()),
                (route) => false,
              );
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: const Color(0xFFE74C3C),
            ),
            child: const Text('تسجيل خروج'),
          ),
        ],
      ),
    );
  }
}
