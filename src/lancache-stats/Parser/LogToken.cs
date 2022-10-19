using Superpower.Display;

namespace lancache_stats.Parser;

public enum LogToken
{
    [Token(Example = "/")]
    Slash,

    [Token(Example = "-")]
    Dash,

    [Token(Example = "[")]
    LSquareBracket,

    Identifier,

    [Token(Example = "]")]
    RSquareBracket,

    String,

    Integer,

    IpAddress,

    Date,

    HitOrMiss
}