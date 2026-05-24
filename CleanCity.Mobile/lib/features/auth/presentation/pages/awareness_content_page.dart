import 'dart:math';
import 'package:cleancityapp/features/auth/domain/entities/awareness_content_entity.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../../../injection_container.dart';
import '../../domain/usecases/awareness/get_awareness_contents_usecase.dart';

// ══════════════════════════════════════════════
// صفحة قائمة المقالات التوعوية
// ══════════════════════════════════════════════
class AwarenessContentPage extends StatefulWidget {
  const AwarenessContentPage({super.key});

  @override
  State<AwarenessContentPage> createState() => _AwarenessContentPageState();
}

class _AwarenessContentPageState extends State<AwarenessContentPage> {
  late final GetAwarenessContentsUseCase _getAwarenessContentsUseCase;
  final TextEditingController _searchController = TextEditingController();

  bool _loading = true;
  String? _error;
  List<AwarenessContent> _items = [];
  String _query = '';

  @override
  void initState() {
    super.initState();
    _getAwarenessContentsUseCase = sl<GetAwarenessContentsUseCase>();
    _load();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final items = await _getAwarenessContentsUseCase();
      if (!mounted) return;
      setState(() {
        _items = items;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.toString();
        _loading = false;
      });
    }
  }

  List<AwarenessContent> get _filteredItems {
    final q = _query.trim().toLowerCase();
    if (q.isEmpty) return _items;
    return _items.where((item) {
      return item.title.toLowerCase().contains(q) ||
          item.content.toLowerCase().contains(q);
    }).toList();
  }

  @override
  Widget build(BuildContext context) {
    final items = _filteredItems;

    return Scaffold(
      backgroundColor: const Color(0xFFF5F5F5),
      appBar: AppBar(
        title: const Text(
          'المحتوى التوعوي',
          style: TextStyle(
            fontSize: 20,
            fontWeight: FontWeight.bold,
            color: Colors.white,
          ),
        ),
        centerTitle: true,
        backgroundColor: const Color.fromARGB(255, 87, 211, 157),
        elevation: 2,
        shadowColor: Colors.white,
        iconTheme: const IconThemeData(color: Colors.white),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.search_rounded),
            onPressed: () => _showSearchSheet(context),
            tooltip: 'بحث',
          ),
          const SizedBox(width: 4),
        ],
        // شريط نتائج البحث عند التفعيل
        bottom: _query.isNotEmpty
            ? PreferredSize(
                preferredSize: const Size.fromHeight(36),
                child: Container(
                  color: const Color.fromARGB(255, 70, 195, 143),
                  padding: const EdgeInsets.fromLTRB(16, 6, 16, 8),
                  child: Row(
                    children: [
                      const Icon(
                        Icons.search_rounded,
                        size: 16,
                        color: Colors.white,
                      ),
                      const SizedBox(width: 6),
                      Expanded(
                        child: Text(
                          'نتائج البحث عن: "$_query"',
                          style: const TextStyle(
                            fontSize: 12.5,
                            fontWeight: FontWeight.w600,
                            color: Colors.white,
                          ),
                        ),
                      ),
                      GestureDetector(
                        onTap: () => setState(() {
                          _query = '';
                          _searchController.clear();
                        }),
                        child: const Text(
                          'مسح',
                          style: TextStyle(
                            fontSize: 12.5,
                            fontWeight: FontWeight.w700,
                            color: Colors.white,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              )
            : null,
      ),
      body: RefreshIndicator(
        onRefresh: _load,
        color: const Color.fromARGB(255, 87, 211, 157),
        child: _buildBody(items),
      ),
    );
  }

  void _showSearchSheet(BuildContext context) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (ctx) => Padding(
        padding: EdgeInsets.only(
          bottom: MediaQuery.of(ctx).viewInsets.bottom,
          left: 16,
          right: 16,
          top: 20,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // مؤشر السحب
            Container(
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: Colors.grey[300],
                borderRadius: BorderRadius.circular(2),
              ),
            ),
            const SizedBox(height: 20),
            TextField(
              controller: _searchController,
              autofocus: true,
              decoration: InputDecoration(
                hintText: 'ابحث في المحتوى التوعوي...',
                prefixIcon: const Icon(
                  Icons.search_rounded,
                  color: Color.fromARGB(255, 87, 211, 157),
                ),
                filled: true,
                fillColor: const Color(0xFFF5F5F5),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(14),
                  borderSide: BorderSide.none,
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(14),
                  borderSide: const BorderSide(
                    color: Color.fromARGB(255, 87, 211, 157),
                    width: 1.5,
                  ),
                ),
                contentPadding: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 14,
                ),
              ),
              onChanged: (v) => setState(() => _query = v),
              onSubmitted: (_) => Navigator.pop(ctx),
            ),
            const SizedBox(height: 20),
          ],
        ),
      ),
    );
  }

  Widget _buildBody(List<AwarenessContent> items) {
    if (_loading) {
      return const Center(
        child: CircularProgressIndicator(
          color: Color.fromARGB(255, 87, 211, 157),
        ),
      );
    }

    if (_error != null) {
      return _ErrorState(message: _error!, onRetry: _load);
    }

    if (items.isEmpty) {
      return const _EmptyState(
        title: 'لا توجد نتائج',
        subtitle: 'جرّب البحث بكلمة مختلفة أو اسحب للتحديث.',
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.symmetric(vertical: 12),
      itemCount: items.length,
      itemBuilder: (context, index) {
        if (index == 0) {
          return Padding(
            padding: const EdgeInsets.fromLTRB(16, 0, 16, 12),
            child: _FeaturedArticleCard(item: items[index]),
          );
        }
        return Padding(
          padding: const EdgeInsets.fromLTRB(16, 0, 16, 12),
          child: _ArticleCard(item: items[index]),
        );
      },
    );
  }
}

// ══════════════════════════════════════════════
// البطاقة المميزة (أول عنصر — كبير)
// ══════════════════════════════════════════════
class _FeaturedArticleCard extends StatelessWidget {
  final AwarenessContent item;
  const _FeaturedArticleCard({required this.item});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => _navigateToArticle(context, item),
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(20),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.06),
              blurRadius: 16,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // صورة المقال
            ClipRRect(
              borderRadius: const BorderRadius.vertical(
                top: Radius.circular(20),
              ),
              child: AspectRatio(
                aspectRatio: 16 / 9,
                child: _ArticleImage(imageUrl: item.imageUrl),
              ),
            ),
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 14, 16, 16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      const _CategoryChip(text: 'توعوي'),
                      const SizedBox(width: 10),
                      Icon(
                        Icons.schedule_outlined,
                        size: 13,
                        color: Colors.grey[500],
                      ),
                      const SizedBox(width: 4),
                      Text(
                        'قراءة لمدة ${_readingTime(item.content)} دقائق',
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey[500],
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  Text(
                    item.title,
                    style: const TextStyle(
                      fontSize: 20,
                      fontWeight: FontWeight.w900,
                      color: Color(0xFF111111),
                      height: 1.35,
                      letterSpacing: -0.4,
                    ),
                  ),
                  const SizedBox(height: 12),
                  // معلومات الكاتب والتاريخ
                  Row(
                    children: [
                      Container(
                        width: 30,
                        height: 30,
                        decoration: BoxDecoration(
                          color: const Color.fromARGB(
                            255,
                            87,
                            211,
                            157,
                          ).withValues(alpha: 0.15),
                          shape: BoxShape.circle,
                        ),
                        child: const Icon(
                          Icons.eco_outlined,
                          size: 16,
                          color: Color.fromARGB(255, 87, 211, 157),
                        ),
                      ),
                      const SizedBox(width: 8),
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            'فريق التوعية البيئية',
                            style: TextStyle(
                              fontSize: 12.5,
                              fontWeight: FontWeight.w700,
                              color: Color(0xFF333333),
                            ),
                          ),
                          Text(
                            DateFormat(
                              'd MMMM yyyy',
                              'ar',
                            ).format(item.createdAt),
                            style: TextStyle(
                              fontSize: 11.5,
                              color: Colors.grey[500],
                              fontWeight: FontWeight.w500,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// ══════════════════════════════════════════════
// بطاقة المقال العادية
// ══════════════════════════════════════════════
class _ArticleCard extends StatelessWidget {
  final AwarenessContent item;
  const _ArticleCard({required this.item});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => _navigateToArticle(context, item),
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.05),
              blurRadius: 12,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Row(
          children: [
            ClipRRect(
              borderRadius: const BorderRadius.horizontal(
                right: Radius.circular(16),
              ),
              child: SizedBox(
                width: 110,
                height: 110,
                child: _ArticleImage(imageUrl: item.imageUrl),
              ),
            ),
            Expanded(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(12, 12, 14, 12),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        const _CategoryChip(text: 'توعوي', small: true),
                        const Spacer(),
                        Text(
                          '${_readingTime(item.content)} د',
                          style: TextStyle(
                            fontSize: 11,
                            color: Colors.grey[400],
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 7),
                    Text(
                      item.title,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        fontSize: 14.5,
                        fontWeight: FontWeight.w800,
                        color: Color(0xFF111111),
                        height: 1.4,
                        letterSpacing: -0.2,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      DateFormat('d MMM yyyy', 'ar').format(item.createdAt),
                      style: TextStyle(
                        fontSize: 11.5,
                        color: Colors.grey[400],
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// ══════════════════════════════════════════════
// صفحة قراءة المقال الكاملة
// ══════════════════════════════════════════════
void _navigateToArticle(BuildContext context, AwarenessContent item) {
  Navigator.push(
    context,
    MaterialPageRoute(builder: (_) => ArticleReaderPage(item: item)),
  );
}

class ArticleReaderPage extends StatelessWidget {
  final AwarenessContent item;
  const ArticleReaderPage({super.key, required this.item});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: CustomScrollView(
        slivers: [
          // ─── AppBar بهوية التطبيق ──────────────────────────────────────
          SliverAppBar(
            pinned: true,
            expandedHeight: 260,
            backgroundColor: const Color.fromARGB(255, 87, 211, 157),
            surfaceTintColor: const Color.fromARGB(255, 87, 211, 157),
            foregroundColor: Colors.white,
            elevation: 2,
            shadowColor: Colors.white,
            iconTheme: const IconThemeData(color: Colors.white),
            leading: IconButton(
              icon: const Icon(Icons.arrow_back),
              onPressed: () => Navigator.pop(context),
            ),
            // ✅ تم إزالة أزرار المشاركة والحفظ
            flexibleSpace: FlexibleSpaceBar(
              // عنوان المقال يظهر عند الانكماش
              title: Text(
                item.title,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: Colors.white,
                ),
              ),
              titlePadding: const EdgeInsets.only(
                right: 16,
                bottom: 14,
                left: 56,
              ),
              background: Stack(
                fit: StackFit.expand,
                children: [
                  _ArticleImage(imageUrl: item.imageUrl),
                  // تدرج لتحسين قراءة العنوان
                  DecoratedBox(
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        begin: Alignment.topCenter,
                        end: Alignment.bottomCenter,
                        colors: [
                          const Color.fromARGB(
                            255,
                            87,
                            211,
                            157,
                          ).withValues(alpha: 0.4),
                          Colors.black.withValues(alpha: 0.35),
                        ],
                        stops: const [0.0, 1.0],
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),

          // ─── محتوى المقال ─────────────────────────────────────────────
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(20, 22, 20, 40),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // التصنيف ووقت القراءة
                  Row(
                    children: [
                      const _CategoryChip(text: 'توعوي'),
                      const SizedBox(width: 12),
                      Icon(
                        Icons.schedule_outlined,
                        size: 14,
                        color: Colors.grey[500],
                      ),
                      const SizedBox(width: 4),
                      Text(
                        'قراءة لمدة ${_readingTime(item.content)} دقائق',
                        style: TextStyle(
                          fontSize: 12.5,
                          color: Colors.grey[500],
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 14),

                  // عنوان المقال
                  Text(
                    item.title,
                    style: const TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.w900,
                      color: Color(0xFF0D0D0D),
                      height: 1.35,
                      letterSpacing: -0.5,
                    ),
                  ),
                  const SizedBox(height: 16),

                  // معلومات الكاتب والتاريخ
                  Row(
                    children: [
                      Container(
                        width: 38,
                        height: 38,
                        decoration: BoxDecoration(
                          color: const Color.fromARGB(
                            255,
                            87,
                            211,
                            157,
                          ).withValues(alpha: 0.15),
                          shape: BoxShape.circle,
                        ),
                        child: const Icon(
                          Icons.eco_outlined,
                          size: 18,
                          color: Color.fromARGB(255, 87, 211, 157),
                        ),
                      ),
                      const SizedBox(width: 10),
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            'فريق التوعية البيئية',
                            style: TextStyle(
                              fontSize: 13.5,
                              fontWeight: FontWeight.w700,
                              color: Color(0xFF222222),
                            ),
                          ),
                          Text(
                            DateFormat(
                              'd MMMM yyyy',
                              'ar',
                            ).format(item.createdAt),
                            style: TextStyle(
                              fontSize: 12,
                              color: Colors.grey[500],
                              fontWeight: FontWeight.w500,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                  const SizedBox(height: 22),

                  Divider(color: Colors.grey[200]),
                  const SizedBox(height: 20),

                  // نص المقال
                  _buildArticleContent(item.content),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildArticleContent(String content) {
    final paragraphs = content
        .split('\n')
        .map((p) => p.trim())
        .where((p) => p.isNotEmpty)
        .toList();

    if (paragraphs.isEmpty) {
      return Text(
        content,
        style: const TextStyle(
          fontSize: 16,
          height: 1.85,
          color: Color(0xFF2C2C2E),
          fontWeight: FontWeight.w400,
          letterSpacing: 0.1,
        ),
      );
    }

    final List<Widget> widgets = [];

    for (int i = 0; i < paragraphs.length; i++) {
      final p = paragraphs[i];
      final isHeading = p.length < 40 && paragraphs.length > 1;
      final addQuote = i == 2 && paragraphs.length > 3;

      if (isHeading && i > 0) {
        widgets.add(const SizedBox(height: 24));
        widgets.add(
          Text(
            p,
            style: const TextStyle(
              fontSize: 19,
              fontWeight: FontWeight.w900,
              color: Color(0xFF111111),
              letterSpacing: -0.3,
              height: 1.4,
            ),
          ),
        );
        widgets.add(const SizedBox(height: 10));
      } else {
        if (i > 0 && !isHeading) widgets.add(const SizedBox(height: 14));
        widgets.add(
          Text(
            p,
            style: const TextStyle(
              fontSize: 16,
              height: 1.85,
              color: Color(0xFF2C2C2E),
              fontWeight: FontWeight.w400,
              letterSpacing: 0.1,
            ),
          ),
        );
      }

      if (addQuote) {
        widgets.add(const SizedBox(height: 22));
        widgets.add(_QuoteBlock(text: _extractQuote(p)));
        widgets.add(const SizedBox(height: 22));
      }
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: widgets,
    );
  }

  String _extractQuote(String text) {
    final words = text.split(' ');
    final len = min(words.length, 12);
    return '"${words.take(len).join(' ')}..."';
  }
}

// ══════════════════════════════════════════════
// صفحة جلب وفتح مقال مباشرة بواسطة ID
// تُستخدم من صفحة الإشعارات
// ══════════════════════════════════════════════
class AwarenessArticleByIdPage extends StatefulWidget {
  final int articleId;
  const AwarenessArticleByIdPage({super.key, required this.articleId});

  @override
  State<AwarenessArticleByIdPage> createState() =>
      _AwarenessArticleByIdPageState();
}

class _AwarenessArticleByIdPageState extends State<AwarenessArticleByIdPage> {
  @override
  void initState() {
    super.initState();
    _loadAndNavigate();
  }

  Future<void> _loadAndNavigate() async {
    try {
      final useCase = sl<GetAwarenessContentsUseCase>();
      final items = await useCase();
      if (!mounted) return;

      final article = items.firstWhere(
        (a) => a.id == widget.articleId,
        orElse: () =>
            throw Exception('المقال رقم #${widget.articleId} غير موجود'),
      );

      Navigator.pushReplacement(
        context,
        MaterialPageRoute(builder: (_) => ArticleReaderPage(item: article)),
      );
    } catch (e) {
      if (!mounted) return;
      Navigator.pop(context);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('تعذر فتح المقال: $e'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        backgroundColor: const Color.fromARGB(255, 87, 211, 157),
        elevation: 2,
        shadowColor: Colors.white,
        iconTheme: const IconThemeData(color: Colors.white),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            CircularProgressIndicator(color: Color.fromARGB(255, 87, 211, 157)),
            SizedBox(height: 16),
            Text(
              'جارٍ تحميل المقال...',
              style: TextStyle(color: Color(0xFF666666), fontSize: 14),
            ),
          ],
        ),
      ),
    );
  }
}

// ══════════════════════════════════════════════
// مكونات مساعدة
// ══════════════════════════════════════════════

class _ArticleImage extends StatelessWidget {
  final String? imageUrl;
  const _ArticleImage({this.imageUrl});

  @override
  Widget build(BuildContext context) {
    if (imageUrl == null || imageUrl!.trim().isEmpty) {
      return Container(
        color: const Color(0xFFE8F5F0),
        alignment: Alignment.center,
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.eco_outlined,
              size: 48,
              color: const Color.fromARGB(
                255,
                87,
                211,
                157,
              ).withValues(alpha: 0.6),
            ),
            const SizedBox(height: 8),
            const Text(
              'CleanCity',
              style: TextStyle(
                color: Color.fromARGB(255, 87, 211, 157),
                fontWeight: FontWeight.w700,
                fontSize: 14,
              ),
            ),
          ],
        ),
      );
    }

    return Image.network(
      imageUrl!,
      fit: BoxFit.cover,
      errorBuilder: (_, __, ___) => Container(
        color: const Color(0xFFEAEAEE),
        alignment: Alignment.center,
        child: const Icon(
          Icons.broken_image_outlined,
          color: Color(0xFF8E8E93),
          size: 30,
        ),
      ),
      loadingBuilder: (context, child, progress) {
        if (progress == null) return child;
        return Container(
          color: const Color(0xFFF0F0F0),
          alignment: Alignment.center,
          child: CircularProgressIndicator(
            value: progress.expectedTotalBytes != null
                ? progress.cumulativeBytesLoaded / progress.expectedTotalBytes!
                : null,
            strokeWidth: 2,
            color: const Color.fromARGB(255, 87, 211, 157),
          ),
        );
      },
    );
  }
}

class _CategoryChip extends StatelessWidget {
  final String text;
  final bool small;
  const _CategoryChip({required this.text, this.small = false});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.symmetric(
        horizontal: small ? 8 : 10,
        vertical: small ? 3 : 4,
      ),
      decoration: BoxDecoration(
        color: const Color.fromARGB(255, 87, 211, 157).withValues(alpha: 0.15),
        borderRadius: BorderRadius.circular(20),
      ),
      child: Text(
        text,
        style: TextStyle(
          fontSize: small ? 10.5 : 11.5,
          fontWeight: FontWeight.w700,
          color: const Color.fromARGB(255, 55, 175, 122),
          letterSpacing: 0.1,
        ),
      ),
    );
  }
}

class _QuoteBlock extends StatelessWidget {
  final String text;
  const _QuoteBlock({required this.text});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.fromLTRB(20, 16, 12, 16),
      decoration: BoxDecoration(
        color: const Color(0xFFF0FBF6),
        borderRadius: BorderRadius.circular(14),
        border: const Border(
          right: BorderSide(color: Color.fromARGB(255, 87, 211, 157), width: 4),
        ),
      ),
      child: Text(
        text,
        style: const TextStyle(
          fontSize: 16.5,
          fontStyle: FontStyle.italic,
          fontWeight: FontWeight.w600,
          color: Color(0xFF333333),
          height: 1.7,
          letterSpacing: 0.1,
        ),
      ),
    );
  }
}

// ─── Empty & Error States ──────────────────────────────────────────────────

class _EmptyState extends StatelessWidget {
  final String title;
  final String subtitle;
  const _EmptyState({required this.title, required this.subtitle});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.article_outlined, size: 56, color: Colors.grey[300]),
            const SizedBox(height: 16),
            Text(
              title,
              style: const TextStyle(
                fontSize: 17,
                fontWeight: FontWeight.w800,
                color: Color(0xFF333333),
              ),
            ),
            const SizedBox(height: 8),
            Text(
              subtitle,
              textAlign: TextAlign.center,
              style: TextStyle(
                fontSize: 13.5,
                color: Colors.grey[500],
                height: 1.5,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ErrorState extends StatelessWidget {
  final String message;
  final Future<void> Function() onRetry;
  const _ErrorState({required this.message, required this.onRetry});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.wifi_off_rounded, size: 52, color: Colors.grey[300]),
            const SizedBox(height: 16),
            const Text(
              'تعذر تحميل البيانات',
              style: TextStyle(
                fontSize: 17,
                fontWeight: FontWeight.w800,
                color: Color(0xFF333333),
              ),
            ),
            const SizedBox(height: 8),
            Text(
              message,
              textAlign: TextAlign.center,
              style: TextStyle(
                fontSize: 13,
                color: Colors.grey[500],
                height: 1.5,
              ),
            ),
            const SizedBox(height: 20),
            ElevatedButton.icon(
              onPressed: onRetry,
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color.fromARGB(255, 87, 211, 157),
                foregroundColor: Colors.white,
                elevation: 0,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                padding: const EdgeInsets.symmetric(
                  horizontal: 24,
                  vertical: 12,
                ),
              ),
              icon: const Icon(Icons.refresh_rounded, size: 18),
              label: const Text(
                'إعادة المحاولة',
                style: TextStyle(fontWeight: FontWeight.w700),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// ─── دوال مساعدة ──────────────────────────────────────────────────────────

int _readingTime(String content) {
  final wordCount = content.split(RegExp(r'\s+')).length;
  return max(1, (wordCount / 200).ceil());
}
