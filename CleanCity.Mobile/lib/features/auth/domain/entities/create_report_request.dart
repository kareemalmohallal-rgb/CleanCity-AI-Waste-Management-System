import 'dart:io';

class CreateReportRequest {
  final double latitude;
  final double longitude;
  final String? description;
  final int anonymousDeviceId;
  final File? imageFile;

  const CreateReportRequest({
    required this.latitude,
    required this.longitude,
    required this.description,
    required this.anonymousDeviceId,
    required this.imageFile,
  });
}