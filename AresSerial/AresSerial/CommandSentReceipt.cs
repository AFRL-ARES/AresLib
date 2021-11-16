using System;
using System.Collections.Generic;
using System.Text;

namespace AresSerial
{
  internal class CommandSentReceipt : SerialCommandRequest
  {
    public CommandSentReceipt(SerialCommandRequest request) : base(false)
    {
      Request = request;
    }

    public SerialCommandRequest Request { get; }
    public override string Serialize()
    {
      throw new NotImplementedException();
    }
  }
}
