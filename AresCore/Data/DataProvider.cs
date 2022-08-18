namespace Ares.Core.Data;

class DataProvider : IDataProvider
{
  public Task StoreData<TComponent>(string name, byte[] data)
  {
    var dirName = typeof(TComponent).Name;
    if (!Directory.Exists(dirName))
      Directory.CreateDirectory(dirName);

    var path = Path.Combine(dirName, name);
    return File.WriteAllBytesAsync(path, data);
  }

  public Task<byte[]> GetData<TComponent>(string name)
  {
    var dirName = typeof(TComponent).Name;
    var path = Path.Combine(dirName, name);
    return File.ReadAllBytesAsync(path);
  }
}
