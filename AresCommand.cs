using System;

namespace AresLib
{
  public class AresCommand
  {
    public CommandMetadata Metadata { get; }
    public CommandParameter[] Arguments { get; }
    public Guid Id { get; } = Guid.NewGuid();
  }
}
