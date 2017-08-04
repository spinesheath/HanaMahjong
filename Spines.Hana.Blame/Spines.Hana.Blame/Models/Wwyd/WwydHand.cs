// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Text;

namespace Spines.Hana.Blame.Models.Wwyd
{
  /// <summary>
  /// Represents a hand for the WWYD page.
  /// </summary>
  internal class WwydHand
  {
    /// <summary>
    /// Whether the hand is valid.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// A normalized string representation of the hand.
    /// </summary>
    public string NormalizedRepresentation { get; }

    /// <summary>
    /// Parses the hand.
    /// </summary>
    /// <param name="hand">The string representation of a hand.</param>
    /// <returns>An instance of WwydHand for the hand.</returns>
    public static WwydHand Parse(string hand)
    {
      const string suits = "mpsz";
      // 0 is the red 5, so it's sorted between 4 and 5.
      const string numbers = "1234056789";
      if (hand == null || hand.Length > 50)
      {
        return new WwydHand();
      }
      try
      {
        var counts = new int[suits.Length * numbers.Length];
        var offset = -1;
        foreach (var c in hand.Reverse())
        {
          var suit = suits.IndexOf(c);
          if (suit >= 0)
          {
            offset = 10 * suit;
            continue;
          }
          // last character must define a suit, and if we get here without an offset that was not the case.
          if (offset < 0)
          {
            return new WwydHand();
          }
          var number = numbers.IndexOf(c);
          if (number >= 0)
          {
            counts[offset + number] += 1;
            continue;
          }
          return new WwydHand();
        }

        // no red 5 of 8 or 9 for honors.
        if (counts[34] + counts[38] + counts[39] > 0)
        {
          return new WwydHand();
        }
        // at most 4 of each tile.
        if (counts.Any(c => c > 4))
        {
          return new WwydHand();
        }
        // at most 1 red 5 each suit.
        if (counts[4] > 1 || counts[14] > 1 || counts[24] > 1)
        {
          return new WwydHand();
        }
        // with a red 5, only 3 other 5 at most.
        if ((counts[4] > 0 || counts[14] > 0 || counts[24] > 0) && (counts[5] > 3 || counts[15] > 3 || counts[25] > 3))
        {
          return new WwydHand();
        }
        var sum = counts.Sum();
        if (sum != 14)
        {
          return new WwydHand();
        }

        var sb = new StringBuilder();
        for (var suit = 0; suit < suits.Length; ++suit)
        {
          var tilesInSuit = 0;
          for (var number = 0; number < numbers.Length; ++number)
          {
            var index = suit * numbers.Length + number;
            var count = counts[index];
            sb.Append(numbers[number], count);
            tilesInSuit += count;
          }
          if (tilesInSuit > 0)
          {
            sb.Append(suits[suit]);
          }
        }
        return new WwydHand(sb.ToString());
      }
      catch
      {
        return new WwydHand();
      }
    }

    private WwydHand()
    {
      IsValid = false;
    }

    private WwydHand(string normalizedRepresentation)
    {
      IsValid = true;
      NormalizedRepresentation = normalizedRepresentation;
    }
  }
}