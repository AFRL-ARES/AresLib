using Ares.Messaging;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Ares.Core.Analyzing;

public abstract class AnalyzerBase<T> : IAnalyzer where T : IMessage, new()
{
  private readonly ISubject<AnalyzerState> _analyzerStateSubject = new BehaviorSubject<AnalyzerState>(AnalyzerState.Disconnected);

  public AnalyzerBase(string name, int port, Version version, string address = "https://localhost")
  {
    Name = name;
    Version = version;
    Port = port;
    Address = address;
    AnalyzerStateObservable = _analyzerStateSubject.AsObservable();
  }

  public string Name { get; }
  public Version Version { get; }
  public int Port { get; }
  public string Address { get; }
  public IObservable<AnalyzerState> AnalyzerStateObservable { get; }
  public AnalyzerState AnalyzerState { get; protected set; }

  public virtual bool InputSupported(string fullTypeName)
    => typeof(T).FullName == fullTypeName;

  public Task<Analysis> Analyze(ExperimentResult results, Any? input, CancellationToken cancellationToken)
  {
    if(input is null)
      return Task.FromResult(GetDefaultResult());

    var unpackedMessage = UnpackMessage(input);
    if(unpackedMessage is null)
      return Task.FromResult(GetDefaultResult());

    return AnalyzeMessage(results, unpackedMessage, cancellationToken);
  }

  private Analysis GetDefaultResult()
  {
    return new Analysis
    {
      UniqueId = Guid.NewGuid().ToString(),
      Analyzer = new AnalyzerInfo { Name = Name, UniqueId = Guid.NewGuid().ToString(), Version = Version.ToString() },
      Result = 0
    };
  }

  private T? UnpackMessage(Any input)
  {
    try
    {
      var unpackedMessage = input.Unpack<T>();
      return unpackedMessage;
    }
    catch(InvalidProtocolBufferException e)
    {
      return default;
    }
  }

  protected abstract Task<Analysis> AnalyzeMessage(ExperimentResult result, T input, CancellationToken cancellationToken);
}
