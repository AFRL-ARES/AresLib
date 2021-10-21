using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib
{
  internal interface ICommandParameterBuilder : IBuilder<CommandParameter>
  {
    CommandParameterMetadata Metadata { get; init; }
    double Value { get; init; }
  }
}
