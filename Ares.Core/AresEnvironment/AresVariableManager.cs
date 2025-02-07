using Ares.Core.Planning;
using Ares.Messaging;

namespace Ares.Core.AresEnvironment
{
  public class AresVariableManager
  {
    public AresVariableManager()
    {
    }

    public bool TryResolveVariable(IEnumerable<Parameter> parameters)
    {
      var parameterArray = parameters.ToArray();

      foreach(var parameter in parameterArray)
      {
        if(!parameter.EnvironmentBased)
          continue;

        var variableValue = AresEnvironment.GetEnvironmentVariable(parameter.VariableType);

        if(variableValue is null)
          return false;

        var val = new ParameterValue
        {
          UniqueId = Guid.NewGuid().ToString(),
          Value = variableValue
        };

        parameter.Value = val;
      }

      return true;
    }
  }
}
