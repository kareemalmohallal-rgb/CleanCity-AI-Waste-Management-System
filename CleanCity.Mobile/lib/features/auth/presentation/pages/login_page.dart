import 'package:cleancityapp/core/notifications/notification_service.dart';
import 'package:flutter/material.dart';
import 'package:cleancityapp/features/auth/data/datasources/auth_local_data_source.dart';
import 'package:cleancityapp/features/auth/data/datasources/auth_remote_data_source.dart';
import 'package:cleancityapp/features/auth/data/datasources/push_remote_data_source.dart';
import 'package:cleancityapp/features/auth/data/repositories/auth_repository.dart';
import 'package:cleancityapp/core/network/api_factory.dart';
class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _formKey = GlobalKey<FormState>();

  final TextEditingController _userNameController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();

  bool _obscurePassword = true;
  bool _isLoading = false;

  @override
  void dispose() {
    _userNameController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  Future<void> _syncDriverNotificationToken(int driverId) async {
    try {
      await NotificationService.instance.initialize();

      final token = await NotificationService.instance.getToken();
      if (token == null || token.trim().isEmpty) {
        debugPrint('FCM token is null or empty');
        return;
      }

      final pushRemote = PushRemoteDataSource(ApiFactory.api);

      await pushRemote.registerDriverToken(driverId: driverId, fcmToken: token);

      debugPrint('DRIVER FCM TOKEN REGISTERED SUCCESSFULLY');

      NotificationService.instance.onTokenRefresh.listen((newToken) async {
        if (newToken.trim().isEmpty) return;

        try {
          await pushRemote.registerDriverToken(
            driverId: driverId,
            fcmToken: newToken,
          );
          debugPrint('DRIVER FCM TOKEN REFRESHED SUCCESSFULLY');
        } catch (e) {
          debugPrint('FCM refresh sync error: $e');
        }
      });
    } catch (e) {
      debugPrint('Driver notification sync error: $e');
    }
  }

  Future<void> _submitLogin() async {
    FocusScope.of(context).unfocus();

    if (!_formKey.currentState!.validate()) return;

    final username = _userNameController.text.trim();
    final password = _passwordController.text;

    setState(() => _isLoading = true);

    try {
      debugPrint('LOGIN BUTTON PRESSED');
      debugPrint('username: $username');
      debugPrint('password length: ${password.length}');

      final authRepository = AuthRepository(
        AuthRemoteDataSource(ApiFactory.api),
        AuthLocalDataSource(),
        ApiFactory.api,
      );

      final res = await authRepository.login(
        username: username,
        password: password,
      );

      debugPrint('AUTH LOGIN SUCCESS');
      debugPrint('TOKEN EMPTY? ${res.token.isEmpty}');
      debugPrint('DRIVER ID FROM API: ${res.driverId}');

      final savedDriverId = await authRepository.getSavedDriverId();
      debugPrint('SAVED DRIVER ID: $savedDriverId');

      if (!mounted) return;

      if (savedDriverId == null) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text(
              'تم تسجيل الدخول لكن لم يتم العثور على DriverId للمستخدم الحالي',
            ),
            backgroundColor: Colors.orange,
            behavior: SnackBarBehavior.floating,
          ),
        );
        return;
      }

      // ✅ ربط FCM token بالسائق بعد نجاح تسجيل الدخول
      await _syncDriverNotificationToken(savedDriverId);

      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('تم تسجيل الدخول بنجاح'),
          behavior: SnackBarBehavior.floating,
        ),
      );

      // ✅ انتقل للصفحة الرئيسية بعد النجاح
      Navigator.pushReplacementNamed(context, '/home');
    } catch (e, s) {
      debugPrint('LOGIN ERROR: $e');
      debugPrintStack(stackTrace: s);

      String errorMsg = e.toString().toLowerCase();
      String showErrorMessage;

      if (errorMsg.contains('401') ||
          errorMsg.contains('unauthorized') ||
          errorMsg.contains('invalid') ||
          errorMsg.contains('مستخدم') ||
          errorMsg.contains('password') ||
          errorMsg.contains('credentials')) {
        showErrorMessage = 'اسم المستخدم أو كلمة المرور غير صحيحة';
      } else {
        showErrorMessage = 'فشل تسجيل الدخول: $e';
      }

      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(showErrorMessage),
          backgroundColor: Colors.red,
          behavior: SnackBarBehavior.floating,
        ),
      );
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    const bgColor = Color(0xFFF5F7FA);
    const cardColor = Colors.white;
    const textPrimary = Color(0xFF2C3E50);
    const textSecondary = Color(0xFF7F8C8D);
    const fieldFill = Color(0xFFF8F9FA);
    const primaryColor = Color(0xFF3498DB);

    return Directionality(
      textDirection: TextDirection.rtl,
      child: Scaffold(
        backgroundColor: bgColor,
        appBar: AppBar(
          iconTheme: const IconThemeData(color: Colors.white),
          leading: IconButton(
            icon: const Icon(Icons.arrow_back, color: Colors.white),
            onPressed: () {
              if (Navigator.canPop(context)) {
                Navigator.pop(context);
              } else {
                // لا يوجد route خلفنا (جاء بعد logout) → الرئيسية
                Navigator.pushReplacementNamed(context, '/home');
              }
            },
          ), // سهم الرجوع أبيض
          title: const Text(
            'تسجيل الدخول',
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
          // حذف أزرار التبديل بين الفاتح والداكن وأي أكشن أخرى
        ),
        body: Center(
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 420),
              child: Container(
                padding: const EdgeInsets.all(24),
                decoration: BoxDecoration(
                  color: cardColor,
                  borderRadius: BorderRadius.circular(18),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.grey.withValues(alpha: 0.10),
                      blurRadius: 14,
                      offset: const Offset(0, 6),
                    ),
                  ],
                ),
                child: Form(
                  key: _formKey,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Container(
                        width: 84,
                        height: 84,
                        margin: const EdgeInsets.only(bottom: 16),
                        decoration: BoxDecoration(
                          color: primaryColor,
                          shape: BoxShape.circle,
                          border: Border.all(
                            color: primaryColor.withValues(alpha: 0.2),
                            width: 4,
                          ),
                        ),
                        child: const Icon(
                          Icons.lock_outline,
                          color: Colors.white,
                          size: 40,
                        ),
                      ),
                      const Text(
                        'مرحبًا بك',
                        textAlign: TextAlign.center,
                        style: TextStyle(
                          fontSize: 24,
                          fontWeight: FontWeight.bold,
                          color: textPrimary,
                        ),
                      ),
                      const SizedBox(height: 8),
                      const Text(
                        'أدخل اسم المستخدم وكلمة السر للمتابعة',
                        textAlign: TextAlign.center,
                        style: TextStyle(fontSize: 14, color: textSecondary),
                      ),
                      const SizedBox(height: 24),
                      const Text(
                        'اسم المستخدم',
                        style: TextStyle(
                          fontWeight: FontWeight.w600,
                          color: textPrimary,
                        ),
                      ),
                      const SizedBox(height: 8),
                      TextFormField(
                        controller: _userNameController,
                        textInputAction: TextInputAction.next,
                        keyboardType: TextInputType.text,
                        style: const TextStyle(color: textPrimary),
                        textDirection: TextDirection.rtl,
                        decoration: const InputDecoration(
                          hintText: 'مثال:123456789 او رقم هاتفك',
                          hintStyle: TextStyle(color: textSecondary),
                          prefixIcon: Icon(
                            Icons.person_outline,
                            color: textSecondary,
                          ),
                          filled: true,
                          fillColor: fieldFill,
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.all(Radius.circular(12)),
                            borderSide: BorderSide.none,
                          ),
                          contentPadding: EdgeInsets.symmetric(
                            horizontal: 14,
                            vertical: 14,
                          ),
                        ),
                        validator: (value) {
                          final v = value?.trim() ?? '';
                          if (v.isEmpty) return 'اسم المستخدم مطلوب';
                          if (v.length < 3) return '��سم المستخدم قصير جدًا';
                          return null;
                        },
                      ),
                      const SizedBox(height: 16),
                      const Text(
                        'كلمة السر',
                        style: TextStyle(
                          fontWeight: FontWeight.w600,
                          color: textPrimary,
                        ),
                      ),
                      const SizedBox(height: 8),
                      TextFormField(
                        controller: _passwordController,
                        textInputAction: TextInputAction.done,
                        obscureText: _obscurePassword,
                        style: const TextStyle(color: textPrimary),
                        textDirection: TextDirection.rtl,
                        onFieldSubmitted: (_) => _submitLogin(),
                        decoration: InputDecoration(
                          hintText: 'أدخل كلمة السر',
                          hintStyle: const TextStyle(color: textSecondary),
                          prefixIcon: const Icon(
                            Icons.lock_outline,
                            color: textSecondary,
                          ),
                          suffixIcon: IconButton(
                            onPressed: () {
                              setState(
                                () => _obscurePassword = !_obscurePassword,
                              );
                            },
                            icon: Icon(
                              _obscurePassword
                                  ? Icons.visibility_off_outlined
                                  : Icons.visibility_outlined,
                              color: textSecondary,
                            ),
                          ),
                          filled: true,
                          fillColor: fieldFill,
                          border: const OutlineInputBorder(
                            borderRadius: BorderRadius.all(Radius.circular(12)),
                            borderSide: BorderSide.none,
                          ),
                          contentPadding: const EdgeInsets.symmetric(
                            horizontal: 14,
                            vertical: 14,
                          ),
                        ),
                        validator: (value) {
                          final v = value ?? '';
                          if (v.isEmpty) return 'كلمة السر مطلوبة';
                          if (v.length < 6) {
                            return 'كلمة السر يجب أن تكون 6 أحرف على الأقل';
                          }
                          return null;
                        },
                      ),
                      const SizedBox(height: 12),

                      const SizedBox(height: 8),
                      SizedBox(
                        height: 52,
                        child: ElevatedButton(
                          onPressed: _isLoading ? null : _submitLogin,
                          style: ElevatedButton.styleFrom(
                            backgroundColor: primaryColor,
                            disabledBackgroundColor: primaryColor.withValues(alpha: 0.6),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                            ),
                            elevation: 0,
                          ),
                          child: _isLoading
                              ? const SizedBox(
                                  width: 22,
                                  height: 22,
                                  child: CircularProgressIndicator(
                                    strokeWidth: 2.5,
                                    valueColor: AlwaysStoppedAnimation<Color>(
                                      Colors.white,
                                    ),
                                  ),
                                )
                              : const Text(
                                  'تسجيل الدخول',
                                  style: TextStyle(
                                    fontSize: 16,
                                    fontWeight: FontWeight.bold,
                                    color: Colors.white,
                                  ),
                                ),
                        ),
                      ),
                      const SizedBox(height: 14),
                      const Text(
                        'الإصدار 1.0.0',
                        textAlign: TextAlign.center,
                        style: TextStyle(color: textSecondary, fontSize: 12),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
