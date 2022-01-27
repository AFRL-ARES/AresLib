namespace Ares.Device.Serial;

public class SimSerialCommandRequest<T> : SerialCommandRequestWithResponse<T> where T : SerialCommandResponse
{
  public SimSerialCommandRequest(SerialCommandRequestWithResponse<T> actualRequest, string expectedResponse = null)
  {
    ActualRequest = actualRequest;
    ExpectedResponse = expectedResponse;
  }

  public static string InputSimIdentifier { get; } = "SIM_IN ";
  public static string OutputSimIdentifier { get; } = "SIM_OUT ";

  public SerialCommandRequestWithResponse<T> ActualRequest { get; }
  public string ExpectedResponse { get; }

  public override T DeserializeResponse(string response)
  {
    return ActualRequest.DeserializeResponse(response);
  }

  public override string Serialize()
  {
    var simInput = InputSimulationIdentify(ActualRequest.Serialize());
    var simOutput = OutputSimulationIdentify(ExpectedResponse);
    return $"{simInput}{simOutput}";
  }

  public static string InputSimulationIdentify(string actualRequest)
  {
    var simInput = $"{InputSimIdentifier}{actualRequest}";
    return simInput;
  }

  public static string OutputSimulationIdentify(string expectedResponse = null)
  {
    if (expectedResponse == null)
      return string.Empty;

    var simOutput = $"{OutputSimIdentifier}{expectedResponse}";
    return simOutput;
  }

  public static string[] GetSimulatedIo(string simulationString)
  {
    var inputStart = simulationString.IndexOf(InputSimIdentifier) + InputSimIdentifier.Length;
    var outputStart = simulationString.IndexOf(OutputSimIdentifier);
    string input = null;
    string output = null;
    if (outputStart < 0)
    {
      input = simulationString.Substring(inputStart);
    }
    else
    {
      outputStart += OutputSimIdentifier.Length;
      output = simulationString.Substring(outputStart);
      var inputEnd = outputStart - OutputSimIdentifier.Length;
      var inputLength = inputEnd - inputStart;
      input = simulationString.Substring(inputStart, inputLength);
    }

    return new[] { input, output };
  }
}
