using Ares.Messaging;
using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Ares.Core.Grpc.Services.Notifications;

public class AresNotificationService : AresNotificationRpc.AresNotificationRpcBase
{
  private static readonly ConcurrentDictionary<string, IServerStreamWriter<AresNotification>> _clients = new();

  public override async Task Subscribe(SubscriptionRequest request, IServerStreamWriter<AresNotification> responseStream, ServerCallContext context)
  {
    string clientId = request.ClientId;
    _clients.TryAdd(clientId, responseStream);

    try
    {
      while(!context.CancellationToken.IsCancellationRequested)
      {
        await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
      }
    }

    catch(OperationCanceledException)
    {
      // Client disconnected
    }

    finally
    {
      _clients.TryRemove(clientId, out _);
    }
  }

  public async Task SendNotification(AresNotification notification)
  {
    //TODO: Maybe expand this to work with multiple clients?
    var client = _clients.FirstOrDefault();

    if(client.Value is not null)
    {
      var stream = client.Value;
      try
      {
        await stream.WriteAsync(notification);
      }
      catch(Exception ex)
      {
        Console.WriteLine($"Error sending notification to client! Exception: {ex.Message}");
        _clients.TryRemove(client);
      }
    }
  }
}
