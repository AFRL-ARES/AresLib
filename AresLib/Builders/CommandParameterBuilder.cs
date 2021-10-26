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
    public CommandParameterBuilder(ParameterMetadata parameterMetadata)
    {
      Metadata = parameterMetadata;
    }

    public Parameter Build()
    {
      var commandParameter = new Parameter();
      commandParameter.Metadata = Metadata;
      commandParameter.Value = (float)Value;
      return commandParameter;
    }

    public ParameterMetadata Metadata { get; }

    public double Value { get; set; }
  }
}
