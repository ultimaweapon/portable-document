using System;
using FluentAssertions;
using Xunit;

namespace PortableDocument.Tests
{
    public sealed class StandardTests
    {
        [Fact]
        public void GetEndOfLineMarker_WithNullRaw_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>("raw", () => Standard.GetEndOfLineMarker(null, 0));
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        [InlineData(1, 1)]
        [InlineData(2, -1)]
        [InlineData(2, 2)]
        public void GetEndOfLineMarker_WithInvalidPosition_ShouldThrow(int length, int position)
        {
            var raw = new byte[length];

            Assert.Throws<ArgumentOutOfRangeException>("position", () => Standard.GetEndOfLineMarker(raw, position));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(5)]
        public void GetEndOfLineMarker_WithNonMarker_ShouldReturnNull(int position)
        {
            var raw = new byte[] { 0x00, 0x0A, 0x00, 0x0D, 0x0A, 0x00 };

            Standard.GetEndOfLineMarker(raw, position).Should().BeNull();
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 2)]
        [InlineData(4, 3)]
        public void GetEndOfLineMarker_WithSingleLineFeed_ShouldIdentifyAsLineFeed(int length, int position)
        {
            // Arrange.
            var raw = new byte[length];

            if (position > 0)
            {
                raw[position - 1] = 0x0D;
            }

            raw[position] = 0x0A;

            // Assert.
            Standard.GetEndOfLineMarker(raw, position).Should().Be(EndOfLine.LineFeed);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 2)]
        [InlineData(4, 3)]
        public void GetEndOfLineMarker_WithSingleCarriageReturn_ShouldIdentifyAsCarriageReturn(int length, int position)
        {
            // Arrange.
            var raw = new byte[length];

            if (position > 0)
            {
                raw[position - 1] = 0x0A;
            }

            raw[position] = 0x0D;

            // Assert.
            Standard.GetEndOfLineMarker(raw, position).Should().Be(EndOfLine.CarriageReturn);
        }

        [Theory]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(3, 1)]
        [InlineData(4, 1)]
        public void GetEndOfLineMarker_WithBoth_ShouldIdentifyAsBoth(int length, int position)
        {
            // Arrange.
            var raw = new byte[length];

            if (position > 0)
            {
                raw[position - 1] = 0x0A;
            }

            raw[position + 0] = 0x0D;
            raw[position + 1] = 0x0A;

            if (position + 2 < raw.Length)
            {
                raw[position + 2] = 0x0A;
            }

            // Assert.
            Standard.GetEndOfLineMarker(raw, position).Should().Be(EndOfLine.Both);
        }
    }
}
