using System.Collections.ObjectModel;
using Ares.Core.Messages;

namespace AresLib.Builders
{
  public interface IStepTemplateBuilder : ITemplateBuilder<StepTemplate>
  {
    ICommandTemplateBuilder AddCommandTemplateBuilder(CommandMetadata commandMetadata);
    void RemoveCommandTemplateBuilder(ICommandTemplateBuilder templateBuilder);
    ReadOnlyObservableCollection<ICommandTemplateBuilder> CommandTemplateBuilders { get; }
  }
}
