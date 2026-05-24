import '../../../../core/result/app_result.dart';
import '../repositories/report_repository.dart';

class DriverRejectReport {
  final ReportRepository repository;

  DriverRejectReport(this.repository);

  Future<AppResult<void>> call({
    required String reportId,
    String? rejectionReason,
  }) {
    return repository.driverRejectReport(
      reportId: reportId,
      rejectionReason: rejectionReason,
    );
  }
}
