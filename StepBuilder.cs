using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal class StepBuilder : IBuilder<ExperimentStep>
  {
    private CommandBuilder[] DeviceCommands { get; }
    public ExperimentStep Build()
    {
      throw new NotImplementedException();
    }
  }
}
