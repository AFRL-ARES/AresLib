using Ares.Core;
using System.Threading.Tasks;

namespace AresLib
{
  internal class BridgedQualifiedCommandComposer : CommandComposer<CommandTemplate>
  {
    public override Task Compose() => QualifiedCommandComposer.Compose();

    public ICommandComposer<CommandTemplate> QualifiedCommandComposer { get; init; }

  }
}
