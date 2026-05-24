// part of 'reports_bloc.dart';

// class ReportsState extends Equatable {
//   final bool loading;
//   final bool actionLoading;
//   final List<ReportEntity> reports;
//   final String? error;
//   final int? submittedReportId;

//   const ReportsState({
//     required this.loading,
//     required this.actionLoading,
//     required this.reports,
//     required this.error,
//     required this.submittedReportId,
//   });

//   const ReportsState.initial()
//     : loading = false,
//       actionLoading = false,
//       reports = const [],
//       error = null,
//       submittedReportId = null;

//   ReportsState copyWith({
//     bool? loading,
//     bool? actionLoading,
//     List<ReportEntity>? reports,
//     String? error,
//     int? submittedReportId,
//     bool clearError = false,
//     bool clearSubmittedReportId = false,
//   }) {
//     return ReportsState(
//       loading: loading ?? this.loading,
//       actionLoading: actionLoading ?? this.actionLoading,
//       reports: reports ?? this.reports,
//       error: clearError ? null : (error ?? this.error),
//       submittedReportId: clearSubmittedReportId
//           ? null
//           : (submittedReportId ?? this.submittedReportId),
//     );
//   }

//   @override
//   List<Object?> get props => [
//     loading,
//     actionLoading,
//     reports,
//     error,
//     submittedReportId,
//   ];
// }
part of 'reports_bloc.dart';

class ReportsState extends Equatable {
  final bool loading;
  final bool actionLoading;
  final List<ReportEntity> reports;
  final String? error;
  final int? submittedReportId;
  final bool savedLocally;

  const ReportsState({
    required this.loading,
    required this.actionLoading,
    required this.reports,
    required this.error,
    required this.submittedReportId,
    required this.savedLocally,
  });

  const ReportsState.initial()
      : loading = false,
        actionLoading = false,
        reports = const [],
        error = null,
        submittedReportId = null,
        savedLocally = false;

  ReportsState copyWith({
    bool? loading,
    bool? actionLoading,
    List<ReportEntity>? reports,
    String? error,
    int? submittedReportId,
    bool? savedLocally,
    bool clearError = false,
    bool clearSubmittedReportId = false,
    bool clearSavedLocally = false,
  }) {
    return ReportsState(
      loading: loading ?? this.loading,
      actionLoading: actionLoading ?? this.actionLoading,
      reports: reports ?? this.reports,
      error: clearError ? null : (error ?? this.error),
      submittedReportId: clearSubmittedReportId
          ? null
          : (submittedReportId ?? this.submittedReportId),
      savedLocally:
          clearSavedLocally ? false : (savedLocally ?? this.savedLocally),
    );
  }

  @override
  List<Object?> get props => [
        loading,
        actionLoading,
        reports,
        error,
        submittedReportId,
        savedLocally,
      ];
}