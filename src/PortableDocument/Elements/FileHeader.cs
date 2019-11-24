using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortableDocument.Elements
{
    /// <summary>
    /// Represents a File Header.
    /// </summary>
    public class FileHeader : DocumentElement
    {
        /// <summary>
        /// The binary to put inside a comment followed immediately the header line to indicate the document is in a
        /// binary format.
        /// </summary>
        public static readonly byte[] BinaryMarker = Enumerable.Repeat((byte)128, 4).ToArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHeader"/> class.
        /// </summary>
        /// <param name="major">
        /// Major version of the document.
        /// </param>
        /// <param name="minor">
        /// Minor version of the document.
        /// </param>
        /// <param name="type">
        /// Type of the document.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="major"/> or <paramref name="minor"/> is negative.
        /// </exception>
        public FileHeader(int major, int minor, DocumentType type)
        {
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(major), major, "The value is negative.");
            }

            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minor), minor, "The value is negative.");
            }

            MajorVersion = major;
            MinorVersion = minor;
            DocumentType = type;
        }

        /// <summary>
        /// Gets type of the document.
        /// </summary>
        /// <value>
        /// The type of the document.
        /// </value>
        public DocumentType DocumentType { get; }

        /// <summary>
        /// Gets major version of the document.
        /// </summary>
        /// <value>
        /// The major version of the document.
        /// </value>
        public int MajorVersion { get; }

        /// <summary>
        /// Gets the minor version of the document.
        /// </summary>
        /// <value>
        /// The minor version of the document.
        /// </value>
        public int MinorVersion { get; }

        public override async Task WriteAsync(ISyntaxWriter writer, CancellationToken cancellationToken)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            await writer.WriteCommentAsync($"PDF-{MajorVersion}.{MinorVersion}", cancellationToken);

            if (DocumentType == DocumentType.Binary)
            {
                await writer.WriteCommentAsync(BinaryMarker, cancellationToken);
            }
        }
    }
}
