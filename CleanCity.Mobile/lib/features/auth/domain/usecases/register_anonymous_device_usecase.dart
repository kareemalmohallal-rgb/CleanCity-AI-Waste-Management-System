import 'package:cleancityapp/features/auth/domain/entities/anonymous_device_entity.dart';
import 'package:cleancityapp/features/auth/domain/entities/register_anonymous_device_request.dart';
import 'package:cleancityapp/features/auth/domain/repositories/anonymous_device_repository.dart';

class RegisterAnonymousDeviceUseCase {
  final AnonymousDeviceRepository repository;

  RegisterAnonymousDeviceUseCase(this.repository);

  Future<AnonymousDeviceEntity> call(RegisterAnonymousDeviceRequest request) {
    return repository.registerAnonymousDevice(request);
  }
}
