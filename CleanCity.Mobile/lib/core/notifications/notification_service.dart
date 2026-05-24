import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';

@pragma('vm:entry-point')
Future<void> firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  debugPrint('BG MESSAGE => ${message.messageId}');
}

class NotificationService {
  NotificationService._();
  static final NotificationService instance = NotificationService._();

  final FirebaseMessaging _messaging = FirebaseMessaging.instance;
  final FlutterLocalNotificationsPlugin _localNotifications =
      FlutterLocalNotificationsPlugin();

  static const AndroidNotificationChannel _channel = AndroidNotificationChannel(
    'high_importance_channel',
    'High Importance Notifications',
    description: 'Used for important notifications.',
    importance: Importance.max,
  );

  bool _initialized = false;

  void Function()? onForegroundNotificationReceived;

  Future<void> initialize() async {
    if (_initialized) return;

    await _messaging.requestPermission(alert: true, badge: true, sound: true);

    const initSettings = InitializationSettings(
      android: AndroidInitializationSettings('@mipmap/ic_launcher'),
      iOS: DarwinInitializationSettings(),
    );

    await _localNotifications.initialize(settings: initSettings);

    // ✅ السطر الصحيح — اكتبه في سطر واحد لتجنب مشكلة النسخ
    final androidImpl = _localNotifications.resolvePlatformSpecificImplementation<AndroidFlutterLocalNotificationsPlugin>();
    await androidImpl?.createNotificationChannel(_channel);

    await _messaging.setForegroundNotificationPresentationOptions(
      alert: true,
      badge: true,
      sound: true,
    );

    FirebaseMessaging.onMessage.listen((RemoteMessage message) async {
      final notification = message.notification;
      if (notification == null) return;

      // ✅ flutter_local_notifications 18+ يستخدم named parameters
      await _localNotifications.show(
        id: notification.hashCode,
        title: notification.title,
        body: notification.body,
        notificationDetails: const NotificationDetails(
          android: AndroidNotificationDetails(
            'high_importance_channel',
            'High Importance Notifications',
            channelDescription: 'Used for important notifications.',
            importance: Importance.max,
            priority: Priority.high,
            icon: '@mipmap/ic_launcher',
          ),
          iOS: DarwinNotificationDetails(),
        ),
      );

      onForegroundNotificationReceived?.call();
    });

    FirebaseMessaging.onMessageOpenedApp.listen((RemoteMessage message) {
      debugPrint('NOTIFICATION OPENED => ${message.data}');
      onForegroundNotificationReceived?.call();
    });

    // ✅ يُعيَّن هنا فقط بعد نجاح جميع الخطوات
    _initialized = true;
  }

  Future<String?> getToken() async {
    return _messaging.getToken();
  }

  Stream<String> get onTokenRefresh => _messaging.onTokenRefresh;

  Future<void> subscribeToAwarenessTopic() async {
    await _messaging.subscribeToTopic('awareness_all');
  }
}