using System;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Ares.Device;
using Ares.Messaging;
using Ares.Messaging.Device;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Ares.Core.Grpc.Services;

public class DevicesService : AresDevices.AresDevicesBase
{
  private readonly ILaboratoryManager _laboratoryManager;

  public DevicesService(ILaboratoryManager laboratoryManager)
  {
    _laboratoryManager = laboratoryManager;
  }

  public override Task<ListServerSerialPortsResponse> GetServerSerialPorts(Empty request, ServerCallContext context)
  {
    var availableSerialPorts = SerialPort.GetPortNames();
    var response = new ListServerSerialPortsResponse { SerialPorts = { availableSerialPorts } };
    return Task.FromResult(response);
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

    return Task.FromResult(aresDevice.Status);
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

  public override async Task<DeviceCommandResult> ExecuteCommand(CommandTemplate request, ServerCallContext context)
  {
    var interpreter = _laboratoryManager.Lab.DeviceInterpreters
      .First(commandInterpreter => commandInterpreter.Device.Name == request.Metadata.DeviceName);

    try
    {
      var deviceCommandTask = interpreter.TemplateToDeviceCommand(request);
      var result = await deviceCommandTask(context.CancellationToken);
      return result;
    }
    catch (Exception e)
    {
      return new DeviceCommandResult { Success = false, Error = e.ToString() };
    }
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
