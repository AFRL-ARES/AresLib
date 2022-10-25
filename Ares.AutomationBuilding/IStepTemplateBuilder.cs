using System.Collections.ObjectModel;
using Ares.Messaging;

namespace Ares.AutomationBuilding;

public interface IStepTemplateBuilder : ITemplateBuilder<StepTemplate>
{
  ReadOnlyObservableCollection<ICommandTemplateBuilder> CommandTemplateBuilders { get; }
  ICommandTemplateBuilder AddCommandTemplateBuilder(CommandMetadata commandMetadata);
  void RemoveCommandTemplateBuilder(ICommandTemplateBuilder templateBuilder);
}
