import 'package:cleancityapp/features/auth/domain/repositories/anonymous_device_repository.dart';

class GetOrCreateDeviceIdentifierUseCase {
  final AnonymousDeviceRepository repository;

  GetOrCreateDeviceIdentifierUseCase(this.repository);

  Future<String> call() {
    return repository.getOrCreateDeviceIdentifier();
  }
}
