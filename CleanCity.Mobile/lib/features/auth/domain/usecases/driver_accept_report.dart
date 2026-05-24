import '../../../../core/result/app_result.dart';
import '../repositories/report_repository.dart';

class DriverAcceptReport {
  final ReportRepository repository;

  DriverAcceptReport(this.repository);

  Future<AppResult<void>> call({required String reportId}) {
    return repository.driverAcceptReport(reportId: reportId);
  }
}
