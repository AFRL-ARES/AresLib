using Ares.Core.Analyzing;
using Ares.Core.Execution.StopConditions;

namespace Ares.Core.Execution;
internal class DesiredAnalysisResultFactory : IDesiredAnalysisResultFactory
{
  readonly AnalysisRepo _analyses;
  public DesiredAnalysisResultFactory(AnalysisRepo analyses)
  {
    _analyses = analyses;
  }

  public DesiredAnalysisResult Create(double desiredResult, double leeway)
  {
    return new DesiredAnalysisResult(_analyses, desiredResult, leeway);
  }
}
