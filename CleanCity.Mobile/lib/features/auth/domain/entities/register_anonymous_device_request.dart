class RegisterAnonymousDeviceRequest {
  final String deviceIdentifier;
  final String fcmToken;

  const RegisterAnonymousDeviceRequest({
    required this.deviceIdentifier,
    required this.fcmToken,
  });
}
