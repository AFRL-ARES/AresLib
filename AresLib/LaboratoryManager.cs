using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresLib.Compilers;

namespace AresLib
{
  internal class LaboratoryManager : ILaboratoryManager
  {


    public void Setup()
    {
      var commandNamesToInterpreters =
        Lab.
          AvailableDeviceCommandCompilerFactories.
          SelectMany(interpreter => 
                       interpreter
                         .CommandsToMetadatas()
                         .Select(commandMetadata => 
                                   new KeyValuePair<string,IDeviceCommandInterpreter<AresDevice>>(StaticStuff.QualifyCommandName(interpreter, commandMetadata), interpreter)))
          .ToArray();

      var lookup = new Dictionary<string, IDeviceCommandInterpreter<AresDevice>>(commandNamesToInterpreters);

    }

    public Laboratory Lab { get; }
  }
}
