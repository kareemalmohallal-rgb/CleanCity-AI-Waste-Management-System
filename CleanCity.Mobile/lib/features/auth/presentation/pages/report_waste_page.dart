// import 'dart:io';

// import 'package:cleancityapp/features/auth/domain/entities/create_report_request.dart';
// import 'package:cleancityapp/features/auth/domain/usecases/get_saved_anonymous_device_id_usecase.dart';
// import 'package:cleancityapp/features/auth/presentation/bloc/reports/reports_bloc.dart';
// import 'package:cleancityapp/injection_container.dart';
// import 'package:flutter/material.dart';
// import 'package:flutter_bloc/flutter_bloc.dart';
// import 'package:geolocator/geolocator.dart';
// import 'package:image_picker/image_picker.dart';

// class ReportWastePage extends StatefulWidget {
//   const ReportWastePage({super.key});

//   @override
//   State<ReportWastePage> createState() => _ReportWastePageState();
// }

// class _ReportWastePageState extends State<ReportWastePage> {
//   File? _selectedImage;
//   bool _isLoadingLocation = true;

//   double? _latitude;
//   double? _longitude;

//   final ImagePicker _picker = ImagePicker();
//   final TextEditingController _descriptionController = TextEditingController();

//   late final GetSavedAnonymousDeviceIdUseCase _getSavedAnonymousDeviceIdUseCase;

//   @override
//   void initState() {
//     super.initState();
//     _getSavedAnonymousDeviceIdUseCase = sl<GetSavedAnonymousDeviceIdUseCase>();
//     _getCurrentLocation();
//   }

//   @override
//   void dispose() {
//     _descriptionController.dispose();
//     super.dispose();
//   }

//   String get _locationText {
//     if (_latitude == null || _longitude == null) {
//       return 'الموقع غير متوفر';
//     }

//     return 'خط العرض: ${_latitude!.toStringAsFixed(6)}, خط الطول: ${_longitude!.toStringAsFixed(6)}';
//   }

//   Future<void> _getCurrentLocation() async {
//     setState(() {
//       _isLoadingLocation = true;
//     });

//     try {
//       final serviceEnabled = await Geolocator.isLocationServiceEnabled();
//       if (!serviceEnabled) {
//         throw 'خدمة الموقع غير مفعلة';
//       }

//       LocationPermission permission = await Geolocator.checkPermission();

//       if (permission == LocationPermission.denied) {
//         permission = await Geolocator.requestPermission();
//         if (permission == LocationPermission.denied) {
//           throw 'تم رفض صلاحية الموقع';
//         }
//       }

//       if (permission == LocationPermission.deniedForever) {
//         throw 'صلاحية الموقع مرفوضة نهائيًا';
//       }

//       final position = await Geolocator.getCurrentPosition(
//         locationSettings: const LocationSettings(
//           accuracy: LocationAccuracy.high,
//         ),
//       );

//       if (!mounted) return;

//       setState(() {
//         _latitude = position.latitude;
//         _longitude = position.longitude;
//         _isLoadingLocation = false;
//       });
//     } catch (e) {
//       if (!mounted) return;

//       setState(() {
//         _isLoadingLocation = false;
//       });

//       _showError(e.toString());
//     }
//   }

//   Future<void> _takePicture() async {
//     final XFile? image = await _picker.pickImage(
//       source: ImageSource.camera,
//       imageQuality: 85,
//       maxWidth: 1200,
//     );

//     if (image != null) {
//       setState(() {
//         _selectedImage = File(image.path);
//       });
//     }
//   }

//   Future<void> _submitReport() async {
//     if (_latitude == null || _longitude == null) {
//       _showError('تعذر تحديد الموقع الحالي');
//       return;
//     }

//     if (_selectedImage == null) {
//       _showError('الرجاء التقاط صورة للبلاغ');
//       return;
//     }

//     try {
//       final anonymousDeviceId = await _getSavedAnonymousDeviceIdUseCase();

//       if (anonymousDeviceId == null || anonymousDeviceId <= 0) {
//         _showError(
//           'لم يتم العثور على معرف الجهاز. أعد تشغيل التطبيق ثم حاول مرة أخرى.',
//         );
//         return;
//       }

//       final request = CreateReportRequest(
//         latitude: _latitude!,
//         longitude: _longitude!,
//         description: _descriptionController.text.trim().isEmpty
//             ? null
//             : _descriptionController.text.trim(),
//         anonymousDeviceId: anonymousDeviceId,
//         imageFile: _selectedImage,
//       );

//       if (!mounted) return;

//       context.read<ReportsBloc>().add(CreateReport(request));
//     } catch (e) {
//       _showError('حدث خطأ أثناء تجهيز البلاغ: $e');
//     }
//   }

//   void _showError(String message) {
//     if (!mounted) return;

//     ScaffoldMessenger.of(context).showSnackBar(
//       SnackBar(
//         content: Text(message),
//         backgroundColor: Colors.red,
//         duration: const Duration(seconds: 3),
//       ),
//     );
//   }

//   void _showSuccessDialog(int reportId) {
//     showDialog(
//       context: context,
//       builder: (dialogContext) => AlertDialog(
//         title: const Text(
//           'تم إرسال البلاغ',
//           textAlign: TextAlign.center,
//           style: TextStyle(color: Color(0xFF29F14D)),
//         ),
//         content: Column(
//           mainAxisSize: MainAxisSize.min,
//           children: [
//             const Icon(Icons.check_circle, color: Color(0xFF29F14D), size: 60),
//             const SizedBox(height: 16),
//             const Text(
//               'تم استلام بلاغك بنجاح',
//               textAlign: TextAlign.center,
//               style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
//             ),
//             const SizedBox(height: 8),
//             Text(
//               'رقم البلاغ: #$reportId',
//               textAlign: TextAlign.center,
//               style: const TextStyle(fontSize: 14, color: Colors.grey),
//             ),
//           ],
//         ),
//         actions: [
//           Center(
//             child: ElevatedButton(
//               onPressed: () {
//                 Navigator.pop(dialogContext);
//                 context.read<ReportsBloc>().add(
//                   const ClearReportSubmissionState(),
//                 );
//                 Navigator.pop(context);
//               },
//               style: ElevatedButton.styleFrom(
//                 backgroundColor: const Color(0xFF29F14D),
//                 foregroundColor: Colors.white,
//                 padding: const EdgeInsets.symmetric(
//                   horizontal: 32,
//                   vertical: 12,
//                 ),
//                 shape: RoundedRectangleBorder(
//                   borderRadius: BorderRadius.circular(8),
//                 ),
//               ),
//               child: const Text('حسناً'),
//             ),
//           ),
//         ],
//       ),
//     );
//   }

//   void _showImageSourceDialog() {
//     showModalBottomSheet(
//       context: context,
//       builder: (context) => Directionality(
//         textDirection: TextDirection.rtl,
//         child: SafeArea(
//           child: Column(
//             mainAxisSize: MainAxisSize.min,
//             children: [
//               ListTile(
//                 leading: const Icon(Icons.camera_alt),
//                 title: const Text('التقاط صورة جديدة'),
//                 onTap: () {
//                   Navigator.pop(context);
//                   _takePicture();
//                 },
//               ),
//               ListTile(
//                 leading: const Icon(Icons.cancel),
//                 title: const Text('إلغاء'),
//                 onTap: () => Navigator.pop(context),
//               ),
//             ],
//           ),
//         ),
//       ),
//     );
//   }

//   @override
//   Widget build(BuildContext context) {
//     return BlocListener<ReportsBloc, ReportsState>(
//       listenWhen: (previous, current) =>
//           previous.error != current.error ||
//           previous.submittedReportId != current.submittedReportId,
//       listener: (context, state) {
//         if (state.error != null && state.error!.trim().isNotEmpty) {
//           _showError(state.error!);
//         }

//         if (state.submittedReportId != null && state.submittedReportId! > 0) {
//           _showSuccessDialog(state.submittedReportId!);
//         }
//       },
//       child: Directionality(
//         textDirection: TextDirection.rtl,
//         child: Scaffold(
//           appBar: AppBar(
//             title: const Text('إرسال البلاغ'),
//             centerTitle: true,
//             backgroundColor: const Color.fromARGB(255, 87, 211, 157),
//             elevation: 2,
//             shadowColor: const Color.fromARGB(255, 255, 255, 255),
//             leading: IconButton(
//               icon: const Icon(Icons.arrow_back),
//               onPressed: () => Navigator.pop(context),
//             ),
//           ),
//           body: SingleChildScrollView(
//             padding: const EdgeInsets.all(20),
//             child: Column(
//               crossAxisAlignment: CrossAxisAlignment.start,
//               children: [
//                 _buildImageSection(),
//                 const SizedBox(height: 30),
//                 _buildLocationSection(),
//                 const SizedBox(height: 30),
//                 _buildDescriptionSection(),
//                 const SizedBox(height: 40),
//                 _buildSubmitButton(),
//               ],
//             ),
//           ),
//         ),
//       ),
//     );
//   }

//   Widget _buildImageSection() {
//     return Column(
//       crossAxisAlignment: CrossAxisAlignment.start,
//       children: [
//         const Text(
//           'التقاط صورة',
//           style: TextStyle(
//             fontSize: 18,
//             fontWeight: FontWeight.bold,
//             color: Colors.black87,
//           ),
//         ),
//         const SizedBox(height: 10),
//         GestureDetector(
//           onTap: _showImageSourceDialog,
//           child: Container(
//             height: 150,
//             width: double.infinity,
//             decoration: BoxDecoration(
//               color: Colors.grey[100],
//               borderRadius: BorderRadius.circular(12),
//               border: Border.all(
//                 color: _selectedImage == null
//                     ? Colors.grey[200]!
//                     : Colors.green,
//                 width: 2,
//               ),
//             ),
//             child: _selectedImage == null
//                 ? Column(
//                     mainAxisAlignment: MainAxisAlignment.center,
//                     children: [
//                       Icon(Icons.camera_alt, size: 60, color: Colors.grey[400]),
//                       const SizedBox(height: 10),
//                       const Text(
//                         'التقط صورة',
//                         style: TextStyle(fontSize: 16, color: Colors.grey),
//                       ),
//                       const SizedBox(height: 5),
//                       Text(
//                         'اضغط لالتقاط صورة',
//                         style: TextStyle(fontSize: 14, color: Colors.grey[500]),
//                       ),
//                     ],
//                   )
//                 : ClipRRect(
//                     borderRadius: BorderRadius.circular(10),
//                     child: Image.file(
//                       _selectedImage!,
//                       fit: BoxFit.cover,
//                       width: double.infinity,
//                       height: double.infinity,
//                     ),
//                   ),
//           ),
//         ),
//         if (_selectedImage != null)
//           Padding(
//             padding: const EdgeInsets.only(top: 8),
//             child: Row(
//               mainAxisAlignment: MainAxisAlignment.center,
//               children: [
//                 TextButton.icon(
//                   onPressed: _showImageSourceDialog,
//                   icon: const Icon(Icons.camera_alt, size: 20),
//                   label: const Text('تغيير الصورة'),
//                   style: TextButton.styleFrom(
//                     foregroundColor: const Color(0xFF29F14D),
//                   ),
//                 ),
//                 const SizedBox(width: 20),
//                 TextButton.icon(
//                   onPressed: () {
//                     setState(() {
//                       _selectedImage = null;
//                     });
//                   },
//                   icon: const Icon(Icons.delete, size: 20),
//                   label: const Text('حذف'),
//                   style: TextButton.styleFrom(foregroundColor: Colors.red),
//                 ),
//               ],
//             ),
//           ),
//       ],
//     );
//   }

//   Widget _buildLocationSection() {
//     return Column(
//       crossAxisAlignment: CrossAxisAlignment.start,
//       children: [
//         const Text(
//           'تحديد الموقع',
//           style: TextStyle(
//             fontSize: 18,
//             fontWeight: FontWeight.bold,
//             color: Colors.black87,
//           ),
//         ),
//         const SizedBox(height: 10),
//         Container(
//           padding: const EdgeInsets.all(16),
//           decoration: BoxDecoration(
//             color: Colors.grey[50],
//             borderRadius: BorderRadius.circular(12),
//             border: Border.all(color: Colors.grey[300]!),
//           ),
//           child: Row(
//             children: [
//               Icon(
//                 Icons.location_on,
//                 color: _isLoadingLocation ? Colors.grey : Colors.red,
//                 size: 24,
//               ),
//               const SizedBox(width: 12),
//               Expanded(
//                 child: _isLoadingLocation
//                     ? const Row(
//                         children: [
//                           SizedBox(
//                             width: 30,
//                             height: 20,
//                             child: CircularProgressIndicator(strokeWidth: 2),
//                           ),
//                           SizedBox(width: 10),
//                           Text('جاري تحديد الموقع...'),
//                         ],
//                       )
//                     : Column(
//                         crossAxisAlignment: CrossAxisAlignment.start,
//                         children: [
//                           Text(
//                             _locationText,
//                             style: const TextStyle(
//                               fontSize: 16,
//                               fontWeight: FontWeight.bold,
//                             ),
//                           ),
//                           const SizedBox(height: 4),
//                           const Text(
//                             'يتم تحديد الموقع تلقائياً عبر GPS',
//                             style: TextStyle(fontSize: 12, color: Colors.grey),
//                           ),
//                         ],
//                       ),
//               ),
//               if (!_isLoadingLocation)
//                 IconButton(
//                   icon: const Icon(Icons.refresh),
//                   onPressed: () {
//                     setState(() {
//                       _isLoadingLocation = true;
//                     });
//                     _getCurrentLocation();
//                   },
//                   tooltip: 'تحديث الموقع',
//                 ),
//             ],
//           ),
//         ),
//       ],
//     );
//   }

//   Widget _buildDescriptionSection() {
//     return Column(
//       crossAxisAlignment: CrossAxisAlignment.start,
//       children: [
//         const Text(
//           'تفاصيل',
//           style: TextStyle(
//             fontSize: 18,
//             fontWeight: FontWeight.bold,
//             color: Colors.black87,
//           ),
//         ),
//         const SizedBox(height: 10),
//         Container(
//           padding: const EdgeInsets.all(16),
//           decoration: BoxDecoration(
//             color: Colors.grey[50],
//             borderRadius: BorderRadius.circular(12),
//             border: Border.all(color: Colors.grey[300]!),
//           ),
//           child: Column(
//             crossAxisAlignment: CrossAxisAlignment.start,
//             children: [
//               const Text(
//                 'أضف وصف (اختياري)',
//                 style: TextStyle(fontSize: 14, color: Colors.grey),
//               ),
//               const SizedBox(height: 8),
//               TextField(
//                 controller: _descriptionController,
//                 minLines: 1,
//                 maxLines: null,
//                 maxLength: 500,
//                 decoration: const InputDecoration(
//                   hintText: 'مثال: هناك كومة نفايات كبيرة بجوار المدرسة...',
//                   border: InputBorder.none,
//                   contentPadding: EdgeInsets.symmetric(vertical: 4),
//                 ),
//               ),
//             ],
//           ),
//         ),
//       ],
//     );
//   }

//   Widget _buildSubmitButton() {
//     return BlocBuilder<ReportsBloc, ReportsState>(
//       builder: (context, state) {
//         return SizedBox(
//           width: double.infinity,
//           height: 56,
//           child: ElevatedButton(
//             onPressed: state.actionLoading ? null : _submitReport,
//             style: ElevatedButton.styleFrom(
//               backgroundColor: const Color(0xFF29F14D),
//               foregroundColor: Colors.white,
//               shape: RoundedRectangleBorder(
//                 borderRadius: BorderRadius.circular(12),
//               ),
//               elevation: 3,
//             ),
//             child: state.actionLoading
//                 ? const Row(
//                     mainAxisAlignment: MainAxisAlignment.center,
//                     children: [
//                       SizedBox(
//                         width: 20,
//                         height: 20,
//                         child: CircularProgressIndicator(
//                           color: Colors.white,
//                           strokeWidth: 2,
//                         ),
//                       ),
//                       SizedBox(width: 10),
//                       Text('جاري الإرسال...'),
//                     ],
//                   )
//                 : const Row(
//                     mainAxisAlignment: MainAxisAlignment.center,
//                     children: [
//                       Icon(Icons.send, size: 20),
//                       SizedBox(width: 10),
//                       Text(
//                         'إرسال البلاغ',
//                         style: TextStyle(
//                           fontSize: 18,
//                           fontWeight: FontWeight.bold,
//                         ),
//                       ),
//                     ],
//                   ),
//           ),
//         );
//       },
//     );
//   }
// }
import 'dart:io';

import 'package:cleancityapp/features/auth/domain/entities/create_report_request.dart';
import 'package:cleancityapp/features/auth/domain/usecases/get_saved_anonymous_device_id_usecase.dart';
import 'package:cleancityapp/features/auth/presentation/bloc/reports/reports_bloc.dart';
import 'package:cleancityapp/injection_container.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:geolocator/geolocator.dart';
import 'package:image_picker/image_picker.dart';

class ReportWastePage extends StatefulWidget {
  const ReportWastePage({super.key});

  @override
  State<ReportWastePage> createState() => _ReportWastePageState();
}

class _ReportWastePageState extends State<ReportWastePage> {
  File? _selectedImage;
  bool _isLoadingLocation = true;

  double? _latitude;
  double? _longitude;

  final ImagePicker _picker = ImagePicker();
  final TextEditingController _descriptionController = TextEditingController();

  late final GetSavedAnonymousDeviceIdUseCase _getSavedAnonymousDeviceIdUseCase;

  @override
  void initState() {
    super.initState();
    _getSavedAnonymousDeviceIdUseCase = sl<GetSavedAnonymousDeviceIdUseCase>();
    _getCurrentLocation();
  }

  @override
  void dispose() {
    _descriptionController.dispose();
    super.dispose();
  }

  String get _locationText {
    if (_latitude == null || _longitude == null) return 'الموقع غير متوفر';
    return 'خط العرض: ${_latitude!.toStringAsFixed(6)}, خط الطول: ${_longitude!.toStringAsFixed(6)}';
  }

  Future<void> _getCurrentLocation() async {
    setState(() => _isLoadingLocation = true);

    try {
      final serviceEnabled = await Geolocator.isLocationServiceEnabled();
      if (!serviceEnabled) throw 'خدمة الموقع غير مفعلة';

      LocationPermission permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied) {
        permission = await Geolocator.requestPermission();
        if (permission == LocationPermission.denied)
          throw 'تم رفض صلاحية الموقع';
      }

      if (permission == LocationPermission.deniedForever) {
        throw 'صلاحية الموقع مرفوضة نهائيًا';
      }

      final position = await Geolocator.getCurrentPosition(
        locationSettings: const LocationSettings(
          accuracy: LocationAccuracy.high,
        ),
      );

      if (!mounted) return;

      setState(() {
        _latitude = position.latitude;
        _longitude = position.longitude;
        _isLoadingLocation = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() => _isLoadingLocation = false);
      _showError(e.toString());
    }
  }

  Future<void> _takePicture() async {
    final XFile? image = await _picker.pickImage(
      source: ImageSource.camera,
      imageQuality: 85,
      maxWidth: 1200,
    );
    if (image != null) setState(() => _selectedImage = File(image.path));
  }

  Future<void> _submitReport() async {
    if (_latitude == null || _longitude == null) {
      _showError('تعذر تحديد الموقع الحالي');
      return;
    }

    if (_selectedImage == null) {
      _showError('الرجاء التقاط صورة للبلاغ');
      return;
    }

    try {
      final anonymousDeviceId = await _getSavedAnonymousDeviceIdUseCase();

      if (anonymousDeviceId == null || anonymousDeviceId <= 0) {
        _showError(
          'لم يتم العثور على معرف الجهاز. أعد تشغيل التطبيق ثم حاول مرة أخرى.',
        );
        return;
      }

      final request = CreateReportRequest(
        latitude: _latitude!,
        longitude: _longitude!,
        description: _descriptionController.text.trim().isEmpty
            ? null
            : _descriptionController.text.trim(),
        anonymousDeviceId: anonymousDeviceId,
        imageFile: _selectedImage,
      );

      if (!mounted) return;

      context.read<ReportsBloc>().add(CreateReport(request));
    } catch (e) {
      _showError('حدث خطأ أثناء تجهيز البلاغ: $e');
    }
  }

  void _showError(String message) {
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red,
        duration: const Duration(seconds: 3),
      ),
    );
  }

  void _showSuccessDialog(int reportId) {
    showDialog(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text(
          'تم إرسال البلاغ',
          textAlign: TextAlign.center,
          style: TextStyle(color: Color(0xFF29F14D)),
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(Icons.check_circle, color: Color(0xFF29F14D), size: 60),
            const SizedBox(height: 16),
            const Text(
              ' تم إرسال بلاغك للتحليل',
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            Text(
              'رقم البلاغ: #$reportId',
              textAlign: TextAlign.center,
              style: const TextStyle(fontSize: 14, color: Colors.grey),
            ),
          ],
        ),
        actions: [
          Center(
            child: ElevatedButton(
              onPressed: () {
                Navigator.pop(dialogContext);
                context.read<ReportsBloc>().add(
                  const ClearReportSubmissionState(),
                );
                Navigator.pop(context);
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color(0xFF29F14D),
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(
                  horizontal: 32,
                  vertical: 12,
                ),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              child: const Text('حسناً'),
            ),
          ),
        ],
      ),
    );
  }

  void _showImageSourceDialog() {
    showModalBottomSheet(
      context: context,
      builder: (context) => Directionality(
        textDirection: TextDirection.rtl,
        child: SafeArea(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              ListTile(
                leading: const Icon(Icons.camera_alt),
                title: const Text('التقاط صورة جديدة'),
                onTap: () {
                  Navigator.pop(context);
                  _takePicture();
                },
              ),
              ListTile(
                leading: const Icon(Icons.cancel),
                title: const Text('إلغاء'),
                onTap: () => Navigator.pop(context),
              ),
            ],
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return BlocListener<ReportsBloc, ReportsState>(
      listenWhen: (previous, current) =>
          previous.error != current.error ||
          previous.submittedReportId != current.submittedReportId ||
          previous.savedLocally != current.savedLocally,
      listener: (context, state) {
        if (state.error != null && state.error!.trim().isNotEmpty) {
          _showError(state.error!);
        }

        if (state.submittedReportId != null && state.submittedReportId! > 0) {
          _showSuccessDialog(state.submittedReportId!);
        }

        // ✅ تم الحفظ محلياً بسبب انقطاع الاتصال
        if (state.savedLocally) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Row(
                children: [
                  Icon(Icons.cloud_off, color: Colors.white),
                  SizedBox(width: 12),
                  Expanded(
                    child: Text(
                      'تم حفظ البلاغ وسيُرسل تلقائياً عند الاتصال بالسيرفر',
                      style: TextStyle(fontWeight: FontWeight.w600),
                    ),
                  ),
                ],
              ),
              backgroundColor: Color(0xFF2196F3),
              duration: Duration(seconds: 4),
              behavior: SnackBarBehavior.floating,
            ),
          );
          context.read<ReportsBloc>().add(const ClearReportSubmissionState());
          Navigator.pop(context);
        }
      },
      child: Directionality(
        textDirection: TextDirection.rtl,
        child: Scaffold(
          appBar: AppBar(
            title: const Text('إرسال البلاغ'),
            centerTitle: true,
            backgroundColor: const Color.fromARGB(255, 87, 211, 157),
            elevation: 2,
            shadowColor: const Color.fromARGB(255, 255, 255, 255),
            leading: IconButton(
              icon: const Icon(Icons.arrow_back),
              onPressed: () => Navigator.pop(context),
            ),
          ),
          body: SingleChildScrollView(
            padding: const EdgeInsets.all(20),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildImageSection(),
                const SizedBox(height: 30),
                _buildLocationSection(),
                const SizedBox(height: 30),
                _buildDescriptionSection(),
                const SizedBox(height: 40),
                _buildSubmitButton(),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildImageSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'التقاط صورة',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: Colors.black87,
          ),
        ),
        const SizedBox(height: 10),
        GestureDetector(
          onTap: _showImageSourceDialog,
          child: Container(
            height: 150,
            width: double.infinity,
            decoration: BoxDecoration(
              color: Colors.grey[100],
              borderRadius: BorderRadius.circular(12),
              border: Border.all(
                color: _selectedImage == null
                    ? Colors.grey[200]!
                    : Colors.green,
                width: 2,
              ),
            ),
            child: _selectedImage == null
                ? Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Icon(Icons.camera_alt, size: 60, color: Colors.grey[400]),
                      const SizedBox(height: 10),
                      const Text(
                        'التقط صورة',
                        style: TextStyle(fontSize: 16, color: Colors.grey),
                      ),
                      const SizedBox(height: 5),
                      Text(
                        'اضغط لالتقاط صورة',
                        style: TextStyle(fontSize: 14, color: Colors.grey[500]),
                      ),
                    ],
                  )
                : ClipRRect(
                    borderRadius: BorderRadius.circular(10),
                    child: Image.file(
                      _selectedImage!,
                      fit: BoxFit.cover,
                      width: double.infinity,
                      height: double.infinity,
                    ),
                  ),
          ),
        ),
        if (_selectedImage != null)
          Padding(
            padding: const EdgeInsets.only(top: 8),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                TextButton.icon(
                  onPressed: _showImageSourceDialog,
                  icon: const Icon(Icons.camera_alt, size: 20),
                  label: const Text('تغيير الصورة'),
                  style: TextButton.styleFrom(
                    foregroundColor: const Color(0xFF29F14D),
                  ),
                ),
                const SizedBox(width: 20),
                TextButton.icon(
                  onPressed: () => setState(() => _selectedImage = null),
                  icon: const Icon(Icons.delete, size: 20),
                  label: const Text('حذف'),
                  style: TextButton.styleFrom(foregroundColor: Colors.red),
                ),
              ],
            ),
          ),
      ],
    );
  }

  Widget _buildLocationSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'تحديد الموقع',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: Colors.black87,
          ),
        ),
        const SizedBox(height: 10),
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Colors.grey[50],
            borderRadius: BorderRadius.circular(12),
            border: Border.all(color: Colors.grey[300]!),
          ),
          child: Row(
            children: [
              Icon(
                Icons.location_on,
                color: _isLoadingLocation ? Colors.grey : Colors.red,
                size: 24,
              ),
              const SizedBox(width: 12),
              Expanded(
                child: _isLoadingLocation
                    ? const Row(
                        children: [
                          SizedBox(
                            width: 30,
                            height: 20,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          ),
                          SizedBox(width: 10),
                          Text('جاري تحديد الموقع...'),
                        ],
                      )
                    : Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            _locationText,
                            style: const TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          const SizedBox(height: 4),
                          const Text(
                            'يتم تحديد الموقع تلقائياً عبر GPS',
                            style: TextStyle(fontSize: 12, color: Colors.grey),
                          ),
                        ],
                      ),
              ),
              if (!_isLoadingLocation)
                IconButton(
                  icon: const Icon(Icons.refresh),
                  onPressed: () {
                    setState(() => _isLoadingLocation = true);
                    _getCurrentLocation();
                  },
                  tooltip: 'تحديث الموقع',
                ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildDescriptionSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'تفاصيل',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: Colors.black87,
          ),
        ),
        const SizedBox(height: 10),
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Colors.grey[50],
            borderRadius: BorderRadius.circular(12),
            border: Border.all(color: Colors.grey[300]!),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                'أضف وصف (اختياري)',
                style: TextStyle(fontSize: 14, color: Colors.grey),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: _descriptionController,
                minLines: 1,
                maxLines: null,
                maxLength: 500,
                decoration: const InputDecoration(
                  hintText: 'مثال: هناك كومة نفايات كبيرة بجوار المدرسة...',
                  border: InputBorder.none,
                  contentPadding: EdgeInsets.symmetric(vertical: 4),
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildSubmitButton() {
    return BlocBuilder<ReportsBloc, ReportsState>(
      builder: (context, state) {
        return SizedBox(
          width: double.infinity,
          height: 56,
          child: ElevatedButton(
            onPressed: state.actionLoading ? null : _submitReport,
            style: ElevatedButton.styleFrom(
              backgroundColor: const Color(0xFF29F14D),
              foregroundColor: Colors.white,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              elevation: 3,
            ),
            child: state.actionLoading
                ? const Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(
                          color: Colors.white,
                          strokeWidth: 2,
                        ),
                      ),
                      SizedBox(width: 10),
                      Text('جاري الإرسال...'),
                    ],
                  )
                : const Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Icon(Icons.send, size: 20),
                      SizedBox(width: 10),
                      Text(
                        'إرسال البلاغ',
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
          ),
        );
      },
    );
  }
}
