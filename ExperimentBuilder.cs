using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace AresLib
{
  internal class ExperimentBuilder : IBuilder<Experiment>
  {

    // TODO: Pick up here
    public ReadOnlyObservableCollection<StepBuilder> StepBuilders { get; }

    public Experiment Build()
    {
      throw new NotImplementedException();
    }
  }
}
