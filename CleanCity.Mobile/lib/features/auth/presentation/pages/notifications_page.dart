// import 'package:cleancityapp/core/network/api_factory.dart';
// import 'package:cleancityapp/core/notifications/notification_service.dart';
// import 'package:cleancityapp/features/auth/data/datasources/notifications_remote_data_source.dart';
// import 'package:cleancityapp/features/auth/data/models/notification_recipient_model.dart';
// import 'package:flutter/material.dart';

// class NotificationsPage extends StatefulWidget {
//   final int? driverId;
//   final int? anonymousDeviceId;

//   const NotificationsPage({super.key, this.driverId, this.anonymousDeviceId});

//   @override
//   State<NotificationsPage> createState() => _NotificationsPageState();
// }

// class _NotificationsPageState extends State<NotificationsPage> {
//   late final NotificationsRemoteDataSource _remote;

//   bool _loading = true;
//   bool _markingRead = false;
//   String? _error;
//   List<NotificationRecipientModel> _notifications = [];

//   @override
//   void initState() {
//     super.initState();
//     _remote = NotificationsRemoteDataSourceImpl(ApiFactory.api);

//     // ✅ الاشتراك في callback وصول الإشعارات أثناء فتح التطبيق
//     NotificationService.instance.onForegroundNotificationReceived = () {
//       if (mounted) _loadNotifications();
//     };

//     _loadNotifications();
//   }

//   @override
//   void dispose() {
//     // ✅ إلغاء الاشتراك لتجنب memory leak عند مغادرة الصفحة
//     NotificationService.instance.onForegroundNotificationReceived = null;
//     super.dispose();
//   }

//   Future<void> _loadNotifications() async {
//     setState(() {
//       _loading = true;
//       _error = null;
//     });

//     try {
//       List<NotificationRecipientModel> items;

//       if (widget.driverId != null) {
//         items = await _remote.getForDriver(widget.driverId!);
//       } else if (widget.anonymousDeviceId != null) {
//         items = await _remote.getForAnonymousDevice(widget.anonymousDeviceId!);
//       } else {
//         throw Exception(
//           'لم يتم تمرير driverId أو anonymousDeviceId إلى الصفحة',
//         );
//       }

//       items.sort(
//         (a, b) => b.notification.createdAt.compareTo(a.notification.createdAt),
//       );

//       if (!mounted) return;
//       setState(() {
//         _notifications = items;
//         _loading = false;
//       });
//     } catch (e) {
//       if (!mounted) return;
//       setState(() {
//         _error = e.toString();
//         _loading = false;
//       });
//     }
//   }

//   int get _unreadCount => _notifications.where((n) => !n.isRead).length;

//   Future<void> _openNotification(NotificationRecipientModel item) async {
//     if (!item.isRead) {
//       await _markAsRead(item);
//     }

//     if (!mounted) return;
//     _showNotificationDetails(context, item);
//   }

//   Future<void> _markAsRead(NotificationRecipientModel item) async {
//     if (_markingRead) return;

//     setState(() => _markingRead = true);

//     try {
//       await _remote.markAsRead(item.recipientId);

//       final index = _notifications.indexWhere(
//         (n) => n.recipientId == item.recipientId,
//       );
//       if (index != -1) {
//         _notifications[index] = _notifications[index].copyWith(
//           isRead: true,
//           readAt: DateTime.now(),
//         );
//       }

//       if (!mounted) return;
//       setState(() {});
//     } catch (e) {
//       if (!mounted) return;
//       ScaffoldMessenger.of(context).showSnackBar(
//         SnackBar(
//           content: Text('تعذر تحديث حالة الإشعار: $e'),
//           backgroundColor: Colors.red,
//         ),
//       );
//     } finally {
//       if (mounted) {
//         setState(() => _markingRead = false);
//       }
//     }
//   }

//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       backgroundColor: const Color(0xFFF8FAFF),
//       appBar: AppBar(
//         title: const Text(
//           'الإشعارات',
//           style: TextStyle(
//             fontSize: 20,
//             fontWeight: FontWeight.bold,
//             color: Colors.white,
//           ),
//         ),
//         centerTitle: true,
//         backgroundColor: const Color.fromARGB(255, 87, 211, 157),
//         leading: IconButton(
//           icon: const Icon(Icons.arrow_back, color: Colors.white),
//           onPressed: () => Navigator.pop(context),
//         ),
//         actions: [
//           if (_unreadCount > 0)
//             Padding(
//               padding: const EdgeInsets.only(right: 16),
//               child: Center(
//                 child: Container(
//                   padding: const EdgeInsets.all(6),
//                   decoration: BoxDecoration(
//                     color: Colors.red,
//                     borderRadius: BorderRadius.circular(12),
//                   ),
//                   child: Text(
//                     '$_unreadCount',
//                     style: const TextStyle(
//                       color: Colors.white,
//                       fontSize: 12,
//                       fontWeight: FontWeight.bold,
//                     ),
//                   ),
//                 ),
//               ),
//             ),
//         ],
//         elevation: 2,
//         shape: const RoundedRectangleBorder(
//           borderRadius: BorderRadius.only(
//             bottomLeft: Radius.circular(16),
//             bottomRight: Radius.circular(16),
//           ),
//         ),
//       ),
//       body: Column(
//         children: [
//           Container(
//             padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
//             decoration: BoxDecoration(
//               color: Colors.white,
//               boxShadow: [
//                 BoxShadow(
//                   color: Colors.grey.withValues(alpha: 0.05),
//                   spreadRadius: 1,
//                   blurRadius: 4,
//                   offset: const Offset(0, 2),
//                 ),
//               ],
//             ),
//             child: Row(
//               mainAxisAlignment: MainAxisAlignment.spaceBetween,
//               children: [
//                 Text(
//                   'الإشعارات ($_unreadCount غير مقروء)',
//                   style: const TextStyle(
//                     fontSize: 14,
//                     color: Color(0xFF555555),
//                     fontWeight: FontWeight.w500,
//                   ),
//                 ),
//                 IconButton(
//                   onPressed: _loading ? null : _loadNotifications,
//                   icon: const Icon(Icons.refresh),
//                   tooltip: 'تحديث',
//                 ),
//               ],
//             ),
//           ),

//           Expanded(child: _buildBody()),
//         ],
//       ),
//     );
//   }

//   Widget _buildBody() {
//     if (_loading) {
//       return const Center(child: CircularProgressIndicator());
//     }

//     if (_error != null) {
//       return Center(
//         child: Padding(
//           padding: const EdgeInsets.all(24),
//           child: Column(
//             mainAxisAlignment: MainAxisAlignment.center,
//             children: [
//               Icon(Icons.wifi_off, size: 64, color: Colors.grey[400]),
//               const SizedBox(height: 16),
//               Text(
//                 'تعذر تحميل الإشعارات',
//                 style: TextStyle(fontSize: 18, color: Colors.grey[700]),
//               ),
//               const SizedBox(height: 8),
//               Text(
//                 _error!,
//                 textAlign: TextAlign.center,
//                 style: const TextStyle(color: Colors.red),
//               ),
//               const SizedBox(height: 16),
//               ElevatedButton(
//                 onPressed: _loadNotifications,
//                 child: const Text('إعادة المحاولة'),
//               ),
//             ],
//           ),
//         ),
//       );
//     }

//     if (_notifications.isEmpty) {
//       return Center(
//         child: Column(
//           mainAxisAlignment: MainAxisAlignment.center,
//           children: [
//             Icon(Icons.notifications_off, size: 64, color: Colors.grey[400]),
//             const SizedBox(height: 16),
//             const Text(
//               'لا توجد إشعارات',
//               style: TextStyle(fontSize: 18, color: Color(0xFF7B8D9E)),
//             ),
//             const SizedBox(height: 8),
//             const Text(
//               'ستظهر هنا التحديثات والإشعارات الجديدة',
//               style: TextStyle(color: Color(0xFF9AA5B5)),
//             ),
//           ],
//         ),
//       );
//     }

//     return RefreshIndicator(
//       onRefresh: _loadNotifications,
//       child: ListView.builder(
//         padding: const EdgeInsets.all(16),
//         itemCount: _notifications.length,
//         itemBuilder: (context, index) {
//           return _buildNotificationCard(_notifications[index]);
//         },
//       ),
//     );
//   }

//   Widget _buildNotificationCard(NotificationRecipientModel item) {
//     final ui = _mapUi(item);
//     final backgroundColor = item.isRead
//         ? Colors.white
//         : const Color(0xFFE8EAF6);

//     return Container(
//       margin: const EdgeInsets.only(bottom: 12),
//       decoration: BoxDecoration(
//         color: backgroundColor,
//         borderRadius: BorderRadius.circular(12),
//         boxShadow: [
//           BoxShadow(
//             color: Colors.grey.withValues(alpha: 0.1),
//             spreadRadius: 1,
//             blurRadius: 4,
//             offset: const Offset(0, 2),
//           ),
//         ],
//         border: item.isRead
//             ? null
//             : Border.all(
//                 color: const Color(0xFF3F51B5).withValues(alpha: 0.2),
//                 width: 1,
//               ),
//       ),
//       child: Material(
//         color: Colors.transparent,
//         child: InkWell(
//           borderRadius: BorderRadius.circular(12),
//           onTap: () => _openNotification(item),
//           child: Padding(
//             padding: const EdgeInsets.all(16),
//             child: Row(
//               crossAxisAlignment: CrossAxisAlignment.start,
//               children: [
//                 Container(
//                   width: 48,
//                   height: 48,
//                   decoration: BoxDecoration(
//                     color: ui.color.withValues(alpha: 0.1),
//                     borderRadius: BorderRadius.circular(10),
//                   ),
//                   child: Icon(ui.icon, color: ui.color, size: 24),
//                 ),
//                 const SizedBox(width: 16),
//                 Expanded(
//                   child: Column(
//                     crossAxisAlignment: CrossAxisAlignment.start,
//                     children: [
//                       Text(
//                         item.notification.title,
//                         style: TextStyle(
//                           fontSize: 16,
//                           fontWeight: item.isRead
//                               ? FontWeight.w500
//                               : FontWeight.bold,
//                           color: const Color(0xFF2C3E50),
//                         ),
//                       ),
//                       const SizedBox(height: 4),
//                       Text(
//                         item.notification.body,
//                         style: const TextStyle(
//                           fontSize: 14,
//                           color: Color(0xFF5A6C7D),
//                         ),
//                         maxLines: 2,
//                         overflow: TextOverflow.ellipsis,
//                       ),
//                       const SizedBox(height: 8),
//                       Row(
//                         mainAxisAlignment: MainAxisAlignment.spaceBetween,
//                         children: [
//                           Row(
//                             children: [
//                               Icon(
//                                 Icons.access_time,
//                                 color: Colors.grey[500],
//                                 size: 14,
//                               ),
//                               const SizedBox(width: 4),
//                               Text(
//                                 item.notification.formattedTime(),
//                                 style: TextStyle(
//                                   fontSize: 12,
//                                   color: Colors.grey[600],
//                                 ),
//                               ),
//                             ],
//                           ),
//                           if (item.notification.reportId != null)
//                             Container(
//                               padding: const EdgeInsets.symmetric(
//                                 horizontal: 8,
//                                 vertical: 4,
//                               ),
//                               decoration: BoxDecoration(
//                                 color: const Color(
//                                   0xFF3F51B5,
//                                 ).withValues(alpha: 0.1),
//                                 borderRadius: BorderRadius.circular(4),
//                               ),
//                               child: Text(
//                                 '#${item.notification.reportId}',
//                                 style: const TextStyle(
//                                   fontSize: 12,
//                                   color: Color(0xFF3F51B5),
//                                   fontWeight: FontWeight.w500,
//                                 ),
//                               ),
//                             ),
//                         ],
//                       ),
//                     ],
//                   ),
//                 ),
//                 if (!item.isRead)
//                   Padding(
//                     padding: const EdgeInsets.only(left: 8),
//                     child: Container(
//                       width: 8,
//                       height: 8,
//                       decoration: BoxDecoration(
//                         color: const Color(0xFFF44336),
//                         borderRadius: BorderRadius.circular(4),
//                       ),
//                     ),
//                   ),
//               ],
//             ),
//           ),
//         ),
//       ),
//     );
//   }

//   void _showNotificationDetails(
//     BuildContext context,
//     NotificationRecipientModel item,
//   ) {
//     final ui = _mapUi(item);

//     showModalBottomSheet(
//       context: context,
//       isScrollControlled: true,
//       shape: const RoundedRectangleBorder(
//         borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
//       ),
//       builder: (context) {
//         return Container(
//           padding: const EdgeInsets.all(24),
//           child: Column(
//             mainAxisSize: MainAxisSize.min,
//             crossAxisAlignment: CrossAxisAlignment.start,
//             children: [
//               Center(
//                 child: Container(
//                   width: 40,
//                   height: 4,
//                   decoration: BoxDecoration(
//                     color: Colors.grey[300],
//                     borderRadius: BorderRadius.circular(2),
//                   ),
//                 ),
//               ),
//               const SizedBox(height: 20),

//               Row(
//                 crossAxisAlignment: CrossAxisAlignment.start,
//                 children: [
//                   Container(
//                     width: 56,
//                     height: 56,
//                     decoration: BoxDecoration(
//                       color: ui.color.withValues(alpha: 0.1),
//                       borderRadius: BorderRadius.circular(12),
//                     ),
//                     child: Icon(ui.icon, color: ui.color, size: 28),
//                   ),
//                   const SizedBox(width: 16),
//                   Expanded(
//                     child: Column(
//                       crossAxisAlignment: CrossAxisAlignment.start,
//                       children: [
//                         Text(
//                           item.notification.title,
//                           style: const TextStyle(
//                             fontSize: 20,
//                             fontWeight: FontWeight.bold,
//                             color: Color(0xFF2C3E50),
//                           ),
//                         ),
//                         const SizedBox(height: 4),
//                         Row(
//                           children: [
//                             Icon(
//                               Icons.access_time,
//                               color: Colors.grey[500],
//                               size: 16,
//                             ),
//                             const SizedBox(width: 4),
//                             Expanded(
//                               child: Text(
//                                 item.notification.formattedTime(),
//                                 style: TextStyle(
//                                   fontSize: 14,
//                                   color: Colors.grey[600],
//                                 ),
//                               ),
//                             ),
//                           ],
//                         ),
//                       ],
//                     ),
//                   ),
//                   IconButton(
//                     icon: const Icon(Icons.close),
//                     onPressed: () => Navigator.pop(context),
//                   ),
//                 ],
//               ),

//               const SizedBox(height: 24),

//               Container(
//                 padding: const EdgeInsets.all(16),
//                 decoration: BoxDecoration(
//                   color: const Color(0xFFF8FAFF),
//                   borderRadius: BorderRadius.circular(12),
//                 ),
//                 child: Text(
//                   item.notification.body,
//                   style: const TextStyle(
//                     fontSize: 16,
//                     color: Color(0xFF34495E),
//                     height: 1.6,
//                   ),
//                 ),
//               ),

//               if (item.notification.reportId != null) ...[
//                 const SizedBox(height: 16),
//                 Container(
//                   padding: const EdgeInsets.all(16),
//                   decoration: BoxDecoration(
//                     color: const Color(0xFFE8EAF6),
//                     borderRadius: BorderRadius.circular(12),
//                   ),
//                   child: Row(
//                     children: [
//                       const Icon(Icons.report, color: Color(0xFF3F51B5)),
//                       const SizedBox(width: 12),
//                       Expanded(
//                         child: Column(
//                           crossAxisAlignment: CrossAxisAlignment.start,
//                           children: [
//                             const Text(
//                               'رقم البلاغ المرتبط',
//                               style: TextStyle(
//                                 fontSize: 14,
//                                 color: Color(0xFF5A6C7D),
//                               ),
//                             ),
//                             const SizedBox(height: 4),
//                             Text(
//                               '#${item.notification.reportId}',
//                               style: const TextStyle(
//                                 fontSize: 16,
//                                 fontWeight: FontWeight.bold,
//                                 color: Color(0xFF3F51B5),
//                               ),
//                             ),
//                           ],
//                         ),
//                       ),
//                     ],
//                   ),
//                 ),
//               ],

//               if (item.notification.awarenessContentId != null) ...[
//                 const SizedBox(height: 16),
//                 Container(
//                   padding: const EdgeInsets.all(16),
//                   decoration: BoxDecoration(
//                     color: const Color(0xFFE8FFF2),
//                     borderRadius: BorderRadius.circular(12),
//                   ),
//                   child: Row(
//                     children: [
//                       const Icon(Icons.menu_book, color: Color(0xFF2E7D32)),
//                       const SizedBox(width: 12),
//                       Expanded(
//                         child: Text(
//                           'مقال توعوي رقم #${item.notification.awarenessContentId}',
//                           style: const TextStyle(
//                             fontSize: 15,
//                             fontWeight: FontWeight.w600,
//                             color: Color(0xFF2E7D32),
//                           ),
//                         ),
//                       ),
//                     ],
//                   ),
//                 ),
//               ],

//               const SizedBox(height: 24),

//               SizedBox(
//                 width: double.infinity,
//                 child: ElevatedButton(
//                   onPressed: () => Navigator.pop(context),
//                   style: ElevatedButton.styleFrom(
//                     backgroundColor: const Color(0xFF3F51B5),
//                     padding: const EdgeInsets.symmetric(vertical: 16),
//                     shape: RoundedRectangleBorder(
//                       borderRadius: BorderRadius.circular(12),
//                     ),
//                   ),
//                   child: const Text(
//                     'حسناً',
//                     style: TextStyle(
//                       fontSize: 16,
//                       fontWeight: FontWeight.bold,
//                       color: Colors.white,
//                     ),
//                   ),
//                 ),
//               ),
//             ],
//           ),
//         );
//       },
//     );
//   }

//   _NotificationUi _mapUi(NotificationRecipientModel item) {
//     if (item.notification.awarenessContentId != null) {
//       return const _NotificationUi(
//         color: Color(0xFF2E7D32),
//         icon: Icons.menu_book_rounded,
//       );
//     }

//     if (item.notification.reportAssignmentId != null) {
//       return const _NotificationUi(
//         color: Color(0xFF2196F3),
//         icon: Icons.assignment_turned_in_rounded,
//       );
//     }

//     if (item.notification.reportId != null) {
//       return const _NotificationUi(
//         color: Color(0xFFFF9800),
//         icon: Icons.report_problem_rounded,
//       );
//     }

//     return const _NotificationUi(
//       color: Color(0xFF757575),
//       icon: Icons.notifications,
//     );
//   }
// }

// class _NotificationUi {
//   final Color color;
//   final IconData icon;

//   const _NotificationUi({required this.color, required this.icon});
// }

// import 'package:cleancityapp/core/network/api_factory.dart';
// import 'package:cleancityapp/core/notifications/notification_service.dart';
// import 'package:cleancityapp/features/auth/data/datasources/notifications_remote_data_source.dart';
// import 'package:cleancityapp/features/auth/data/models/notification_recipient_model.dart';
// import 'package:flutter/material.dart';

// class NotificationsPage extends StatefulWidget {
//   final int? driverId;
//   final int? anonymousDeviceId;

//   const NotificationsPage({super.key, this.driverId, this.anonymousDeviceId});

//   @override
//   State<NotificationsPage> createState() => _NotificationsPageState();
// }

// class _NotificationsPageState extends State<NotificationsPage> {
//   late final NotificationsRemoteDataSource _remote;

//   bool _loading = true;
//   bool _markingRead = false;
//   String? _error;
//   List<NotificationRecipientModel> _notifications = [];

//   @override
//   void initState() {
//     super.initState();
//     _remote = NotificationsRemoteDataSourceImpl(ApiFactory.api);

//     NotificationService.instance.onForegroundNotificationReceived = () {
//       if (mounted) _loadNotifications();
//     };

//     _loadNotifications();
//   }

//   @override
//   void dispose() {
//     NotificationService.instance.onForegroundNotificationReceived = null;
//     super.dispose();
//   }

//   Future<void> _loadNotifications() async {
//     setState(() {
//       _loading = true;
//       _error = null;
//     });

//     try {
//       // ✅ المنطق الجديد:
//       // السائق (مسجل دخول): يجلب من endpoint السائق + endpoint الجهاز معاً
//       // المجهول: يجلب من endpoint الجهاز فقط

//       final List<NotificationRecipientModel> items = [];
//       final Set<int> seenIds = {}; // لمنع التكرار عند الدمج

//       if (widget.driverId != null) {
//         // ✅ اشعارات الإسناد الخاصة بالسائق
//         final driverItems = await _remote.getForDriver(widget.driverId!);
//         for (final item in driverItems) {
//           if (seenIds.add(item.recipientId)) {
//             items.add(item);
//           }
//         }
//       }

//       if (widget.anonymousDeviceId != null) {
//         // ✅ اشعارات بلاغات الجهاز + المحتوى التوعوي
//         final deviceItems =
//             await _remote.getForAnonymousDevice(widget.anonymousDeviceId!);
//         for (final item in deviceItems) {
//           if (seenIds.add(item.recipientId)) {
//             items.add(item);
//           }
//         }
//       }

//       if (widget.driverId == null && widget.anonymousDeviceId == null) {
//         throw Exception('لم يتم تمرير driverId أو anonymousDeviceId إلى الصفحة');
//       }

//       // ✅ ترتيب من الأحدث إلى الأقدم
//       items.sort(
//         (a, b) => b.notification.createdAt.compareTo(a.notification.createdAt),
//       );

//       if (!mounted) return;
//       setState(() {
//         _notifications = items;
//         _loading = false;
//       });
//     } catch (e) {
//       if (!mounted) return;
//       setState(() {
//         _error = e.toString();
//         _loading = false;
//       });
//     }
//   }

//   int get _unreadCount => _notifications.where((n) => !n.isRead).length;

//   Future<void> _openNotification(NotificationRecipientModel item) async {
//     if (!item.isRead) {
//       await _markAsRead(item);
//     }
//     if (!mounted) return;
//     _showNotificationDetails(context, item);
//   }

//   Future<void> _markAsRead(NotificationRecipientModel item) async {
//     if (_markingRead) return;
//     setState(() => _markingRead = true);

//     try {
//       await _remote.markAsRead(item.recipientId);

//       final index = _notifications.indexWhere(
//         (n) => n.recipientId == item.recipientId,
//       );
//       if (index != -1) {
//         _notifications[index] = _notifications[index].copyWith(
//           isRead: true,
//           readAt: DateTime.now(),
//         );
//       }

//       if (!mounted) return;
//       setState(() {});
//     } catch (e) {
//       if (!mounted) return;
//       ScaffoldMessenger.of(context).showSnackBar(
//         SnackBar(
//           content: Text('تعذر تحديث حالة الإشعار: $e'),
//           backgroundColor: Colors.red,
//         ),
//       );
//     } finally {
//       if (mounted) setState(() => _markingRead = false);
//     }
//   }

//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       backgroundColor: const Color(0xFFF8FAFF),
//       appBar: AppBar(
//         title: const Text(
//           'الإشعارات',
//           style: TextStyle(
//             fontSize: 20,
//             fontWeight: FontWeight.bold,
//             color: Colors.white,
//           ),
//         ),
//         centerTitle: true,
//         backgroundColor: const Color.fromARGB(255, 87, 211, 157),
//         leading: IconButton(
//           icon: const Icon(Icons.arrow_back, color: Colors.white),
//           onPressed: () => Navigator.pop(context),
//         ),
//         actions: [
//           if (_unreadCount > 0)
//             Padding(
//               padding: const EdgeInsets.only(right: 16),
//               child: Center(
//                 child: Container(
//                   padding: const EdgeInsets.all(6),
//                   decoration: BoxDecoration(
//                     color: Colors.red,
//                     borderRadius: BorderRadius.circular(12),
//                   ),
//                   child: Text(
//                     '$_unreadCount',
//                     style: const TextStyle(
//                       color: Colors.white,
//                       fontSize: 12,
//                       fontWeight: FontWeight.bold,
//                     ),
//                   ),
//                 ),
//               ),
//             ),
//         ],
//         elevation: 2,
//         shape: const RoundedRectangleBorder(
//           borderRadius: BorderRadius.only(
//             bottomLeft: Radius.circular(16),
//             bottomRight: Radius.circular(16),
//           ),
//         ),
//       ),
//       body: Column(
//         children: [
//           Container(
//             padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
//             decoration: BoxDecoration(
//               color: Colors.white,
//               boxShadow: [
//                 BoxShadow(
//                   color: Colors.grey.withValues(alpha: 0.05),
//                   spreadRadius: 1,
//                   blurRadius: 4,
//                   offset: const Offset(0, 2),
//                 ),
//               ],
//             ),
//             child: Row(
//               mainAxisAlignment: MainAxisAlignment.spaceBetween,
//               children: [
//                 Text(
//                   'الإشعارات ($_unreadCount غير مقروء)',
//                   style: const TextStyle(
//                     fontSize: 14,
//                     color: Color(0xFF555555),
//                     fontWeight: FontWeight.w500,
//                   ),
//                 ),
//                 IconButton(
//                   onPressed: _loading ? null : _loadNotifications,
//                   icon: const Icon(Icons.refresh),
//                   tooltip: 'تحديث',
//                 ),
//               ],
//             ),
//           ),
//           Expanded(child: _buildBody()),
//         ],
//       ),
//     );
//   }

//   Widget _buildBody() {
//     if (_loading) {
//       return const Center(child: CircularProgressIndicator());
//     }

//     if (_error != null) {
//       return Center(
//         child: Padding(
//           padding: const EdgeInsets.all(24),
//           child: Column(
//             mainAxisAlignment: MainAxisAlignment.center,
//             children: [
//               Icon(Icons.wifi_off, size: 64, color: Colors.grey[400]),
//               const SizedBox(height: 16),
//               Text(
//                 'تعذر تحميل الإشعارات',
//                 style: TextStyle(fontSize: 18, color: Colors.grey[700]),
//               ),
//               const SizedBox(height: 8),
//               Text(
//                 _error!,
//                 textAlign: TextAlign.center,
//                 style: const TextStyle(color: Colors.red),
//               ),
//               const SizedBox(height: 16),
//               ElevatedButton(
//                 onPressed: _loadNotifications,
//                 child: const Text('إعادة المحاولة'),
//               ),
//             ],
//           ),
//         ),
//       );
//     }

//     if (_notifications.isEmpty) {
//       return Center(
//         child: Column(
//           mainAxisAlignment: MainAxisAlignment.center,
//           children: [
//             Icon(Icons.notifications_off, size: 64, color: Colors.grey[400]),
//             const SizedBox(height: 16),
//             const Text(
//               'لا توجد إشعارات',
//               style: TextStyle(fontSize: 18, color: Color(0xFF7B8D9E)),
//             ),
//             const SizedBox(height: 8),
//             const Text(
//               'ستظهر هنا التحديثات والإشعارات الجديدة',
//               style: TextStyle(color: Color(0xFF9AA5B5)),
//             ),
//           ],
//         ),
//       );
//     }

//     return RefreshIndicator(
//       onRefresh: _loadNotifications,
//       child: ListView.builder(
//         padding: const EdgeInsets.all(16),
//         itemCount: _notifications.length,
//         itemBuilder: (context, index) =>
//             _buildNotificationCard(_notifications[index]),
//       ),
//     );
//   }

//   Widget _buildNotificationCard(NotificationRecipientModel item) {
//     final ui = _mapUi(item);
//     final backgroundColor =
//         item.isRead ? Colors.white : const Color(0xFFE8EAF6);

//     return Container(
//       margin: const EdgeInsets.only(bottom: 12),
//       decoration: BoxDecoration(
//         color: backgroundColor,
//         borderRadius: BorderRadius.circular(12),
//         boxShadow: [
//           BoxShadow(
//             color: Colors.grey.withValues(alpha: 0.1),
//             spreadRadius: 1,
//             blurRadius: 4,
//             offset: const Offset(0, 2),
//           ),
//         ],
//         border: item.isRead
//             ? null
//             : Border.all(
//                 color: const Color(0xFF3F51B5).withValues(alpha: 0.2),
//                 width: 1,
//               ),
//       ),
//       child: Material(
//         color: Colors.transparent,
//         child: InkWell(
//           borderRadius: BorderRadius.circular(12),
//           onTap: () => _openNotification(item),
//           child: Padding(
//             padding: const EdgeInsets.all(16),
//             child: Row(
//               crossAxisAlignment: CrossAxisAlignment.start,
//               children: [
//                 Container(
//                   width: 48,
//                   height: 48,
//                   decoration: BoxDecoration(
//                     color: ui.color.withValues(alpha: 0.1),
//                     borderRadius: BorderRadius.circular(10),
//                   ),
//                   child: Icon(ui.icon, color: ui.color, size: 24),
//                 ),
//                 const SizedBox(width: 16),
//                 Expanded(
//                   child: Column(
//                     crossAxisAlignment: CrossAxisAlignment.start,
//                     children: [
//                       Text(
//                         item.notification.title,
//                         style: TextStyle(
//                           fontSize: 16,
//                           fontWeight:
//                               item.isRead ? FontWeight.w500 : FontWeight.bold,
//                           color: const Color(0xFF2C3E50),
//                         ),
//                       ),
//                       const SizedBox(height: 4),
//                       Text(
//                         item.notification.body,
//                         style: const TextStyle(
//                           fontSize: 14,
//                           color: Color(0xFF5A6C7D),
//                         ),
//                         maxLines: 2,
//                         overflow: TextOverflow.ellipsis,
//                       ),
//                       const SizedBox(height: 8),
//                       Row(
//                         mainAxisAlignment: MainAxisAlignment.spaceBetween,
//                         children: [
//                           Row(
//                             children: [
//                               Icon(Icons.access_time,
//                                   color: Colors.grey[500], size: 14),
//                               const SizedBox(width: 4),
//                               Text(
//                                 item.notification.formattedTime(),
//                                 style: TextStyle(
//                                     fontSize: 12, color: Colors.grey[600]),
//                               ),
//                             ],
//                           ),
//                           if (item.notification.reportId != null)
//                             Container(
//                               padding: const EdgeInsets.symmetric(
//                                   horizontal: 8, vertical: 4),
//                               decoration: BoxDecoration(
//                                 color: const Color(0xFF3F51B5)
//                                     .withValues(alpha: 0.1),
//                                 borderRadius: BorderRadius.circular(4),
//                               ),
//                               child: Text(
//                                 '#${item.notification.reportId}',
//                                 style: const TextStyle(
//                                   fontSize: 12,
//                                   color: Color(0xFF3F51B5),
//                                   fontWeight: FontWeight.w500,
//                                 ),
//                               ),
//                             ),
//                         ],
//                       ),
//                     ],
//                   ),
//                 ),
//                 if (!item.isRead)
//                   Padding(
//                     padding: const EdgeInsets.only(left: 8),
//                     child: Container(
//                       width: 8,
//                       height: 8,
//                       decoration: BoxDecoration(
//                         color: const Color(0xFFF44336),
//                         borderRadius: BorderRadius.circular(4),
//                       ),
//                     ),
//                   ),
//               ],
//             ),
//           ),
//         ),
//       ),
//     );
//   }

//   void _showNotificationDetails(
//       BuildContext context, NotificationRecipientModel item) {
//     final ui = _mapUi(item);

//     showModalBottomSheet(
//       context: context,
//       isScrollControlled: true,
//       shape: const RoundedRectangleBorder(
//         borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
//       ),
//       builder: (context) {
//         return Container(
//           padding: const EdgeInsets.all(24),
//           child: Column(
//             mainAxisSize: MainAxisSize.min,
//             crossAxisAlignment: CrossAxisAlignment.start,
//             children: [
//               Center(
//                 child: Container(
//                   width: 40,
//                   height: 4,
//                   decoration: BoxDecoration(
//                     color: Colors.grey[300],
//                     borderRadius: BorderRadius.circular(2),
//                   ),
//                 ),
//               ),
//               const SizedBox(height: 20),
//               Row(
//                 crossAxisAlignment: CrossAxisAlignment.start,
//                 children: [
//                   Container(
//                     width: 56,
//                     height: 56,
//                     decoration: BoxDecoration(
//                       color: ui.color.withValues(alpha: 0.1),
//                       borderRadius: BorderRadius.circular(12),
//                     ),
//                     child: Icon(ui.icon, color: ui.color, size: 28),
//                   ),
//                   const SizedBox(width: 16),
//                   Expanded(
//                     child: Column(
//                       crossAxisAlignment: CrossAxisAlignment.start,
//                       children: [
//                         Text(
//                           item.notification.title,
//                           style: const TextStyle(
//                             fontSize: 20,
//                             fontWeight: FontWeight.bold,
//                             color: Color(0xFF2C3E50),
//                           ),
//                         ),
//                         const SizedBox(height: 4),
//                         Row(
//                           children: [
//                             Icon(Icons.access_time,
//                                 color: Colors.grey[500], size: 16),
//                             const SizedBox(width: 4),
//                             Expanded(
//                               child: Text(
//                                 item.notification.formattedTime(),
//                                 style: TextStyle(
//                                     fontSize: 14, color: Colors.grey[600]),
//                               ),
//                             ),
//                           ],
//                         ),
//                       ],
//                     ),
//                   ),
//                   IconButton(
//                     icon: const Icon(Icons.close),
//                     onPressed: () => Navigator.pop(context),
//                   ),
//                 ],
//               ),
//               const SizedBox(height: 24),
//               Container(
//                 padding: const EdgeInsets.all(16),
//                 decoration: BoxDecoration(
//                   color: const Color(0xFFF8FAFF),
//                   borderRadius: BorderRadius.circular(12),
//                 ),
//                 child: Text(
//                   item.notification.body,
//                   style: const TextStyle(
//                       fontSize: 16, color: Color(0xFF34495E), height: 1.6),
//                 ),
//               ),
//               if (item.notification.reportId != null) ...[
//                 const SizedBox(height: 16),
//                 Container(
//                   padding: const EdgeInsets.all(16),
//                   decoration: BoxDecoration(
//                     color: const Color(0xFFE8EAF6),
//                     borderRadius: BorderRadius.circular(12),
//                   ),
//                   child: Row(
//                     children: [
//                       const Icon(Icons.report, color: Color(0xFF3F51B5)),
//                       const SizedBox(width: 12),
//                       Expanded(
//                         child: Column(
//                           crossAxisAlignment: CrossAxisAlignment.start,
//                           children: [
//                             const Text('رقم البلاغ المرتبط',
//                                 style: TextStyle(
//                                     fontSize: 14, color: Color(0xFF5A6C7D))),
//                             const SizedBox(height: 4),
//                             Text(
//                               '#${item.notification.reportId}',
//                               style: const TextStyle(
//                                 fontSize: 16,
//                                 fontWeight: FontWeight.bold,
//                                 color: Color(0xFF3F51B5),
//                               ),
//                             ),
//                           ],
//                         ),
//                       ),
//                     ],
//                   ),
//                 ),
//               ],
//               if (item.notification.awarenessContentId != null) ...[
//                 const SizedBox(height: 16),
//                 Container(
//                   padding: const EdgeInsets.all(16),
//                   decoration: BoxDecoration(
//                     color: const Color(0xFFE8FFF2),
//                     borderRadius: BorderRadius.circular(12),
//                   ),
//                   child: Row(
//                     children: [
//                       const Icon(Icons.menu_book, color: Color(0xFF2E7D32)),
//                       const SizedBox(width: 12),
//                       Expanded(
//                         child: Text(
//                           'مقال توعوي رقم #${item.notification.awarenessContentId}',
//                           style: const TextStyle(
//                             fontSize: 15,
//                             fontWeight: FontWeight.w600,
//                             color: Color(0xFF2E7D32),
//                           ),
//                         ),
//                       ),
//                     ],
//                   ),
//                 ),
//               ],
//               const SizedBox(height: 24),
//               SizedBox(
//                 width: double.infinity,
//                 child: ElevatedButton(
//                   onPressed: () => Navigator.pop(context),
//                   style: ElevatedButton.styleFrom(
//                     backgroundColor: const Color(0xFF3F51B5),
//                     padding: const EdgeInsets.symmetric(vertical: 16),
//                     shape: RoundedRectangleBorder(
//                         borderRadius: BorderRadius.circular(12)),
//                   ),
//                   child: const Text(
//                     'حسناً',
//                     style: TextStyle(
//                         fontSize: 16,
//                         fontWeight: FontWeight.bold,
//                         color: Colors.white),
//                   ),
//                 ),
//               ),
//             ],
//           ),
//         );
//       },
//     );
//   }

//   _NotificationUi _mapUi(NotificationRecipientModel item) {
//     if (item.notification.awarenessContentId != null) {
//       return const _NotificationUi(
//           color: Color(0xFF2E7D32), icon: Icons.menu_book_rounded);
//     }
//     if (item.notification.reportAssignmentId != null) {
//       return const _NotificationUi(
//           color: Color(0xFF2196F3),
//           icon: Icons.assignment_turned_in_rounded);
//     }
//     if (item.notification.reportId != null) {
//       return const _NotificationUi(
//           color: Color(0xFFFF9800), icon: Icons.report_problem_rounded);
//     }
//     return const _NotificationUi(
//         color: Color(0xFF757575), icon: Icons.notifications);
//   }
// }

// class _NotificationUi {
//   final Color color;
//   final IconData icon;
//   const _NotificationUi({required this.color, required this.icon});
// }

// // notifications_page.dart
// import 'package:cleancityapp/core/network/api_factory.dart';
// import 'package:cleancityapp/core/notifications/notification_service.dart';
// import 'package:cleancityapp/features/auth/data/datasources/notifications_remote_data_source.dart';
// import 'package:cleancityapp/features/auth/data/models/notification_recipient_model.dart';
// // ✅ import الجديد
// import 'package:cleancityapp/features/auth/presentation/pages/awareness_content_page.dart';
// import 'package:flutter/material.dart';

// class NotificationsPage extends StatefulWidget {
//   final int? driverId;
//   final int? anonymousDeviceId;

//   const NotificationsPage({super.key, this.driverId, this.anonymousDeviceId});

//   @override
//   State<NotificationsPage> createState() => _NotificationsPageState();
// }

// class _NotificationsPageState extends State<NotificationsPage> {
//   late final NotificationsRemoteDataSource _remote;

//   bool _loading = true;
//   bool _markingRead = false;
//   String? _error;
//   List<NotificationRecipientModel> _notifications = [];

//   @override
//   void initState() {
//     super.initState();
//     _remote = NotificationsRemoteDataSourceImpl(ApiFactory.api);

//     NotificationService.instance.onForegroundNotificationReceived = () {
//       if (mounted) _loadNotifications();
//     };

//     _loadNotifications();
//   }

//   @override
//   void dispose() {
//     NotificationService.instance.onForegroundNotificationReceived = null;
//     super.dispose();
//   }

//   Future<void> _loadNotifications() async {
//     setState(() {
//       _loading = true;
//       _error = null;
//     });

//     try {
//       final List<NotificationRecipientModel> items = [];
//       final Set<int> seenIds = {};

//       if (widget.driverId != null) {
//         final driverItems = await _remote.getForDriver(widget.driverId!);
//         for (final item in driverItems) {
//           if (seenIds.add(item.recipientId)) {
//             items.add(item);
//           }
//         }
//       }

//       if (widget.anonymousDeviceId != null) {
//         final deviceItems = await _remote.getForAnonymousDevice(
//           widget.anonymousDeviceId!,
//         );
//         for (final item in deviceItems) {
//           if (seenIds.add(item.recipientId)) {
//             items.add(item);
//           }
//         }
//       }

//       if (widget.driverId == null && widget.anonymousDeviceId == null) {
//         throw Exception(
//           'لم يتم تمرير driverId أو anonymousDeviceId إلى الصفحة',
//         );
//       }

//       items.sort(
//         (a, b) => b.notification.createdAt.compareTo(a.notification.createdAt),
//       );

//       if (!mounted) return;
//       setState(() {
//         _notifications = items;
//         _loading = false;
//       });
//     } catch (e) {
//       if (!mounted) return;
//       setState(() {
//         _error = e.toString();
//         _loading = false;
//       });
//     }
//   }

//   int get _unreadCount => _notifications.where((n) => !n.isRead).length;

//   Future<void> _openNotification(NotificationRecipientModel item) async {
//     if (!item.isRead) {
//       await _markAsRead(item);
//     }
//     if (!mounted) return;

//     // ✅ إشعار مقال توعوي → انتقل مباشرة للمقال بدون bottom sheet
//     if (item.notification.awarenessContentId != null) {
//       Navigator.push(
//         context,
//         MaterialPageRoute(
//           builder: (_) => AwarenessArticleByIdPage(
//             articleId: item.notification.awarenessContentId!,
//           ),
//         ),
//       );
//       return;
//     }

//     // باقي أنواع الإشعارات → bottom sheet كالمعتاد
//     _showNotificationDetails(context, item);
//   }

//   Future<void> _markAsRead(NotificationRecipientModel item) async {
//     if (_markingRead) return;
//     setState(() => _markingRead = true);

//     try {
//       await _remote.markAsRead(item.recipientId);

//       final index = _notifications.indexWhere(
//         (n) => n.recipientId == item.recipientId,
//       );
//       if (index != -1) {
//         _notifications[index] = _notifications[index].copyWith(
//           isRead: true,
//           readAt: DateTime.now(),
//         );
//       }

//       if (!mounted) return;
//       setState(() {});
//     } catch (e) {
//       if (!mounted) return;
//       ScaffoldMessenger.of(context).showSnackBar(
//         SnackBar(
//           content: Text('تعذر تحديث حالة الإشعار: $e'),
//           backgroundColor: Colors.red,
//         ),
//       );
//     } finally {
//       if (mounted) setState(() => _markingRead = false);
//     }
//   }

//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       backgroundColor: const Color(0xFFF8FAFF),
//       appBar: AppBar(
//         title: const Text(
//           'الإشعارات',
//           style: TextStyle(
//             fontSize: 20,
//             fontWeight: FontWeight.bold,
//             color: Colors.white,
//           ),
//         ),
//         centerTitle: true,
//         backgroundColor: const Color.fromARGB(255, 87, 211, 157),
//         leading: IconButton(
//           icon: const Icon(Icons.arrow_back, color: Colors.white),
//           onPressed: () => Navigator.pop(context),
//         ),
//         actions: [
//           if (_unreadCount > 0)
//             Padding(
//               padding: const EdgeInsets.only(right: 16),
//               child: Center(
//                 child: Container(
//                   padding: const EdgeInsets.all(6),
//                   decoration: BoxDecoration(
//                     color: Colors.red,
//                     borderRadius: BorderRadius.circular(12),
//                   ),
//                   child: Text(
//                     '$_unreadCount',
//                     style: const TextStyle(
//                       color: Colors.white,
//                       fontSize: 12,
//                       fontWeight: FontWeight.bold,
//                     ),
//                   ),
//                 ),
//               ),
//             ),
//         ],
//         elevation: 2,
//         shape: const RoundedRectangleBorder(
//           borderRadius: BorderRadius.only(
//             bottomLeft: Radius.circular(16),
//             bottomRight: Radius.circular(16),
//           ),
//         ),
//       ),
//       body: Column(
//         children: [
//           Container(
//             padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
//             decoration: BoxDecoration(
//               color: Colors.white,
//               boxShadow: [
//                 BoxShadow(
//                   color: Colors.grey.withValues(alpha: 0.05),
//                   spreadRadius: 1,
//                   blurRadius: 4,
//                   offset: const Offset(0, 2),
//                 ),
//               ],
//             ),
//             child: Row(
//               mainAxisAlignment: MainAxisAlignment.spaceBetween,
//               children: [
//                 Text(
//                   'الإشعارات ($_unreadCount غير مقروء)',
//                   style: const TextStyle(
//                     fontSize: 14,
//                     color: Color(0xFF555555),
//                     fontWeight: FontWeight.w500,
//                   ),
//                 ),
//                 IconButton(
//                   onPressed: _loading ? null : _loadNotifications,
//                   icon: const Icon(Icons.refresh),
//                   tooltip: 'تحديث',
//                 ),
//               ],
//             ),
//           ),
//           Expanded(child: _buildBody()),
//         ],
//       ),
//     );
//   }

//   Widget _buildBody() {
//     if (_loading) {
//       return const Center(child: CircularProgressIndicator());
//     }

//     if (_error != null) {
//       return Center(
//         child: Padding(
//           padding: const EdgeInsets.all(24),
//           child: Column(
//             mainAxisAlignment: MainAxisAlignment.center,
//             children: [
//               Icon(Icons.wifi_off, size: 64, color: Colors.grey[400]),
//               const SizedBox(height: 16),
//               Text(
//                 'تعذر تحميل الإشعارات',
//                 style: TextStyle(fontSize: 18, color: Colors.grey[700]),
//               ),
//               const SizedBox(height: 8),
//               Text(
//                 _error!,
//                 textAlign: TextAlign.center,
//                 style: const TextStyle(color: Colors.red),
//               ),
//               const SizedBox(height: 16),
//               ElevatedButton(
//                 onPressed: _loadNotifications,
//                 child: const Text('إعادة المحاولة'),
//               ),
//             ],
//           ),
//         ),
//       );
//     }

//     if (_notifications.isEmpty) {
//       return Center(
//         child: Column(
//           mainAxisAlignment: MainAxisAlignment.center,
//           children: [
//             Icon(Icons.notifications_off, size: 64, color: Colors.grey[400]),
//             const SizedBox(height: 16),
//             const Text(
//               'لا توجد إشعارات',
//               style: TextStyle(fontSize: 18, color: Color(0xFF7B8D9E)),
//             ),
//             const SizedBox(height: 8),
//             const Text(
//               'ستظهر هنا التحديثات والإشعارات الجديدة',
//               style: TextStyle(color: Color(0xFF9AA5B5)),
//             ),
//           ],
//         ),
//       );
//     }

//     return RefreshIndicator(
//       onRefresh: _loadNotifications,
//       child: ListView.builder(
//         padding: const EdgeInsets.all(16),
//         itemCount: _notifications.length,
//         itemBuilder: (context, index) =>
//             _buildNotificationCard(_notifications[index]),
//       ),
//     );
//   }

//   Widget _buildNotificationCard(NotificationRecipientModel item) {
//     final ui = _mapUi(item);
//     final backgroundColor = item.isRead
//         ? Colors.white
//         : const Color(0xFFE8EAF6);

//     return Container(
//       margin: const EdgeInsets.only(bottom: 12),
//       decoration: BoxDecoration(
//         color: backgroundColor,
//         borderRadius: BorderRadius.circular(12),
//         boxShadow: [
//           BoxShadow(
//             color: Colors.grey.withValues(alpha: 0.1),
//             spreadRadius: 1,
//             blurRadius: 4,
//             offset: const Offset(0, 2),
//           ),
//         ],
//         border: item.isRead
//             ? null
//             : Border.all(
//                 color: const Color(0xFF3F51B5).withValues(alpha: 0.2),
//                 width: 1,
//               ),
//       ),
//       child: Material(
//         color: Colors.transparent,
//         child: InkWell(
//           borderRadius: BorderRadius.circular(12),
//           onTap: () => _openNotification(item),
//           child: Padding(
//             padding: const EdgeInsets.all(16),
//             child: Row(
//               crossAxisAlignment: CrossAxisAlignment.start,
//               children: [
//                 Container(
//                   width: 48,
//                   height: 48,
//                   decoration: BoxDecoration(
//                     color: ui.color.withValues(alpha: 0.1),
//                     borderRadius: BorderRadius.circular(10),
//                   ),
//                   child: Icon(ui.icon, color: ui.color, size: 24),
//                 ),
//                 const SizedBox(width: 16),
//                 Expanded(
//                   child: Column(
//                     crossAxisAlignment: CrossAxisAlignment.start,
//                     children: [
//                       Text(
//                         item.notification.title,
//                         style: TextStyle(
//                           fontSize: 16,
//                           fontWeight: item.isRead
//                               ? FontWeight.w500
//                               : FontWeight.bold,
//                           color: const Color(0xFF2C3E50),
//                         ),
//                       ),
//                       const SizedBox(height: 4),
//                       Text(
//                         item.notification.body,
//                         style: const TextStyle(
//                           fontSize: 14,
//                           color: Color(0xFF5A6C7D),
//                         ),
//                         maxLines: 2,
//                         overflow: TextOverflow.ellipsis,
//                       ),
//                       const SizedBox(height: 8),
//                       Row(
//                         mainAxisAlignment: MainAxisAlignment.spaceBetween,
//                         children: [
//                           Row(
//                             children: [
//                               Icon(
//                                 Icons.access_time,
//                                 color: Colors.grey[500],
//                                 size: 14,
//                               ),
//                               const SizedBox(width: 4),
//                               Text(
//                                 item.notification.formattedTime(),
//                                 style: TextStyle(
//                                   fontSize: 12,
//                                   color: Colors.grey[600],
//                                 ),
//                               ),
//                             ],
//                           ),
//                           if (item.notification.reportId != null)
//                             Container(
//                               padding: const EdgeInsets.symmetric(
//                                 horizontal: 8,
//                                 vertical: 4,
//                               ),
//                               decoration: BoxDecoration(
//                                 color: const Color(
//                                   0xFF3F51B5,
//                                 ).withValues(alpha: 0.1),
//                                 borderRadius: BorderRadius.circular(4),
//                               ),
//                               child: Text(
//                                 '#${item.notification.reportId}',
//                                 style: const TextStyle(
//                                   fontSize: 12,
//                                   color: Color(0xFF3F51B5),
//                                   fontWeight: FontWeight.w500,
//                                 ),
//                               ),
//                             ),
//                           // ✅ شارة للمقالات التوعوية
//                           if (item.notification.awarenessContentId != null)
//                             Container(
//                               padding: const EdgeInsets.symmetric(
//                                 horizontal: 8,
//                                 vertical: 4,
//                               ),
//                               decoration: BoxDecoration(
//                                 color: const Color(
//                                   0xFF2E7D32,
//                                 ).withValues(alpha: 0.1),
//                                 borderRadius: BorderRadius.circular(4),
//                               ),
//                               child: const Row(
//                                 mainAxisSize: MainAxisSize.min,
//                                 children: [
//                                   Icon(
//                                     Icons.menu_book_rounded,
//                                     size: 12,
//                                     color: Color(0xFF2E7D32),
//                                   ),
//                                   SizedBox(width: 4),
//                                   Text(
//                                     'اقرأ المقال',
//                                     style: TextStyle(
//                                       fontSize: 11,
//                                       color: Color(0xFF2E7D32),
//                                       fontWeight: FontWeight.w600,
//                                     ),
//                                   ),
//                                 ],
//                               ),
//                             ),
//                         ],
//                       ),
//                     ],
//                   ),
//                 ),
//                 if (!item.isRead)
//                   Padding(
//                     padding: const EdgeInsets.only(left: 8),
//                     child: Container(
//                       width: 8,
//                       height: 8,
//                       decoration: BoxDecoration(
//                         color: const Color(0xFFF44336),
//                         borderRadius: BorderRadius.circular(4),
//                       ),
//                     ),
//                   ),
//               ],
//             ),
//           ),
//         ),
//       ),
//     );
//   }

//   void _showNotificationDetails(
//     BuildContext context,
//     NotificationRecipientModel item,
//   ) {
//     final ui = _mapUi(item);

//     showModalBottomSheet(
//       context: context,
//       isScrollControlled: true,
//       shape: const RoundedRectangleBorder(
//         borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
//       ),
//       builder: (context) {
//         return Container(
//           padding: const EdgeInsets.all(24),
//           child: Column(
//             mainAxisSize: MainAxisSize.min,
//             crossAxisAlignment: CrossAxisAlignment.start,
//             children: [
//               Center(
//                 child: Container(
//                   width: 40,
//                   height: 4,
//                   decoration: BoxDecoration(
//                     color: Colors.grey[300],
//                     borderRadius: BorderRadius.circular(2),
//                   ),
//                 ),
//               ),
//               const SizedBox(height: 20),
//               Row(
//                 crossAxisAlignment: CrossAxisAlignment.start,
//                 children: [
//                   Container(
//                     width: 56,
//                     height: 56,
//                     decoration: BoxDecoration(
//                       color: ui.color.withValues(alpha: 0.1),
//                       borderRadius: BorderRadius.circular(12),
//                     ),
//                     child: Icon(ui.icon, color: ui.color, size: 28),
//                   ),
//                   const SizedBox(width: 16),
//                   Expanded(
//                     child: Column(
//                       crossAxisAlignment: CrossAxisAlignment.start,
//                       children: [
//                         Text(
//                           item.notification.title,
//                           style: const TextStyle(
//                             fontSize: 20,
//                             fontWeight: FontWeight.bold,
//                             color: Color(0xFF2C3E50),
//                           ),
//                         ),
//                         const SizedBox(height: 4),
//                         Row(
//                           children: [
//                             Icon(
//                               Icons.access_time,
//                               color: Colors.grey[500],
//                               size: 16,
//                             ),
//                             const SizedBox(width: 4),
//                             Expanded(
//                               child: Text(
//                                 item.notification.formattedTime(),
//                                 style: TextStyle(
//                                   fontSize: 14,
//                                   color: Colors.grey[600],
//                                 ),
//                               ),
//                             ),
//                           ],
//                         ),
//                       ],
//                     ),
//                   ),
//                   IconButton(
//                     icon: const Icon(Icons.close),
//                     onPressed: () => Navigator.pop(context),
//                   ),
//                 ],
//               ),
//               const SizedBox(height: 24),
//               Container(
//                 padding: const EdgeInsets.all(16),
//                 decoration: BoxDecoration(
//                   color: const Color(0xFFF8FAFF),
//                   borderRadius: BorderRadius.circular(12),
//                 ),
//                 child: Text(
//                   item.notification.body,
//                   style: const TextStyle(
//                     fontSize: 16,
//                     color: Color(0xFF34495E),
//                     height: 1.6,
//                   ),
//                 ),
//               ),
//               if (item.notification.reportId != null) ...[
//                 const SizedBox(height: 16),
//                 Container(
//                   padding: const EdgeInsets.all(16),
//                   decoration: BoxDecoration(
//                     color: const Color(0xFFE8EAF6),
//                     borderRadius: BorderRadius.circular(12),
//                   ),
//                   child: Row(
//                     children: [
//                       const Icon(Icons.report, color: Color(0xFF3F51B5)),
//                       const SizedBox(width: 12),
//                       Expanded(
//                         child: Column(
//                           crossAxisAlignment: CrossAxisAlignment.start,
//                           children: [
//                             const Text(
//                               'رقم البلاغ المرتبط',
//                               style: TextStyle(
//                                 fontSize: 14,
//                                 color: Color(0xFF5A6C7D),
//                               ),
//                             ),
//                             const SizedBox(height: 4),
//                             Text(
//                               '#${item.notification.reportId}',
//                               style: const TextStyle(
//                                 fontSize: 16,
//                                 fontWeight: FontWeight.bold,
//                                 color: Color(0xFF3F51B5),
//                               ),
//                             ),
//                           ],
//                         ),
//                       ),
//                     ],
//                   ),
//                 ),
//               ],
//               const SizedBox(height: 24),
//               SizedBox(
//                 width: double.infinity,
//                 child: ElevatedButton(
//                   onPressed: () => Navigator.pop(context),
//                   style: ElevatedButton.styleFrom(
//                     backgroundColor: const Color(0xFF3F51B5),
//                     padding: const EdgeInsets.symmetric(vertical: 16),
//                     shape: RoundedRectangleBorder(
//                       borderRadius: BorderRadius.circular(12),
//                     ),
//                   ),
//                   child: const Text(
//                     'حسناً',
//                     style: TextStyle(
//                       fontSize: 16,
//                       fontWeight: FontWeight.bold,
//                       color: Colors.white,
//                     ),
//                   ),
//                 ),
//               ),
//             ],
//           ),
//         );
//       },
//     );
//   }

//   _NotificationUi _mapUi(NotificationRecipientModel item) {
//     if (item.notification.awarenessContentId != null) {
//       return const _NotificationUi(
//         color: Color(0xFF2E7D32),
//         icon: Icons.menu_book_rounded,
//       );
//     }
//     if (item.notification.reportAssignmentId != null) {
//       return const _NotificationUi(
//         color: Color(0xFF2196F3),
//         icon: Icons.assignment_turned_in_rounded,
//       );
//     }
//     if (item.notification.reportId != null) {
//       return const _NotificationUi(
//         color: Color(0xFFFF9800),
//         icon: Icons.report_problem_rounded,
//       );
//     }
//     return const _NotificationUi(
//       color: Color(0xFF757575),
//       icon: Icons.notifications,
//     );
//   }
// }

// class _NotificationUi {
//   final Color color;
//   final IconData icon;
//   const _NotificationUi({required this.color, required this.icon});
// }
// notifications_page.dart
import 'package:cleancityapp/core/network/api_factory.dart';
import 'package:cleancityapp/core/notifications/notification_service.dart';
import 'package:cleancityapp/features/auth/data/datasources/notifications_remote_data_source.dart';
import 'package:cleancityapp/features/auth/data/models/notification_recipient_model.dart';
import 'package:cleancityapp/features/auth/presentation/pages/awareness_content_page.dart';
import 'package:flutter/material.dart';

class NotificationsPage extends StatefulWidget {
  final int? driverId;
  final int? anonymousDeviceId;

  // ✅ معرف المستخدم المسجل دخوله (Identity userId)
  // يُمرَّر عند تسجيل الدخول لجلب إشعاراته الشخصية + المقالات التوعوية
  final String? userId;

  const NotificationsPage({
    super.key,
    this.driverId,
    this.anonymousDeviceId,
    this.userId,
  });

  @override
  State<NotificationsPage> createState() => _NotificationsPageState();
}

class _NotificationsPageState extends State<NotificationsPage> {
  late final NotificationsRemoteDataSource _remote;

  bool _loading = true;
  bool _markingRead = false;
  String? _error;
  List<NotificationRecipientModel> _notifications = [];

  @override
  void initState() {
    super.initState();
    _remote = NotificationsRemoteDataSourceImpl(ApiFactory.api);

    // الاشتراك في callback وصول الإشعارات أثناء فتح التطبيق
    NotificationService.instance.onForegroundNotificationReceived = () {
      if (mounted) _loadNotifications();
    };

    _loadNotifications();
  }

  @override
  void dispose() {
    // إلغاء الاشتراك لتجنب memory leak عند مغادرة الصفحة
    NotificationService.instance.onForegroundNotificationReceived = null;
    super.dispose();
  }

  Future<void> _loadNotifications() async {
    setState(() {
      _loading = true;
      _error = null;
    });

    try {
      // التحقق من أن معرفاً واحداً على الأقل قد تم تمريره
      final bool hasAnyId =
          widget.driverId != null ||
          widget.anonymousDeviceId != null ||
          widget.userId != null;

      if (!hasAnyId) {
        throw Exception(
          'لم يتم تمرير أي معرف (driverId / anonymousDeviceId / userId) إلى الصفحة',
        );
      }

      final List<NotificationRecipientModel> items = [];
      // Set لمنع تكرار الإشعارات عند دمج نتائج أكثر من endpoint
      final Set<int> seenIds = {};

      // ─── 1) مستخدم مسجل دخوله (Identity User) ────────────────────────────
      // يجلب إشعاراته الشخصية: بلاغاته + المقالات التوعوية المرتبطة بحسابه
      if (widget.userId != null) {
        final userItems = await _remote.getForUser(widget.userId!);
        for (final item in userItems) {
          if (seenIds.add(item.recipientId)) {
            items.add(item);
          }
        }
      }

      // ─── 2) إشعارات الجهاز (Anonymous Device) ────────────────────────────
      // تُجلب دائماً لأن:
      // - المستخدم المجهول: هذا مصدره الوحيد
      // - المستخدم المسجل: يضم إشعارات المقالات التوعوية المرسلة قبل تسجيل الدخول
      // - السائق المسجل: يضم إشعارات التوعية العامة أيضاً
      if (widget.anonymousDeviceId != null) {
        final deviceItems = await _remote.getForAnonymousDevice(
          widget.anonymousDeviceId!,
        );
        for (final item in deviceItems) {
          if (seenIds.add(item.recipientId)) {
            items.add(item);
          }
        }
      }

      // ─── 3) إشعارات السائق ────────────────────────────────────────────────
      // إشعارات الإسناد الخاصة بالسائق
      if (widget.driverId != null) {
        final driverItems = await _remote.getForDriver(widget.driverId!);
        for (final item in driverItems) {
          if (seenIds.add(item.recipientId)) {
            items.add(item);
          }
        }
      }

      // ترتيب من الأحدث إلى الأقدم
      items.sort(
        (a, b) => b.notification.createdAt.compareTo(a.notification.createdAt),
      );

      if (!mounted) return;
      setState(() {
        _notifications = items;
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

  int get _unreadCount => _notifications.where((n) => !n.isRead).length;

  Future<void> _openNotification(NotificationRecipientModel item) async {
    if (!item.isRead) {
      await _markAsRead(item);
    }
    if (!mounted) return;

    // إشعار مقال توعوي → انتقل مباشرةً إلى المقال
    if (item.notification.awarenessContentId != null) {
      Navigator.push(
        context,
        MaterialPageRoute(
          builder: (_) => AwarenessArticleByIdPage(
            articleId: item.notification.awarenessContentId!,
          ),
        ),
      );
      return;
    }

    // باقي أنواع الإشعارات → bottom sheet
    _showNotificationDetails(context, item);
  }

  Future<void> _markAsRead(NotificationRecipientModel item) async {
    if (_markingRead) return;
    setState(() => _markingRead = true);

    try {
      await _remote.markAsRead(item.recipientId);

      final index = _notifications.indexWhere(
        (n) => n.recipientId == item.recipientId,
      );
      if (index != -1) {
        _notifications[index] = _notifications[index].copyWith(
          isRead: true,
          readAt: DateTime.now(),
        );
      }

      if (!mounted) return;
      setState(() {});
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('تعذر تحديث حالة الإشعار: $e'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      if (mounted) setState(() => _markingRead = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF8FAFF),
      appBar: AppBar(
        title: const Text(
          'الإشعارات',
          style: TextStyle(
            fontSize: 20,
            fontWeight: FontWeight.bold,
            color: Colors.white,
          ),
        ),
        centerTitle: true,
        backgroundColor: const Color.fromARGB(255, 87, 211, 157),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Colors.white),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          if (_unreadCount > 0)
            Padding(
              padding: const EdgeInsets.only(right: 16),
              child: Center(
                child: Container(
                  padding: const EdgeInsets.all(6),
                  decoration: BoxDecoration(
                    color: Colors.red,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    '$_unreadCount',
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ),
            ),
        ],
        elevation: 2,
        shape: const RoundedRectangleBorder(
          borderRadius: BorderRadius.only(
            bottomLeft: Radius.circular(16),
            bottomRight: Radius.circular(16),
          ),
        ),
      ),
      body: Column(
        children: [
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            decoration: BoxDecoration(
              color: Colors.white,
              boxShadow: [
                BoxShadow(
                  color: Colors.grey.withValues(alpha: 0.05),
                  spreadRadius: 1,
                  blurRadius: 4,
                  offset: const Offset(0, 2),
                ),
              ],
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'الإشعارات ($_unreadCount غير مقروء)',
                  style: const TextStyle(
                    fontSize: 14,
                    color: Color(0xFF555555),
                    fontWeight: FontWeight.w500,
                  ),
                ),
                IconButton(
                  onPressed: _loading ? null : _loadNotifications,
                  icon: const Icon(Icons.refresh),
                  tooltip: 'تحديث',
                ),
              ],
            ),
          ),
          Expanded(child: _buildBody()),
        ],
      ),
    );
  }

  Widget _buildBody() {
    if (_loading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_error != null) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(Icons.wifi_off, size: 64, color: Colors.grey[400]),
              const SizedBox(height: 16),
              Text(
                'تعذر تحميل الإشعارات',
                style: TextStyle(fontSize: 18, color: Colors.grey[700]),
              ),
              const SizedBox(height: 8),
              Text(
                _error!,
                textAlign: TextAlign.center,
                style: const TextStyle(color: Colors.red),
              ),
              const SizedBox(height: 16),
              ElevatedButton(
                onPressed: _loadNotifications,
                child: const Text('إعادة المحاولة'),
              ),
            ],
          ),
        ),
      );
    }

    if (_notifications.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.notifications_off, size: 64, color: Colors.grey[400]),
            const SizedBox(height: 16),
            const Text(
              'لا توجد إشعارات',
              style: TextStyle(fontSize: 18, color: Color(0xFF7B8D9E)),
            ),
            const SizedBox(height: 8),
            const Text(
              'ستظهر هنا التحديثات والإشعارات الجديدة',
              style: TextStyle(color: Color(0xFF9AA5B5)),
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _loadNotifications,
      child: ListView.builder(
        padding: const EdgeInsets.all(16),
        itemCount: _notifications.length,
        itemBuilder: (context, index) =>
            _buildNotificationCard(_notifications[index]),
      ),
    );
  }

  Widget _buildNotificationCard(NotificationRecipientModel item) {
    final ui = _mapUi(item);
    final backgroundColor = item.isRead
        ? Colors.white
        : const Color(0xFFE8EAF6);

    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      decoration: BoxDecoration(
        color: backgroundColor,
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: Colors.grey.withValues(alpha: 0.1),
            spreadRadius: 1,
            blurRadius: 4,
            offset: const Offset(0, 2),
          ),
        ],
        border: item.isRead
            ? null
            : Border.all(
                color: const Color(0xFF3F51B5).withValues(alpha: 0.2),
                width: 1,
              ),
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          borderRadius: BorderRadius.circular(12),
          onTap: () => _openNotification(item),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  width: 48,
                  height: 48,
                  decoration: BoxDecoration(
                    color: ui.color.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Icon(ui.icon, color: ui.color, size: 24),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        item.notification.title,
                        style: TextStyle(
                          fontSize: 16,
                          fontWeight: item.isRead
                              ? FontWeight.w500
                              : FontWeight.bold,
                          color: const Color(0xFF2C3E50),
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        item.notification.body,
                        style: const TextStyle(
                          fontSize: 14,
                          color: Color(0xFF5A6C7D),
                        ),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                      ),
                      const SizedBox(height: 8),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Row(
                            children: [
                              Icon(
                                Icons.access_time,
                                color: Colors.grey[500],
                                size: 14,
                              ),
                              const SizedBox(width: 4),
                              Text(
                                item.notification.formattedTime(),
                                style: TextStyle(
                                  fontSize: 12,
                                  color: Colors.grey[600],
                                ),
                              ),
                            ],
                          ),
                          // شارة رقم البلاغ
                          if (item.notification.reportId != null)
                            Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 8,
                                vertical: 4,
                              ),
                              decoration: BoxDecoration(
                                color: const Color(
                                  0xFF3F51B5,
                                ).withValues(alpha: 0.1),
                                borderRadius: BorderRadius.circular(4),
                              ),
                              child: Text(
                                '#${item.notification.reportId}',
                                style: const TextStyle(
                                  fontSize: 12,
                                  color: Color(0xFF3F51B5),
                                  fontWeight: FontWeight.w500,
                                ),
                              ),
                            ),
                          // ✅ شارة المقال التوعوي
                          if (item.notification.awarenessContentId != null)
                            Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 8,
                                vertical: 4,
                              ),
                              decoration: BoxDecoration(
                                color: const Color(
                                  0xFF2E7D32,
                                ).withValues(alpha: 0.1),
                                borderRadius: BorderRadius.circular(4),
                              ),
                              child: const Row(
                                mainAxisSize: MainAxisSize.min,
                                children: [
                                  Icon(
                                    Icons.menu_book_rounded,
                                    size: 12,
                                    color: Color(0xFF2E7D32),
                                  ),
                                  SizedBox(width: 4),
                                  Text(
                                    'اقرأ المقال',
                                    style: TextStyle(
                                      fontSize: 11,
                                      color: Color(0xFF2E7D32),
                                      fontWeight: FontWeight.w600,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                        ],
                      ),
                    ],
                  ),
                ),
                if (!item.isRead)
                  Padding(
                    padding: const EdgeInsets.only(left: 8),
                    child: Container(
                      width: 8,
                      height: 8,
                      decoration: BoxDecoration(
                        color: const Color(0xFFF44336),
                        borderRadius: BorderRadius.circular(4),
                      ),
                    ),
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  void _showNotificationDetails(
    BuildContext context,
    NotificationRecipientModel item,
  ) {
    final ui = _mapUi(item);

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      builder: (context) {
        return Container(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Center(
                child: Container(
                  width: 40,
                  height: 4,
                  decoration: BoxDecoration(
                    color: Colors.grey[300],
                    borderRadius: BorderRadius.circular(2),
                  ),
                ),
              ),
              const SizedBox(height: 20),
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Container(
                    width: 56,
                    height: 56,
                    decoration: BoxDecoration(
                      color: ui.color.withValues(alpha: 0.1),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Icon(ui.icon, color: ui.color, size: 28),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          item.notification.title,
                          style: const TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.bold,
                            color: Color(0xFF2C3E50),
                          ),
                        ),
                        const SizedBox(height: 4),
                        Row(
                          children: [
                            Icon(
                              Icons.access_time,
                              color: Colors.grey[500],
                              size: 16,
                            ),
                            const SizedBox(width: 4),
                            Expanded(
                              child: Text(
                                item.notification.formattedTime(),
                                style: TextStyle(
                                  fontSize: 14,
                                  color: Colors.grey[600],
                                ),
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.close),
                    onPressed: () => Navigator.pop(context),
                  ),
                ],
              ),
              const SizedBox(height: 24),
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: const Color(0xFFF8FAFF),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Text(
                  item.notification.body,
                  style: const TextStyle(
                    fontSize: 16,
                    color: Color(0xFF34495E),
                    height: 1.6,
                  ),
                ),
              ),
              if (item.notification.reportId != null) ...[
                const SizedBox(height: 16),
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: const Color(0xFFE8EAF6),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Row(
                    children: [
                      const Icon(Icons.report, color: Color(0xFF3F51B5)),
                      const SizedBox(width: 12),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Text(
                              'رقم البلاغ المرتبط',
                              style: TextStyle(
                                fontSize: 14,
                                color: Color(0xFF5A6C7D),
                              ),
                            ),
                            const SizedBox(height: 4),
                            Text(
                              '#${item.notification.reportId}',
                              style: const TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                                color: Color(0xFF3F51B5),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ],
              const SizedBox(height: 24),
              SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  onPressed: () => Navigator.pop(context),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF3F51B5),
                    padding: const EdgeInsets.symmetric(vertical: 16),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  child: const Text(
                    'حسناً',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: Colors.white,
                    ),
                  ),
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  _NotificationUi _mapUi(NotificationRecipientModel item) {
    if (item.notification.awarenessContentId != null) {
      return const _NotificationUi(
        color: Color(0xFF2E7D32),
        icon: Icons.menu_book_rounded,
      );
    }
    if (item.notification.reportAssignmentId != null) {
      return const _NotificationUi(
        color: Color(0xFF2196F3),
        icon: Icons.assignment_turned_in_rounded,
      );
    }
    if (item.notification.reportId != null) {
      return const _NotificationUi(
        color: Color(0xFFFF9800),
        icon: Icons.report_problem_rounded,
      );
    }
    return const _NotificationUi(
      color: Color(0xFF757575),
      icon: Icons.notifications,
    );
  }
}

class _NotificationUi {
  final Color color;
  final IconData icon;
  const _NotificationUi({required this.color, required this.icon});
}
