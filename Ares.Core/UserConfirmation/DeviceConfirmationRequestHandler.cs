using Ares.Device;

namespace Ares.Core.UserConfirmation;

public class DeviceConfirmationRequestHandler : IDeviceConfirmationRequestHandler
{
  private IEnumerable<IUserConfirmationRequestHandler> _handlers;
  public DeviceConfirmationRequestHandler(IEnumerable<IUserConfirmationRequestHandler> handlers)
  {
    _handlers = handlers;
  }

  public async Task RequestConfirmation(string message)
  {
    foreach(var handler in _handlers)
    {
      await handler.Handle(message);
    }
  }
}
