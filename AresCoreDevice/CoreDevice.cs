using System;
namespace AresLib.AresCoreDevice
{
  internal class CoreDevice : IAresDevice
  {

    public IDeviceDomainTranslator DomainTranslator { get; }
    public Guid Id { get; } = Guid.NewGuid();
    public CommandIssueResult IssueCommand(AresCommand command)
    {
      var success = Enum.TryParse<CoreDeviceCommandType>(command.Name, out var commandType);
      if (!success)
        return new CommandIssueResult
        {
          Success = false,
          Error = $"{this} does not support a command of name {command.Name}"
        };



      return new CommandIssueResult { Success = true };
    }
  }
}
