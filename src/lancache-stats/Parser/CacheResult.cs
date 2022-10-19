namespace lancache_stats.Parser;

public record CacheResult(string System, DateTimeOffset DateTimeOffset, HitOrMiss HitOrMiss, uint Bytes) : Result;