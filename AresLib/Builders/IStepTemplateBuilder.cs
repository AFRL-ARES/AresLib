using Ares.Core;
using System.Collections.ObjectModel;

namespace AresLib.Builders
{
  public interface IStepTemplateBuilder : ITemplateBuilder<StepTemplate>
  {
    ICommandTemplateBuilder AddCommandTemplateBuilder(CommandMetadata commandMetadata);
    void RemoveCommandTemplateBuilder(ICommandTemplateBuilder templateBuilder);
    ReadOnlyObservableCollection<ICommandTemplateBuilder> CommandTemplateBuilders { get; }
  }
}
