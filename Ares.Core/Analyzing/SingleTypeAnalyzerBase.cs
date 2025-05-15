using System.Reactive.Linq;
using Ares.Messaging;
using Google.Protobuf;

namespace Ares.Core.Analyzing;

public abstract class SingleTypeAnalyzerBase<T>(string name, Version version) : AnalyzerBase(name, version) where T : IMessage, new()
{
  public override Task<Analysis> Analyze(ExperimentExecutionSummary summary, IEnumerable<AnalyzerInput> inputs, CancellationToken cancellationToken)
  {
    var inputsArr = inputs.ToArray();
    if(!inputsArr.Any())
      return Task.FromResult(GetDefaultResult());

    var unpackedInputs = UnpackInputs(inputs).ToArray();
    var failedToUnpack = unpackedInputs.Where(ui => ui.Value is null);
    if(failedToUnpack.Any())
      throw new InvalidOperationException(
        $"Failed to unpack the following analyzer inputs: ${string.Join(',', failedToUnpack.Select(ftu => ftu.Key))}");

    return AnalyzeInputs(summary, unpackedInputs.Select(ui => ui.Value!), cancellationToken);
  }

  private static IEnumerable<KeyValuePair<string, T?>> UnpackInputs(IEnumerable<AnalyzerInput> inputs)
  {
    return inputs.Select(
      i =>
      {
        var unpacked = i.Data.TryUnpack<T>(out var result);
        KeyValuePair<string, T?> pair = new(i.Key, result);
        return pair;
      });
  }

  protected abstract Task<Analysis> AnalyzeInputs(ExperimentExecutionSummary summary, IEnumerable<T> inputs, CancellationToken cancellationToken);
}
