using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Ares.Device;
using Ares.Messaging;
using Ares.Messaging.Device;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Ares.Core.Grpc;

public class DevicesService : AresDevices.AresDevicesBase
{
  private readonly ILaboratoryManager _laboratoryManager;

  public DevicesService(ILaboratoryManager laboratoryManager)
  {
    _laboratoryManager = laboratoryManager;
  }

  public override Task<ListAresDevicesResponse> ListAresDevices(Empty _, ServerCallContext context)
  {
    var aresDeviceMessages = _laboratoryManager.Lab.DeviceInterpreters
      .Select(interpreter => interpreter.Device)
      .Select(device => device.Name)
      .Select(s => new AresDeviceInfo { Name = s });

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

  public override Task<CommandMetadatasResponse> GetCommandMetadatas(CommandMetadatasRequest request, ServerCallContext context)
  {
    var interpreter = _laboratoryManager.Lab.DeviceInterpreters
      .First(commandInterpreter => commandInterpreter.Device.Name == request.DeviceName);

    var commands = interpreter.CommandsToIndexedMetadatas();

    var response = new CommandMetadatasResponse();
    response.Metadatas.AddRange(commands);

    return Task.FromResult(response);
  }

  public override async Task<CommandResult> ExecuteCommand(CommandTemplate request, ServerCallContext context)
  {
    var test = Any.Pack(new Int32Value { Value = 12 });
    var interpreter = _laboratoryManager.Lab.DeviceInterpreters
      .First(commandInterpreter => commandInterpreter.Device.Name == request.Metadata.DeviceName);

    var aaaa = interpreter.TemplateToDeviceCommand(request);
    aaaa.Start();
    await aaaa;

    return new CommandResult { Success = true, Result = test };
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
