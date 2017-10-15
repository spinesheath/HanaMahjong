// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spines.Hana.Blame.Models;

namespace Spines.Hana.Blame.Data
{
  public class RoomInitializer
  {
    public RoomInitializer(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task Seed()
    {
      var existing = await _context.Rooms.Select(r => r.Name).ToArrayAsync();
      var missing = Services.ReplayManager.Room.Rooms.Where(r => !existing.Contains(r.Name));

      var allToAdd = missing.Select(ToEntity).ToList();
      if (allToAdd.Any())
      {
        await _context.Rooms.AddRangeAsync(allToAdd);
        await _context.SaveChangesAsync();
      }
    }

    private readonly ApplicationDbContext _context;

    private static Room ToEntity(Services.ReplayManager.Room ruleSet)
    {
      return new Room {Name = ruleSet.Name};
    }
  }
}