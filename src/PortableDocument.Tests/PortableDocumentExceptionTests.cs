using System;
using FluentAssertions;
using Xunit;

namespace PortableDocument.Tests
{
    public sealed class PortableDocumentExceptionTests
    {
        [Fact]
        public void Constructor_WithNullMessage_ShouldGenerateMessageProperty()
        {
            var subject = new PortableDocumentException(null);

            subject.Message.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WhenSpecifiedMessage_ShouldAssignToMessageProperty()
        {
            var message = "abc";

            var subject1 = new PortableDocumentException(message);
            var subject2 = new PortableDocumentException(message, new Exception());

            subject1.Message.Should().Be(message);
            subject2.Message.Should().Be(message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(typeof(Exception))]
        public void Constructor_WhenSpecifiedInner_ShouldAssignToInnerExceptionProperty(Type exception)
        {
            var inner = (exception != null) ? (Exception)Activator.CreateInstance(exception) : null;

            var subject = new PortableDocumentException(null, inner);

            subject.InnerException.Should().Be(inner);
        }
    }
}
