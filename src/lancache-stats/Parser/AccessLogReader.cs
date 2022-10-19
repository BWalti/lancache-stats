namespace lancache_stats.Parser;

public class AccessLogReader : IDisposable
{
    private readonly FileStream _sourceStream;
    private readonly StreamReader _sourceStreamReader;
    private readonly FileSystemWatcher _fsw;
    private readonly AutoResetEvent _resetLatch;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public AccessLogReader(string path)
    {
        var fullPathToAccessLog = Path.Combine(path, "access.log");

        _sourceStream = File.Open(fullPathToAccessLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        _sourceStreamReader = new StreamReader(_sourceStream);

        _resetLatch = new AutoResetEvent(false);
        _cancellationTokenSource = new CancellationTokenSource();

        _fsw = new FileSystemWatcher(path);
        _fsw.Filter = "access.log";
        _fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
        _fsw.Changed += OnAccessLogChanged;
        _fsw.EnableRaisingEvents = true;
    }

    public async IAsyncEnumerable<string> ReadContent()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var line = await _sourceStreamReader.ReadLineAsync();
            if (line == null)
            {
                _resetLatch.WaitOne(TimeSpan.FromSeconds(2));
            }
            else
            {
                yield return line;
            }
        }
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private void OnAccessLogChanged(object sender, FileSystemEventArgs e)
    {
        _resetLatch.Set();
    }

    public void Dispose()
    {
        _sourceStream.Dispose();
        _sourceStreamReader.Dispose();
        _fsw.Dispose();
        _resetLatch.Dispose();
        _cancellationTokenSource.Dispose();
    }
}