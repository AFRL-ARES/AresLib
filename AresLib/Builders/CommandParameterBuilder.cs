using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib.Builders
{
  internal class CommandParameterBuilder : ICommandParameterBuilder
  {
    public CommandParameter Build()
    {
      throw new NotImplementedException();
    }

    public CommandParameterMetadata Metadata { get; init; }

    public double Value { get; init; }
  }
}
