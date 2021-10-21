using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresLib.AresCoreDevice;

namespace AresLib
{
  public abstract class TestingDerpStuffDeleteMe : Enum
  {
    public TestingDerpStuffDeleteMe()
    {}
    public abstract void Boop();
  }

  public class Boop : TestingDerpStuffDeleteMe
  {
    public Boop()
    {
      
    }

    public override void Boop()
    {
      throw new NotImplementedException();
    }
  }
}
