using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using FluentAssertions;
using Xunit;

namespace PortableDocument.Tests
{
    public sealed class BinaryStringTests
    {
        readonly byte[] data;
        BinaryString subject;

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
            this.subject.Equals((object)this.data).Should().BeFalse();
        }

        [Fact]
        public void Equals_WithBothNull_ShouldReturnTrue()
        {
            TestBothNullEquality(true, (left, right) => left.Equals(right));
            TestBothNullEquality(true, (left, right) => left.Equals((object)right));
        }

        [Fact]
        public void Equals_CurrentIsNull_ShouldReturnFalse()
        {
            TestLeftIsNullEquality(false, (left, right) => left.Equals(right));
            TestLeftIsNullEquality(false, (left, right) => left.Equals((object)right));
        }

        [Fact]
        public void Equals_OtherIsNull_shouldReturnFalse()
        {
            TestRightIsNullEquality(false, (left, right) => left.Equals(right));
            TestRightIsNullEquality(false, (left, right) => left.Equals((object)right));
        }

        [Fact]
        public void Equals_BothHaveZeroLength_ShouldReturnTrue()
        {
            TestBothZeroLengthEquality(true, (left, right) => left.Equals(right));
            TestBothZeroLengthEquality(true, (left, right) => left.Equals((object)right));
        }

        [Fact]
        public void Equals_WithDifferentLength_ShouldReturnFalse()
        {
            TestDifferentLengthEquality(false, (left, right) => left.Equals(right));
            TestDifferentLengthEquality(false, (left, right) => left.Equals((object)right));
        }

        [Fact]
        public void Equals_WithSameLengthButDifferentData_ShouldReturnFalse()
        {
            TestSameLengthButDifferentDataEquality(false, (left, right) => left.Equals(right));
            TestSameLengthButDifferentDataEquality(false, (left, right) => left.Equals((object)right));
        }

        [Fact]
        public void Equals_WithSameData_ShouldReturnTrue()
        {
            TestSameDataEquality(true, (left, right) => left.Equals(right));
            TestSameDataEquality(true, (left, right) => left.Equals((object)right));
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
        public void GetEnumerator_WithNullInstance_ShouldThrow()
        {
            BinaryString.Null.Invoking(s => s.GetEnumerator())
                             .Should().ThrowExactly<InvalidOperationException>();
            BinaryString.Null.As<IEnumerable>().Invoking(s => s.GetEnumerator())
                             .Should().ThrowExactly<InvalidOperationException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void GetEnumerator_WithNonNullInstance_ShouldReturnEnumeratorWithCorrectOrder(int length)
        {
            // Arrange.
            var data = new byte[length];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(data);
            }

            var subject = new BinaryString(data);

            // Assert.
            Assert(subject.GetEnumerator());
            Assert(((IEnumerable)subject).GetEnumerator());

            void Assert(IEnumerator enumerator)
            {
                foreach (var b in data)
                {
                    enumerator.MoveNext().Should().BeTrue();
                    enumerator.Current.Should().Be(b);
                }

                enumerator.MoveNext().Should().BeFalse();
            }
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

        [Fact]
        public void Equality_WithBothNull_ShouldReturnTrue()
        {
            TestBothNullEquality(true, (left, right) => left == right);
        }

        [Fact]
        public void Equality_LeftIsNull_ShouldReturnFalse()
        {
            TestLeftIsNullEquality(false, (left, right) => left == right);
        }

        [Fact]
        public void Equality_RightIsNull_shouldReturnFalse()
        {
            TestRightIsNullEquality(false, (left, right) => left == right);
        }

        [Fact]
        public void Equality_BothHaveZeroLength_ShouldReturnTrue()
        {
            TestBothZeroLengthEquality(true, (left, right) => left == right);
        }

        [Fact]
        public void Equality_WithDifferentLength_ShouldReturnFalse()
        {
            TestDifferentLengthEquality(false, (left, right) => left == right);
        }

        [Fact]
        public void Equality_WithSameLengthButDifferentData_ShouldReturnFalse()
        {
            TestSameLengthButDifferentDataEquality(false, (left, right) => left == right);
        }

        [Fact]
        public void Equality_WithSameData_ShouldReturnTrue()
        {
            TestSameDataEquality(true, (left, right) => left == right);
        }

        [Fact]
        public void Inequality_WithBothNull_ShouldReturnFalse()
        {
            TestBothNullEquality(false, (left, right) => left != right);
        }

        [Fact]
        public void Inequality_LeftIsNull_ShouldReturnTrue()
        {
            TestLeftIsNullEquality(true, (left, right) => left != right);
        }

        [Fact]
        public void Inequality_RightIsNull_shouldReturnTrue()
        {
            TestRightIsNullEquality(true, (left, right) => left != right);
        }

        [Fact]
        public void Inequality_BothHaveZeroLength_ShouldReturnFalse()
        {
            TestBothZeroLengthEquality(false, (left, right) => left != right);
        }

        [Fact]
        public void Inequality_WithDifferentLength_ShouldReturnTrue()
        {
            TestDifferentLengthEquality(true, (left, right) => left != right);
        }

        [Fact]
        public void Inequality_WithSameLengthButDifferentData_ShouldReturnTrue()
        {
            TestSameLengthButDifferentDataEquality(true, (left, right) => left != right);
        }

        [Fact]
        public void Inequality_WithSameData_ShouldReturnFalse()
        {
            TestSameDataEquality(false, (left, right) => left != right);
        }

        [Fact]
        public void ImplicitByteArrarToBinaryString_WithNull_ShouldReturnNullInstance()
        {
            this.subject = null;

            this.subject.IsNull.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ImplicitByteArrarToBinaryString_WithNonNull_ShouldReturnNonNullInstance(int length)
        {
            this.subject = new byte[length];

            this.subject.IsNull.Should().BeFalse();
        }

        void TestBothNullEquality(bool expected, Func<BinaryString, BinaryString, bool> test)
        {
            var first = new BinaryString(null);
            var second = new BinaryString(null);

            test(first, second).Should().Be(expected);
        }

        void TestLeftIsNullEquality(bool expected, Func<BinaryString, BinaryString, bool> test)
        {
            var other = new BinaryString(new byte[0]);

            test(BinaryString.Null, other).Should().Be(expected);
        }

        void TestRightIsNullEquality(bool expected, Func<BinaryString, BinaryString, bool> test)
        {
            test(this.subject, BinaryString.Null).Should().Be(expected);
        }

        void TestBothZeroLengthEquality(bool expected, Func<BinaryString, BinaryString, bool> test)
        {
            var first = new BinaryString(new byte[0]);
            var second = new BinaryString(new byte[0]);

            test(first, second).Should().Be(expected);
        }

        void TestDifferentLengthEquality(bool expected, Func<BinaryString, BinaryString, bool> test)
        {
            var cases = new (int left, int right)[]
            {
                (0, 1),
                (1, 0),
                (1, 2),
                (2, 1)
            };

            foreach (var c in cases)
            {
                var first = new BinaryString(new byte[c.left]);
                var second = new BinaryString(new byte[c.right]);

                test(first, second).Should().Be(expected);
            }
        }

        void TestSameLengthButDifferentDataEquality(bool expected, Func<BinaryString, BinaryString, bool> test)
        {
            foreach (var length in Enumerable.Range(1, 3))
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

                test(first, second).Should().Be(expected);
            }
        }

        void TestSameDataEquality(bool expected, Func<BinaryString, BinaryString, bool> test)
        {
            foreach (var length in Enumerable.Range(1, 3))
            {
                var data1 = new byte[length];
                var data2 = new byte[length];

                using (var random = RandomNumberGenerator.Create())
                {
                    random.GetBytes(data1);
                    Buffer.BlockCopy(data1, 0, data2, 0, data1.Length);
                }

                var first = new BinaryString(data1);
                var second = new BinaryString(data2);

                test(first, second).Should().Be(expected);
            }
        }
    }
}
