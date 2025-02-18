using System.Threading.Tasks;
using Ares.Messaging;
using Grpc.Core;

namespace Ares.Core.Grpc.Services;

class EventService : AresEvent.AresEventBase
{
  public EventService()
  {
    
  }

  public override Task<NotificationResponse> GetAllNotifications(NotificationRequest request, ServerCallContext context)
  {
    return base.GetAllNotifications(request, context);
  }
}
