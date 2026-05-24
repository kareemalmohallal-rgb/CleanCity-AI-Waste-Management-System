import 'package:cleancityapp/features/auth/domain/entities/awareness_content_entity.dart';
import '../../repositories/awareness_repository.dart';

class GetAwarenessContentsUseCase {
  final AwarenessRepository repository;

  GetAwarenessContentsUseCase(this.repository);

  Future<List<AwarenessContent>> call() {
    return repository.getAllAwarenessContents();
  }
}
