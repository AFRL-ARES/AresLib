using Ares.Core.Analyzing;
using Ares.Messaging;
using Ares.Test;

namespace Ares.Core.Tests.Data.Analyzer;

public class TestReplyAnalyzer : AnalyzerBase
{
  public TestReplyAnalyzer() : base("Test Analyzer", new Version(1, 0))
  {
  }

  public override Task<RequestedAnalysisData[]> GetSupportedInputs()
  {
    throw new NotImplementedException();
  }

  protected override Task<Analysis> AnalyzeInputs(ExperimentExecutionSummary summary, IEnumerable<AnalyzerInput> inputs, CancellationToken cancellationToken)
  {
    var firstData = inputs.First().Data;
    var reply = firstData.Unpack<TestReply>();

    var analysis = new Analysis
    {
      Analyzer = new AnalyzerInfo
      {
        Name = Name,
        Type = nameof(TestReplyAnalyzer),
        UniqueId = Guid.NewGuid().ToString(),
        Version = Version.ToString()
      },
      Result = reply.Number,
      UniqueId = Guid.NewGuid().ToString()
    };

    return Task.FromResult(analysis);
  }
}
