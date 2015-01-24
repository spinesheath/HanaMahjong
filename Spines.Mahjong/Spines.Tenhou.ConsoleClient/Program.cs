using System;
using Spines.Tenhou.Client;
using Spines.Utility;

namespace Spines.Tenhou.ConsoleClient
{
  class Program
  {
    static void Main()
    {
      //var l = new ConsoleLogger();
      //using (var t = new TenhouTcpClient(l))
      //{
      //  t.Connect();
      //  var c = new TenhouReceiver(t, "ID0160262B-SG8PcR2h", "M", 0);
      //  c.LogOn();
      //  Console.ReadKey();
      //  //c.RequestMatch();
      //  Console.ReadKey();
      //}
      //Console.ReadKey();

      var l = new ConsoleLogger();
      var t = new DummyTenhouTcpClient(l);
      var i = new LogOnInformation("ID0160262B-SG8PcR2h", "M", 0);
      var s = new TenhouSender(t, i);
      var a = new TsumokiriAi(s);
      var c = new TenhouReceiver(t, s, null, a);
      s.LogOn();
      s.RequestMatch();
      Console.ReadKey();
    }
  }
}
