namespace AresLib
{
  public abstract class AresDevice : IAresDevice
  {

    protected abstract IDeviceDomainTranslator DomainTranslator { get; }
    public void IssueCommand(AresCommand command)
    {
      throw new System.NotImplementedException();
    }
  }
}
