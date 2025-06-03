using Ares.Core.Grpc.Services.Notifications;
using Ares.Core.Grpc.Services.UserConfirmation;
using Ares.Core.Notifications;
using Ares.Core.UserConfirmation;
using Ares.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Ares.Core.Grpc;

public static class ServiceCollectionExtensions
{
  public static void AddNotificationHandlers(this IServiceCollection services)
  {
    services.AddSingleton<IAresNotificationRepo, AresNotificationRepo>();
    var notificationService = new AresNotificationService(new AresNotificationRepo());
    var handler = new NotificationHandler(notificationService);
    services.AddSingleton<INotificationHandler>(handler);
  }

  public static void AddConfirmationRequestHandlers(this IServiceCollection services)
  {
    var service = new AresUserConfirmationService();
    var handler = new UserConfirmationRequestHandler(service);
    services.AddSingleton<IUserConfirmationRequestHandler>(handler);
  }
}