import 'package:cleancityapp/features/auth/domain/repositories/anonymous_device_repository.dart';

class GetSavedAnonymousDeviceIdUseCase {
  final AnonymousDeviceRepository repository;

  GetSavedAnonymousDeviceIdUseCase(this.repository);

  Future<int?> call() {
    return repository.getSavedAnonymousDeviceId();
  }
}
