using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib.Builders
{
  public interface IExperimentTemplateBuilder : ITemplateBuilder<ExperimentTemplate>
  {
    IStepTemplateBuilder AddStepTemplateBuilder(string stepName, bool isParallel);
    void RemoveStepTemplateBuilder(string stepName);
    ReadOnlyObservableCollection<IStepTemplateBuilder> StepTemplateBuilders { get; }
  }
}
