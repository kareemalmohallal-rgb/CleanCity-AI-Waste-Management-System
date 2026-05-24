using CleanCity.Application.Interfaces.Services;
using FirebaseAdmin.Messaging;

namespace CleanCity.Infrastructure.Notifications
{
    public class FirebasePushNotificationSender : IPushNotificationSender
    {
        public async Task SendToTokenAsync(
            string token,
            string title,
            string body,
            Dictionary<string, string>? data = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(token))
                return;

            var message = new Message
            {
                Token = token,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data ?? new Dictionary<string, string>(),
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ChannelId = "high_importance_channel"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message, ct);
        }

        public async Task SendToTopicAsync(
            string topic,
            string title,
            string body,
            Dictionary<string, string>? data = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return;

            var message = new Message
            {
                Topic = topic,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data ?? new Dictionary<string, string>(),
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ChannelId = "high_importance_channel"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message, ct);
        }
    }
}