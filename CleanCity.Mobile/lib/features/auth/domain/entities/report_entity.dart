import 'package:equatable/equatable.dart';

class ReportEntity extends Equatable {
  final String id;
  final String type;
  final String locationText;
  final String? imagePath;
  final String description;

  // حالة البلاغ
  final String reportStatus;

  // حالة الإسناد
  final String assignmentStatus;

  // التبويب/الحالة الجاهزة للواجهة
  final String uiBucket;

  final DateTime createdAt;

  const ReportEntity({
    required this.id,
    required this.type,
    required this.locationText,
    required this.description,
    required this.reportStatus,
    required this.assignmentStatus,
    required this.uiBucket,
    required this.createdAt,
    this.imagePath,
  });

  @override
  List<Object?> get props => [
    id,
    type,
    locationText,
    imagePath,
    description,
    reportStatus,
    assignmentStatus,
    uiBucket,
    createdAt,
  ];
}
