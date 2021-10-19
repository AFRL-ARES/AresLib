using System.Threading.Tasks;

namespace AresLib
{
  public interface IDeviceDomainTranslator
  {
    Task GetDeviceCommand(AresCommand aresCommand);
  }
}
