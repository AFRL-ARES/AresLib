using Ares.Messaging;

namespace Ares.Core.AresEnvironment
{
  public static class AresEnvironment
  {
    public static void SetEnvironmentVariable(VariableType variableType, string value)
    {
      AresEnvironmentVariables[variableType] = value;
    }

    public static string? GetEnvironmentVariable(VariableType variableType)
    {
      return AresEnvironmentVariables.GetValueOrDefault(variableType);
    }

    private static Dictionary<VariableType, string> AresEnvironmentVariables { get; } = new();
  }
}