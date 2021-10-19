using System;
namespace AresLib.AresCoreDevice
{
  internal class CoreDevice : IAresDevice
  {

    public IDeviceDomainTranslator DomainTranslator { get; }
    public void IssueCommand(AresCommand command)
    {
      throw new NotImplementedException();
    }
  }
}
