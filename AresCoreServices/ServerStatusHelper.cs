using System.Reactive.Subjects;
using Ares.Core.Messages;

namespace AresCoreServices;

/// <summary>
/// Helper class that a server can use to publish a status update to any client that is currently subscribed.
/// </summary>
public static class ServerStatusHelper
{
  private static readonly ISubject<ServerStatusResponse> _serverStatusSubject = new BehaviorSubject<ServerStatusResponse>(new ServerStatusResponse { ServerStatus = ServerStatus.Idle, StatusMessage = "Not doing anything" });
  public static ISubject<ServerStatusResponse> ServerStatusSubject { get; } = Subject.Synchronize(_serverStatusSubject);
}
