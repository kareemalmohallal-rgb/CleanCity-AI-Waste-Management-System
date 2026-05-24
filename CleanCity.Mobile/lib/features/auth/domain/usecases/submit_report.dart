import '../../../../core/result/app_result.dart';
import '../entities/create_report_request.dart';
import '../repositories/report_repository.dart';

class SubmitReport {
  final ReportRepository repository;

  SubmitReport(this.repository);

  Future<AppResult<int>> call(CreateReportRequest request) {
    return repository.submitReport(request);
  }
}
