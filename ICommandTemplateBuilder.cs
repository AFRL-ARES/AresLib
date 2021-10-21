using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using DynamicData;

namespace AresLib
{
  internal interface ICommandTemplateBuilder : ITemplateBuilder<CommandTemplate>
  {
    ISourceCache<ICommandParameterBuilder, string> CommandParameterBuildersSource { get; }
  }
}
