using Ares.Core.Analyzing;
using Ares.Core.Validation;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ares.Core.Tests;

internal class AnalyzerManagerTests
{
  private IAnalyzerRepo _analyzerRepo = new AnalyzerRepo();


  [SetUp]
  public void SetUp()
  {
    _analyzerRepo = new AnalyzerRepo();
  }

  [Test]
  public void Manager_Should_Get_Typed_Analyzer_By_Version()
  {
    var analyzer = new TempAnalyzer("Test", new Version(1, 0));
    _analyzerRepo.RegisterAnalyzer(analyzer);
    var returnedAnalyzer = _analyzerRepo.GetAnalyzer<TempAnalyzer>(new Version(1, 0));
    Assert.That(analyzer, Is.SameAs(returnedAnalyzer));
  }

  [Test]
  public void Manager_Should_Throw_When_No_Analyzer_By_Version()
  {
    Assert.Throws<KeyNotFoundException>(() => _analyzerRepo.GetAnalyzer<TempAnalyzer>(new Version(1, 0)));
  }

  private class TempAnalyzer : IAnalyzer
  {
    public TempAnalyzer(string name, Version version)
    {
      Name = name;
      Version = version;
    }

    public string Name { get; set; }
    public Version Version { get; set; }
    public string Address { get; set; }
    public string UniqueId { get; set; } = new Guid().ToString();
    public IObservable<AnalyzerState> AnalyzerStateObservable { get; }
    public AnalyzerState AnalyzerState { get; }

    public bool InputsSupported(string fullTypeName)
      => true;

    public Task<Analysis> Analyze(ExperimentResult result, Any input, CancellationToken cancellationToken)
      => throw new NotImplementedException();

    public Task<ValidationResult> ValidateInput(AnalyzerInputValidationRequest validationRequest)
    {
      throw new NotImplementedException();
    }

    public Task<ValidationResult> ValidateInputs(IEnumerable<AnalyzerInputValidationRequest> validationRequests)
    {
      throw new NotImplementedException();
    }

    public Task<RequestedAnalysisData[]> GetSupportedInputs()
    {
      throw new NotImplementedException();
    }

    public Task<Analysis> Analyze(ExperimentExecutionSummary summary, IEnumerable<AnalyzerInput> inputs, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }
}
