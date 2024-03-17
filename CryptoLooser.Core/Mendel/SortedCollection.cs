namespace CryptoLooser.Core.Mendel;

internal class SortedCollection<TKey, TValue> where TKey : notnull
{
    private readonly SortedDictionary<TKey, List<TValue>> _data;
    private readonly Func<TValue, TKey> _valueToKeyFunc;

    public SortedCollection(
        IEnumerable<TValue> items,
        Func<TValue, TKey> valueToKeyFunc,
        IComparer<TKey> comparer)
    {
        _valueToKeyFunc = valueToKeyFunc;

        var dictionary = items
            .Select(item => (Key: valueToKeyFunc(item), Value: item))
            .GroupBy(i => i.Key)
            .ToDictionary(
                g => g.Key,
                g => g.Select(kvp => kvp.Value).ToList());

        _data = new SortedDictionary<TKey, List<TValue>>(dictionary, comparer);
    }

    public void Add(TValue value)
    {
        var key = _valueToKeyFunc(value);

        if (!_data.TryGetValue(key, out var items))
        {
            items = new List<TValue>();
            _data.Add(key, items);
        }

        items.Add(value);
    }

    public TValue GetMaxValue()
    {
        var maxList = _data.First();
        return maxList.Value[^1];
    }

    public TValue PopMaxValue()
    {
        var maxList = _data.First();

        if (maxList.Value.Count == 1)
        {
            _data.Remove(maxList.Key);
            return maxList.Value[0];
        }

        var result = maxList.Value[^1];
        maxList.Value.RemoveAt(maxList.Value.Count - 1);

        return result;
    }

    public void RemoveMinValue()
    {
        var minList = _data.Last();

        if (minList.Value.Count == 1)
        {
            _data.Remove(minList.Key);
        }

        minList.Value.RemoveAt(minList.Value.Count - 1);
    }
}
