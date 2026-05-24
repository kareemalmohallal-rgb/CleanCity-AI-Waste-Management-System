using CleanCity.Domain.Entities;

namespace CleanCity.Domain.Entities
{
    public class NotificationRecipient
    {
        public int Id { get; set; }

        public int NotificationId { get; set; }
        public Notification Notification { get; set; } = default!;

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public int? AnonymousDeviceId { get; set; }
        public AnonymousDevice? AnonymousDevice { get; set; }

        public int? DriverId { get; set; }
        public Driver? Driver { get; set; }

        public string? UserId { get; set; }

        public int? DeviceTokenId { get; set; }
        public DeviceToken? DeviceToken { get; set; }

        public void ValidateRecipient()
        {
            int count =
                (AnonymousDeviceId.HasValue ? 1 : 0) +
                (DriverId.HasValue ? 1 : 0) +
                (!string.IsNullOrWhiteSpace(UserId) ? 1 : 0) +
                (DeviceTokenId.HasValue ? 1 : 0);

            if (count != 1)
                throw new InvalidOperationException("NotificationRecipient يجب أن يحتوي على مستلم واحد فقط (AnonymousDevice أو Driver أو UserId أو DeviceToken).");
        }

    }

}
