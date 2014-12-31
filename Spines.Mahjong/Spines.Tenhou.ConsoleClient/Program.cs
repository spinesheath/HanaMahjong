using System;
using Spines.Tenhou.Client;

namespace Spines.Tenhou.ConsoleClient
{
  class Program
  {
    static void Main(string[] args)
    {
      var c = new TenhouConnection();
      c.Open();
      Console.ReadKey();
      c.Close();
    }
  }
}
