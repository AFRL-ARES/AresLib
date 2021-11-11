namespace AresSerial
{
  public enum ConnectionResult
  {
    Unattempted = 1,
    FailedConnection = 2,
    Connected = 3,
    Listening = 4,
    ListenerPaused = 5,
    ValidResponse = 6,
    InvalidResponse = 7,
    ManuallyClosed = 8,
    AbrubtlyDisconnected = 9,
  }
}
