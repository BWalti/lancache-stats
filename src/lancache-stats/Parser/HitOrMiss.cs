using Superpower.Model;

namespace lancache_stats.Parser;

public enum HitOrMiss
{
    HIT,
    MISS,
    Unknown
}

public static class HitOrMissMatcher
{
    public static HitOrMiss Match(string stringValue)
    {
        return stringValue switch
        {
            "\"MISS\"" => HitOrMiss.MISS,
            "\"HIT\"" => HitOrMiss.HIT,
            _ => HitOrMiss.Unknown
        };
    }
}