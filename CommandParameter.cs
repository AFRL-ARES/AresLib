using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace AresLib
{
  public class CommandParameter
  {
    public string Name { get; init; }
    public double Value { get; init; }
    public string Unit { get; init; }
    public double[] Constraints { get; init; }
    public Guid Id { get; init; } = Guid.NewGuid();
  }
}
