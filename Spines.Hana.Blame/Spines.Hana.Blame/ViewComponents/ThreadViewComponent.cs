// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Spines.Hana.Blame.Data;
using Spines.Hana.Blame.Models.ThreadViewModels;

namespace Spines.Hana.Blame.ViewComponents
{
  public class ThreadViewComponent : ViewComponent
  {
    private readonly ApplicationDbContext _context;

    public ThreadViewComponent(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(int howMany = 3)
    {
      return View(await LoadThread());
    }

    private async Task<ThreadViewModel> LoadThread()
    {
      await Task.Delay(10);
      var r = new Random(DateTime.Now.Millisecond);
      var count = r.Next(5, 10);
      var values = Enumerable.Range(0, count).Select(x => r.Next());
      return new ThreadViewModel(values.Select(c => c.ToString()));
    }
  }
}