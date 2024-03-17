using CryptoLooser.Core.NeuralNetwork;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace CryptoLooser.Core.UnitTests;

[TestFixture]
public class CircularBufferTests
{
    [Test]
    public void add_as_many_elements_as_size_and_check_if_copied_correctly()
    {
        var buffer = new CircularBuffer<int>(6);

        buffer.Append(6);
        buffer.Append(5);
        buffer.Append(4);
        buffer.Append(3);
        buffer.Append(2);
        buffer.Append(1);

        var destination = new int[6];
        buffer.CopyTo(destination);

        CollectionAssert.AreEqual(
            new[] { 1, 2, 3, 4, 5, 6 },
            destination.ToArray());
    }

    [Test]
    public void add_more_elements_than_size_and_check_if_copied_correctly()
    {
        var buffer = new CircularBuffer<int>(6);

        buffer.Append(6);
        buffer.Append(5);
        buffer.Append(4);
        buffer.Append(3);
        buffer.Append(2);
        buffer.Append(1);
        buffer.Append(0);
        buffer.Append(-1);

        var destination = new int[6];
        buffer.CopyTo(destination);

        CollectionAssert.AreEqual(
            new[] { -1, 0, 1, 2, 3, 4 },
            destination.ToArray());
    }

    [Test]
    public void add_less_elements_than_size_and_it_should_not_be_full()
    {
        var buffer = new CircularBuffer<double>(5);

        buffer.Append(4.03d);
        buffer.Append(12.24d);
        buffer.Append(2.55d);

        Assert.That(buffer.IsFull, Is.False);
    }

    [Test]
    public void add_as_many_elements_as_size_and_it_should_be_full()
    {
        var buffer = new CircularBuffer<double>(5);

        buffer.Append(4.03d);
        buffer.Append(12.24d);
        buffer.Append(2.55d);
        buffer.Append(22.31d);
        buffer.Append(11.05d);

        Assert.That(buffer.IsFull, Is.True);
    }

    [Test]
    public void add_more_elements_than_size_and_it_should_be_full()
    {
        var buffer = new CircularBuffer<double>(5);

        buffer.Append(4.03d);
        buffer.Append(12.24d);
        buffer.Append(2.55d);
        buffer.Append(22.31d);
        buffer.Append(11.05d);
        buffer.Append(-12.35d);
        buffer.Append(4.05d);

        Assert.That(buffer.IsFull, Is.True);
    }
}
