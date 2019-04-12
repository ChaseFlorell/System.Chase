
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace System.Chase.Tests.Unit
{
    [TestFixture]
    public class SequentialGuidTests
    {

        [Test]
        public void ShouldGenerateGuid()
        {
            // execute
            var sequentialGuid = SequentialGuid.NewGuid();

            // assert
            sequentialGuid.Should().NotBeEmpty();
        }

        [Test]
        public void ShouldGetTimestamp()
        {
            // setup
            var sequentialGuid = SequentialGuid.NewGuid();
            
            // execute
            var timestamp = SequentialGuid.GetTimestamp(sequentialGuid);

            // assert
            timestamp.Should().BeAfter(System.DateTime.UtcNow.Subtract(TimeSpan.FromMilliseconds(1)));
            timestamp.Should().BeBefore(System.DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        [TestCase("0B77CE03-F4C5-4204-8B32-ABA22EFB5580")]
        [TestCase("74E6B8F3-DBE6-4ECF-8DE3-C4CE17573026")]
        [TestCase("0CC49837-1461-4409-ADCE-ACF85EE15AFD")]
        [TestCase("42E33BA2-40B4-40E3-8834-1C341BE70486")]
        [TestCase("9E4E69DE-EE59-448E-AC64-ED58D7F69D3A")]
        [TestCase("E6019C40-A734-4BD5-A247-5CE3FDB13527")]
        [TestCase("9DE73266-4A2C-4B26-8352-3AD57AD61BB5")]
        [TestCase("A7528345-ABDA-452E-8AA9-6F32C5420420")]
        [TestCase("B35E7CD2-AB8D-4BC0-93ED-05E4CE22F56B")]
        [TestCase("83D9DE21-C1A9-4B13-99C3-DF38E4D6C8D4")]
        public void ShouldFailToGetTimestamp(string seed)
        {
            // setup 
            var guid = new Guid(seed);
            
            // execute
            Action action = () => SequentialGuid.GetTimestamp(guid);

            // assert
            action.Should().Throw<InvalidOperationException>().WithMessage($"Cannot extract a valid DateTime from {seed}");
        }

        [Test]
        [TestCase("0B77CE03-F4C5-4204-8B32-ABA22EFB5580")]
        [TestCase("74E6B8F3-DBE6-4ECF-8DE3-C4CE17573026")]
        [TestCase("0CC49837-1461-4409-ADCE-ACF85EE15AFD")]
        [TestCase("42E33BA2-40B4-40E3-8834-1C341BE70486")]
        [TestCase("9E4E69DE-EE59-448E-AC64-ED58D7F69D3A")]
        [TestCase("E6019C40-A734-4BD5-A247-5CE3FDB13527")]
        [TestCase("9DE73266-4A2C-4B26-8352-3AD57AD61BB5")]
        [TestCase("A7528345-ABDA-452E-8AA9-6F32C5420420")]
        [TestCase("B35E7CD2-AB8D-4BC0-93ED-05E4CE22F56B")]
        [TestCase("83D9DE21-C1A9-4B13-99C3-DF38E4D6C8D4")]
        public void ShouldGenerateSequentialGuidsWithKnownSeed(string seed)
        {
            // setup
            const int expectedLength = 50;
            var guidList = new List<Guid>();

            // execute
            for (var i = 0; i < expectedLength; i++)
            {
                guidList.Add(SequentialGuid.NewGuid(seed));
            }

            // assert
            guidList.Count.Should().Be(expectedLength);
            guidList.Distinct().Count().Should().Be(expectedLength, "all guids are unique");

            var previousStamp = System.DateTime.MinValue;
            foreach (var guid in guidList.OrderBy(SequentialGuid.GetTimestamp))
            {
                var stamp = SequentialGuid.GetTimestamp(guid);
                stamp.Should().BeOnOrAfter(previousStamp);
                previousStamp = stamp;
            }
        }
        
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(8)]
        [TestCase(13)]
        [TestCase(21)]
        [TestCase(34)]
        [TestCase(55)]
        [TestCase(89)]
        public void ShouldGenerateSequentialGuidsWithDynamicSeed(int expectedLength)
        {
            // setup
            var guidList = new List<Guid>();

            // execute
            for (var i = 0; i < expectedLength; i++)
            {
                guidList.Add(SequentialGuid.NewGuid());
            }

            // assert
            guidList.Count.Should().Be(expectedLength);
            guidList.Distinct().Count().Should().Be(expectedLength, "all guids are unique");

            var previousStamp = System.DateTime.MinValue;
            foreach (var guid in guidList.OrderBy(SequentialGuid.GetTimestamp))
            {
                var stamp = SequentialGuid.GetTimestamp(guid);
                stamp.Should().BeOnOrAfter(previousStamp);
                previousStamp = stamp;
            }
        }
        
        [Test]
        public void ShouldGenerateSequentialGuidsWithNoSeed()
        {
            // setup
            const int expectedLength = 50;
            var guidList = new List<Guid>();

            // execute
            for (var i = 0; i < expectedLength; i++)
            {
                guidList.Add(SequentialGuid.NewGuid());
            }

            // assert
            guidList.Count.Should().Be(expectedLength);
            guidList.Distinct().Count().Should().Be(expectedLength, "all guids are unique");

            var previousStamp = System.DateTime.MinValue;
            foreach (var guid in guidList.OrderBy(SequentialGuid.GetTimestamp))
            {
                var stamp = SequentialGuid.GetTimestamp(guid);
                stamp.Should().BeOnOrAfter(previousStamp);
                previousStamp = stamp;
            }
        }
    }
}