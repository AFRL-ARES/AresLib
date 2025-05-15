using Ares.Messaging;
using System.Diagnostics.CodeAnalysis;

namespace Ares.Core.Analyzing;

public class RequestedAnalysisDataComparer : IEqualityComparer<RequestedAnalysisData>
{
  public bool Equals(RequestedAnalysisData? x, RequestedAnalysisData? y)
  {
    if(x is null || y is null)
      return false;

    return x.Key == y.Key && x.Type == y.Type;
  }

  public int GetHashCode([DisallowNull] RequestedAnalysisData obj)
  {
    return obj.GetHashCode();
  }
}
