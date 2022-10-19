namespace lancache_stats.Parser;

public class CacheResult : Result
{
    public string System { get; }
    public DateTimeOffset DateTimeOffset { get; }
    public HitOrMiss HitOrMiss { get; }
    public uint Bytes { get; }

    public CacheResult(string system, DateTimeOffset dateTimeOffset, HitOrMiss hitOrMiss, uint bytes)
    {
        System = system;
        DateTimeOffset = dateTimeOffset;
        HitOrMiss = hitOrMiss;
        Bytes = bytes;
    }
}