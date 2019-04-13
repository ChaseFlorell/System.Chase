
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace System.Chase.Tests.Unit
{
    [TestFixture]
    public class SequentialGuidTests
    {
        private Guid _knownSeed;
        private const int _oneMillion = 1000000;

        public SequentialGuidTests()
        {
            _knownSeed = new Guid("012AA8E1-BCC1-4EC9-92A6-D96EABC881B4");
        }
        
        [Test]
        public void ShouldGenerateGuid()
        {
            // execute
            var sequentialGuid = SequentialGuid.NewSequentialGuid();

            // assert
            sequentialGuid.Should().NotBeEmpty();
        }

        [Test]
        public void ShouldGetTimestamp()
        {
            // setup
            var sequentialGuid = SequentialGuid.NewSequentialGuid();
            
            // execute
            var timestamp = SequentialGuid.GetTimestamp(sequentialGuid);

            // assert
            timestamp.Should().BeAfter(System.DateTime.UtcNow.Subtract(TimeSpan.FromMilliseconds(1)));
            timestamp.Should().BeBefore(System.DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        [TestCase("C6B66B5C-A007-43EE-BB66-DE0432B44A20")]
        [TestCase("74E6B8F3-DBE6-4ECF-8DE3-C4CE17573026")]
        [TestCase("0CC49837-1461-4409-ADCE-ACF85EE15AFD")]
        [TestCase("42E33BA2-40B4-40E3-8834-1C341BE70486")]
        [TestCase("9E4E69DE-EE59-448E-AC64-ED58D7F69D3A")]
        [TestCase("E6019C40-A734-4BD5-A247-5CE3FDB13527")]
        [TestCase("9DE73266-4A2C-4B26-8352-3AD57AD61BB5")]
        [TestCase("A7528345-ABDA-452E-8AA9-6F32C5420420")]
        [TestCase("B35E7CD2-AB8D-4BC0-93ED-05E4CE22F56B")]
        [TestCase("ED54DDEC-547D-4179-8644-97E4B75F2112")]
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
        [TestCase("I 🧡 Xamarin")]
        [TestCase("Words With Spaces")]
        [TestCase("Microsoft")]
        public void ShouldGenerateSequentialGuidsWithKnownSeed(string seed)
        {
            // setup
            const int expectedLength = 50;
            var guidList = new HashSet<Guid>();

            // execute
            for (var i = 0; i < expectedLength; i++)
            {
                guidList.Add(SequentialGuid.NewSequentialGuid(seed));
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
        public void ShouldGenerateSequentialGuidsWithUnknownSeedSeed(int expectedLength)
        {
            // setup
            var guidList = new HashSet<Guid>();

            // execute
            for (var i = 0; i < expectedLength; i++)
            {
                guidList.Add(SequentialGuid.NewSequentialGuid());
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
            var guidList = new HashSet<Guid>();

            // execute
            for (var i = 0; i < expectedLength; i++)
            {
                guidList.Add(SequentialGuid.NewSequentialGuid());
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
        public void TimeGeneratingOneMillionGuids()
        {
            var set = new HashSet<Guid>();
            for (var i = 0; i < _oneMillion; i++)
            {
                set.Add(Guid.NewGuid());
            }

            set.Count.Should().Be(_oneMillion);
            set.Distinct().Count().Should().Be(_oneMillion);
        }

        [Test]
        public void TimeGeneratingOneMillionSequentialGuids()
        {
            // setup
            var set = new HashSet<Guid>();
            
            // execute
            for (var i = 0; i < _oneMillion; i++)
            {
                set.Add(SequentialGuid.NewSequentialGuid());
            }

            // assert
            set.Count.Should().Be(_oneMillion);
            set.Distinct().Count().Should().Be(_oneMillion);
        }

        [Test]
        public void TimeGeneratingOneMillionSequentialGuidsWithKnownSeed()
        {
            // setup
            var set = new HashSet<Guid>();
            
            // execute
            for (var i = 0; i < _oneMillion; i++)
            {
                set.Add(SequentialGuid.NewSequentialGuid(_knownSeed));
            }

            // assert
            set.Count.Should().Be(_oneMillion);
            set.Distinct().Count().Should().Be(_oneMillion);
        }

        [Test]
        [TestCase("")]
        [TestCase("a")]
        [TestCase("as")]
        [TestCase("asd")]
        [TestCase("asdf")]
        [TestCase("asdfj")]
        [TestCase("asdfjk")]
        [TestCase("asdfjkl")]
        public void ShouldThrowWhenSeedIsTooShort(string seed)
        {
            // setup
            Action action = () => SequentialGuid.NewSequentialGuid(seed);
            
            // execute
            action.Should().Throw<ArgumentException>().WithMessage("Seed must be a minimum of 8 characters\nParameter name: seed");
        }

        [Test]
        public void ShouldExtractTimestampFromSequentialGuid()
        {
            // setup
            var utcNow = System.DateTime.UtcNow;
            var sequentialGuid = SequentialGuid.NewSequentialGuid();
            
            // execute
            var extractedTime = SequentialGuid.GetTimestamp(sequentialGuid);
            
            // assert
            extractedTime.Year.Should().Be(utcNow.Year);
            extractedTime.Month.Should().Be(utcNow.Month);
            extractedTime.Day.Should().Be(utcNow.Day);
            extractedTime.Hour.Should().Be(utcNow.Hour);
            extractedTime.Minute.Should().Be(utcNow.Minute);
            extractedTime.Second.Should().Be(utcNow.Second);
        }
    }
}
