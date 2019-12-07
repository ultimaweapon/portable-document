using System;
using System.Security.Cryptography;
using FluentAssertions;
using Xunit;

namespace PortableDocument.Tests
{
    public sealed class BinaryStringTests
    {
        readonly byte[] data;
        readonly BinaryString subject;

        public BinaryStringTests()
        {
            this.data = new byte[0];
            this.subject = new BinaryString(this.data);
        }

        [Fact]
        public void Construct_WithDefaultValue_IsNullShouldReturnTrue()
        {
            var subject = default(BinaryString);

            subject.IsNull.Should().BeTrue();
        }

        [Fact]
        public void Construct_WithNull_IsNullShouldReturnTrue()
        {
            var subject = new BinaryString(null);

            subject.IsNull.Should().BeTrue();
        }

        [Fact]
        public void Construct_WithNonNull_IsNullShouldReturnFalse()
        {
            var subject = new BinaryString(new byte[0]);

            subject.IsNull.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            this.subject.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void Equals_WithDifferentType_ShouldReturnFalse()
        {
            this.subject.Equals(this.data).Should().BeFalse();
        }

        [Fact]
        public void Equals_WithBothNull_ShouldReturnTrue()
        {
            var first = new BinaryString(null);
            var second = new BinaryString(null);

            first.Equals(second).Should().BeTrue();
            first.Equals((object)second).Should().BeTrue();
        }

        [Fact]
        public void Equals_CurrentIsNull_ShouldReturnFalse()
        {
            var other = new BinaryString(new byte[0]);

            BinaryString.Null.Equals(other).Should().BeFalse();
            BinaryString.Null.Equals((object)other).Should().BeFalse();
        }

        [Fact]
        public void Equals_OtherIsNull_shouldReturnFalse()
        {
            this.subject.Equals(BinaryString.Null).Should().BeFalse();
            this.subject.Equals((object)BinaryString.Null).Should().BeFalse();
        }

        [Fact]
        public void Equals_BothHaveZeroLength_ShouldReturnTrue()
        {
            var first = new BinaryString(new byte[0]);
            var second = new BinaryString(new byte[0]);

            first.Equals(second).Should().BeTrue();
            first.Equals((object)second).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        public void Equals_WithDifferentLength_ShouldReturnFalse(int left, int right)
        {
            var first = new BinaryString(new byte[left]);
            var second = new BinaryString(new byte[right]);

            first.Equals(second).Should().BeFalse();
            first.Equals((object)second).Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Equals_WithSameLengthButDifferentData_ShouldReturnFalse(int length)
        {
            var data1 = new byte[length];
            var data2 = new byte[length];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(data1);
                random.GetBytes(data2);
            }

            var first = new BinaryString(data1);
            var second = new BinaryString(data2);

            first.Equals(second).Should().BeFalse();
            first.Equals((object)second).Should().BeFalse();
        }

        [Fact]
        public void FindEndOfLine_WithNullInstance_ShouldThrow()
        {
            BinaryString.Null.Invoking(s => s.FindEndOfLine())
                             .Should().ThrowExactly<InvalidOperationException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void FindEndOfLine_WithInvalidStart_ShouldThrow(int length)
        {
            var subject = new BinaryString(new byte[length]);

            subject.Invoking(s => s.FindEndOfLine(length))
                   .Should().ThrowExactly<ArgumentOutOfRangeException>();
            subject.Invoking(s => s.FindEndOfLine(-1))
                   .Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        public void FindEndOfLine_NoMarker_ShouldReturnMinusOne(int length, int position)
        {
            // Arrange.
            var data = new byte[length];

            if (position == 1)
            {
                data[0] = Character.LineFeed;
            }
            else if (position > 1)
            {
                data[position - 2] = Character.CarriageReturn;
                data[position - 1] = Character.LineFeed;
            }

            var subject = new BinaryString(data);

            // Assert.
            subject.FindEndOfLine(position).Should().Be((-1, default(EndOfLine)));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 0)]
        [InlineData(4, 0)]
        [InlineData(5, 0)]
        [InlineData(6, 0)]
        [InlineData(6, 1)]
        public void FindEndOfLine_ContainsLineFeed_ShouldReturnFirstIndex(int length, int position)
        {
            // Arrange.
            var data = new byte[length];

            data[position] = Character.LineFeed;

            switch (length - position)
            {
                case 5:
                    data[position + 3] = Character.CarriageReturn;
                    data[position + 4] = Character.LineFeed;
                    goto case 3;
                case 3:
                    data[position + 2] = Character.CarriageReturn;
                    goto case 2;
                case 2:
                    data[position + 1] = Character.LineFeed;
                    break;
            }

            var subject = new BinaryString(data);

            // Assert.
            subject.FindEndOfLine().Should().Be((position, EndOfLine.LineFeed));
            subject.FindEndOfLine(position).Should().Be((position, EndOfLine.LineFeed));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 0)]
        [InlineData(4, 0)]
        [InlineData(5, 0)]
        [InlineData(6, 0)]
        [InlineData(6, 1)]
        public void FindEndOfLine_ContainsCarriageReturn_ShouldReturnFirstIndex(int length, int position)
        {
            // Arrange.
            var data = new byte[length];

            data[position] = Character.CarriageReturn;

            switch (length - position)
            {
                case 5:
                    data[position + 3] = Character.CarriageReturn;
                    data[position + 4] = Character.LineFeed;
                    goto case 3;
                case 3:
                    data[position + 2] = Character.LineFeed;
                    goto case 2;
                case 2:
                    data[position + 1] = Character.CarriageReturn;
                    break;
            }

            var subject = new BinaryString(data);

            // Assert.
            subject.FindEndOfLine().Should().Be((position, EndOfLine.CarriageReturn));
            subject.FindEndOfLine(position).Should().Be((position, EndOfLine.CarriageReturn));
        }

        [Theory]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(3, 1)]
        [InlineData(4, 0)]
        [InlineData(4, 2)]
        public void FindEndOfLine_ContainsBoth_ShouldReturnFirstIndex(int length, int position)
        {
            // Arrange.
            var data = new byte[length];

            data[position + 0] = Character.CarriageReturn;
            data[position + 1] = Character.LineFeed;

            switch (length - position)
            {
                case 4:
                    data[position + 3] = Character.LineFeed;
                    goto case 3;
                case 3:
                    data[position + 2] = Character.CarriageReturn;
                    break;
            }

            var subject = new BinaryString(data);

            // Assert.
            subject.FindEndOfLine().Should().Be((position, EndOfLine.Both));
            subject.FindEndOfLine(position).Should().Be((position, EndOfLine.Both));
        }

        [Fact]
        public void IsEndOfLine_WithNullInstance_ShouldThrow()
        {
            BinaryString.Null.Invoking(s => s.IsEndOfLine(0))
                             .Should().ThrowExactly<InvalidOperationException>();
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        [InlineData(1, 1)]
        [InlineData(2, -1)]
        [InlineData(2, 2)]
        public void IsEndOfLine_WithInvalidPosition_ShouldThrow(int length, int position)
        {
            var data = new byte[length];
            var subject = new BinaryString(data);

            subject.Invoking(s => s.IsEndOfLine(position))
                   .Should().ThrowExactly<ArgumentOutOfRangeException>()
                   .And.ParamName.Should().Be("position");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(5)]
        public void IsEndOfLine_WithNonMarker_ShouldReturnNull(int position)
        {
            var data = new byte[]
            {
                0x00,
                Character.LineFeed,
                0x00,
                Character.CarriageReturn,
                Character.LineFeed,
                0x00
            };

            var subject = new BinaryString(data);

            subject.IsEndOfLine(position).Should().BeNull();
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 2)]
        [InlineData(4, 3)]
        public void IsEndOfLine_WithSingleLineFeed_ShouldIdentifyAsLineFeed(int length, int position)
        {
            // Arrange.
            var data = new byte[length];

            if (position > 0)
            {
                data[position - 1] = Character.CarriageReturn;
            }

            data[position] = Character.LineFeed;

            var subject = new BinaryString(data);

            // Assert.
            subject.IsEndOfLine(position).Should().Be(EndOfLine.LineFeed);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 2)]
        [InlineData(4, 3)]
        public void IsEndOfLine_WithSingleCarriageReturn_ShouldIdentifyAsCarriageReturn(int length, int position)
        {
            // Arrange.
            var data = new byte[length];

            if (position > 0)
            {
                data[position - 1] = Character.LineFeed;
            }

            data[position] = Character.CarriageReturn;

            var subject = new BinaryString(data);

            // Assert.
            subject.IsEndOfLine(position).Should().Be(EndOfLine.CarriageReturn);
        }

        [Theory]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(3, 1)]
        [InlineData(4, 1)]
        public void IsEndOfLine_WithBoth_ShouldIdentifyAsBoth(int length, int position)
        {
            // Arrange.
            var data = new byte[length];

            if (position > 0)
            {
                data[position - 1] = Character.LineFeed;
            }

            data[position + 0] = Character.CarriageReturn;
            data[position + 1] = Character.LineFeed;

            if (position + 2 < data.Length)
            {
                data[position + 2] = Character.LineFeed;
            }

            var subject = new BinaryString(data);

            // Assert.
            subject.IsEndOfLine(position).Should().Be(EndOfLine.Both);
        }
    }
}
