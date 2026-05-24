import 'package:cleancityapp/core/result/app_result.dart';
import 'package:cleancityapp/features/auth/domain/repositories/report_repository.dart';

class DriverCompleteExecution {
  final ReportRepository repository;

  DriverCompleteExecution(this.repository);

  Future<AppResult<void>> call({
    required String reportId,
    required int finalStatus,
  }) {
    return repository.driverCompleteExecution(
      reportId: reportId,
      finalStatus: finalStatus,
    );
  }
}
