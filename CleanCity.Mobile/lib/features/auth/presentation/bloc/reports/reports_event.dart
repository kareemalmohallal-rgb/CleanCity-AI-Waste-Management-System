// part of 'reports_bloc.dart';

// sealed class ReportsEvent extends Equatable {
//   const ReportsEvent();

//   @override
//   List<Object?> get props => [];
// }

// class LoadReports extends ReportsEvent {
//   const LoadReports();
// }

// class CreateReport extends ReportsEvent {
//   final CreateReportRequest request;

//   const CreateReport(this.request);

//   @override
//   List<Object?> get props => [request];
// }

// class ChangeReportStatus extends ReportsEvent {
//   final String reportId;
//   final String status;

//   const ChangeReportStatus({required this.reportId, required this.status});

//   @override
//   List<Object?> get props => [reportId, status];
// }

// class AcceptAssignedReport extends ReportsEvent {
//   final String reportId;

//   const AcceptAssignedReport({required this.reportId});

//   @override
//   List<Object?> get props => [reportId];
// }

// class RejectAssignedReport extends ReportsEvent {
//   final String reportId;
//   final String? rejectionReason;

//   const RejectAssignedReport({required this.reportId, this.rejectionReason});

//   @override
//   List<Object?> get props => [reportId, rejectionReason];
// }

// class CompleteDriverExecution extends ReportsEvent {
//   final String reportId;
//   final int finalStatus;

//   const CompleteDriverExecution({
//     required this.reportId,
//     required this.finalStatus,
//   });

//   @override
//   List<Object?> get props => [reportId, finalStatus];
// }

// class ClearReportSubmissionState extends ReportsEvent {
//   const ClearReportSubmissionState();
// }
part of 'reports_bloc.dart';

sealed class ReportsEvent extends Equatable {
  const ReportsEvent();

  @override
  List<Object?> get props => [];
}

class LoadReports extends ReportsEvent {
  const LoadReports();
}

class CreateReport extends ReportsEvent {
  final CreateReportRequest request;
  const CreateReport(this.request);

  @override
  List<Object?> get props => [request];
}

class ChangeReportStatus extends ReportsEvent {
  final String reportId;
  final String status;
  const ChangeReportStatus({required this.reportId, required this.status});

  @override
  List<Object?> get props => [reportId, status];
}

class AcceptAssignedReport extends ReportsEvent {
  final String reportId;
  const AcceptAssignedReport({required this.reportId});

  @override
  List<Object?> get props => [reportId];
}

class RejectAssignedReport extends ReportsEvent {
  final String reportId;
  final String? rejectionReason;
  const RejectAssignedReport({required this.reportId, this.rejectionReason});

  @override
  List<Object?> get props => [reportId, rejectionReason];
}

class CompleteDriverExecution extends ReportsEvent {
  final String reportId;
  final int finalStatus;
  const CompleteDriverExecution({
    required this.reportId,
    required this.finalStatus,
  });

  @override
  List<Object?> get props => [reportId, finalStatus];
}

class ClearReportSubmissionState extends ReportsEvent {
  const ClearReportSubmissionState();
}

class SyncPendingReports extends ReportsEvent {
  const SyncPendingReports();
}