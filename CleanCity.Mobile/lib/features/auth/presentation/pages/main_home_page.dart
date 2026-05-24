import 'package:cleancityapp/core/network/api_factory.dart';
import 'package:cleancityapp/features/auth/data/datasources/anonymous_device_local_data_source.dart';
import 'package:cleancityapp/features/auth/data/datasources/auth_local_data_source.dart';
import 'package:cleancityapp/features/auth/data/datasources/notifications_remote_data_source.dart';
import 'package:cleancityapp/features/auth/presentation/pages/awareness_content_page.dart';
import 'package:cleancityapp/features/auth/presentation/pages/notifications_page.dart';
import 'package:cleancityapp/features/auth/presentation/pages/profile_page.dart'
    show ProfilePage;
import 'package:cleancityapp/features/auth/presentation/pages/report_sewage_page.dart';
import 'package:cleancityapp/features/auth/presentation/pages/report_waste_page.dart';
import 'package:cleancityapp/features/auth/presentation/pages/track_reports_page.dart';
import 'package:cleancityapp/features/auth/presentation/pages/login_page.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

class MainHomePage extends StatefulWidget {
  const MainHomePage({super.key});

  @override
  State<MainHomePage> createState() => _MainHomePageState();
}

class _MainHomePageState extends State<MainHomePage> {
  int _unreadCount = 0;

  @override
  void initState() {
    super.initState();
    _loadUnreadCount();
  }

  /// ✅ يجلب الإشعارات من كلا المصدرين عند الحاجة ويحسب غير المقروءة
  Future<void> _loadUnreadCount() async {
    try {
      final authLocal = AuthLocalDataSource();
      final driverId = await authLocal.getDriverId();

      final prefs = SharedPreferencesAsync();
      final anonymousLocal = AnonymousDeviceLocalDataSourceImpl(prefs);
      final anonymousDeviceId = await anonymousLocal
          .getSavedAnonymousDeviceId();

      final remote = NotificationsRemoteDataSourceImpl(ApiFactory.api);
      final Set<int> seenIds = {};
      int unread = 0;

      // ✅ اشعارات السائق
      if (driverId != null) {
        final driverItems = await remote.getForDriver(driverId);
        for (final item in driverItems) {
          if (seenIds.add(item.recipientId) && !item.isRead) unread++;
        }
      }

      // ✅ اشعارات الجهاز (بلاغات + توعوية) — لكلا النوعين
      if (anonymousDeviceId != null) {
        final deviceItems = await remote.getForAnonymousDevice(
          anonymousDeviceId,
        );
        for (final item in deviceItems) {
          if (seenIds.add(item.recipientId) && !item.isRead) unread++;
        }
      }

      if (!mounted) return;
      setState(() => _unreadCount = unread);
    } catch (_) {
      // في حالة الخطأ نُبقي العداد كما هو
    }
  }

  @override
  Widget build(BuildContext context) {
    return Directionality(
      textDirection: TextDirection.rtl,
      child: Scaffold(appBar: _buildAppBar(), body: _buildBody()),
    );
  }

  AppBar _buildAppBar() {
    return AppBar(
      title: const Text(
        'الرئيسية',
        style: TextStyle(
          fontSize: 22,
          fontWeight: FontWeight.bold,
          color: Colors.white,
        ),
      ),
      centerTitle: true,
      backgroundColor: const Color.fromARGB(255, 87, 211, 157),
      elevation: 2,
      shadowColor: Colors.white,
      iconTheme: const IconThemeData(color: Colors.white),
      actions: [
        Stack(
          clipBehavior: Clip.none,
          children: [
            IconButton(
              icon: const Icon(Icons.notifications_none_outlined),
              color: Colors.white,
              onPressed: _showNotifications,
            ),
            if (_unreadCount > 0)
              Positioned(
                left: 6,
                top: 6,
                child: Container(
                  padding: const EdgeInsets.all(2),
                  decoration: BoxDecoration(
                    color: Colors.red,
                    borderRadius: BorderRadius.circular(10),
                  ),
                  constraints: const BoxConstraints(
                    minWidth: 16,
                    minHeight: 16,
                  ),
                  child: Text(
                    _unreadCount > 99 ? '99+' : '$_unreadCount',
                    style: const TextStyle(color: Colors.white, fontSize: 10),
                    textAlign: TextAlign.center,
                  ),
                ),
              ),
          ],
        ),
        IconButton(
          icon: const Icon(Icons.person_outline),
          color: Colors.white,
          onPressed: _goToProfile,
        ),
      ],
    );
  }

  Widget _buildBody() {
    return SingleChildScrollView(
      physics: const BouncingScrollPhysics(),
      child: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            _buildWelcomeMessage(),
            const SizedBox(height: 30),
            _buildActionButton(
              icon: Icons.delete_outline,
              title: 'إبلاغ عن نفايات',
              subtitle: 'هل وجدت كومة نفايات؟',
              color: const Color(0xFF29F14D),
              onPressed: _reportWaste,
            ),
            const SizedBox(height: 20),
            _buildActionButton(
              icon: Icons.water_drop_outlined,
              title: 'إبلاغ عن صرف صحي',
              subtitle: 'مشكلة في شبكة الصرف الصحي؟',
              color: const Color(0xFF2196F3),
              onPressed: _reportSewage,
            ),
            const SizedBox(height: 20),
            _buildActionButton(
              icon: Icons.location_on_outlined,
              title: 'تتبع بلاغاتي',
              subtitle: 'تابع حالة البلاغات السابقة',
              color: const Color(0xFF03A9F4),
              onPressed: _trackReports,
            ),
            const SizedBox(height: 20),
            _buildInfoButton(),
            const SizedBox(height: 30),
          ],
        ),
      ),
    );
  }

  Widget _buildWelcomeMessage() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: const Color(0xFFE8F5E9),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: const Color(0xFFC8E6C9), width: 1),
      ),
      child: Row(
        children: [
          const Icon(Icons.eco_outlined, color: Color(0xFF29F14D), size: 40),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'مرحباً بك في تطبيق نظافة صنعاء',
                  style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF1B5E20),
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  'ساهم معنا في الحفاظ على نظافة مدينتك',
                  style: TextStyle(fontSize: 14, color: Colors.grey[700]),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildActionButton({
    required IconData icon,
    required String title,
    required String subtitle,
    required Color color,
    required VoidCallback onPressed,
  }) {
    return ElevatedButton(
      onPressed: onPressed,
      style: ElevatedButton.styleFrom(
        backgroundColor: color,
        foregroundColor: Colors.white,
        padding: const EdgeInsets.all(16),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        elevation: 2,
        shadowColor: color.withValues(alpha: 0.3),
      ),
      child: Row(
        children: [
          Icon(icon, size: 30),
          const SizedBox(width: 15),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  subtitle,
                  style: const TextStyle(fontSize: 14, color: Colors.white70),
                ),
              ],
            ),
          ),
          const Icon(Icons.arrow_back_ios, size: 20),
        ],
      ),
    );
  }

  Widget _buildInfoButton() {
    return ElevatedButton(
      onPressed: _showInstructions,
      style: ElevatedButton.styleFrom(
        backgroundColor: Colors.white,
        foregroundColor: const Color(0xFF29F14D),
        padding: const EdgeInsets.all(16),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
          side: const BorderSide(color: Color(0xFF29F14D), width: 2),
        ),
        elevation: 1,
        shadowColor: Colors.grey[200],
      ),
      child: Row(
        children: [
          const Icon(Icons.info_outline, color: Color(0xFF29F14D), size: 30),
          const SizedBox(width: 15),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'تعليمات وتوصيات',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: Colors.grey[800],
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  'إرشادات ونصائح من وزارة الصحة',
                  style: TextStyle(fontSize: 14, color: Colors.grey[600]),
                ),
              ],
            ),
          ),
          const Icon(Icons.arrow_back_ios, color: Color(0xFF29F14D), size: 20),
        ],
      ),
    );
  }

  /// ✅ يمرر كلا الـ IDs معاً للسائق، وفقط anonymousDeviceId للمجهول
  Future<void> _showNotifications() async {
    final authLocal = AuthLocalDataSource();
    final driverId = await authLocal.getDriverId();

    final prefs = SharedPreferencesAsync();
    final anonymousLocal = AnonymousDeviceLocalDataSourceImpl(prefs);
    final anonymousDeviceId = await anonymousLocal.getSavedAnonymousDeviceId();

    if (!mounted) return;

    if (driverId != null || anonymousDeviceId != null) {
      await Navigator.push(
        context,
        MaterialPageRoute(
          builder: (context) => NotificationsPage(
            driverId: driverId,
            anonymousDeviceId: anonymousDeviceId,
          ),
        ),
      );
      _loadUnreadCount();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('لم يتم تسجيل الجهاز بعد. حاول مجدداً لاحقاً.'),
          backgroundColor: Colors.orange,
        ),
      );
    }
  }

  // الكود المصحح ✅
  Future<void> _goToProfile() async {
    final authLocal = AuthLocalDataSource();
    final driverId = await authLocal.getDriverId();

    if (!mounted) return;

    if (driverId != null) {
      // ✅ مستخدم مسجل دخول → صفحة البروفايل
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => const ProfilePage()),
      );
    } else {
      // ✅ مستخدم مجهول → صفحة تسجيل الدخول
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => const LoginPage()),
      );
    }
  }

  void _reportWaste() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => const ReportWastePage()),
    );
  }

  void _reportSewage() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => const SewageReportPage()),
    );
  }

  Future<void> _trackReports() async {
    final authLocal = AuthLocalDataSource();
    final driverId = await authLocal.getDriverId();

    if (!mounted) return;

    if (driverId != null) {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => const TrackReportsPage()),
      );
    } else {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => const LoginPage()),
      );
    }
  }

  void _showInstructions() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => const AwarenessContentPage()),
    );
  }
}
