using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib
{
  internal class BridgedQualifiedCommandComposer : CommandComposer<CommandTemplate>
  {
    public override Task Compose() => QualifiedCommandComposer.Compile();

    public ICommandComposer<CommandTemplate> QualifiedCommandComposer { get; init; }

  }
}
