﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using Ares.Core;
using AresLib.Device;
using AresLib.Executors;

namespace AresLib.Composers
{
  internal class StepComposer : CommandComposer<StepTemplate, StepExecutor>
  {
    public StepComposer(StepTemplate template, ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> availableDeviceCommandInterpreters) : base(template, availableDeviceCommandInterpreters)
    {
    }

    public override StepExecutor Compose()
    {
      var executables =
        Template
          .CommandTemplates
          .Select
            (
             commandTemplate =>
             {
               var commandInterpreter =
                 AvailableDeviceCommandInterpreters
                   .First(interpreter => 
                            interpreter
                              .Device
                              .Name
                              .Equals(commandTemplate.Metadata.DeviceName));
               
               var executable = commandInterpreter.TemplateToDeviceCommand(commandTemplate);
               return executable;
             }
            )
          .ToArray();

      return Template.IsParallel
               ? new ParallelStepExecutor(Template.Name, executables)
               : new SequentialStepExecutor(Template.Name, executables);
    }


  }
}