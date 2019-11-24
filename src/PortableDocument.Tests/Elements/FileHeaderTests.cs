using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PortableDocument.Elements;
using Xunit;

namespace PortableDocument.Tests.Elements
{
    public sealed class FileHeaderTests
    {
        [Fact]
        public void BinaryMarker_WhenRead_ShouldHave4BytesWithEachLargerThan127()
        {
            FileHeader.BinaryMarker.Should().HaveCount(4)
                                   .And.NotContain(b => b <= 127);
        }

        [Fact]
        public void Constructor_WithNegativeMajor_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>("major", () => new FileHeader(-1, 0, DocumentType.Binary));
        }

        [Fact]
        public void Constructor_WithNegativeMinor_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>("minor", () => new FileHeader(0, -1, DocumentType.Binary));
        }

        [Theory]
        [InlineData(0, 0, DocumentType.Binary)]
        [InlineData(0, 0, DocumentType.Text)]
        [InlineData(1, 0, DocumentType.Binary)]
        [InlineData(1, 0, DocumentType.Text)]
        [InlineData(1, 7, DocumentType.Binary)]
        [InlineData(1, 7, DocumentType.Text)]
        [InlineData(2, 0, DocumentType.Binary)]
        [InlineData(2, 0, DocumentType.Text)]
        public void Constructor_WithValidArgs_ShouldInitializesProperties(int major, int minor, DocumentType type)
        {
            var subject = new FileHeader(major, minor, type);

            subject.MajorVersion.Should().Be(major);
            subject.MinorVersion.Should().Be(minor);
            subject.DocumentType.Should().Be(type);
        }

        [Fact]
        public void WriteAsync_WithNullWriter_ShouldThrow()
        {
            var subject = new FileHeader(1, 7, DocumentType.Binary);

            subject.Invoking(s => s.WriteAsync(null, CancellationToken.None))
                   .Should().ThrowExactly<ArgumentNullException>()
                   .And.ParamName.Should().Be("writer");
        }

        [Fact]
        public async Task WriteAsync_WithTextDocument_ShouldNotWriteBinaryMarker()
        {
            // Arrange.
            var writer = new Mock<ISyntaxWriter>();
            var subject = new FileHeader(1, 7, DocumentType.Text);

            using (var cancellationSource = new CancellationTokenSource())
            {
                // Act.
                await subject.WriteAsync(writer.Object, cancellationSource.Token);

                // Assert.
                writer.Verify(w => w.WriteCommentAsync("PDF-1.7", cancellationSource.Token), Times.Once());
                writer.VerifyNoOtherCalls();
            }
        }

        [Fact]
        public async Task WriteAsync_WithBinaryDocument_ShouldWriteBinaryMarker()
        {
            // Arrange.
            var writer = new Mock<ISyntaxWriter>();
            var subject = new FileHeader(1, 6, DocumentType.Binary);

            using (var cancellationSource = new CancellationTokenSource())
            {
                // Act.
                await subject.WriteAsync(writer.Object, cancellationSource.Token);

                // Assert.
                writer.Verify(w => w.WriteCommentAsync("PDF-1.6", cancellationSource.Token), Times.Once());
                writer.Verify(
                    w => w.WriteCommentAsync(FileHeader.BinaryMarker, cancellationSource.Token),
                    Times.Once()
                );
                writer.VerifyNoOtherCalls();
            }
        }
    }
}
