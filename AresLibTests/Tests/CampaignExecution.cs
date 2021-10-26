using System;
using AresLibTests.DummyModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AresLibTests.Tests
{
  [TestClass]
  public class CampaignExecution
  {

    [TestMethod]
    public void Run()
    {
      Console.WriteLine("Yay, a test");
      var testLabManager = new TestLaboratoryManager();

    }
  }
}
