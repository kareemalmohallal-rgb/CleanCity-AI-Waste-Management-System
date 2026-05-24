import '../../../../core/result/app_result.dart';
import '../../../../core/usecase/usecase.dart';
import '../repositories/report_repository.dart';

class UpdateReportStatusParams {
  final String reportId;
  final String status;
  UpdateReportStatusParams({required this.reportId, required this.status});
}

class UpdateReportStatus implements UseCase<AppResult<void>, UpdateReportStatusParams> {
  final ReportRepository repo;
  UpdateReportStatus(this.repo);

  @override
  Future<AppResult<void>> call(UpdateReportStatusParams params) {
    return repo.updateStatus(reportId: params.reportId, status: params.status);
  }
}
