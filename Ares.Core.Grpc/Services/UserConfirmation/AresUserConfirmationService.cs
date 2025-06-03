using Ares.Messaging;
using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Ares.Core.Grpc.Services.UserConfirmation;

public class AresUserConfirmationService : AresUserConfirmationRpc.AresUserConfirmationRpcBase
{
  private static ConcurrentDictionary<string, IServerStreamWriter<ServiceResponse>> _clients = new();
  private static ConcurrentDictionary<string, TaskCompletionSource<ConfirmationResponse>> _pendingConfirmations = new();

  public override async Task ConfirmationStream(IAsyncStreamReader<UIRequest> requestStream,
    IServerStreamWriter<ServiceResponse> responseStream,
    ServerCallContext context)
  {
    string? clientId = null;

    //Task to read incoming message from the UI client
    var readTask = Task.Run(async () =>
    {
      await foreach(var request in requestStream.ReadAllAsync())
      {
        if(request.RequestContentCase == UIRequest.RequestContentOneofCase.InitialConnect)
        {
          clientId = request.InitialConnect.ClientId;
          _clients[clientId] = responseStream;
          Console.WriteLine($"Confirmation service connected new client {clientId}");

          //Send Acknowledge
          await responseStream.WriteAsync(new ServiceResponse()
          {
            Acknowledge = new Acknowledge()
            {
              MessageId = "initial_connect_ack",
              Status = "CONNECTED"
            }
          });
        }

        else if(request.RequestContentCase == UIRequest.RequestContentOneofCase.ConfirmationResponse)
        {
          string requestId = request.ConfirmationResponse.RequestId;

          if(_pendingConfirmations.TryRemove(requestId, out var tcs))
          {
            tcs.SetResult(request.ConfirmationResponse);
          }

          else
          {
            //TODO: Notify?
            Console.WriteLine($"Warning: Received ActionResponse for unknown RequestId: {requestId}");
          }

          //Acknowledge the action response
          await responseStream.WriteAsync(new ServiceResponse()
          {
            Acknowledge = new Acknowledge()
            {
              MessageId = requestId,
              Status = "ACTION_RESPONSE_RECEIVED"
            }
          });
        }
      }
    });

    await readTask;

    if(clientId != null)
    {
      _clients.TryRemove(clientId, out _);

      foreach(var kvp in _pendingConfirmations.Where(p => p.Value.Task.Status == TaskStatus.WaitingForActivation))
      {
        if(kvp.Value.Task.IsCompleted == false)
        {
          kvp.Value.TrySetCanceled();
          //TODO: Notify?
          Console.WriteLine($"Cancelled pending confirmation {kvp.Key} due to UI disconnet");
        }
      }
    }
  }

  public async Task<ConfirmationResponse> RequestUserConfirmation(string details)
  {
    if(!_clients.Any())
      throw new InvalidOperationException("ARES was unable to find a valid client to connect to!");

    //TODO: Handle possibility of multiple clients?
    var clientStream = _clients.FirstOrDefault().Value;

    string requestId = Guid.NewGuid().ToString();
    var tcs = new TaskCompletionSource<ConfirmationResponse>();
    _pendingConfirmations[requestId] = tcs;

    await clientStream.WriteAsync(new ServiceResponse
    {
      ConfirmationRequest = new ConfirmationRequest()
      {
        RequestId = requestId,
        UserComment = details
      }
    });

    var response = await tcs.Task;

    return response;
  }

}
