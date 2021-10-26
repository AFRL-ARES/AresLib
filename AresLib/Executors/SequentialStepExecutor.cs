﻿using System;
using System.Threading.Tasks;

namespace AresLib.Executors
{
  internal class SequentialStepExecutor : StepExecutor
  {
    public SequentialStepExecutor(string name, Task[] commands) : base(name, commands)
    {
    }

    public override async Task Execute()
    {
      Console.WriteLine($"Executing Step: {Name}");
      foreach (var command in Commands)
      {
        command.Start();
        await command;
      }
    }

    
  }
}