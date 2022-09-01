using System;

namespace Ares.Device.Serial.Simulation;

public class SimRequestWithResponse<T> : SerialCommandRequestWithResponse<T> where T : SerialCommandResponse
{
  public SimRequestWithResponse(SerialCommandRequestWithResponse<T> actualRequest, string expectedResponse)
  {
    ActualRequest = actualRequest;
    ExpectedResponse = expectedResponse;
  }

  public static string InputSimIdentifier { get; } = "SIM_IN ";
  public static string OutputSimIdentifier { get; } = "SIM_OUT ";

  public SerialCommandRequestWithResponse<T> ActualRequest { get; }
  public string ExpectedResponse { get; }

  public override T DeserializeResponse(string response)
    => ActualRequest.DeserializeResponse(response);

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

  public static string OutputSimulationIdentify(string expectedResponse)
  {
    var simOutput = $"{OutputSimIdentifier}{expectedResponse}";
    return simOutput;
  }

  public static string?[] GetSimulatedIo(string simulationString)
  {
    var inputStart = simulationString.IndexOf(InputSimIdentifier, StringComparison.InvariantCultureIgnoreCase) + InputSimIdentifier.Length;
    var outputStart = simulationString.IndexOf(OutputSimIdentifier, StringComparison.InvariantCultureIgnoreCase);

    outputStart += OutputSimIdentifier.Length;
    var output = simulationString.Substring(outputStart);
    var inputEnd = outputStart - OutputSimIdentifier.Length;
    var inputLength = inputEnd - inputStart;
    var input = simulationString.Substring(inputStart, inputLength);

    return new[] { input, output };
  }
}
