using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal interface ILaboratoryManager
  {
    // TODO: User, authentication/availability, etc.
    void Setup();
    Laboratory Lab { get; }
  }
}
