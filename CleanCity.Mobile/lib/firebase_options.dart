// استيراد FirebaseOptions فقط من firebase_core
import 'package:firebase_core/firebase_core.dart' show FirebaseOptions;

// استيراد معلومات المنصة الحالية
import 'package:flutter/foundation.dart'
    show defaultTargetPlatform, kIsWeb, TargetPlatform;

/// هذا الكلاس يحتوي إعدادات Firebase لكل منصة
class DefaultFirebaseOptions {

  // ==================== اختيار إعدادات المنصة الحالية ====================
  static FirebaseOptions get currentPlatform {

    // إذا التطبيق يعمل على الويب
    if (kIsWeb) {
      return web;
    }

    // اختيار الإعدادات حسب نوع الجهاز
    switch (defaultTargetPlatform) {

      // إعدادات Android
      case TargetPlatform.android:
        return android;

      // إعدادات iPhone
      case TargetPlatform.iOS:
        return ios;

      // إعدادات macOS
      case TargetPlatform.macOS:
        return macos;

      // إعدادات Windows
      case TargetPlatform.windows:
        return windows;

      // Linux غير مدعوم حاليًا
      case TargetPlatform.linux:
        throw UnsupportedError(
          'Firebase غير مهيأ لنظام Linux. '
          'يمكنك إعادة التهيئة باستخدام FlutterFire CLI.',
        );

      // أي منصة غير معروفة
      default:
        throw UnsupportedError(
          'Firebase غير مدعوم لهذه المنصة.',
        );
    }
  }


  // ==================== إعدادات WEB ====================
  static const FirebaseOptions web = FirebaseOptions(
    apiKey: 'YOUR_WEB_API_KEY',
    appId: 'YOUR_WEB_APP_ID',
    messagingSenderId: '31533410717',
    projectId: 'clean-city-ye-2026-acdff',
    authDomain: 'clean-city-ye-2026-acdff.firebaseapp.com',
    storageBucket: 'clean-city-ye-2026-acdff.firebasestorage.app',
  );


  // ==================== إعدادات ANDROID ====================
  static const FirebaseOptions android = FirebaseOptions(
    apiKey: 'YOUR_ANDROID_API_KEY',
    appId: 'YOUR_ANDROID_APP_ID',
    messagingSenderId: '31533410717',
    projectId: 'clean-city-ye-2026-acdff',
    storageBucket: 'clean-city-ye-2026-acdff.firebasestorage.app',
  );


  // ==================== إعدادات IOS ====================
  static const FirebaseOptions ios = FirebaseOptions(
    apiKey: 'YOUR_IOS_API_KEY',
    appId: 'YOUR_IOS_APP_ID',
    messagingSenderId: '31533410717',
    projectId: 'clean-city-ye-2026-acdff',
    storageBucket: 'clean-city-ye-2026-acdff.firebasestorage.app',

    // معرف التطبيق في iOS
    iosBundleId: 'com.example.cleancityapp',
  );


  // ==================== إعدادات MAC ====================
  static const FirebaseOptions macos = FirebaseOptions(
    apiKey: 'YOUR_MAC_API_KEY',
    appId: 'YOUR_MAC_APP_ID',
    messagingSenderId: '31533410717',
    projectId: 'clean-city-ye-2026-acdff',
    storageBucket: 'clean-city-ye-2026-acdff.firebasestorage.app',
    iosBundleId: 'com.example.cleancityapp',
  );


  // ==================== إعدادات WINDOWS ====================
  static const FirebaseOptions windows = FirebaseOptions(
    apiKey: 'YOUR_WINDOWS_API_KEY',
    appId: 'YOUR_WINDOWS_APP_ID',
    messagingSenderId: '31533410717',
    projectId: 'clean-city-ye-2026-acdff',
    authDomain: 'clean-city-ye-2026-acdff.firebaseapp.com',
    storageBucket: 'clean-city-ye-2026-acdff.firebasestorage.app',
  );
}