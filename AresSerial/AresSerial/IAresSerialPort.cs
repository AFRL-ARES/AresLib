namespace AresSerial
{
  public interface IAresSerialPort
  {
    void Open();

    string ReadLine();

    void WriteLine(string input);
    string PortName { get; set; }
    bool IsOpen { get; }
  }
}
