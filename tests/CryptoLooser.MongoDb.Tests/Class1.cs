using NUnit.Framework;

namespace CryptoLooser.MongoDb.Tests;

[TestFixture]
public class Class1
{
    [Test]
    public async Task test()
    {
        await Task.Delay(10);
        
        Assert.Fail();
    }
}