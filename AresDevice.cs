using System;
using System.Collections.ObjectModel;
using Ares.Core;

namespace AresLib
{
  public abstract class AresDevice : IAresDevice
  {
    public string Name { get; init; }
  }
}
