// See https://aka.ms/new-console-template for more information

using Humanizer;
using Humanizer.Bytes;
using lancache_stats.Parser;
using lancache_stats.Stats;
using Spectre.Console;

await Task.Delay(1000);

var currentDirectory = Directory.GetCurrentDirectory();
var reader = new AccessLogReader(currentDirectory);
var enumerator = reader.ReadContent().GetAsyncEnumerator();

var parser = new AccessLogParser();
var statsBuilder = new StatsBuilder();

var outerTable = new Table().Centered();
outerTable.AddColumn("");

var table = new Table().Centered();
table.Title("[bold]Total[/]");
table.AddColumn("[bold]System[/]");
table.AddColumn("Hits");
table.AddColumn("Misses");
table.AddColumn("Cache Reads");
table.AddColumn("Remote Reads");
outerTable.AddRow(table);


var rowCounter = 0;
var itemCountLookup = new Dictionary<string, int>();

await AnsiConsole
    .Live(outerTable)
    .StartAsync(async ctx =>
    {
        while (await enumerator.MoveNextAsync())
        {
            var line = enumerator.Current;
            var result = parser.Parse(line);
            statsBuilder.Apply(result);

            foreach (var stats in statsBuilder.SystemStats)
            {
                var valueStats = stats.Value.Stats;

                var hitCell = $"[lime]{valueStats.Hits.ToMetric()}[/]";
                var missCell = $"[red]{valueStats.Misses.ToMetric()}[/]";
                var cachedBytesCell = $"[lime]{ByteSize.FromBytes(valueStats.CachedBytes)}[/]";
                var remoteBytesCell = $"[red]{ByteSize.FromBytes(valueStats.RemoteBytes)}[/]";

                if (!itemCountLookup.TryGetValue(stats.Key, out var itemIndex))
                {
                    itemIndex = rowCounter++;
                    itemCountLookup.Add(stats.Key, itemIndex);
                    table.AddRow($"[bold]{stats.Key}[/]", hitCell, missCell, cachedBytesCell, remoteBytesCell);
                }
                else
                {
                    table.UpdateCell(itemIndex, 1, hitCell);
                    table.UpdateCell(itemIndex, 2, missCell);
                    table.UpdateCell(itemIndex, 3, cachedBytesCell);
                    table.UpdateCell(itemIndex, 4, remoteBytesCell);
                }
            }

            ctx.Refresh();
        }
    });

