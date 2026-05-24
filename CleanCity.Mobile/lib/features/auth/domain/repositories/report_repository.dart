// import '../../../../core/result/app_result.dart';
// import '../entities/create_report_request.dart';
// import '../entities/report_entity.dart';

// abstract class ReportRepository {
//   Future<AppResult<List<ReportEntity>>> getReports();

//   Future<AppResult<int>> submitReport(CreateReportRequest request);

//   Future<AppResult<void>> updateStatus({
//     required String reportId,
//     required String status,
//   });

//   Future<AppResult<void>> driverAcceptReport({required String reportId});

//   Future<AppResult<void>> driverRejectReport({
//     required String reportId,
//     String? rejectionReason,
//   });
//   Future<AppResult<void>> driverCompleteExecution({
//     required String reportId,
//     required int finalStatus,
//   });
// }
import '../../../../core/result/app_result.dart';
import '../entities/create_report_request.dart';
import '../entities/report_entity.dart';

abstract class ReportRepository {
  Future<AppResult<List<ReportEntity>>> getReports();
  Future<AppResult<int>> submitReport(CreateReportRequest request);
  Future<AppResult<void>> updateStatus({
    required String reportId,
    required String status,
  });
  Future<AppResult<void>> driverAcceptReport({required String reportId});
  Future<AppResult<void>> driverRejectReport({
    required String reportId,
    String? rejectionReason,
  });
  Future<AppResult<void>> driverCompleteExecution({
    required String reportId,
    required int finalStatus,
  });
  Future<AppResult<int>> syncPendingReports();
}