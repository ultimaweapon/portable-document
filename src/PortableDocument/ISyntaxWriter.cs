using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortableDocument
{
    /// <summary>
    /// Represents a stateful document's syntax writer.
    /// </summary>
    public interface ISyntaxWriter
    {
        /// <summary>
        /// Write a comment with binary data.
        /// </summary>
        /// <param name="value">
        /// A binary represents the comment to write.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous write operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="value"/> contains one or more EOL marker.
        /// </exception>
        /// <exception cref="LineTooLongException">
        /// The resulting line is too long.
        /// </exception>
        Task WriteCommentAsync(byte[] value, CancellationToken cancellationToken);

        /// <summary>
        /// Write a comment with ASCII string.
        /// </summary>
        /// <param name="value">
        /// A <see cref="string"/> represents the comment to write.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous write operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="value"/> is not ASCII string.
        /// </exception>
        /// <exception cref="LineTooLongException">
        /// The resulting line is too long.
        /// </exception>
        Task WriteCommentAsync(string value, CancellationToken cancellationToken);

        /// <summary>
        /// Write a name object.
        /// </summary>
        /// <param name="id">
        /// The identifier of the name.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous write operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="id"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="id"/> contains one or more entries with zero value.
        /// </exception>
        Task WriteNameAsync(byte[] id, CancellationToken cancellationToken);
    }
}
