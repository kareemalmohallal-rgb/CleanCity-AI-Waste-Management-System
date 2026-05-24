import '../../features/auth/data/datasources/auth_local_data_source.dart';
import 'api_client.dart';

class ApiFactory {
  ApiFactory._();

  static final AuthLocalDataSource _local = AuthLocalDataSource();
  static final ApiClient api = ApiClient(_local);
}
