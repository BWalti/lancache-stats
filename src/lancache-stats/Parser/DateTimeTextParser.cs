using System.Globalization;
using Superpower;
using Superpower.Parsers;

namespace lancache_stats.Parser;

public static class DateTimeTextParser
{
    static TextParser<int> IntDigits(int count) =>
        Character.Digit
            .Repeat(count)
            .Select(chars => int.Parse(new string(chars)));

    static TextParser<int> TwoDigits { get; } = IntDigits(2);
    static TextParser<int> FourDigits { get; } = IntDigits(4);

    static TextParser<char> Slash { get; } = Character.EqualTo('/');
    static TextParser<char> Colon { get; } = Character.EqualTo(':');
    static TextParser<char> TimeSeparator { get; } = Character.In('T', ' ', ':');

    static TextParser<TimeSpan> TimeZone { get; } =
        from pm in Character.In('+', '-')
        from hours in IntDigits(2)
        from minutes in IntDigits(2)
        let timeSpan = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes)
        select pm == '+' ? timeSpan : -timeSpan;

    private static TextParser<DateTime> Date { get; } =
        from day in TwoDigits
        from _ in Slash
        from month in Character.AnyChar.Repeat(3)
        from __ in Slash
        from year in FourDigits
        select System.DateTime.ParseExact($"{year} {new string(month)} {day}", "yyyy MMM dd", CultureInfo.CurrentCulture);

    static TextParser<TimeSpan> Time { get; } =
        from hour in TwoDigits
        from _ in Colon
        from minute in TwoDigits
        from second in Colon
            .IgnoreThen(TwoDigits)
            .OptionalOrDefault()
        select new TimeSpan(hour, minute, second);

    static TextParser<DateTimeOffset> DateTime { get; } =
        from date in Date
        from time in TimeSeparator
            .IgnoreThen(Time)
            .OptionalOrDefault()
        from timeZone in TimeSeparator
            .IgnoreThen(TimeZone)
            .OptionalOrDefault()
        select new DateTimeOffset(date + time, timeZone);

    static TextParser<DateTimeOffset> DateTimeOnly { get; } = DateTime.AtEnd();

    public static DateTimeOffset Parse(string input)
    {
        return DateTimeOnly.Parse(input);
    }
}