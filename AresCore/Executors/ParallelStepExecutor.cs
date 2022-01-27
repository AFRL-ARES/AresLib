namespace Ares.Core.Executors;

internal class ParallelStepExecutor : StepExecutor
{
  public ParallelStepExecutor(string name, Task[] commands) : base(name, commands)
  {
  }

  public override async Task Execute()
  {
    foreach (var command in Commands)
      command.Start();

    await Task.WhenAll(Commands);
  }
}
