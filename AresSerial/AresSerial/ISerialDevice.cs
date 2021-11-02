using System;
using System.Collections.Generic;
using System.Text;

namespace AresSerial
{
  public interface ISerialDevice<TConnection> where TConnection : ISerialConnection
  {

  }
}
