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
    public CommandParameterBuilder(CommandParameterMetadata parameterMetadata)
    {
      Metadata = parameterMetadata;
    }

    public CommandParameter Build()
    {
      var commandParameter = new CommandParameter();
      commandParameter.Metadata = Metadata;
      commandParameter.Value = (float)Value;
      return commandParameter;
    }

    public CommandParameterMetadata Metadata { get; }

    public double Value { get; set; }
  }
}
