import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:cleancityapp/features/auth/presentation/pages/main_home_page.dart';
import 'package:cleancityapp/features/auth/presentation/widgets/hero_image.dart';
import 'package:shared_preferences/shared_preferences.dart';

class OnboardingScreen extends StatefulWidget {
  const OnboardingScreen({super.key});

  @override
  State<OnboardingScreen> createState() => _OnboardingScreenState();
}

class _OnboardingScreenState extends State<OnboardingScreen> {
  final PageController _pageController = PageController();
  int _currentPage = 0;

  final List<Map<String, String>> _pagesData = [
    {
      'title': 'ابلغ عن النفايات بسهولة',
      'subtitle':
          'ابلغ عن النفايات بسهولة وفعالية\nباستخدام تطبيقنا. ساعدنا في الحفاظ على مدينة صنعاء نظيفة',
      'image': 'images/1.png', //صورة المدينة النظيفة
    },
    {
      'title': 'تتبع البلاغ في الوقت الفعلي',
      'subtitle':
          'ابقَ على اطلاع بحالة بلاغاتك من خلال التحديثات في الوقت الفعلي',
      'image': 'images/2.png', //صورة المدينة النظيفة
    },
    {
      'title': 'ساهم في نظافة المدينة',
      'subtitle':
          'انضم إلينا في مهمتنا للحفاظ على بيئة نظيفة وصحية للجميع في صنعاء',
      'image': 'images/3.png', //صورة المدينة النظيفة
    },
  ];

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  void _nextPage() {
    if (_currentPage < _pagesData.length - 1) {
      _pageController.nextPage(
        duration: const Duration(milliseconds: 260),
        curve: Curves.easeOutCubic,
      );
    } else {
      _goToHomePage();
    }
  }

  /// ✅ حفظ flag أن المستخدم شاهد الـ onboarding ثم الانتقال للرئيسية
  Future<void> _goToHomePage() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setBool('onboarding_seen', true);

    if (!mounted) return;

    Navigator.pushAndRemoveUntil(
      context,
      MaterialPageRoute(builder: (_) => const MainHomePage()),
      (route) => false,
    );
  }

  void _skipToHome() => _goToHomePage();

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;

    return Directionality(
      textDirection: TextDirection.rtl,
      child: Scaffold(
        backgroundColor: const Color(0xFFF2F2F7),
        body: SafeArea(
          child: Stack(
            children: [
              Positioned.fill(
                child: DecoratedBox(
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      begin: Alignment.topCenter,
                      end: Alignment.bottomCenter,
                      colors: [
                        Colors.white.withValues(alpha: 0.90),
                        const Color(0xFFF2F2F7),
                      ],
                    ),
                  ),
                  child: PageView.builder(
                    controller: _pageController,
                    itemCount: _pagesData.length,
                    physics: const BouncingScrollPhysics(),
                    onPageChanged: (index) =>
                        setState(() => _currentPage = index),
                    itemBuilder: (context, index) {
                      final page = _pagesData[index];
                      return _OnboardingPage(
                        title: page['title']!,
                        subtitle: page['subtitle']!,
                        imagePath: page['image']!,
                        imageSize: size.width * 0.70,
                      );
                    },
                  ),
                ),
              ),

              Positioned(
                top: 10,
                left: 16,
                right: 16,
                child: _TopGlassBar(
                  showSkip: _currentPage != _pagesData.length - 1,
                  onSkip: _skipToHome,
                ),
              ),

              Positioned(
                bottom: 18,
                left: 16,
                right: 16,
                child: _BottomControls(
                  count: _pagesData.length,
                  currentIndex: _currentPage,
                  primaryText: _currentPage == _pagesData.length - 1
                      ? 'ابدأ الآن'
                      : 'التالي',
                  onPrimaryPressed: _nextPage,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _OnboardingPage extends StatelessWidget {
  final String title;
  final String subtitle;
  final String imagePath;
  final double imageSize;

  const _OnboardingPage({
    required this.title,
    required this.subtitle,
    required this.imagePath,
    required this.imageSize,
  });

  @override
  Widget build(BuildContext context) {
    final viewPadding = MediaQuery.of(context).padding;
    final minH =
        MediaQuery.of(context).size.height -
        viewPadding.top -
        viewPadding.bottom;

    return SingleChildScrollView(
      physics: const BouncingScrollPhysics(),
      child: ConstrainedBox(
        constraints: BoxConstraints(minHeight: minH),
        child: Padding(
          padding: const EdgeInsets.fromLTRB(24, 70, 24, 140),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              ClipRRect(
                borderRadius: BorderRadius.circular(26),
                child: BackdropFilter(
                  filter: ImageFilter.blur(sigmaX: 18, sigmaY: 18),
                  child: Container(
                    padding: const EdgeInsets.all(18),
                    decoration: BoxDecoration(
                      color: Colors.white.withValues(alpha: 0.70),
                      borderRadius: BorderRadius.circular(26),
                      border: Border.all(
                        color: Colors.black.withValues(alpha: 0.06),
                      ),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withValues(alpha: 0.06),
                          blurRadius: 26,
                          offset: const Offset(0, 14),
                        ),
                      ],
                    ),
                    child: HeroImage(
                      imagePath: imagePath,
                      width: imageSize,
                      height: imageSize,
                    ),
                  ),
                ),
              ),

              const SizedBox(height: 24),

              Text(
                title,
                textAlign: TextAlign.center,
                style: const TextStyle(
                  fontSize: 26,
                  fontWeight: FontWeight.w900,
                  letterSpacing: -0.3,
                  color: Color(0xFF111111),
                  height: 1.25,
                ),
              ),

              const SizedBox(height: 12),

              Text(
                subtitle,
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.w600,
                  color: const Color(0xFF3C3C43).withValues(alpha: 0.68),
                  height: 1.55,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _TopGlassBar extends StatelessWidget {
  final bool showSkip;
  final VoidCallback onSkip;

  const _TopGlassBar({required this.showSkip, required this.onSkip});

  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      borderRadius: BorderRadius.circular(16),
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 18, sigmaY: 18),
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
          decoration: BoxDecoration(
            color: Colors.white.withValues(alpha: 0.72),

            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: Colors.black.withValues(alpha: 0.06)),
          ),
          child: Row(
            children: [
              const Expanded(
                child: Text(
                  'Clean City',
                  style: TextStyle(
                    fontSize: 13.5,
                    fontWeight: FontWeight.w800,
                    letterSpacing: 0.1,
                    color: Color(0xFF111111),
                  ),
                  overflow: TextOverflow.ellipsis,
                ),
              ),
              if (showSkip)
                TextButton(
                  onPressed: onSkip,
                  style: TextButton.styleFrom(
                    foregroundColor: const Color(0xFF0A84FF),
                    padding: const EdgeInsets.symmetric(
                      horizontal: 12,
                      vertical: 8,
                    ),
                    textStyle: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w800,
                      letterSpacing: -0.1,
                    ),
                  ),
                  child: const Text('تخطي'),
                ),
            ],
          ),
        ),
      ),
    );
  }
}

class _BottomControls extends StatelessWidget {
  final int count;
  final int currentIndex;
  final String primaryText;
  final VoidCallback onPrimaryPressed;

  const _BottomControls({
    required this.count,
    required this.currentIndex,
    required this.primaryText,
    required this.onPrimaryPressed,
  });

  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      borderRadius: BorderRadius.circular(22),
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 20, sigmaY: 20),
        child: Container(
          padding: const EdgeInsets.fromLTRB(16, 14, 16, 14),
          decoration: BoxDecoration(
            color: Colors.white.withValues(alpha: 0.76),
            borderRadius: BorderRadius.circular(22),
            border: Border.all(color: Colors.black.withValues(alpha: 0.06)),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.08),
                blurRadius: 22,
                offset: const Offset(0, 12),
              ),
            ],
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              _PillIndicators(count: count, currentIndex: currentIndex),
              const SizedBox(height: 12),
              SizedBox(
                width: double.infinity,
                height: 52,
                child: ElevatedButton(
                  onPressed: onPrimaryPressed,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF0A84FF),
                    foregroundColor: Colors.white,
                    elevation: 0,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(16),
                    ),
                  ),
                  child: Text(
                    primaryText,
                    style: const TextStyle(
                      fontSize: 16.5,
                      fontWeight: FontWeight.w900,
                      letterSpacing: -0.2,
                    ),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _PillIndicators extends StatelessWidget {
  final int count;
  final int currentIndex;

  const _PillIndicators({required this.count, required this.currentIndex});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: List.generate(count, (i) {
        final active = i == currentIndex;
        return AnimatedContainer(
          duration: const Duration(milliseconds: 220),
          curve: Curves.easeOut,
          margin: const EdgeInsets.symmetric(horizontal: 4),
          width: active ? 22 : 8,
          height: 8,
          decoration: BoxDecoration(
            color: active
                ? const Color(0xFF0A84FF)
                : const Color(0xFF8E8E93).withValues(alpha: 0.35),
            borderRadius: BorderRadius.circular(999),
          ),
        );
      }),
    );
  }
}
