import '../entities/anonymous_device_entity.dart';
import '../entities/register_anonymous_device_request.dart';

abstract class AnonymousDeviceRepository {
  Future<AnonymousDeviceEntity> registerAnonymousDevice(
    RegisterAnonymousDeviceRequest request,
  );

  Future<String> getOrCreateDeviceIdentifier();
  Future<void> saveAnonymousDeviceId(int id);
  Future<int?> getSavedAnonymousDeviceId();

  Future<void> saveFcmToken(String token);
  Future<String?> getSavedFcmToken();
}
