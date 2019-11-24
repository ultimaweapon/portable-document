using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortableDocument
{
    /// <summary>
    /// Represents an item to construct the document.
    /// </summary>
    public abstract class DocumentElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentElement"/> class.
        /// </summary>
        protected DocumentElement()
        {
        }

        /// <summary>
        /// Generate the document syntax for this element.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="ISyntaxWriter"/> to output the syntax of this element.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous write operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="writer"/> is <c>null</c>.
        /// </exception>
        public abstract Task WriteAsync(ISyntaxWriter writer, CancellationToken cancellationToken);
    }
}
