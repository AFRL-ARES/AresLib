﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal interface IExecutableCompiler
  {
    Task GenerateExecutable();
    DeviceTranslatorRepoBridge DeviceTranslatorRepoBridge { get; init; }
  }
}
