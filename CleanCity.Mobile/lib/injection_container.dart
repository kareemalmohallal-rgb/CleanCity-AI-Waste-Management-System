// import 'package:cleancityapp/features/auth/data/datasources/anonymous_device_local_data_source.dart';
// import 'package:cleancityapp/features/auth/data/datasources/anonymous_device_remote_data_source.dart';
// import 'package:cleancityapp/features/auth/data/datasources/awareness_remote_data_source.dart';
// import 'package:cleancityapp/features/auth/data/repositories/anonymous_device_repository_impl.dart';
// import 'package:cleancityapp/features/auth/data/repositories/awareness_repository_impl.dart';
// import 'package:cleancityapp/features/auth/domain/repositories/anonymous_device_repository.dart';
// import 'package:cleancityapp/features/auth/domain/repositories/awareness_repository.dart';
// import 'package:cleancityapp/features/auth/domain/usecases/awareness/driver_complete_execution.dart';
// import 'package:cleancityapp/features/auth/domain/usecases/awareness/get_awareness_contents_usecase.dart';
// import 'package:cleancityapp/features/auth/domain/usecases/driver_accept_report.dart';
// import 'package:cleancityapp/features/auth/domain/usecases/driver_reject_report.dart';
// import 'package:cleancityapp/features/auth/domain/usecases/get_or_create_device_identifier_usecase.dart';
// import 'package:cleancityapp/features/auth/domain/usecases/get_saved_anonymous_device_id_usecase.dart';
// import 'package:cleancityapp/features/auth/domain/usecases/initialize_anonymous_device_usecase.dart';
// import 'package:cleancityapp/features/auth/domain/usecases/register_anonymous_device_usecase.dart';
// import 'package:connectivity_plus/connectivity_plus.dart';
// import 'package:get_it/get_it.dart';
// import 'package:shared_preferences/shared_preferences.dart';

// import 'core/network/api_client.dart';
// import 'core/network/api_factory.dart';
// import 'core/network/network_info.dart';

// import 'features/auth/data/datasources/auth_local_data_source.dart';
// import 'features/auth/data/datasources/notification_local_datasource.dart';
// import 'features/auth/data/datasources/report_local_datasource.dart';
// import 'features/auth/data/datasources/report_remote_datasource.dart';

// import 'features/auth/data/repositories/notification_repository_impl.dart';
// import 'features/auth/data/repositories/report_repository_impl.dart';

// import 'features/auth/domain/repositories/notification_repository.dart';
// import 'features/auth/domain/repositories/report_repository.dart';

// import 'features/auth/domain/usecases/get_reports.dart';
// import 'features/auth/domain/usecases/submit_report.dart';
// import 'features/auth/domain/usecases/update_report_status.dart';

// import 'features/auth/presentation/bloc/notifications/notifications_bloc.dart';
// import 'features/auth/presentation/bloc/reports/reports_bloc.dart';

// final sl = GetIt.instance;

// bool _initialized = false;

// Future<void> init() async {
//   if (_initialized) return;
//   _initialized = true;

//   // =========================
//   // External
//   // =========================
//   sl.registerLazySingleton<Connectivity>(() => Connectivity());

//   // SharedPreferencesAsync (كما تستخدمه أنت)
//   final prefsAsync = SharedPreferencesAsync();
//   sl.registerLazySingleton<SharedPreferencesAsync>(() => prefsAsync);

//   // =========================
//   // Core
//   // =========================
//   sl.registerLazySingleton<NetworkInfo>(
//     () => NetworkInfoImpl(sl<Connectivity>()),
//   );

//   // =========================
//   // Auth Local (Secure Storage)
//   // =========================
//   // مهم: ApiClient/AuthInterceptor يعتمد على هذا
//   sl.registerLazySingleton<AuthLocalDataSource>(() => AuthLocalDataSource());

//   // =========================
//   // ApiClient (Singleton واحد فقط)
//   // =========================
//   // ApiFactory.api مبني على AuthLocalDataSource داخل ApiFactory
//   // لذلك لا تضف أي interceptor هنا يقرأ token من SharedPreferences
//   sl.registerLazySingleton<ApiClient>(() => ApiFactory.api);

//   // =========================
//   // DataSources
//   // =========================
//   sl.registerLazySingleton<NotificationLocalDataSource>(
//     () => NotificationLocalDataSourceImpl(sl<SharedPreferencesAsync>()),
//   );

//   sl.registerLazySingleton<ReportLocalDataSource>(
//     () => ReportLocalDataSourceImpl(sl<SharedPreferencesAsync>()),
//   );

//   // إذا كان ReportRemoteDataSourceImpl عندك يتطلب (apiClient, local) أبقه:
//   sl.registerLazySingleton<ReportRemoteDataSource>(
//     () => ReportRemoteDataSourceImpl(
//       apiClient: sl<ApiClient>(),
//       // local: sl<AuthLocalDataSource>(),
//     ),
//   );
//   sl.registerLazySingleton<AwarenessRemoteDataSource>(
//     () => AwarenessRemoteDataSourceImpl(sl<ApiClient>()),
//   );
//   sl.registerLazySingleton<AnonymousDeviceLocalDataSource>(
//     () => AnonymousDeviceLocalDataSourceImpl(sl<SharedPreferencesAsync>()),
//   );
//   sl.registerLazySingleton<AnonymousDeviceRemoteDataSource>(
//     () => AnonymousDeviceRemoteDataSourceImpl(sl<ApiClient>()),
//   );
//   // =========================
//   // Repositories
//   // =========================
//   sl.registerLazySingleton<ReportRepository>(
//     () => ReportRepositoryImpl(
//       local: sl<ReportLocalDataSource>(),
//       remote: sl<ReportRemoteDataSource>(),
//     ),
//   );

//   sl.registerLazySingleton<NotificationRepository>(
//     () => NotificationRepositoryImpl(local: sl<NotificationLocalDataSource>()),
//   );
//   sl.registerLazySingleton<AwarenessRepository>(
//     () => AwarenessRepositoryImpl(sl<AwarenessRemoteDataSource>()),
//   );
//   sl.registerLazySingleton<AnonymousDeviceRepository>(
//     () => AnonymousDeviceRepositoryImpl(
//       local: sl<AnonymousDeviceLocalDataSource>(),
//       remote: sl<AnonymousDeviceRemoteDataSource>(),
//     ),
//   );
//   sl.registerLazySingleton<DriverAcceptReport>(
//     () => DriverAcceptReport(sl<ReportRepository>()),
//   );

//   sl.registerLazySingleton<DriverRejectReport>(
//     () => DriverRejectReport(sl<ReportRepository>()),
//   );
//   sl.registerLazySingleton<DriverCompleteExecution>(
//     () => DriverCompleteExecution(sl<ReportRepository>()),
//   );
//   // =========================
//   // UseCases
//   // =========================
//   sl.registerLazySingleton<GetReports>(
//     () => GetReports(sl<ReportRepository>()),
//   );
//   sl.registerLazySingleton<SubmitReport>(
//     () => SubmitReport(sl<ReportRepository>()),
//   );
//   sl.registerLazySingleton<UpdateReportStatus>(
//     () => UpdateReportStatus(sl<ReportRepository>()),
//   );
//   sl.registerLazySingleton<GetAwarenessContentsUseCase>(
//     () => GetAwarenessContentsUseCase(sl<AwarenessRepository>()),
//   );

//   sl.registerLazySingleton<RegisterAnonymousDeviceUseCase>(
//     () => RegisterAnonymousDeviceUseCase(sl<AnonymousDeviceRepository>()),
//   );
//   sl.registerLazySingleton<GetSavedAnonymousDeviceIdUseCase>(
//     () => GetSavedAnonymousDeviceIdUseCase(sl<AnonymousDeviceRepository>()),
//   );
//   sl.registerLazySingleton<GetOrCreateDeviceIdentifierUseCase>(
//     () => GetOrCreateDeviceIdentifierUseCase(sl<AnonymousDeviceRepository>()),
//   );
//   sl.registerLazySingleton<InitializeAnonymousDeviceUseCase>(
//     () => InitializeAnonymousDeviceUseCase(
//       registerAnonymousDeviceUseCase: sl<RegisterAnonymousDeviceUseCase>(),
//       repository: sl<AnonymousDeviceRepository>(),
//     ),
//   );
//   // =========================
//   // Blocs
//   // =========================غ
//   // sl.registerFactory<ReportsBloc>(
//   //   () => ReportsBloc(
//   //     getReports: sl<GetReports>(),
//   //     submitReport: sl<SubmitReport>(),
//   //     updateStatus: sl<UpdateReportStatus>(),
//   //   ),
//   // );
//   sl.registerFactory<ReportsBloc>(
//     () => ReportsBloc(
//       getReports: sl<GetReports>(),
//       submitReport: sl<SubmitReport>(),
//       updateStatus: sl<UpdateReportStatus>(),
//       driverAcceptReport: sl<DriverAcceptReport>(),
//       driverRejectReport: sl<DriverRejectReport>(),
//       driverCompleteExecution: sl<DriverCompleteExecution>(),
//     ),
//   );

//   sl.registerFactory<NotificationsBloc>(
//     () => NotificationsBloc(sl<NotificationRepository>()),
//   );
// }

// ==================== IMPORTS ====================
// استيراد جميع الـ DataSources والـ Repositories والـ UseCases والـ Blocs

import 'package:cleancityapp/features/auth/data/datasources/anonymous_device_local_data_source.dart';
import 'package:cleancityapp/features/auth/data/datasources/anonymous_device_remote_data_source.dart';
import 'package:cleancityapp/features/auth/data/datasources/awareness_remote_data_source.dart';

import 'package:cleancityapp/features/auth/data/repositories/anonymous_device_repository_impl.dart';
import 'package:cleancityapp/features/auth/data/repositories/awareness_repository_impl.dart';

import 'package:cleancityapp/features/auth/domain/repositories/anonymous_device_repository.dart';
import 'package:cleancityapp/features/auth/domain/repositories/awareness_repository.dart';

import 'package:cleancityapp/features/auth/domain/usecases/awareness/driver_complete_execution.dart';
import 'package:cleancityapp/features/auth/domain/usecases/awareness/get_awareness_contents_usecase.dart';

import 'package:cleancityapp/features/auth/domain/usecases/driver_accept_report.dart';
import 'package:cleancityapp/features/auth/domain/usecases/driver_reject_report.dart';

import 'package:cleancityapp/features/auth/domain/usecases/get_or_create_device_identifier_usecase.dart';
import 'package:cleancityapp/features/auth/domain/usecases/get_saved_anonymous_device_id_usecase.dart';
import 'package:cleancityapp/features/auth/domain/usecases/initialize_anonymous_device_usecase.dart';
import 'package:cleancityapp/features/auth/domain/usecases/register_anonymous_device_usecase.dart';

import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:get_it/get_it.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'core/network/api_client.dart';
import 'core/network/api_factory.dart';
import 'core/network/network_info.dart';

import 'features/auth/data/datasources/auth_local_data_source.dart';
import 'features/auth/data/datasources/notification_local_datasource.dart';
import 'features/auth/data/datasources/report_local_datasource.dart';
import 'features/auth/data/datasources/report_remote_datasource.dart';

import 'features/auth/data/repositories/notification_repository_impl.dart';
import 'features/auth/data/repositories/report_repository_impl.dart';

import 'features/auth/domain/repositories/notification_repository.dart';
import 'features/auth/domain/repositories/report_repository.dart';

import 'features/auth/domain/usecases/get_reports.dart';
import 'features/auth/domain/usecases/submit_report.dart';
import 'features/auth/domain/usecases/update_report_status.dart';

import 'features/auth/presentation/bloc/notifications/notifications_bloc.dart';
import 'features/auth/presentation/bloc/reports/reports_bloc.dart';

// ==================== SERVICE LOCATOR ====================
// هذا هو الكائن الرئيسي الذي يخزن كل dependencies
final sl = GetIt.instance;

// لمنع تهيئة الـ dependencies أكثر من مرة
bool _initialized = false;

// ==================== INIT FUNCTION ====================
Future<void> init() async {
  if (_initialized) return;
  _initialized = true;

  // ==================== EXTERNAL ====================
  // فحص الاتصال بالإنترنت
  sl.registerLazySingleton<Connectivity>(() => Connectivity());

  // تخزين محلي Async
  final prefsAsync = SharedPreferencesAsync();
  sl.registerLazySingleton<SharedPreferencesAsync>(() => prefsAsync);

  // ==================== CORE ====================
  // كلاس لمعرفة هل يوجد إنترنت أم لا
  sl.registerLazySingleton<NetworkInfo>(
    () => NetworkInfoImpl(sl<Connectivity>()),
  );

  // تخزين بيانات التوكن أو المستخدم محليًا
  sl.registerLazySingleton<AuthLocalDataSource>(
    () => AuthLocalDataSource(),
  );

  // عميل API موحد لكل الطلبات
  sl.registerLazySingleton<ApiClient>(
    () => ApiFactory.api,
  );

  // ==================== DATA SOURCES ====================
  // تخزين الإشعارات محليًا
  sl.registerLazySingleton<NotificationLocalDataSource>(
    () => NotificationLocalDataSourceImpl(
      sl<SharedPreferencesAsync>(),
    ),
  );

  // تخزين البلاغات محليًا
  sl.registerLazySingleton<ReportLocalDataSource>(
    () => ReportLocalDataSourceImpl(
      sl<SharedPreferencesAsync>(),
    ),
  );

  // مصدر البيانات البعيد للبلاغات
  sl.registerLazySingleton<ReportRemoteDataSource>(
    () => ReportRemoteDataSourceImpl(
      apiClient: sl<ApiClient>(),
    ),
  );

  // مصدر بيانات التوعية
  sl.registerLazySingleton<AwarenessRemoteDataSource>(
    () => AwarenessRemoteDataSourceImpl(
      sl<ApiClient>(),
    ),
  );

  // تخزين معرف الجهاز محليًا
  sl.registerLazySingleton<AnonymousDeviceLocalDataSource>(
    () => AnonymousDeviceLocalDataSourceImpl(
      sl<SharedPreferencesAsync>(),
    ),
  );

  // رفع معرف الجهاز للسيرفر
  sl.registerLazySingleton<AnonymousDeviceRemoteDataSource>(
    () => AnonymousDeviceRemoteDataSourceImpl(
      sl<ApiClient>(),
    ),
  );

  // ==================== REPOSITORIES ====================
  // Repository البلاغات
  sl.registerLazySingleton<ReportRepository>(
    () => ReportRepositoryImpl(
      local: sl<ReportLocalDataSource>(),
      remote: sl<ReportRemoteDataSource>(),
      networkInfo: sl<NetworkInfo>(),
    ),
  );

  // Repository الإشعارات
  sl.registerLazySingleton<NotificationRepository>(
    () => NotificationRepositoryImpl(
      local: sl<NotificationLocalDataSource>(),
    ),
  );

  // Repository التوعية
  sl.registerLazySingleton<AwarenessRepository>(
    () => AwarenessRepositoryImpl(
      sl<AwarenessRemoteDataSource>(),
    ),
  );

  // Repository معرف الجهاز
  sl.registerLazySingleton<AnonymousDeviceRepository>(
    () => AnonymousDeviceRepositoryImpl(
      local: sl<AnonymousDeviceLocalDataSource>(),
      remote: sl<AnonymousDeviceRemoteDataSource>(),
    ),
  );

  // ==================== USE CASES ====================
  // قبول البلاغ من السائق
  sl.registerLazySingleton<DriverAcceptReport>(
    () => DriverAcceptReport(sl<ReportRepository>()),
  );

  // رفض البلاغ
  sl.registerLazySingleton<DriverRejectReport>(
    () => DriverRejectReport(sl<ReportRepository>()),
  );

  // إنهاء التنفيذ
  sl.registerLazySingleton<DriverCompleteExecution>(
    () => DriverCompleteExecution(sl<ReportRepository>()),
  );

  // جلب البلاغات
  sl.registerLazySingleton<GetReports>(
    () => GetReports(sl<ReportRepository>()),
  );

  // إرسال بلاغ جديد
  sl.registerLazySingleton<SubmitReport>(
    () => SubmitReport(sl<ReportRepository>()),
  );

  // تحديث حالة البلاغ
  sl.registerLazySingleton<UpdateReportStatus>(
    () => UpdateReportStatus(sl<ReportRepository>()),
  );

  // جلب محتوى التوعية
  sl.registerLazySingleton<GetAwarenessContentsUseCase>(
    () => GetAwarenessContentsUseCase(
      sl<AwarenessRepository>(),
    ),
  );

  // تسجيل الجهاز
  sl.registerLazySingleton<RegisterAnonymousDeviceUseCase>(
    () => RegisterAnonymousDeviceUseCase(
      sl<AnonymousDeviceRepository>(),
    ),
  );

  // جلب ID الجهاز المحفوظ
  sl.registerLazySingleton<GetSavedAnonymousDeviceIdUseCase>(
    () => GetSavedAnonymousDeviceIdUseCase(
      sl<AnonymousDeviceRepository>(),
    ),
  );

  // إنشاء أو جلب ID الجهاز
  sl.registerLazySingleton<GetOrCreateDeviceIdentifierUseCase>(
    () => GetOrCreateDeviceIdentifierUseCase(
      sl<AnonymousDeviceRepository>(),
    ),
  );

  // تهيئة الجهاز مجهول الهوية
  sl.registerLazySingleton<InitializeAnonymousDeviceUseCase>(
    () => InitializeAnonymousDeviceUseCase(
      registerAnonymousDeviceUseCase: sl<RegisterAnonymousDeviceUseCase>(),
      repository: sl<AnonymousDeviceRepository>(),
    ),
  );

  // ==================== BLOCS ====================
  // Bloc إدارة البلاغات
  sl.registerFactory<ReportsBloc>(
    () => ReportsBloc(
      getReports: sl<GetReports>(),
      submitReport: sl<SubmitReport>(),
      updateStatus: sl<UpdateReportStatus>(),
      driverAcceptReport: sl<DriverAcceptReport>(),
      driverRejectReport: sl<DriverRejectReport>(),
      driverCompleteExecution: sl<DriverCompleteExecution>(),
      reportRepository: sl<ReportRepository>(),
    ),
  );

  // Bloc إدارة الإشعارات
  sl.registerFactory<NotificationsBloc>(
    () => NotificationsBloc(
      sl<NotificationRepository>(),
    ),
  );
}
