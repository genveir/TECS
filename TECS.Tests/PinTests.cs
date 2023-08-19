using FluentAssertions;
using NUnit.Framework;
using TECS.HDLSimulator.Chips;
using TECS.Tests.Builders;

namespace TECS.Tests;

public class PinTests
{
    [TestCase(1)]
    [TestCase(5)]
    [TestCase(16)]
    public void CanCreatePin(int bitSize)
    {
        var pin = new PinBuilder(bitSize).Build();

        pin.Value.Length.Should().Be(bitSize);
    }

    [TestCase(1, 5)]
    [TestCase(4, 5)]
    [TestCase(16, 5)]
    public void CanUpdateCorrectLengthValue(int length, int numTests)
    {
        var pin = new PinBuilder(length).Build();

        for (int n = 0; n < numTests; n++)
        {
            var value = TestData.GenerateBoolArray(length);
            
            pin.Value = value;

            pin.Value.Should().BeEquivalentTo(value);
        }
    }

    [Test]
    public void ShortValueOverwritesEnd()
    {
        var pin = new PinBuilder(4).Build();

        pin.Value = new[] { true, true, true };

        pin.Value.Should().BeEquivalentTo(new[] { false, true, true, true });
    }

    [Test]
    public void LongValueWritesLateValues()
    {
        var pin = new PinBuilder(4).Build();

        pin.Value = new[] { true, false, true, false, true };

        pin.Value.Should().BeEquivalentTo(new[] { false, true, false, true });
    }

    [Test]
    public void ConnectedNotifyableIsNotified()
    {
        var notifyable = new TestNotifyable();

        var pin = new PinBuilder(1).Build();

        pin.Connect(notifyable);
        pin.Value = new[] { true };

        notifyable.WasNotified.Should().BeTrue();
    }

    [Test]
    public void CanNotifyMultiple()
    {
        var notifyA = new TestNotifyable();
        var notifyB = new TestNotifyable();

        var pin = new PinBuilder(1).Build();

        pin.Connect(notifyA);
        pin.Connect(notifyB);
        pin.Value = new[] { true };

        notifyA.WasNotified.Should().BeTrue();
        notifyB.WasNotified.Should().BeTrue();
    }

    private class TestNotifyable : INotifyable
    {
        public bool WasNotified;

        public void Notify()
        {
            WasNotified = true;
        }
    }

    private static class TestData
    {
        private static readonly Random Random = new ();
        
        public static bool[] GenerateBoolArray(int length)
        {
            var result = new bool[length];

            for (int n = 0; n < length; n++) result[n] = Random.Next(2) == 1;

            return result;
        }
    }
}