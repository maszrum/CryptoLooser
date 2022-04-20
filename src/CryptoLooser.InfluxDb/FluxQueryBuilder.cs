using System.Text;

namespace CryptoLooser.InfluxDb;

internal class FluxQueryBuilder
{
    private string? _bucket;
    private DateTime? _start;
    private DateTime? _stop;
    private string? _pivotRowKey;
    private string? _pivotColumnKey;
    private string? _pivotValueColumn;
    private readonly List<KeyValuePair<string, string>> _filters = new();

    public FluxQueryBuilder FromBucket(string bucket)
    {
        _bucket = bucket;
        return this;
    }

    public FluxQueryBuilder InRange(DateTime start, DateTime stop)
    {
        _start = start;
        _stop = stop;
        return this;
    }

    public FluxQueryBuilder Filter(string tagName, string value)
    {
        _filters.Add(new KeyValuePair<string, string>(tagName, value));
        return this;
    }

    public FluxQueryBuilder WithDefaultPivot()
    {
        _pivotRowKey = "_time";
        _pivotColumnKey = "_field";
        _pivotValueColumn = "_value";
        return this;
    }

    public string Build()
    {
        if (string.IsNullOrWhiteSpace(_bucket))
        {
            throw new InvalidOperationException(
                $"Bucket is empty. Call {nameof(FromBucket)} method.");
        }

        if (!_start.HasValue || !_stop.HasValue)
        {
            throw new InvalidOperationException(
                $"Start and end date are empty. Call {nameof(InRange)} method.");
        }

        var stringBuilder = new StringBuilder()
            .Append("from(bucket: \"")
            .Append(_bucket)
            .AppendLine("\")")
            .Append(" |> range(start: ")
            .Append(_start.Value.ToJsonFormat())
            .Append(", stop: ")
            .Append(_stop.Value.ToJsonFormat())
            .AppendLine(")");

        if (_filters.Count > 0)
        {
            var filterCriteria = _filters.Select(kvp => $"r[\"{kvp.Key}\"] == \"{kvp.Value}\"");
            var criteriaJoined = string.Join(" and ", filterCriteria);

            stringBuilder
                .Append(" |> filter(fn: (r) => ")
                .Append(criteriaJoined)
                .AppendLine(")");
        }

        if (_pivotColumnKey is not null &&
            _pivotRowKey is not null &&
            _pivotValueColumn is not null)
        {
            stringBuilder
                .Append(" |> pivot(rowKey: [\"")
                .Append(_pivotRowKey)
                .Append("\"], columnKey: [\"")
                .Append(_pivotColumnKey)
                .Append("\"], valueColumn: \"")
                .Append(_pivotValueColumn)
                .AppendLine("\")");
        }
        
        return stringBuilder.ToString();
    }
}