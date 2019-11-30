using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PortableDocument.Elements;
using Xunit;

namespace PortableDocument.Tests.Elements
{
    public sealed class NameTests
    {
        readonly Mock<ISyntaxWriter> writer;
        readonly Name subject;

        public NameTests()
        {
            this.writer = new Mock<ISyntaxWriter>();
            this.subject = new Name("☉");
        }

        [Fact]
        public void Encoding_WithUnicodeInput_ShouldSuccess()
        {
            var utf8 = Name.Encoding.GetBytes("☉");

            utf8.Should().Equal(0xE2, 0x98, 0x89);
        }

        [Fact]
        public void Constructor_WithNullRawId_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>("id", () => new Name((byte[])null));
        }

        [Fact]
        public void Constructor_WithInvalidRawId_ShouldThrow()
        {
            Assert.Throws<ArgumentException>("id", () => new Name(new byte[] { 0x00 }));
            Assert.Throws<ArgumentException>("id", () => new Name(new byte[] { 0x01, 0x00 }));
            Assert.Throws<ArgumentException>("id", () => new Name(new byte[] { 0x00, 0x01 }));
            Assert.Throws<ArgumentException>("id", () => new Name(new byte[] { 0x01, 0x00, 0xFF }));
        }

        [Fact]
        public void Constructor_WithEmptyRawId_ShouldSuccess()
        {
            var name = new Name(new byte[0]);

            name.Id.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WithNonEmptyAndValidRawId_ShouldSuccess()
        {
            var id = new byte[] { 0x01, 0xFF };

            var name = new Name(id);

            name.Id.Should().Equal(id);
        }

        [Fact]
        public void Constructor_WithNullString_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>("id", () => new Name((string)null));
        }

        [Fact]
        public void Constructor_WithEmptyString_ShouldSuccess()
        {
            var name = new Name("");

            name.Id.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WithNonEmptyString_ShouldSuccess()
        {
            var name = new Name("☉");

            name.Id.Should().Equal(0xE2, 0x98, 0x89);
        }

        [Fact]
        public void WriteAsync_WithNullWriter_ShouldThrow()
        {
            this.subject.Invoking(s => s.WriteAsync(null, CancellationToken.None))
                        .Should().ThrowExactly<ArgumentNullException>()
                        .And.ParamName.Should().Be("writer");
        }

        [Fact]
        public async Task WriteAsync_WithValidArgs_ShouldWriteNameObject()
        {
            using (var cancellationSource = new CancellationTokenSource())
            {
                await this.subject.WriteAsync(this.writer.Object, cancellationSource.Token);

                this.writer.Verify(w => w.WriteNameAsync(this.subject.Id, cancellationSource.Token), Times.Once());
                this.writer.VerifyNoOtherCalls();
            }
        }
    }
}
