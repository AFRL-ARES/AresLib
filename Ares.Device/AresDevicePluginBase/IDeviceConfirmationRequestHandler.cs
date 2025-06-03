using System.Threading.Tasks;

namespace Ares.Device;

public interface IDeviceConfirmationRequestHandler
{
  Task RequestConfirmation(string message);
}
