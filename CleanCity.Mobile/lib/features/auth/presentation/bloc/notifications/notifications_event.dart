part of 'notifications_bloc.dart';

sealed class NotificationsEvent extends Equatable {
  const NotificationsEvent();
  @override
  List<Object?> get props => [];
}

class LoadNotifications extends NotificationsEvent {
  const LoadNotifications();
}

class MarkNotificationRead extends NotificationsEvent {
  final String id;
  const MarkNotificationRead(this.id);

  @override
  List<Object?> get props => [id];
}
