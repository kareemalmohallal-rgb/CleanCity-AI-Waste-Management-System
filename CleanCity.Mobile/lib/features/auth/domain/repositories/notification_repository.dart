import '../../../../core/result/app_result.dart';
import '../entities/notification_entity.dart';

abstract class NotificationRepository {
  Future<AppResult<List<NotificationEntity>>> getNotifications();
  Future<AppResult<void>> markRead(String notificationId);
}
