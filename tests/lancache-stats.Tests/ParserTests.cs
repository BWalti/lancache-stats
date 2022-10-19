using lancache_stats.Parser;
using lancache_stats.Stats;
using Xunit;

namespace lancache_stats.Tests;

public class ParserTests
{
    private const string InitialLine =
        "[192.168.10.138] 172.18.0.1 / 192.168.10.149 - - [18/Oct/2022:22:07:10 +0200] \"GET / HTTP/1.0\" 508 0 \"-\" \"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36 Edg/106.0.1370.47\" \"-\" \"192.168.10.138\" \"bytes=0-1048575\"";

    private const string SteamLine =
        "[steam] 192.168.10.149 / - - - [18/Oct/2022:22:21:18 +0200] \"GET /depot/620980/chunk/1c034cb196413ba029ba3cef9ab5ac2a4f41b304 HTTP/1.1\" 200 2272 \"-\" \"Valve/Steam HTTP Client 1.0\" \"MISS\" \"cache9-fra2.steamcontent.com\" \"-\"";

    private const string WsusLine =
        "[wsus] 192.168.10.149 / - - - [18/Oct/2022:22:46:51 +0200] \"GET /msdownload/update/v3/static/trustedr/en/pinrulesstl.cab?9eb96bbda8815d59 HTTP/1.1\" 304 0 \"-\" \"Microsoft-CryptoAPI/10.0\" \"-\" \"ctldl.windowsupdate.com\" \"-\"";

    private const string BattleNetLine =
        "[blizzard] 192.168.10.149 / - - - [18/Oct/2022:22:47:17 +0200] \"GET /tpr/catalogs/config/ea/5d/ea5d741bff7492ba4261d4f2172b7049 HTTP/1.1\" 200 275 \"-\" \"Battle.net/2.16.0.13763 (retail)\" \"MISS\" \"level3.blizzard.com\" \"-\"";

    private const string UplayLine =
        "[uplay] 192.168.10.149 / - - - [19/Oct/2022:15:49:33 +0200] \"GET /uplaypc/downloads/3454/slices_v3/c/2C07CE48391B66154C8635BE99DCEBF019FD9A44?_tkn_=exp=1666190961~acl=%2fuplaypc%2fdownloads%2f3454%2fslices_v3%2fc%2f2C07CE48391B66154C8635BE99DCEBF019FD9A44~data=c98b00e7-2004-4126-96c1-14a607086047~hmac=8984a5c86e518a09c1b95792883bdda911c9e5f3aeeec60a45cdae4b67a19bfa HTTP/1.1\" 200 2196142 \"-\" \"Massgate\" \"MISS\" \"uplaypc-s-ubisoft.cdn.ubi.com\" \"-\"";

    private readonly AccessLogParser _testee;

    public ParserTests()
    {
        _testee = new AccessLogParser();
    }

    [Fact]
    public void TestInitial()
    {
        _testee.Parse(InitialLine);
    }

    [Fact]
    public void TestSteam()
    {
        var result = _testee.Parse(SteamLine);
        if (result is CacheResult cr) Assert.Equal(HitOrMiss.Miss, cr.HitOrMiss);
    }

    [Fact]
    public void TestBattlenet()
    {
        var result = _testee.Parse(BattleNetLine);
        if (result is CacheResult cr) Assert.Equal(HitOrMiss.Miss, cr.HitOrMiss);
    }

    [Fact]
    public void TestUplay()
    {
        var result = _testee.Parse(UplayLine);
        if (result is CacheResult cr) Assert.Equal(HitOrMiss.Miss, cr.HitOrMiss);
    }

    [Fact]
    public void TestWsus()
    {
        _testee.Parse(WsusLine);
    }

    [Fact]
    public async Task CanHandleWholeFile()
    {
        await using var stream = File.OpenRead("source_access.log");
        using var reader = new StreamReader(stream);

        var statsBuilder = new StatsBuilder();

        while (await reader.ReadLineAsync() is { } line)
        {
            var result = _testee.Parse(line);
            statsBuilder.Apply(result);
        }
    }
}
