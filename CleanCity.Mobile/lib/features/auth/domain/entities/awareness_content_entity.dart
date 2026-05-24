import 'package:flutter/foundation.dart';

@immutable
class AwarenessContent {
  final int id;
  final String title;
  final String content;
  final String? imageUrl;
  final DateTime createdAt;

  const AwarenessContent({
    required this.id,
    required this.title,
    required this.content,
    required this.imageUrl,
    required this.createdAt,
  });
}
