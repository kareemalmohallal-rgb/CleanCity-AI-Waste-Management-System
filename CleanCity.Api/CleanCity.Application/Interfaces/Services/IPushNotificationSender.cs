namespace CleanCity.Application.Interfaces.Services
{
    public interface IPushNotificationSender
    {
        Task SendToTokenAsync(
            string token,
            string title,
            string body,
            Dictionary<string, string>? data = null,
            CancellationToken ct = default);

        Task SendToTopicAsync(
            string topic,
            string title,
            string body,
            Dictionary<string, string>? data = null,
            CancellationToken ct = default);
    }
}