using System.Threading.Tasks;

namespace AresLib
{
  public interface IDeviceDomainTranslator
  {
    IAresDevice Device { get; }
    CommandDomainCoupling GenerateCommandDomains(CommandMetadata commandMeta);
  }
}
