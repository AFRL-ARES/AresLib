using Ares.Core.Analyzing;
using Ares.Core.Composers;
using Ares.Core.Execution;
using Ares.Core.Execution.Executors;
using Ares.Core.Execution.StartConditions;
using Ares.Core.Planning;
using Ares.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Ares.Core;

public static class ServiceCollectionExtensions
{
  public static void AddAresCoreComponents(this IServiceCollection services)
  {
    services.AddSingleton<IExecutionManager, ExecutionManager>();
    services.AddSingleton<ICommandComposer<StepTemplate, StepExecutor>, StepComposer>();
    services.AddSingleton<ICommandComposer<ExperimentTemplate, ExperimentExecutor>, ExperimentComposer>();
    services.AddSingleton<IPlanningHelper, PlanningHelper>();
    services.AddSingleton<IPlannerManager, PlannerManager>();
    services.AddTransient<IStartCondition, CampaignInProgressStartCondition>();
    services.AddSingleton<IExecutionReporter, ExecutionReporter>();
    services.AddSingleton<IStartConditionCollector, StartConditionCollector>();
    services.AddSingleton<IAnalyzerManager, AnalyzerManager>();
  }
}
