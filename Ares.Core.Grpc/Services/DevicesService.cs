using Ares.Core.Device;
using Ares.Device;
using Ares.Messaging;
using Ares.Messaging.Device;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace Ares.Core.Grpc.Services;

public class DevicesService : AresDevices.AresDevicesBase
{
  private readonly IDbContextFactory<CoreDatabaseContext> _dbContextFactory;
  private readonly IDeviceCommandInterpreterRepo _deviceCommandInterpreterRepo;

  public DevicesService(IDeviceCommandInterpreterRepo deviceCommandInterpreterRepo, IDbContextFactory<CoreDatabaseContext> contextFactory)
  {
    _deviceCommandInterpreterRepo = deviceCommandInterpreterRepo;
    _dbContextFactory = contextFactory;
  }

  public override Task<ListServerSerialPortsResponse> GetServerSerialPorts(Empty request, ServerCallContext context)
  {
    var availableSerialPorts = SerialPort.GetPortNames();
    var cleanPorts = CleanSerialPorts(availableSerialPorts);
    var response = new ListServerSerialPortsResponse { SerialPorts = { cleanPorts } };
    return Task.FromResult(response);
  }

  private IEnumerable<string> CleanSerialPorts(IEnumerable<string> dirtyPortNames)
  {
    return dirtyPortNames.Select(s => s.IndexOf('\0') > 0 ? s[..s.IndexOf('\0')] : s);
  }

  public override async Task<Empty> Activate(DeviceActivateRequest request, ServerCallContext context)
  {
    var device = GetAresDevice(request.DeviceName);
    if (device.Status.DeviceState == DeviceState.Active)
      return new Empty();

    await device.Activate();
    return new Empty();
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

  public override async Task<DeviceConfigResponse> GetAllDeviceConfigs(DeviceConfigRequest request, ServerCallContext context)
  {
    await using var dbContext = _dbContextFactory.CreateDbContext();
    var configQuery = dbContext.DeviceConfigs.AsQueryable();
    if (!string.IsNullOrEmpty(request.DeviceType))
      configQuery = configQuery.Where(config => config.DeviceType == request.DeviceType);

    var configs = await configQuery.ToArrayAsync();
    var response = new DeviceConfigResponse();
    response.Configs.AddRange(configs);
    return response;
  }

  public override async Task<StateResponse> GetStateLogs(StateRequest request, ServerCallContext context)
  {
    var response = new StateResponse();
    var states = await GetFilteredStates(request);
    response.States.AddRange(states);
    return response;
  }

  private async Task<IEnumerable<DeviceStateLog>> GetFilteredStates(StateRequest request)
  {
    var dbContext = _dbContextFactory.CreateDbContext();
    var states = dbContext.DeviceStates.AsQueryable();
    if (request.Start is not null)
    {
      states = states.Where(state => state.Timestamp >= request.Start);
    }
    if (request.End is not null)
    {
      states = states.Where(state => state.Timestamp <= request.End);
    }
    if (request.DeviceIds.Any())
    {
      states = states.Where(state => request.DeviceIds.Contains(state.DeviceId));
    }
    if (!string.IsNullOrEmpty(request.CompletedCampaignId))
    {
      var completedCampaign = await dbContext.CampaignResults.FirstOrDefaultAsync(result => result.CampaignId == request.CompletedCampaignId);
      if (completedCampaign is not null)
        states = states.Where(state => state.Timestamp >= completedCampaign.ExecutionInfo.TimeStarted && state.Timestamp <= completedCampaign.ExecutionInfo.TimeFinished);
    }
    if (!string.IsNullOrEmpty(request.CompletedExperimentId))
    {
      var completedExperiment = await dbContext.CampaignResults
        .SelectMany(result => result.ExperimentResults)
        .FirstOrDefaultAsync(result => result.CompletedExperiment.UniqueId == request.CompletedExperimentId);

      if (completedExperiment is not null)
        states = states.Where(state => state.Timestamp >= completedExperiment.ExecutionInfo.TimeStarted && state.Timestamp <= completedExperiment.ExecutionInfo.TimeFinished);
    }

    var requestedStates = await states.OrderBy(state => state.Timestamp).ToArrayAsync();

    return requestedStates;
  }
}
