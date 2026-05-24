import '../../../../core/result/app_result.dart';
import '../../../../core/usecase/usecase.dart';
import '../entities/report_entity.dart';
import '../repositories/report_repository.dart';

class GetReports implements UseCase<AppResult<List<ReportEntity>>, NoParams> {
  final ReportRepository repo;
  GetReports(this.repo);

  @override
  Future<AppResult<List<ReportEntity>>> call(NoParams params) => repo.getReports();
}