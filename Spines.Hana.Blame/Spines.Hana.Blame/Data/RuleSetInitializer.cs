// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spines.Hana.Blame.Models;

namespace Spines.Hana.Blame.Data
{
  public class RuleSetInitializer
  {
    public RuleSetInitializer(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task Seed()
    {
      var existing = await _context.RuleSets.Select(r => r.Name).ToArrayAsync();
      var missing = Services.ReplayManager.RuleSet.RuleSets.Where(r => !existing.Contains(r.Name));

      var allToAdd = missing.Select(ToEntity).ToList();
      if (allToAdd.Any())
      { 
        await _context.RuleSets.AddRangeAsync(allToAdd);
        await _context.SaveChangesAsync();
      }
    }

    private readonly ApplicationDbContext _context;

    private static RuleSet ToEntity(Services.ReplayManager.RuleSet ruleSet)
    {
      return new RuleSet
      {
        Name = ruleSet.Name,
        Aka = ruleSet.Aka,
        ExtraSecondsPerGame = ruleSet.ExtraSecondsPerGame,
        Kuitan = ruleSet.Kuitan,
        PlayerCount = ruleSet.PlayerCount,
        Rounds = ruleSet.Rounds,
        SecondsPerAction = ruleSet.SecondsPerAction
      };
    }
  }
}