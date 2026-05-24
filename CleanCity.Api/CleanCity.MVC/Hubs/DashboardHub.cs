using Microsoft.AspNetCore.SignalR;

namespace CleanCity.MVC.Hubs
{
    /// <summary>
    /// Hub مركزي لإرسال إشعارات التحديث الفوري لجميع الصفحات.
    /// يُستخدم عبر IHubContext&lt;DashboardHub&gt; من أي Controller أو Service.
    /// </summary>
    public class DashboardHub : Hub { }
}