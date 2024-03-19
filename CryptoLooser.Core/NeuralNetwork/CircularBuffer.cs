namespace CryptoLooser.Core.NeuralNetwork;

internal class CircularBuffer<T>(int size) where T : struct
{
    private readonly T[] _buffer = new T[size];
    private int _position = size - 1;

    public bool IsFull { get; private set; } = size == 0;

    public void Append(T value)
    {
        if (_buffer.Length == 0)
        {
            return;
        }

        _buffer[_position] = value;
        _position--;

        if (_position == -1)
        {
            _position = _buffer.Length - 1;
            IsFull = true;
        }
    }

    public void CopyTo(Span<T> destination)
    {
        if (_buffer.Length == 0)
        {
            return;
        }

        var lastWrotePosition = _position + 1;
        lastWrotePosition = lastWrotePosition == _buffer.Length ? 0 : lastWrotePosition;

        _buffer.AsSpan(lastWrotePosition).CopyTo(destination);

        if (lastWrotePosition != 0)
        {
            _buffer.AsSpan(0, lastWrotePosition).CopyTo(destination.Slice(_buffer.Length - lastWrotePosition));
        }
    }

    public T GetMinValue() => _buffer.Min();

    public T GetMaxValue() => _buffer.Max();
}
