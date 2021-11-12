namespace AresSerial
{
  public interface IAresSerialPort
  {
    public void Open();

    public string ReadLine();

    public void WriteLine(string input);
    public string PortName { get; set; }
    public bool IsOpen { get; }
  }
}
