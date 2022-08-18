namespace Ares.Core.Data;

public interface IDataProvider
{
  Task StoreData<TComponent>(string name, byte[] data);
  Task<byte[]> GetData<TComponent>(string name);
}