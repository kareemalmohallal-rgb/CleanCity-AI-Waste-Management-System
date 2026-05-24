// استيراد الصفحة الرئيسية بعد تسجيل الدخول أو بعد تجاوز الـ onboarding
import 'package:cleancityapp/features/auth/presentation/pages/main_home_page.dart';

// استيراد شاشة التعريف الأولى الخاصة بالتطبيق
import 'package:cleancityapp/features/auth/presentation/pages/onboarding_screen.dart';

// ملف إعدادات Firebase الذي يتم إنشاؤه من flutterfire
import 'package:cleancityapp/firebase_options.dart';

// مكتبة Firebase الأساسية
import 'package:firebase_core/firebase_core.dart';

// مكتبة إشعارات Firebase
import 'package:firebase_messaging/firebase_messaging.dart';

// مكتبة واجهات Flutter الأساسية
import 'package:flutter/material.dart';

// مكتبة إدارة الحالة BLoC
import 'package:flutter_bloc/flutter_bloc.dart';

// دعم اللغة العربية والـ localization
import 'package:flutter_localizations/flutter_localizations.dart';

// تخزين بيانات بسيطة محليًا مثل onboarding
import 'package:shared_preferences/shared_preferences.dart';

// ملف Dependency Injection
import 'injection_container.dart' as di;

// استدعاء Service Locator
import 'injection_container.dart' show sl;

// خدمة الإشعارات المحلية
import 'core/notifications/notification_service.dart';

// BLoC الخاص بالبلاغات
import 'features/auth/presentation/bloc/reports/reports_bloc.dart';

// BLoC الخاص بالإشعارات
import 'features/auth/presentation/bloc/notifications/notifications_bloc.dart';

// UseCase لإنشاء معرف مجهول للجهاز
import 'features/auth/domain/usecases/initialize_anonymous_device_usecase.dart';


// ==================== نقطة بداية التطبيق ====================
void main() async {
  // تهيئة Flutter قبل أي async code
  WidgetsFlutterBinding.ensureInitialized();

  // تهيئة Firebase قبل تشغيل التطبيق
  await Firebase.initializeApp(
    options: DefaultFirebaseOptions.currentPlatform,
  );

  // استقبال الإشعارات حتى لو التطبيق بالخلفية
  FirebaseMessaging.onBackgroundMessage(
    firebaseMessagingBackgroundHandler,
  );

  // تهيئة جميع dependencies مثل repositories و blocs
  await di.init();

  // تشغيل التطبيق
  runApp(const MyApp());
}


// ==================== التطبيق الرئيسي ====================
class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        // توفير BLoC البلاغات لكل التطبيق
        BlocProvider<ReportsBloc>(
          create: (_) => sl<ReportsBloc>(),
        ),

        // توفير BLoC الإشعارات
        BlocProvider<NotificationsBloc>(
          create: (_) => sl<NotificationsBloc>(),
        ),
      ],
      child: MaterialApp(
        title: 'نظافة صنعاء',
        debugShowCheckedModeBanner: false,

        // تحديد لغة التطبيق عربي
        locale: const Locale('ar'),

        // اللغات المدعومة
        supportedLocales: const [Locale('ar')],

        // مكتبات الترجمة والتعريب
        localizationsDelegates: const [
          GlobalMaterialLocalizations.delegate,
          GlobalWidgetsLocalizations.delegate,
          GlobalCupertinoLocalizations.delegate,
        ],

        // إعداد ثيم التطبيق
        theme: ThemeData(
          colorScheme: ColorScheme.fromSeed(
            seedColor: const Color(0xFF29F14D),
          ),
          useMaterial3: true,
          fontFamily: 'Cairo',
        ),

        // أول شاشة تظهر عند فتح التطبيق
        home: const _SplashScreen(),

        // مسارات التنقل
        routes: {
          '/home': (context) => const MainHomePage(),
        },
      ),
    );
  }
}


// ==================== شاشة Splash ====================
class _SplashScreen extends StatefulWidget {
  const _SplashScreen();

  @override
  State<_SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<_SplashScreen>
    with SingleTickerProviderStateMixin {

  // متحكم الحركة
  late final AnimationController _ctrl;

  // حركة الشفافية
  late final Animation<double> _fade;

  // حركة التكبير
  late final Animation<double> _scale;

  @override
  void dispose() {
    // تنظيف ال animation عند إغلاق الصفحة
    _ctrl.dispose();
    super.dispose();
  }

  @override
  void initState() {
    super.initState();

    // مدة الأنيميشن
    _ctrl = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 600),
    );

    // تأثير fade
    _fade = CurvedAnimation(
      parent: _ctrl,
      curve: Curves.easeOut,
    );

    // تأثير scale
    _scale = Tween<double>(
      begin: 0.85,
      end: 1.0,
    ).animate(
      CurvedAnimation(
        parent: _ctrl,
        curve: Curves.easeOutBack,
      ),
    );

    // تشغيل الحركة
    _ctrl.forward();

    // بدء التهيئة والانتقال
    _initAndNavigate();
  }

  // تنفيذ مهام الخلفية ثم الانتقال
  Future<void> _initAndNavigate() async {
    await Future.wait([
      // تأخير ثانية واحدة لإظهار splash
      Future.delayed(const Duration(seconds: 1)),

      // تنفيذ العمليات بالخلفية
      _runBackgroundTasks(),
    ]);

    if (!mounted) return;

    _navigateNext();
  }

  // تشغيل العمليات الثقيلة بالخلفية
  Future<void> _runBackgroundTasks() async {
    try {
      // تهيئة الإشعارات المحلية
      await NotificationService.instance.initialize();
    } catch (_) {}

    try {
      // إنشاء معرف مجهول للجهاز
      await sl<InitializeAnonymousDeviceUseCase>()();
    } catch (_) {}

    try {
      // الاشتراك في قناة التوعية
      await FirebaseMessaging.instance
          .subscribeToTopic('awareness_all');
    } catch (_) {}
  }

  // الانتقال للواجهة المناسبة
  Future<void> _navigateNext() async {
    bool onboardingSeen = false;

    try {
      final prefs = await SharedPreferences.getInstance();

      // قراءة هل المستخدم شاهد onboarding
      onboardingSeen =
          prefs.getBool('onboarding_seen') ?? false;
    } catch (_) {}

    if (!mounted) return;

    Navigator.pushReplacement(
      context,
      PageRouteBuilder(
        pageBuilder: (_, __, ___) =>
            onboardingSeen
                ? const MainHomePage()
                : const OnboardingScreen(),

        // حركة fade بين الصفحات
        transitionsBuilder: (_, anim, __, child) =>
            FadeTransition(opacity: anim, child: child),

        transitionDuration:
            const Duration(milliseconds: 350),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,

      body: Center(
        child: FadeTransition(
          opacity: _fade,
          child: ScaleTransition(
            scale: _scale,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                // لوجو التطبيق
                Container(
                  width: 116,
                  height: 116,
                  decoration: BoxDecoration(
                    color: const Color(0xFFE8FFF4),
                    borderRadius: BorderRadius.circular(30),
                    boxShadow: [
                      BoxShadow(
                        color: const Color(0xFF57D39D)
                            .withValues(alpha: 0.22),
                        blurRadius: 30,
                        offset: const Offset(0, 12),
                      ),
                    ],
                  ),
                  padding: const EdgeInsets.all(18),
                  child: Image.asset(
                    'images/logo.png',
                    fit: BoxFit.contain,
                  ),
                ),

                const SizedBox(height: 22),

                // اسم التطبيق
                const Text(
                  'Clean City',
                  style: TextStyle(
                    fontSize: 26,
                    fontWeight: FontWeight.w900,
                    color: Color(0xFF111111),
                  ),
                ),

                const SizedBox(height: 6),

                // الشعار العربي
                Text(
                  'نظافة صنعاء',
                  style: TextStyle(
                    fontSize: 14.5,
                    fontWeight: FontWeight.w600,
                    color: const Color(0xFF57D39D)
                        .withValues(alpha: 0.9),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}