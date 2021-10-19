using System;

namespace AresLib
{
  class CommandParameterBuilder : IBuilder<CommandParameter>
  {
    public string Name { get; set; }
    public double Value { get; set; }
    public string Unit { get; set; }
    public double[] Constraints { get; set; }
    public CommandParameter Build()
    {
      return new CommandParameter
      {
        Name = Name,
        Value = Value,
        Unit = Unit,
        Constraints = Constraints
      };
    }
  }
}
