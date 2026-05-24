// import 'package:cleancityapp/features/auth/domain/usecases/awareness/driver_complete_execution.dart';
// import 'package:equatable/equatable.dart';
// import 'package:flutter_bloc/flutter_bloc.dart';

// import '../../../../../core/usecase/usecase.dart';
// import '../../../domain/entities/create_report_request.dart';
// import '../../../domain/entities/report_entity.dart';
// import '../../../domain/usecases/driver_accept_report.dart';
// import '../../../domain/usecases/driver_reject_report.dart';
// import '../../../domain/usecases/get_reports.dart';
// import '../../../domain/usecases/submit_report.dart';
// import '../../../domain/usecases/update_report_status.dart';

// part 'reports_event.dart';
// part 'reports_state.dart';

// class ReportsBloc extends Bloc<ReportsEvent, ReportsState> {
//   final GetReports getReports;
//   final SubmitReport submitReport;
//   final UpdateReportStatus updateStatus;
//   final DriverAcceptReport driverAcceptReport;
//   final DriverRejectReport driverRejectReport;
//   final DriverCompleteExecution driverCompleteExecution;
//   ReportsBloc({
//     required this.getReports,
//     required this.submitReport,
//     required this.updateStatus,
//     required this.driverAcceptReport,
//     required this.driverRejectReport,
//     required this.driverCompleteExecution,
//   }) : super(const ReportsState.initial()) {
//     on<LoadReports>(_onLoad);
//     on<CreateReport>(_onCreate);
//     on<ChangeReportStatus>(_onChangeStatus);
//     on<AcceptAssignedReport>(_onAcceptAssignedReport);
//     on<RejectAssignedReport>(_onRejectAssignedReport);
//     on<CompleteDriverExecution>(_onCompleteDriverExecution);
//     on<ClearReportSubmissionState>(_onClearSubmissionState);
//   }

//   Future<void> _onLoad(LoadReports event, Emitter<ReportsState> emit) async {
//     emit(state.copyWith(loading: true, clearError: true));

//     final res = await getReports(const NoParams());

//     res.fold(
//       (f) => emit(state.copyWith(loading: false, error: f.message)),
//       (list) =>
//           emit(state.copyWith(loading: false, reports: list, clearError: true)),
//     );
//   }

//   Future<void> _onCreate(CreateReport event, Emitter<ReportsState> emit) async {
//     emit(
//       state.copyWith(
//         actionLoading: true,
//         clearError: true,
//         clearSubmittedReportId: true,
//       ),
//     );

//     final res = await submitReport(event.request);

//     res.fold(
//       (f) => emit(state.copyWith(actionLoading: false, error: f.message)),
//       (reportId) => emit(
//         state.copyWith(
//           actionLoading: false,
//           submittedReportId: reportId,
//           clearError: true,
//         ),
//       ),
//     );
//   }

//   Future<void> _onChangeStatus(
//     ChangeReportStatus event,
//     Emitter<ReportsState> emit,
//   ) async {
//     emit(state.copyWith(actionLoading: true, clearError: true));

//     final res = await updateStatus(
//       UpdateReportStatusParams(reportId: event.reportId, status: event.status),
//     );

//     await res.fold(
//       (f) async => emit(state.copyWith(actionLoading: false, error: f.message)),
//       (_) async {
//         emit(state.copyWith(actionLoading: false));
//         add(const LoadReports());
//       },
//     );
//   }

//   Future<void> _onAcceptAssignedReport(
//     AcceptAssignedReport event,
//     Emitter<ReportsState> emit,
//   ) async {
//     emit(state.copyWith(actionLoading: true, clearError: true));

//     final res = await driverAcceptReport(reportId: event.reportId);

//     await res.fold(
//       (f) async => emit(state.copyWith(actionLoading: false, error: f.message)),
//       (_) async {
//         emit(state.copyWith(actionLoading: false));
//         add(const LoadReports());
//       },
//     );
//   }

//   Future<void> _onRejectAssignedReport(
//     RejectAssignedReport event,
//     Emitter<ReportsState> emit,
//   ) async {
//     emit(state.copyWith(actionLoading: true, clearError: true));

//     final res = await driverRejectReport(
//       reportId: event.reportId,
//       rejectionReason: event.rejectionReason,
//     );

//     await res.fold(
//       (f) async => emit(state.copyWith(actionLoading: false, error: f.message)),
//       (_) async {
//         emit(state.copyWith(actionLoading: false));
//         add(const LoadReports());
//       },
//     );
//   }

//   Future<void> _onCompleteDriverExecution(
//     CompleteDriverExecution event,
//     Emitter<ReportsState> emit,
//   ) async {
//     emit(state.copyWith(actionLoading: true, clearError: true));

//     final res = await driverCompleteExecution(
//       reportId: event.reportId,
//       finalStatus: event.finalStatus,
//     );

//     await res.fold(
//       (f) async => emit(state.copyWith(actionLoading: false, error: f.message)),
//       (_) async {
//         emit(state.copyWith(actionLoading: false));
//         add(const LoadReports());
//       },
//     );
//   }

//   void _onClearSubmissionState(
//     ClearReportSubmissionState event,
//     Emitter<ReportsState> emit,
//   ) {
//     emit(state.copyWith(clearSubmittedReportId: true, clearError: true));
//   }
// }

import 'dart:async';

import 'package:cleancityapp/features/auth/domain/usecases/awareness/driver_complete_execution.dart';
import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../../core/usecase/usecase.dart';
import '../../../data/repositories/report_repository_impl.dart';
import '../../../domain/entities/create_report_request.dart';
import '../../../domain/entities/report_entity.dart';
import '../../../domain/repositories/report_repository.dart';
import '../../../domain/usecases/driver_accept_report.dart';
import '../../../domain/usecases/driver_reject_report.dart';
import '../../../domain/usecases/get_reports.dart';
import '../../../domain/usecases/submit_report.dart';
import '../../../domain/usecases/update_report_status.dart';

part 'reports_event.dart';
part 'reports_state.dart';

class ReportsBloc extends Bloc<ReportsEvent, ReportsState> {
  final GetReports getReports;
  final SubmitReport submitReport;
  final UpdateReportStatus updateStatus;
  final DriverAcceptReport driverAcceptReport;
  final DriverRejectReport driverRejectReport;
  final DriverCompleteExecution driverCompleteExecution;
  final ReportRepository reportRepository;

  // StreamSubscription<List<ConnectivityResult>>? _connectivitySub;

  StreamSubscription<ConnectivityResult>? _connectivitySub;
  ReportsBloc({
    required this.getReports,
    required this.submitReport,
    required this.updateStatus,
    required this.driverAcceptReport,
    required this.driverRejectReport,
    required this.driverCompleteExecution,
    required this.reportRepository,
  }) : super(const ReportsState.initial()) {
    on<LoadReports>(_onLoad);
    on<CreateReport>(_onCreate);
    on<ChangeReportStatus>(_onChangeStatus);
    on<AcceptAssignedReport>(_onAcceptAssignedReport);
    on<RejectAssignedReport>(_onRejectAssignedReport);
    on<CompleteDriverExecution>(_onCompleteDriverExecution);
    on<ClearReportSubmissionState>(_onClearSubmissionState);
    on<SyncPendingReports>(_onSync);

    // ✅ مزامنة تلقائية عند عودة الاتصال
    // _connectivitySub = Connectivity().onConnectivityChanged.listen((results) {
    //   final hasConnection = results.any((r) => r != ConnectivityResult.none);
    //   if (hasConnection) add(const SyncPendingReports());
    // });

    _connectivitySub = Connectivity().onConnectivityChanged.listen((result) {
      final hasConnection = result != ConnectivityResult.none;
      if (hasConnection) add(const SyncPendingReports());
    });
  }

  @override
  Future<void> close() async {
    await _connectivitySub?.cancel();
    return super.close();
  }

  Future<void> _onLoad(LoadReports event, Emitter<ReportsState> emit) async {
    emit(state.copyWith(loading: true, clearError: true));
    final res = await getReports(const NoParams());
    res.fold(
      (f) => emit(state.copyWith(loading: false, error: f.message)),
      (list) =>
          emit(state.copyWith(loading: false, reports: list, clearError: true)),
    );
  }

  Future<void> _onCreate(CreateReport event, Emitter<ReportsState> emit) async {
    // ✅ منع الإرسال المزدوج
    if (state.actionLoading) return;

    emit(
      state.copyWith(
        actionLoading: true,
        clearError: true,
        clearSubmittedReportId: true,
        clearSavedLocally: true,
      ),
    );

    final res = await submitReport(event.request);

    res.fold(
      (f) => emit(state.copyWith(actionLoading: false, error: f.message)),
      (reportId) {
        if (reportId == kSavedLocally) {
          emit(
            state.copyWith(
              actionLoading: false,
              savedLocally: true,
              clearError: true,
            ),
          );
        } else {
          emit(
            state.copyWith(
              actionLoading: false,
              submittedReportId: reportId,
              clearError: true,
            ),
          );
        }
      },
    );
  }

  Future<void> _onSync(
    SyncPendingReports event,
    Emitter<ReportsState> emit,
  ) async {
    final res = await reportRepository.syncPendingReports();
    res.fold((_) => null, (count) {
      if (count > 0) add(const LoadReports());
    });
  }

  Future<void> _onChangeStatus(
    ChangeReportStatus event,
    Emitter<ReportsState> emit,
  ) async {
    emit(state.copyWith(actionLoading: true, clearError: true));
    final res = await updateStatus(
      UpdateReportStatusParams(reportId: event.reportId, status: event.status),
    );
    await res.fold(
      (f) async => emit(state.copyWith(actionLoading: false, error: f.message)),
      (_) async {
        emit(state.copyWith(actionLoading: false));
        add(const LoadReports());
      },
    );
  }

  Future<void> _onAcceptAssignedReport(
    AcceptAssignedReport event,
    Emitter<ReportsState> emit,
  ) async {
    emit(state.copyWith(actionLoading: true, clearError: true));
    final res = await driverAcceptReport(reportId: event.reportId);
    await res.fold(
      (f) async => emit(state.copyWith(actionLoading: false, error: f.message)),
      (_) async {
        emit(state.copyWith(actionLoading: false));
        add(const LoadReports());
      },
    );
  }

  Future<void> _onRejectAssignedReport(
    RejectAssignedReport event,
    Emitter<ReportsState> emit,
  ) async {
    emit(state.copyWith(actionLoading: true, clearError: true));
    final res = await driverRejectReport(
      reportId: event.reportId,
      rejectionReason: event.rejectionReason,
    );
    await res.fold(
      (f) async => emit(state.copyWith(actionLoading: false, error: f.message)),
      (_) async {
        emit(state.copyWith(actionLoading: false));
        add(const LoadReports());
      },
    );
  }

  Future<void> _onCompleteDriverExecution(
    CompleteDriverExecution event,
    Emitter<ReportsState> emit,
  ) async {
    emit(state.copyWith(actionLoading: true, clearError: true));
    final res = await driverCompleteExecution(
      reportId: event.reportId,
      finalStatus: event.finalStatus,
    );
    await res.fold(
      (f) async => emit(state.copyWith(actionLoading: false, error: f.message)),
      (_) async {
        emit(state.copyWith(actionLoading: false));
        add(const LoadReports());
      },
    );
  }

  void _onClearSubmissionState(
    ClearReportSubmissionState event,
    Emitter<ReportsState> emit,
  ) {
    emit(
      state.copyWith(
        clearSubmittedReportId: true,
        clearError: true,
        clearSavedLocally: true,
      ),
    );
  }
}
