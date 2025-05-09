using Lofi2D.Math;

namespace Lofi2D.Tests.Math;

public class RangeFTests
{
    [Test]
    public void OrderedTest()
    {
        var range = new RangeF(2, 1);
        Assert.Multiple(() =>
        {
            Assert.That(range.Start, Is.EqualTo(2f));
            Assert.That(range.End, Is.EqualTo(1f));
        });

        var ordered = range.Ordered;
        Assert.Multiple(() =>
        {
            Assert.That(ordered.Start, Is.EqualTo(1f));
            Assert.That(ordered.End, Is.EqualTo(2f));
        });
    }
}