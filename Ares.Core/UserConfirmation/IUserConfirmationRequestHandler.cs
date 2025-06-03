namespace Ares.Core.UserConfirmation;

/// <summary>
/// Interface <c>IUserConfirmationRequestHandler</c> handles confirmation request moving between Ares.Core.Grpc and Ares.Core
/// </summary>

public interface IUserConfirmationRequestHandler
{
  Task Handle(string message);
}
