using System.Collections.Immutable;
using System.IO.Compression;
using System.Runtime.CompilerServices;

namespace CryptoLooser.Core.Parsing;

public class BinanceMarketDataFileParser(IEnumerable<string> filePaths)
{
    private readonly ImmutableArray<string> _filePaths = filePaths.ToImmutableArray();

    public async IAsyncEnumerable<MarketDataRow> GetMarketData(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var rowBuilder = new MarketDataRowBuilder();

        foreach (var filePath in _filePaths)
        {
            var zipBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            using var memoryStream = new MemoryStream(zipBytes);
            using var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

            foreach (var zipEntry in zipArchive.Entries)
            {
                await using var zipStream = zipEntry.Open();
                using var zipReader = new StreamReader(zipStream);

                string? line;
                do
                {
                    line = await zipReader.ReadLineAsync(cancellationToken);

                    if (!string.IsNullOrEmpty(line))
                    {
                        yield return ParseLine(line, rowBuilder);
                    }
                } while (line is not null);
            }
        }
    }

    private static MarketDataRow ParseLine(string line, MarketDataRowBuilder rowBuilder)
    {
        var span = line.AsSpan();

        var column = 0;

        while (!span.IsEmpty)
        {
            var separatorIndex = span.IndexOf(',');

            var partSpan = separatorIndex >= 0
                ? span.Slice(0, separatorIndex)
                : span;

            span = separatorIndex >= 0
                ? span.Slice(separatorIndex + 1)
                : ReadOnlySpan<char>.Empty;

            rowBuilder.FeedData(column, partSpan);

            column++;
        }

        return rowBuilder.Build();
    }

    public static BinanceMarketDataFileParser Create(string marketSymbol, string searchLocation)
    {
        var foundFiles = Directory.EnumerateFiles(
            path: searchLocation,
            searchPattern: $"{marketSymbol}-*.zip",
            searchOption: SearchOption.TopDirectoryOnly);

        var filePathsOrdered = foundFiles
            .Select(filePath =>
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                return (FilePath: filePath, Date: ExtractDateFromFileName(fileName));
            })
            .OrderBy(t => t.Date)
            .Select(t => t.FilePath);

        return new BinanceMarketDataFileParser(filePathsOrdered);
    }

    private static DateOnly ExtractDateFromFileName(string fileNameWithoutExtension)
    {
        var dateSpan = fileNameWithoutExtension.AsSpan(fileNameWithoutExtension.Length - 7);
        return DateOnly.ParseExact(dateSpan, "yyyy-MM");
    }
}
