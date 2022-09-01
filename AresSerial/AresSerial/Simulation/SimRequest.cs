using System;

namespace Ares.Device.Serial.Simulation;

public class SimRequest : SerialCommandRequest
{

  protected static string InputSimIdentifier { get; } = "SIM_IN ";

  public SerialCommandRequest ActualRequest { get; }

  public override string Serialize()
  {
    var simInput = InputSimulationIdentify(ActualRequest.Serialize());
    return $"{simInput}";
  }

  protected static string InputSimulationIdentify(string actualRequest)
  {
    var simInput = $"{InputSimIdentifier}{actualRequest}";
    return simInput;
  }

  public static string?[] GetSimulatedIo(string simulationString)
  {
    var inputStart = simulationString.IndexOf(InputSimIdentifier, StringComparison.InvariantCultureIgnoreCase) + InputSimIdentifier.Length;
    string input;

    input = simulationString.Substring(inputStart);

    return new[] { input };
  }
}
