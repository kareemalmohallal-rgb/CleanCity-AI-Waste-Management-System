import 'package:equatable/equatable.dart';

class NotificationEntity extends Equatable {
  final String id;
  final String title;
  final String details;
  final DateTime createdAt;
  final bool isRead;
  final String? reportId;

  const NotificationEntity({
    required this.id,
    required this.title,
    required this.details,
    required this.createdAt,
    required this.isRead,
    this.reportId,
  });

  @override
  List<Object?> get props => [id, title, details, createdAt, isRead, reportId];
}
