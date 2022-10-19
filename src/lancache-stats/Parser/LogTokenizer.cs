using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace lancache_stats.Parser;

public static class LogTokenizer
{
    public static TextParser<string> StringToken { get; } = QuotedString.CStyle;

    public static TextParser<string> IpAddressToken { get; } =
        from a in Character.Digit.Many()
        from apoint in Character.EqualTo('.')
        from b in Character.Digit.Many()
        from bpoint in Character.EqualTo('.')
        from c in Character.Digit.Many()
        from cpoint in Character.EqualTo('.')
        from d in Character.Digit.Many()
        select $"{new string(a)}.{new string(b)}.{new string(c)}.{new string(d)}";

    public static TextParser<HitOrMiss> HitOrMiss =
        from begin in Character.EqualTo('"')
        from value in Character.In('H', 'I', 'T', 'M', 'S', '-').Many()
        from end in Character.EqualTo('"')
        let enumValue = HitOrMissMatcher.Match(new string(value))
        select enumValue;

    public static TextParser<TextSpan> DateToken =
        Span.Regex("\\d+\\/\\w+\\/\\d{4}:\\d+:\\d+:\\d+ [+-]\\d+");

    public static Tokenizer<LogToken> Instance { get; } =
        new TokenizerBuilder<LogToken>()
            .Ignore(Span.WhiteSpace)
            .Match(Character.EqualTo('/'), LogToken.Slash)
            .Match(Character.EqualTo('-'), LogToken.Dash)
            .Match(Character.EqualTo('['), LogToken.LSquareBracket)
            .Match(Character.EqualTo(']'), LogToken.RSquareBracket)
            .Match(DateToken, LogToken.Date)
            .Match(IpAddressToken, LogToken.IpAddress)
            .Match(Numerics.Integer, LogToken.Integer)
            .Match(HitOrMiss, LogToken.HitOrMiss)
            .Match(StringToken, LogToken.String)
            .Match(Identifier.CStyle, LogToken.Identifier, requireDelimiters: true)
            .Build();
}