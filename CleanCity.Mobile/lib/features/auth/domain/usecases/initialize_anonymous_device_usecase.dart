// import 'dart:async';

// import 'package:cleancityapp/features/auth/domain/entities/register_anonymous_device_request.dart';
// import 'package:cleancityapp/features/auth/domain/repositories/anonymous_device_repository.dart';
// import 'package:firebase_messaging/firebase_messaging.dart';

// import 'register_anonymous_device_usecase.dart';

// class InitializeAnonymousDeviceUseCase {
//   final RegisterAnonymousDeviceUseCase registerAnonymousDeviceUseCase;
//   final AnonymousDeviceRepository repository;

//   StreamSubscription<String>? _tokenRefreshSubscription;

//   InitializeAnonymousDeviceUseCase({
//     required this.registerAnonymousDeviceUseCase,
//     required this.repository,
//   });

//   Future<void> call() async {
//     final messaging = FirebaseMessaging.instance;

//     await messaging.requestPermission();

//     final deviceIdentifier = await repository.getOrCreateDeviceIdentifier();
//     final token = await messaging.getToken();

//     if (token != null && token.trim().isNotEmpty) {
//       await registerAnonymousDeviceUseCase(
//         RegisterAnonymousDeviceRequest(
//           deviceIdentifier: deviceIdentifier,
//           fcmToken: token,
//         ),
//       );
//     }

//     _tokenRefreshSubscription?.cancel();
//     _tokenRefreshSubscription = messaging.onTokenRefresh.listen((
//       newToken,
//     ) async {
//       if (newToken.trim().isEmpty) return;

//       final stableDeviceIdentifier = await repository
//           .getOrCreateDeviceIdentifier();

//       await registerAnonymousDeviceUseCase(
//         RegisterAnonymousDeviceRequest(
//           deviceIdentifier: stableDeviceIdentifier,
//           fcmToken: newToken,
//         ),
//       );
//     });
//   }

//   Future<void> dispose() async {
//     await _tokenRefreshSubscription?.cancel();
//   }
// }
import 'dart:async';

import 'package:cleancityapp/features/auth/domain/entities/register_anonymous_device_request.dart';
import 'package:cleancityapp/features/auth/domain/repositories/anonymous_device_repository.dart';
import 'package:firebase_messaging/firebase_messaging.dart';

import 'register_anonymous_device_usecase.dart';

class InitializeAnonymousDeviceUseCase {
  final RegisterAnonymousDeviceUseCase registerAnonymousDeviceUseCase;
  final AnonymousDeviceRepository repository;

  StreamSubscription<String>? _tokenRefreshSubscription;

  InitializeAnonymousDeviceUseCase({
    required this.registerAnonymousDeviceUseCase,
    required this.repository,
  });

  Future<void> call() async {
    final messaging = FirebaseMessaging.instance;

    // ✅ حُذف requestPermission() — تم استدعاؤه مسبقاً في NotificationService.initialize()

    final deviceIdentifier = await repository.getOrCreateDeviceIdentifier();
    final token = await messaging.getToken();

    if (token != null && token.trim().isNotEmpty) {
      await registerAnonymousDeviceUseCase(
        RegisterAnonymousDeviceRequest(
          deviceIdentifier: deviceIdentifier,
          fcmToken: token,
        ),
      );
    }

    _tokenRefreshSubscription?.cancel();
    _tokenRefreshSubscription = messaging.onTokenRefresh.listen((
      newToken,
    ) async {
      if (newToken.trim().isEmpty) return;

      final stableDeviceIdentifier = await repository
          .getOrCreateDeviceIdentifier();

      await registerAnonymousDeviceUseCase(
        RegisterAnonymousDeviceRequest(
          deviceIdentifier: stableDeviceIdentifier,
          fcmToken: newToken,
        ),
      );
    });
  }

  Future<void> dispose() async {
    await _tokenRefreshSubscription?.cancel();
  }
}
