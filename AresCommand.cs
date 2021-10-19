using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  public class AresCommand
  {
    private string Name { get; }
    CommandParameter[] Arguments { get; }
    public Guid Id { get; }
  }
}
