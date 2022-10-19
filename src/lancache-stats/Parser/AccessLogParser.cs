using Superpower;
using Superpower.Parsers;

namespace lancache_stats.Parser;

public class AccessLogParser
{

    private static TokenListParser<LogToken, Result> GeneralParser { get; } =
        from lb in Token.EqualTo(LogToken.LSquareBracket)
        from host in Token.EqualTo(LogToken.IpAddress)
        from rb in Token.EqualTo(LogToken.RSquareBracket)
        select new Result();

    private static TokenListParser<LogToken, Result> UpdateParser { get; } =
        from lb1 in Token.EqualTo(LogToken.LSquareBracket)
        from identifier in Token.EqualTo(LogToken.Identifier)
        from rb1 in Token.EqualTo(LogToken.RSquareBracket)
        from realSourceIp in Token.EqualTo(LogToken.IpAddress)
        from slash1 in Token.EqualTo(LogToken.Slash)
        from dash1 in Token.EqualTo(LogToken.Dash)
        from dash2 in Token.EqualTo(LogToken.Dash)
        from dash3 in Token.EqualTo(LogToken.Dash)
        from lb2 in Token.EqualTo(LogToken.LSquareBracket)
        from date in Token.EqualTo(LogToken.Date)
        from rb2 in Token.EqualTo(LogToken.RSquareBracket)
        from action in Token.EqualTo(LogToken.String)
        from statusCode in Token.EqualTo(LogToken.Integer)
        from bytesRead in Token.EqualTo(LogToken.Integer)
        from dashString1 in Token.EqualTo(LogToken.HitOrMiss)
        from clientString in Token.EqualTo(LogToken.String).Try().Or(Token.EqualTo(LogToken.HitOrMiss))
        from hitOrMiss in Token.EqualTo(LogToken.HitOrMiss)
        from dataSource in Token.EqualTo(LogToken.String)
        from dashString2 in Token.EqualTo(LogToken.HitOrMiss).Try().Or(Token.EqualTo(LogToken.String))
        select new CacheResult(identifier.ToStringValue(), DateTimeTextParser.Parse(date.Span.ToStringValue()), HitOrMissMatcher.Match(hitOrMiss.Span.ToStringValue()), uint.Parse(bytesRead.ToStringValue())) as Result;

    private static TokenListParser<LogToken, Result> Instance { get; } = GeneralParser.Try().Or(UpdateParser.Try());

    public Result Parse(string line)
    {
        var tokens = LogTokenizer.Instance.Tokenize(line);
        var result = Instance.Parse(tokens);

        return result;
    }
}