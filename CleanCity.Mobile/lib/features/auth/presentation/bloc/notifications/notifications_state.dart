part of 'notifications_bloc.dart';

class NotificationsState extends Equatable {
  final bool loading;
  final List<NotificationEntity> items;
  final String? error;

  const NotificationsState({required this.loading, required this.items, required this.error});

  const NotificationsState.initial()
      : loading = false,
        items = const [],
        error = null;

  NotificationsState copyWith({bool? loading, List<NotificationEntity>? items, String? error}) {
    return NotificationsState(
      loading: loading ?? this.loading,
      items: items ?? this.items,
      error: error,
    );
  }

  @override
  List<Object?> get props => [loading, items, error];
}
