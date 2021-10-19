using System;

namespace AresLib
{
  public class AresCommand
  {
    private string Name { get; }
    CommandParameter[] Arguments { get; }
    public Guid Id { get; }
  }
}
