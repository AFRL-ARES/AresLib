namespace AresLib.Builders
{
  public interface IBuilder<out T>
  {
    T Build();
  }
}
