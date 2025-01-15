using System.Globalization;

namespace NewsletterIntegrationTests.Extension;

public static class DateTimeExtensions
{
    /// <summary>
    /// Returns a <see cref="DateTime"/> representing the next half-hour mark after the specified date and time.
    /// Usage: DateTime.Now.GetNextHalfHour();
    /// </summary>
    /// <param name="date">The base <see cref="DateTime"/> instance. Typically DateTime.Now.</param>
    /// <returns>
    /// A <see cref="DateTime"/> object set to the next half-hour mark relative to the provided <paramref name="date"/>.
    /// </returns>
    public static DateTime GetNextHalfHour(this DateTime date)
    {
        var minutes = date.Minute;

        var nextHalfHour = new DateTime(
            date.Year,
            date.Month,
            date.Day,
            date.Hour < 30 ? date.Hour : date.Hour - 1,
            minutes < 30 ? 30 : 0,
            0,
            0,
            date.Kind
        ).AddMinutes(minutes >= 30 ? 60 : 0);

        return nextHalfHour;
    }

    /// <summary>
    /// Returns a DateTime representing tomorrow at the specified time.
    /// Usage: DateTime.Now.TomorrowAt("14:30");
    /// </summary>
    /// <param name="date">The base DateTime instance. Typically DateTime.Now.</param>
    /// <param name="time">The time in "HH:mm" format (24-hour).</param>
    /// <returns>A DateTime object set to tomorrow's date at the specified time.</returns>
    /// <exception cref="ArgumentException">Thrown when the time format is invalid.</exception>
    public static DateTime TomorrowAt(this DateTime date, string time)
    {
        if (string.IsNullOrWhiteSpace(time))
            throw new ArgumentException("Time string cannot be null or empty.", nameof(time));

        time = time.Replace("\"", string.Empty);

        if (!TimeSpan.TryParseExact(time, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan parsedTime))
            throw new ArgumentException("Invalid time format. Expected format is 'HH:mm'.", nameof(time));

        var tomorrow = date.Date.AddDays(1);

        return new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, parsedTime.Hours, parsedTime.Minutes, 0);
    }
}
