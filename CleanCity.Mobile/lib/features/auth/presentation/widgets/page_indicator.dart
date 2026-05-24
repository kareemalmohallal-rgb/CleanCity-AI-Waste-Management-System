import 'package:flutter/material.dart';

class PageIndicator extends StatelessWidget {
  final int count;
  final int currentIndex;

  const PageIndicator({
    super.key,
    required this.count,
    required this.currentIndex,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: List.generate(count, (index) {
        bool isActive = index == currentIndex;
        return AnimatedContainer(
          duration: const Duration(milliseconds: 250),
          margin: const EdgeInsets.symmetric(horizontal: 6),
          width: isActive ? 12 : 8,
          height: 8,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            color: isActive ? const Color(0xFF29F14D) : const Color(0xFFD1D5DB),
          ),
        );
      }),
    );
  }
}
