using Ares.Core.UserConfirmation;
using System.Threading.Tasks;

namespace Ares.Core.Grpc.Services.UserConfirmation;

public class UserConfirmationRequestHandler : IUserConfirmationRequestHandler
{
  private readonly AresUserConfirmationService _confirmationService;

  public UserConfirmationRequestHandler(AresUserConfirmationService confirmationService)
  {
    _confirmationService = confirmationService;
  }

  public async Task Handle(string message)
  {
    await _confirmationService.RequestUserConfirmation(message);
  }
}
