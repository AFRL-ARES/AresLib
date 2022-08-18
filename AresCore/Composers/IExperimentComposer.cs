using Ares.Core.Execution.Executors;
using Ares.Messaging;

namespace Ares.Core.Composers;

public interface IExperimentComposer
{
  ExperimentExecutor Compose(ExperimentTemplate template, IEnumerable<ParameterMetadata> plannableParameters);
  ExperimentExecutor Compose(ExperimentTemplate template);
}
