class AnonymousDeviceEntity {
  final int id;
  final String deviceIdentifier;
  final String fcmToken;

  const AnonymousDeviceEntity({
    required this.id,
    required this.deviceIdentifier,
    required this.fcmToken,
  });
}
