using System.IO.Ports;
using System.Reactive.Linq;
using System.Text;
using Ares.Device.Serial.Commands;
using Ares.Device.Serial.Simulation;

namespace Ares.Device.Serial.Tests;

internal class SerialPortTests
{
  [Test]
  [Timeout(5000)]
  public async Task AresSerialPort_Returns_Good_Response_From_Simple_Request()
  {
    const string stringToTest = "<-Oh Hello->";
    var port = new TestPort(new SerialPortConnectionInfo(0, Parity.Even, 0, StopBits.None));
    var response = await port.SendOutboundCommand(new SomeCommandWithResponse(stringToTest));
    Assert.That(await port.DataBufferState.FirstAsync(), Is.Empty);
    Assert.That(response, Is.Not.Null);
    StringAssert.AreEqualIgnoringCase(stringToTest, response.Response);
    var currentBuffer = await port.DataBufferState.FirstAsync();
    Assert.That(currentBuffer, Is.Empty);
  }

  [Test]
  [Timeout(5000)]
  public async Task AresSerialPort_Returns_Good_Response_From_Multiple_Data_Adds()
  {
    const string stringToTest = "<-Oh Hello->";
    var port = new TestPort2(new SerialPortConnectionInfo(0, Parity.Even, 0, StopBits.None));
    var response = await port.SendOutboundCommand(new SomeCommandWithResponse(stringToTest));
    Assert.That(await port.DataBufferState.FirstAsync(), Is.Empty);
    Assert.That(response, Is.Not.Null);
    StringAssert.AreEqualIgnoringCase(stringToTest, response.Response);
    var currentBuffer = await port.DataBufferState.FirstAsync();
    Assert.That(currentBuffer, Is.Empty);
  }

  [Test]
  [Timeout(5000)]
  public async Task AresSerialPort_Returns_Good_Response_From_Multiple_Data_And_Commands()
  {
    const string stringToTest = "<-Oh Hello->";
    const string stringToTest2 = "<-Noice->";
    const string stringToTest3 = "<-This Is A Test->";
    var port = new TestPort2(new SerialPortConnectionInfo(0, Parity.Even, 0, StopBits.None));
    var test1 = await port.SendOutboundCommand(new SomeCommandWithResponse(stringToTest));
    Assert.That(await port.DataBufferState.FirstAsync(), Is.Empty);
    var test2 = await port.SendOutboundCommand(new SomeCommandWithResponse(stringToTest2));
    Assert.That(await port.DataBufferState.FirstAsync(), Is.Empty);
    var test3 = await port.SendOutboundCommand(new SomeCommandWithResponse(stringToTest3));
    Assert.That(await port.DataBufferState.FirstAsync(), Is.Empty);
    Assert.Multiple(() => {
      Assert.That(test1, Is.Not.Null);
      Assert.That(test2, Is.Not.Null);
      Assert.That(test3, Is.Not.Null);
    });

    Assert.Multiple(() => {
      StringAssert.AreEqualIgnoringCase(stringToTest, test1.Response);
      StringAssert.AreEqualIgnoringCase(stringToTest2, test2.Response);
      StringAssert.AreEqualIgnoringCase(stringToTest3, test3.Response);
    });

    Assert.That(await port.DataBufferState.FirstAsync(), Is.Empty);
  }

  [Test]
  [Timeout(5000)]
  public async Task AresSerialPort_Returns_Good_Response_From_Multiple_Types_Of_Commands()
  {
    const string stringToTest = "<-Oh Hello->";
    const string stringToTest2 = "!-Noice-!";
    const string stringToTest3 = "<-This Is A Test->";
    const string stringToTest4 = "!-More Tests-!";
    var port = new TestPort2(new SerialPortConnectionInfo(0, Parity.Even, 0, StopBits.None));
    var test1 = await port.SendOutboundCommand(new SomeCommandWithResponse(stringToTest));
    Assert.That(await port.DataBufferState.FirstAsync(), Is.Empty);
    var test2 = await port.SendOutboundCommand(new SomeCommandWithResponse2(stringToTest2));
    Assert.That(await port.DataBufferState.FirstAsync(), Is.Empty);
    var test3 = await port.SendOutboundCommand(new SomeCommandWithResponse(stringToTest3));
    Assert.That(await port.DataBufferState.FirstAsync(), Is.Empty);
    var test4 = await port.SendOutboundCommand(new SomeCommandWithResponse2(stringToTest4));
    Assert.That(await port.DataBufferState.FirstAsync(), Is.Empty);
    Assert.Multiple(() => {
      Assert.That(test1, Is.Not.Null);
      Assert.That(test2, Is.Not.Null);
      Assert.That(test3, Is.Not.Null);
      Assert.That(test4, Is.Not.Null);
    });

    Assert.Multiple(() => {
      StringAssert.AreEqualIgnoringCase(stringToTest, test1.Response);
      StringAssert.AreEqualIgnoringCase(stringToTest2, test2.OtherResponse);
      StringAssert.AreEqualIgnoringCase(stringToTest3, test3.Response);
      StringAssert.AreEqualIgnoringCase(stringToTest4, test4.OtherResponse);
    });

    var currentBuffer = await port.DataBufferState.FirstAsync();
    Assert.That(currentBuffer, Is.Empty);
  }

  [Test]
  [Timeout(5000)]
  [Repeat(20)]
  public async Task AresSerialPort_Returns_Good_Response_From_Multiple_Types_Of_Commands_Asynchronously()
  {
    const string stringToTest1 = "<-Oh Hello->";
    const string stringToTest2 = "!-Noice-!";
    const string stringToTest3 = "<-This Is A Test->";
    const string stringToTest4 = "!-More Tests-!";
    var port = new TestPort(new SerialPortConnectionInfo(0, Parity.Even, 0, StopBits.None));
    var test1 = port.SendOutboundCommand(new SomeCommandWithResponse(stringToTest1));
    var test2 = port.SendOutboundCommand(new SomeCommandWithResponse2(stringToTest2));
    var test3 = port.SendOutboundCommand(new SomeCommandWithResponse(stringToTest3));
    var test4 = port.SendOutboundCommand(new SomeCommandWithResponse2(stringToTest4));
    await Task.WhenAll(test1, test2, test3, test4);
    Assert.Multiple(() => {
      Assert.That(test1.Result, Is.Not.Null);
      Assert.That(test2.Result, Is.Not.Null);
      Assert.That(test3.Result, Is.Not.Null);
      Assert.That(test4.Result, Is.Not.Null);
    });

    // since the TestPort does not guarantee the responses in order, it can only guarantee
    // that the proper parser is used for each of the commands
    // ex.: two parsers expecting a "<- ->" type string are added to the queue
    // the first result coming from the port will be parsed with the first available parser
    // so the result may not match.
    Assert.Multiple(() => {
      StringAssert.StartsWith("<-", test1.Result.Response);
      StringAssert.StartsWith("!-", test2.Result.OtherResponse);
      StringAssert.StartsWith("<-", test3.Result.Response);
      StringAssert.StartsWith("!-", test4.Result.OtherResponse);
    });

    var currentBuffer = await port.DataBufferState.FirstAsync();
    Assert.That(currentBuffer, Is.Empty);
  }
}
internal class SomeResponse : ISerialResponse
{
  public SomeResponse(string response)
  {
    Response = response;
  }

  public string Response { get; }
}
internal class SomeResponse2 : ISerialResponse
{
  public SomeResponse2(string otherResponse)
  {
    OtherResponse = otherResponse;
  }

  public string OtherResponse { get; }
}
internal class SomeCommandWithResponse : SerialCommandWithResponse<SomeResponse>
{
  public SomeCommandWithResponse(string message)
  {
    Message = message;
  }

  public string Message { get; }

  protected override byte[] Serialize()
    => Encoding.ASCII.GetBytes(Message);

  public override bool TryParse(IEnumerable<byte> buffer, out SomeResponse? response, out ArraySegment<byte>? dataToRemove)
  {
    var bufferArr = buffer.ToArray();
    try
    {
      var parsed = Encoding.ASCII.GetString(bufferArr.ToArray());
      var startIdx = parsed.IndexOf("<-", StringComparison.InvariantCultureIgnoreCase);
      var endIdx = startIdx >= 0 ? parsed.IndexOf("->", startIdx, StringComparison.InvariantCultureIgnoreCase) : -1;
      endIdx = endIdx > 0 ? endIdx + "->".Length : endIdx;
      if (startIdx < 0 || endIdx < 0 || string.IsNullOrEmpty(parsed))
      {
        response = null;
        dataToRemove = null;
        return false;
      }

      response = new SomeResponse(parsed[startIdx..endIdx]);
      dataToRemove = new ArraySegment<byte>(bufferArr.ToArray(), startIdx, endIdx - startIdx);
      return true;
    }
    catch (Exception)
    {
      response = null;
      dataToRemove = null;
      return false;
    }
  }
}
internal class SomeCommandWithResponse2 : SerialCommandWithResponse<SomeResponse2>
{
  public SomeCommandWithResponse2(string otherMessage)
  {
    OtherMessage = otherMessage;
  }

  public string OtherMessage { get; }

  protected override byte[] Serialize()
    => Encoding.ASCII.GetBytes(OtherMessage);

  public override bool TryParse(IEnumerable<byte> buffer, out SomeResponse2? response, out ArraySegment<byte>? dataToRemove)
  {
    var bufferArr = buffer.ToArray();
    try
    {
      var parsed = Encoding.ASCII.GetString(bufferArr.ToArray());
      var startIdx = parsed.IndexOf("!-", StringComparison.InvariantCultureIgnoreCase);
      var endIdx = startIdx >= 0 ? parsed.IndexOf("-!", startIdx, StringComparison.InvariantCultureIgnoreCase) : -1;
      endIdx = endIdx > 0 ? endIdx + "-!".Length : endIdx;
      if (startIdx < 0 || endIdx <= 0 || string.IsNullOrEmpty(parsed))
      {
        response = null;
        dataToRemove = null;
        return false;
      }

      response = new SomeResponse2(parsed[startIdx..endIdx]);
      dataToRemove = new ArraySegment<byte>(bufferArr.ToArray(), startIdx, endIdx - startIdx);
      return true;
    }
    catch (Exception)
    {
      response = null;
      dataToRemove = null;
      return false;
    }
  }
}
public class TestPort : AresSimPort
{

  public TestPort(SerialPortConnectionInfo connectionInfo) : base(connectionInfo)
  {
  }

  public override void Disconnect()
  {
  }

  public override void SendOutboundMessage(SerialCommand command)
  {
    var random = new Random();
    Task.Delay(random.Next(100, 300)).ContinueWith(_ => {
      AddDataReceived(command.SerializedData);
    });
  }
}
public class TestPort2 : AresSimPort
{

  public TestPort2(SerialPortConnectionInfo connectionInfo) : base(connectionInfo)
  {
  }

  public override void Disconnect()
  {
  }

  public override void SendOutboundMessage(SerialCommand command)
  {
    var slice1 = command.SerializedData[..1];
    var slice2 = command.SerializedData[1..2];
    var slice3 = command.SerializedData[2..];
    Task.Run(async () => {
      AddDataReceived(slice1);
      await Task.Delay(100);
      AddDataReceived(slice2);
      await Task.Delay(200);
      AddDataReceived(slice3);
    });
  }
}
