﻿using System;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Ares.Core.Device;
using Ares.Device;
using Ares.Messaging;
using Ares.Messaging.Device;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.Grpc.Services;

public class DevicesService : AresDevices.AresDevicesBase
{
  private readonly IDbContextFactory<CoreDatabaseContext> _contextFactory;
  private readonly IDeviceCommandInterpreterRepo _deviceCommandInterpreterRepo;
  private readonly IDeviceConfigSaver _deviceConfigSaver;

  public DevicesService(IDeviceCommandInterpreterRepo deviceCommandInterpreterRepo, IDeviceConfigSaver deviceConfigSaver, IDbContextFactory<CoreDatabaseContext> contextFactory)
  {
    _deviceCommandInterpreterRepo = deviceCommandInterpreterRepo;
    _deviceConfigSaver = deviceConfigSaver;
    _contextFactory = contextFactory;
  }

  public override Task<ListServerSerialPortsResponse> GetServerSerialPorts(Empty request, ServerCallContext context)
  {
    var availableSerialPorts = SerialPort.GetPortNames();
    var response = new ListServerSerialPortsResponse { SerialPorts = { availableSerialPorts } };
    return Task.FromResult(response);
  }

  public override Task<ListAresDevicesResponse> ListAresDevices(Empty _, ServerCallContext context)
  {
    var aresDeviceMessages = _deviceCommandInterpreterRepo
      .Select(interpreter => interpreter.Device)
      .Select(device => new AresDeviceInfo { Name = device.Name, Type = device.GetType().FullName });

    var response = new ListAresDevicesResponse
    {
      AresDevices = { aresDeviceMessages }
    };

    return Task.FromResult(response);
  }

  public override Task<DeviceStatus> GetDeviceStatus(DeviceStatusRequest request, ServerCallContext context)
  {
    try
    {
      var aresDevice = GetAresDevice(request.DeviceName);

      return Task.FromResult(aresDevice.Status);
    }
    catch (InvalidOperationException e)
    {
      return Task.FromResult(new DeviceStatus { DeviceState = DeviceState.Error, Message = e.Message });
    }
  }

  public override Task<CommandMetadatasResponse> GetCommandMetadatas(CommandMetadatasRequest request, ServerCallContext context)
  {
    var interpreter = _deviceCommandInterpreterRepo
      .First(commandInterpreter => commandInterpreter.Device.Name == request.DeviceName);

    var commands = interpreter.CommandsToIndexedMetadatas();

    var response = new CommandMetadatasResponse();
    response.Metadatas.AddRange(commands);

    return Task.FromResult(response);
  }

  public override async Task<DeviceCommandResult> ExecuteCommand(CommandTemplate request, ServerCallContext context)
  {
    var interpreter = _deviceCommandInterpreterRepo
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
    var aresDevice = _deviceCommandInterpreterRepo
      .Select(interpreter => interpreter.Device)
      .FirstOrDefault(device => device.Name == name);

    if (aresDevice is null)
      throw new InvalidOperationException($"Could not find ARES device: {name}");

    return aresDevice;
  }

  public override Task<Empty> AddDeviceConfig(DeviceConfig request, ServerCallContext context)
  {
    return _deviceConfigSaver.AddConfig(request).ContinueWith(_ => new Empty());
  }

  public override Task<Empty> RemoveDeviceConfig(RemoveDeviceConfigRequest request, ServerCallContext context)
  {
    return _deviceConfigSaver.RemoveConfig(Guid.Parse(request.ConfigId)).ContinueWith(_ => new Empty());
  }

  public override Task<Empty> UpdateDeviceConfig(DeviceConfig request, ServerCallContext context)
  {
    return _deviceConfigSaver.UpdateConfig(request).ContinueWith(_ => new Empty());
  }

  public override async Task<DeviceConfigResponse> GetAllDeviceConfigs(DeviceConfigRequest request, ServerCallContext context)
  {
    await using var dbContext = _contextFactory.CreateDbContext();
    var configQuery = dbContext.DeviceConfigs.AsQueryable();
    if (!string.IsNullOrEmpty(request.DeviceType))
      configQuery = configQuery.Where(config => config.DeviceType == request.DeviceType);

    var configs = await configQuery.ToArrayAsync();
    var response = new DeviceConfigResponse();
    response.Configs.AddRange(configs);
    return response;
  }
}
