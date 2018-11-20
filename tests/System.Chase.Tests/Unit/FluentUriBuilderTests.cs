using FluentAssertions;
using NUnit.Framework;

namespace System.Chase.Tests.Unit
{
    [TestFixture]
    public class FluentUriBuilderTests
    {   
        [Test]
        public void ShouldBuildUriWithFragment()
        {
            // setup
            var expectedPort = 80;
            var expectedHost = "example.com";
            var fragment = "frag";
            var root = "http://example.com";
            var builder = new FluentUriBuilder(root);

            // execute
            var result = builder.Fragment(fragment)
                .Build();

            // assert
            result.Fragment.Should().Be($"#{fragment}");
            result.Port.Should().Be(expectedPort);
            result.Host.Should().Be(expectedHost);
        }

        [Test]
        public void ShouldBuildUriAndChangePort()
        {
            // setup
            var expectedPort = 8080;
            var expectedHost = "example.com";
            var fragment = "frag";
            var root = "http://example.com";
            var builder = new FluentUriBuilder(root);

            // execute
            var result = builder.Fragment(fragment)
                .Port(expectedPort)
                .Build();

            // assert
            result.Fragment.Should().Be($"#{fragment}");
            result.Port.Should().Be(expectedPort);
            result.Host.Should().Be(expectedHost);
        }
    }
}
