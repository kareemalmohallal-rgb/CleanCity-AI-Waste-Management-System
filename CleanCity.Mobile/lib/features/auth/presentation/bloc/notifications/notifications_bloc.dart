import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../domain/entities/notification_entity.dart';
import '../../../domain/repositories/notification_repository.dart';

part 'notifications_event.dart';
part 'notifications_state.dart';

class NotificationsBloc extends Bloc<NotificationsEvent, NotificationsState> {
  final NotificationRepository repo;

  NotificationsBloc(this.repo) : super(const NotificationsState.initial()) {
    on<LoadNotifications>(_onLoad);
    on<MarkNotificationRead>(_onMarkRead);
  }

  Future<void> _onLoad(LoadNotifications event, Emitter<NotificationsState> emit) async {
    emit(state.copyWith(loading: true, error: null));
    final res = await repo.getNotifications();
    res.fold(
      (f) => emit(state.copyWith(loading: false, error: f.message)),
      (list) => emit(state.copyWith(loading: false, items: list, error: null)),
    );
  }

  Future<void> _onMarkRead(MarkNotificationRead event, Emitter<NotificationsState> emit) async {
    final res = await repo.markRead(event.id);
    res.fold(
      (f) => emit(state.copyWith(error: f.message)),
      (_) => add(const LoadNotifications()),
    );
  }
}
