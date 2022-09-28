using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;

namespace Ares.Core.Execution.StartConditions;

public class StartConditionRegistry : IStartConditionRegistry
{
  private readonly ISubject<bool> _canStartSubject = new BehaviorSubject<bool>(true);
  private readonly IList<IDisposable> _conditionObservers = new List<IDisposable>();
  private readonly IDictionary<IStartCondition, bool> _startConditions = new ConcurrentDictionary<IStartCondition, bool>();

  public StartConditionRegistry(IEnumerable<IStartCondition> startConditions)
  {
    CanStart = _canStartSubject.AsObservable();
    var test = startConditions.Select(condition => condition.CanStartObservable.Subscribe(b => {
      _startConditions[condition] = b;
      _canStartSubject.OnNext(_startConditions.Values.All(x => x));
    }));

    _conditionObservers.Add(test);
  }

  public IObservable<bool> CanStart { get; }
}
