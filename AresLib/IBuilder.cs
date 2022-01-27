namespace Ares.AutomationBuilding;

public interface IBuilder<out T>
{
  T Build();
}
