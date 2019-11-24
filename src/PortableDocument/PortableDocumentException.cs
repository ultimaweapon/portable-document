using System;

namespace PortableDocument
{
    /// <summary>
    /// The exception that is thrown when there is a specific error for portable document library.
    /// </summary>
    public class PortableDocumentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortableDocumentException"/> class.
        /// </summary>
        public PortableDocumentException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortableDocumentException"/> class with a specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public PortableDocumentException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortableDocumentException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception.
        /// </param>
        public PortableDocumentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
