using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ares.Core.Validation;
using Ares.Messaging;

namespace Ares.Core.Analyzing;

public abstract class AnalyzerBase : IAnalyzer
{
  protected readonly ISubject<AnalyzerState> _analyzerStateSubject = new BehaviorSubject<AnalyzerState>(AnalyzerState.Disconnected);

  public AnalyzerBase(string name, Version version)
  {
    Name = name;
    Version = version;
    AnalyzerStateObservable = _analyzerStateSubject.AsObservable();
  }

  public string Name { get; }
  public Version Version { get; }
  public IObservable<AnalyzerState> AnalyzerStateObservable { get; }
  public AnalyzerState AnalyzerState { get; protected set; }

  public virtual Task<Analysis> Analyze(ExperimentExecutionSummary summary, IEnumerable<AnalyzerInput> inputs, CancellationToken cancellationToken)
  {
    var inputsArr = inputs.ToArray();
    if(!inputsArr.Any())
      return Task.FromResult(GetDefaultResult());

    return AnalyzeInputs(summary, inputs, cancellationToken);
  }

  protected Analysis GetDefaultResult()
  {
    return new Analysis
    {
      UniqueId = Guid.NewGuid().ToString(),
      Analyzer = new AnalyzerInfo { Name = Name, UniqueId = Guid.NewGuid().ToString(), Version = Version.ToString() },
      Result = 0
    };
  }

  protected abstract Task<Analysis> AnalyzeInputs(ExperimentExecutionSummary summary, IEnumerable<AnalyzerInput> inputs, CancellationToken cancellationToken);

  public abstract Task<RequestedAnalysisData[]> GetSupportedInputs();

  public async Task<ValidationResult> ValidateInput(AnalyzerInputValidationRequest validationRequest)
  {
    var inputs = await GetSupportedInputs();
    return ValidateInputHelper(validationRequest, inputs);
  }

  private ValidationResult ValidateInputHelper(AnalyzerInputValidationRequest validationRequest, IEnumerable<RequestedAnalysisData> supportedAnalysisInputs)
  {
    var requestedInput = supportedAnalysisInputs.FirstOrDefault(i => i.Key == validationRequest.Key);
    if(requestedInput is null)
    {
      return new ValidationResult(
        false,
        $"Input with key of {validationRequest.Key} is unsupported.");
    }

    if(requestedInput.Type != validationRequest.TypeName)
    {
      return new ValidationResult(
        false,
        $"Input type mismatch. Input {requestedInput.Key} expects type of {requestedInput.Key} but received {validationRequest.TypeName}");
    }

    return new ValidationResult(true);
  }

  public async Task<ValidationResult> ValidateInputs(IEnumerable<AnalyzerInputValidationRequest> validationRequests)
  {
    var inputs = await GetSupportedInputs();
    var validationResults = validationRequests.Select(vr => ValidateInputHelper(vr, inputs)).ToArray();
    return new ValidationResult(validationResults);
  }
}
