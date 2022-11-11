using Ares.Messaging.Device;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.Device;

public class DeviceConfigSaver : IDeviceConfigSaver
{
  private readonly IDbContextFactory<CoreDatabaseContext> _dbContextFactory;

  public DeviceConfigSaver(IDbContextFactory<CoreDatabaseContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }

  public async Task AddConfig(DeviceConfig config)
  {
    await using var dbContext = _dbContextFactory.CreateDbContext();
    var configExists = await dbContext.DeviceConfigs.AnyAsync(deviceConfig => deviceConfig.DeviceName == config.DeviceName);
    if (configExists)
      throw new InvalidOperationException($"A config already exists for a device named {config.DeviceName}. Please pick a different name.");

    dbContext.DeviceConfigs.Add(config);
    await dbContext.SaveChangesAsync();
  }

  public async Task RemoveConfig(Guid configId)
  {
    await using var dbContext = _dbContextFactory.CreateDbContext();
    var existingConfig = await dbContext.DeviceConfigs.FirstOrDefaultAsync(deviceConfig => deviceConfig.UniqueId == configId.ToString());
    if (existingConfig is null)
      return;

    dbContext.DeviceConfigs.Remove(existingConfig);
    await dbContext.SaveChangesAsync();
  }

  public async Task UpdateConfig(DeviceConfig config)
  {
    await using var dbContext = _dbContextFactory.CreateDbContext();
    var existingConfig = await dbContext.DeviceConfigs.FirstOrDefaultAsync(deviceConfig => deviceConfig.UniqueId == config.UniqueId);
    if (existingConfig is null)
      return;

    existingConfig.ConfigData = config.ConfigData;
    await dbContext.SaveChangesAsync();
  }
}
