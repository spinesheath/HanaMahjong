// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Spines.Hana.Blame
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseIISIntegration()
        .UseStartup<Startup>()
        .Build();

      host.Run();
    }
  }
}