import 'package:cleancityapp/features/auth/domain/entities/awareness_content_entity.dart';

abstract class AwarenessRepository {
  Future<List<AwarenessContent>> getAllAwarenessContents();
  Future<AwarenessContent> getAwarenessContentById(int id);
}
