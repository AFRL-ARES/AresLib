using System;
namespace AresLib
{
  public abstract class AresDevice : IAresDevice
  {

    protected abstract IDeviceDomainTranslator DomainTranslator { get; }
    public Guid Id { get; }
    public CommandIssueResult IssueCommand(AresCommand command)
    {
      throw new System.NotImplementedException();
    }
  }
}
