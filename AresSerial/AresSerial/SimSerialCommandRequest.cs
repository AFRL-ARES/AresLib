using System;
using System.Collections.Generic;
using System.Text;

namespace AresSerial
{
  public class SimSerialCommandRequest: SerialCommandRequest
  {
    public SimSerialCommandRequest(SerialCommandRequest actualRequest, string expectedResponse = null) : base(expectedResponse != null)
    {
      ActualRequest = actualRequest;
      ExpectedResponse = expectedResponse;
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
      {
        return string.Empty;
      }
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
    public static string InputSimIdentifier { get; } = "SIM_IN ";
    public static string OutputSimIdentifier { get; } = "SIM_OUT ";

    public SerialCommandRequest ActualRequest { get; }
    public string ExpectedResponse { get; }
  }
}
