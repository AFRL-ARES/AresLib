using System;

namespace AresLib
{
  public class AresCommand
  {
    public string Name { get; }
    public string Description { get; }
    CommandParameter[] Arguments { get; }
    public Guid Id { get; } = Guid.NewGuid();
  }
}
