namespace lancache_stats.Parser;

public enum HitOrMiss
{
    Hit,
    Miss,
    Unknown
}

public static class HitOrMissMatcher
{
    public static HitOrMiss Match(string stringValue)
    {
        return stringValue switch
        {
            "\"MISS\"" => HitOrMiss.Miss,
            "\"HIT\"" => HitOrMiss.Hit,
            _ => HitOrMiss.Unknown
        };
    }
}