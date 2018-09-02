using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Spines.Hana.Blame.Services.ReplayManager
{
  public class ReplayId
  {
    public ReplayId(string id)
    {
      _id = id;
      IsValid = !string.IsNullOrEmpty(id) && ReplayIdRegex.IsMatch(id);
      if (IsValid)
      {
        CreationTime = ParseCreationTime();
      }
    }

    public override string ToString()
    {
      return _id;
    }

    /// <summary>
    /// The time the replay was created, in UTC.
    /// </summary>
    public DateTime CreationTime { get; }

    public bool IsValid { get; }

    /// <summary>
    /// The first 10 characters of the id defined the hour the match was played, in japanese time.
    /// </summary>
    /// <returns>The time the replay was created, in UTC.</returns>
    private DateTime ParseCreationTime()
    {
      var timeString = ReplayIdRegex.Match(_id).Groups[1].Value;
      var dateTime = DateTime.ParseExact(timeString, "yyyyMMddHH", CultureInfo.InvariantCulture);
      var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
      return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
    }

    private static readonly Regex ReplayIdRegex = new Regex(@"\A(\d{10})gm-[\da-f]{4}-[\da-f]{4}-[\da-f]{8}\z");
    private readonly string _id;
  }
}