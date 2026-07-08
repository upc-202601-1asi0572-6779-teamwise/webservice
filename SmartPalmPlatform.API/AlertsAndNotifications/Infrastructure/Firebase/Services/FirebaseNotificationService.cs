using System.Text;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using SmartPalmPlatform.API.AlertsAndNotifications.Application.OutboundServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Infrastructure.Firebase.Services;

public class FirebaseNotificationService : IFirebaseNotificationService
{
    private readonly FirebaseMessaging? _messaging;

    public FirebaseNotificationService()
    {
        var json = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");
        if (string.IsNullOrEmpty(json))
        {
            Console.WriteLine(
                "[Firebase] FIREBASE_CREDENTIALS_JSON not set. Notifications disabled."
            );
            return;
        }

        if (FirebaseApp.DefaultInstance is null)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var serviceCredential = ServiceAccountCredential.FromServiceAccountData(stream);
            FirebaseApp.Create(
                new AppOptions { Credential = serviceCredential.ToGoogleCredential() }
            );
        }

        _messaging = FirebaseMessaging.DefaultInstance;
    }

    public async Task SendNotificationAsync(Alert alert)
    {
        if (_messaging is null)
        {
            Console.WriteLine(
                $"[Firebase] Skipped notification for alert {alert.Id}: {alert.Message}"
            );
            return;
        }

        var message = new Message
        {
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = alert.Level.ToString(),
                Body = alert.Message,
            },
            Topic = "alerts",
        };

        await _messaging.SendAsync(message);
    }
}
