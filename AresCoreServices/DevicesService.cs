using Ares.Core.Messages.Device;
using AresLib;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;

namespace AresCoreServices
{
  public class DevicesService : AresDevices.AresDevicesBase
  {
    private readonly ILaboratoryManager _laboratoryManager;
    public DevicesService(ILaboratoryManager laboratoryManager)
    {
      _laboratoryManager = laboratoryManager;
    }

    public override Task<ListAresDevicesResponse> ListAresDevices(ListAresDevicesRequest _, ServerCallContext context)
    {
      var aresDeviceMessages = _laboratoryManager.Lab.DeviceInterpreters
        .Select(interpreter => interpreter.Device)
        .Select(device => device.Name)
        .Select(s => new AresDevice { Name = s });

      var response = new ListAresDevicesResponse
      {
        AresDevices = { aresDeviceMessages }
      };

      return Task.FromResult(response);
    }
  }
}
