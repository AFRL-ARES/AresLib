using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal class Analysis
  {
    public Guid Id { get; }
    public Guid ExperimentId { get; } // To get byte[] data for analyzing
    public double Result { get; }
    public string Analyser { get; }
  }
}
