using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib.Builders
{
  public interface IStepTemplateBuilder : ITemplateBuilder<StepTemplate>
  {
    ICommandTemplateBuilder AddCommandTemplateBuilder(CommandMetadata commandMetadata);
    void RemoveCommandTemplateBuilder(string templateBuilderName);
    ReadOnlyObservableCollection<ICommandTemplateBuilder> CommandTemplateBuilders { get; }
  }
}
