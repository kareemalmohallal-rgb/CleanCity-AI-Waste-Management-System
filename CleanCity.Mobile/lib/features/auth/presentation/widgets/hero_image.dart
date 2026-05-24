import 'package:flutter/material.dart';

class HeroImage extends StatefulWidget {
  final String imagePath;
  final double width;
  final double height;

  const HeroImage({
    super.key,
    required this.imagePath,
    required this.width,
    required this.height,
  });

  @override
  State<HeroImage> createState() => _HeroImageState();
}

class _HeroImageState extends State<HeroImage> {
  bool _isImageLoading = true;

  @override
  void initState() {
    super.initState();
    // محاكاة تحميل الصورة
    Future.delayed(const Duration(milliseconds: 500), () {
      if (mounted) {
        setState(() {
          _isImageLoading = false;
        });
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      width: widget.width,
      height: widget.height,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(24),
        color: _isImageLoading ? Colors.grey[200] : null,
        image: !_isImageLoading
            ? DecorationImage(
                image: AssetImage(widget.imagePath),
                fit: BoxFit.cover,
              )
            : null,
      ),
      child: _isImageLoading
          ? const Center(
              child: CircularProgressIndicator(
                color: Color(0xFF29F14D),
              ),
            )
          : null,
    );
  }
}
