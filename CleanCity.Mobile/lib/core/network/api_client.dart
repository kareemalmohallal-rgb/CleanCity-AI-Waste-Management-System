import 'package:cleancityapp/core/network/interceptors/auth_interceptor.dart';
import 'package:dio/dio.dart';
import 'api_config.dart';
import '../../features/auth/data/datasources/auth_local_data_source.dart';

class ApiClient {
  final Dio dio;

  ApiClient(AuthLocalDataSource local)
    : dio = Dio(
        BaseOptions(
          baseUrl: ApiConfig.baseUrl,
          connectTimeout: const Duration(seconds: 15),
          receiveTimeout: const Duration(seconds: 20),
          headers: {'Content-Type': 'application/json'},
        ),
      ) {
    // ✅ يضيف Authorization تلقائياً من SecureStorage
    dio.interceptors.add(AuthInterceptor(local));

    // ✅ Debug: اطبع الهيدر النهائي
    dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) {
          // ignore: avoid_print
          print('FINAL HEADERS => ${options.headers}');
          handler.next(options);
        },
      ),
    );
  }

  Future<Response<dynamic>> get(
    String path, {
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) {
    return dio.get(path, queryParameters: queryParameters, options: options);
  }

  Future<Response<dynamic>> post(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) {
    return dio.post(
      path,
      data: data,
      queryParameters: queryParameters,
      options: options,
    );
  }

  Future<Response<dynamic>> put(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) {
    return dio.put(
      path,
      data: data,
      queryParameters: queryParameters,
      options: options,
    );
  }

  Future<Response<dynamic>> delete(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) {
    return dio.delete(
      path,
      data: data,
      queryParameters: queryParameters,
      options: options,
    );
  }

  // (اختياري للتوافق)
  void setBearerToken(String token) =>
      dio.options.headers['Authorization'] = 'Bearer $token';
  void clearBearerToken() => dio.options.headers.remove('Authorization');
}
