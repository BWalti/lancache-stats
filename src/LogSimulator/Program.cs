using Serilog;

var outputPath = "D:\\git\\experiments\\lancache-stats\\src\\lancache-stats\\bin\\Debug\\net6.0";
var fullOutputPath = Path.Combine(outputPath, "access.log");
File.Delete(fullOutputPath);

using var log = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "{Message}{NewLine}")
    .WriteTo.File(fullOutputPath, outputTemplate: "{Message}{NewLine}", shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1))
    .CreateLogger();

await using var sourceStream = File.OpenRead("source_access.log");
using var sourceStreamReader = new StreamReader(sourceStream);

//await using var targetStream = File.Open(fullOutputPath, new FileStreamOptions
//{
//    Mode = FileMode.CreateNew,
//    Access = FileAccess.Write,
//    Share = FileShare.Read,
//    Options = FileOptions.Asynchronous | FileOptions.SequentialScan
//});
//await using var targetStreamWriter = new StreamWriter(targetStream, Encoding.UTF8);
//targetStreamWriter.AutoFlush = true;

while (await sourceStreamReader.ReadLineAsync() is { } line)
{
    log.Information(line);
    await Task.Delay(TimeSpan.FromMilliseconds(100));
}