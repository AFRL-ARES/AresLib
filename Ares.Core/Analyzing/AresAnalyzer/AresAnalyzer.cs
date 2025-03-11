using Ares.Core.Analyzing;
using Ares.Messaging;
using Ares.Messaging.Analyzing;

namespace AresAnalyzer;

public class AresAnalyzer : AnalyzerBase<AnalysisInformation>
{
  readonly Uri _address;

  public AresAnalyzer(string name, Uri fullAddress) : base(name, new Version(1, 0), fullAddress.OriginalString)
  {
    _address = fullAddress;
  }

  protected override async Task<Analysis> AnalyzeMessage(ExperimentResult result, AnalysisInformation input, CancellationToken cancellationToken)
  {
    var client = ClientStore.AresAnalyzingClient;
    var response = await client.AnalyzerAsync(new AnalysisRequest { Placeholder = -1.0 });

    return new Analysis
    {
      Analyzer = new() { Name = Name, Type = GetType().Name, Version = Version.ToString() },
      Result = Convert.ToSingle(response.Value)
    };
  }

  public void Init()
  {
    ClientStore.CreateClient(_address);
  }

  public override bool InputSupported(string fullTypeName)
  {
    //Given the limited nature of educational ARES, we'll assume we support whatever input students provide
    return true;
  }
}
