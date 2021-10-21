using System;

namespace AresLib.AnalysisSupport
{
  internal class Analysis
  {
    public Guid Id { get; }
    public Guid ExperimentId { get; } // To get byte[] data for analyzing
    public double Result { get; }
    public string Analyser { get; }
  }
}
