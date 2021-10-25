using System;
using System.Linq;
using System.Threading.Tasks;

namespace AresLib
{
  internal class ExecutableStep : IBaseExecutable
  {
    public Func<Task>[] Commands { get; init; }
    public bool IsParallel { get; init; }
    public string Name { get; init; }

    public async Task Execute()
    {
      Console.WriteLine($"Executing Step: {Name}");
      if (IsParallel)
        await Task.WhenAll(Commands.Select(func => func()));
      else
      {
        foreach (var command in Commands)
        {
          await command();
        }
      }
    }
  }
}
