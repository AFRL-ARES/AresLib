using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Ares.Core.Messages.Device;
using AresDevicePluginBase;
using AresLib;
using Grpc.Core;
namespace AresCoreServices;

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
      .Select(s => new Ares.Core.Messages.Device.AresDevice { Name = s });

    var response = new ListAresDevicesResponse
    {
      AresDevices = { aresDeviceMessages }
    };

    return Task.FromResult(response);
  }

  public override Task<DeviceStatus> GetDeviceStatus(DeviceStatusRequest request, ServerCallContext context)
  {
    var aresDevice = GetAresDevice(request.DeviceName);

    return aresDevice.Status.FirstAsync().ToTask();
  }

  private IAresDevice GetAresDevice(string name)
  {
    var aresDevice = _laboratoryManager.Lab.DeviceInterpreters
      .Select(interpreter => interpreter.Device)
      .FirstOrDefault(device => device.Name == name);

    if (aresDevice is null)
      throw new InvalidOperationException($"Could not find ARES device: {name}");

    return aresDevice;
  }
}
