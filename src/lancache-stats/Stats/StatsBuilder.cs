using lancache_stats.Parser;

namespace lancache_stats.Stats;

public class StatsBuilder
{
    public void Apply(Result result)
    {
        if (result is not CacheResult cr) return;

        if (!SystemStats.TryGetValue(cr.System, out var stats))
        {
            stats = new SystemStats(cr.System);
            SystemStats.Add(cr.System, stats);
        }
        stats.Apply(cr);

        if (!BuckedSystemStats.TryGetValue(cr.System, out var bucketStats))
        {
            bucketStats = new BuckedSystemStats(cr.System);
            BuckedSystemStats.Add(cr.System, bucketStats);
        }
        bucketStats.Apply(cr);
    }

    public Dictionary<string, BuckedSystemStats> BuckedSystemStats { get; } = new();
    public Dictionary<string, SystemStats> SystemStats { get; } = new();
}

public class SystemStats
{
    public SystemStats(string system)
    {
        System = system;
    }

    public string System { get; }
    public Stats Stats { get; } = new ();

    public void Apply(CacheResult cr)
    {
        switch (cr.HitOrMiss)
        {
            case HitOrMiss.Hit:
                Stats.Hits++;
                Stats.CachedBytes += cr.Bytes;
                break;

            case HitOrMiss.Miss:
                Stats.Misses++;
                Stats.RemoteBytes += cr.Bytes;
                break;

            default:
                Stats.Unknown++;
                break;
        }
    }
}

public class BuckedSystemStats
{
    public BuckedSystemStats(string crSystem)
    {
        System = crSystem;
        DailyBuckets = new Dictionary<DateTime, BucketStats>(3);
        HourlyBuckets = new Dictionary<DateTime, BucketStats>(24);
        MinuteBuckets = new Dictionary<DateTime, BucketStats>(60);
    }

    public string System { get; }

    public Dictionary<DateTime, BucketStats> MinuteBuckets { get; }

    public Dictionary<DateTime, BucketStats> HourlyBuckets { get; }

    public Dictionary<DateTime, BucketStats> DailyBuckets { get; }

    public void Apply(CacheResult cr)
    {
        var day = cr.DateTimeOffset.Date;
        if (!DailyBuckets.TryGetValue(day, out var bucketStats))
        {
            bucketStats = new BucketStats(day);
            DailyBuckets.Add(day, bucketStats);
        }

        bucketStats.Apply(cr);

        var localDateTime = cr.DateTimeOffset.LocalDateTime;

        var hour = new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day, localDateTime.Hour, 0, 0,
            localDateTime.Kind);
        if (!HourlyBuckets.TryGetValue(hour, out bucketStats))
        {
            bucketStats = new BucketStats(day);
            HourlyBuckets.Add(hour, bucketStats);
        }

        bucketStats.Apply(cr);


        var minute = new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day, localDateTime.Hour,
            localDateTime.Minute, 0,
            localDateTime.Kind);
        if (!MinuteBuckets.TryGetValue(minute, out bucketStats))
        {
            bucketStats = new BucketStats(day);
            MinuteBuckets.Add(minute, bucketStats);
        }

        bucketStats.Apply(cr);

        //CleanOldStuff();
    }

    private void CleanOldStuff()
    {
        var keys = DailyBuckets.Where(db => db.Key < DateTime.Today.AddDays(-2)).Select(db => db.Key).ToList();
        keys.ForEach(k => DailyBuckets.Remove(k));

        keys = HourlyBuckets.Where(db => db.Key < DateTime.Now.AddHours(-24)).Select(db => db.Key).ToList();
        keys.ForEach(k => HourlyBuckets.Remove(k));

        keys = MinuteBuckets.Where(db => db.Key < DateTime.Now.AddMinutes(-60)).Select(db => db.Key).ToList();
        keys.ForEach(k => MinuteBuckets.Remove(k));
    }
}

public class BucketStats
{
    public BucketStats(DateTime dateTime)
    {
        DateTime = dateTime;
        Stats = new Stats();
    }

    public DateTime DateTime { get; }
    public Stats Stats { get; }

    public void Apply(CacheResult cr)
    {
        switch (cr.HitOrMiss)
        {
            case HitOrMiss.Hit:
                Stats.Hits++;
                Stats.CachedBytes += cr.Bytes;
                break;

            case HitOrMiss.Miss:
                Stats.Misses++;
                Stats.RemoteBytes += cr.Bytes;
                break;

            default:
                Stats.Unknown++;
                break;
        }
    }
}

public class Stats
{
    public int Hits { get; set; }
    public int Misses { get; set; }
    public int Unknown { get; set; }
    public ulong CachedBytes { get; set; }
    public ulong RemoteBytes { get; set; }
}