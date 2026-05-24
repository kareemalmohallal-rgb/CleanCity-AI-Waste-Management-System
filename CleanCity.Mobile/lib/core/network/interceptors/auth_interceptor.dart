import 'package:dio/dio.dart';
import '../../../features/auth/data/datasources/auth_local_data_source.dart';

class AuthInterceptor extends Interceptor {
  final AuthLocalDataSource local;

  AuthInterceptor(this.local);

  @override
  void onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    final token = await local.getToken();

    if (token != null && token.isNotEmpty) {
      options.headers['Authorization'] = 'Bearer $token';
    }

    super.onRequest(options, handler);
  }
}
