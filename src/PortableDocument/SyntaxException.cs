using System;

namespace PortableDocument
{
    /// <summary>
    /// The exception that is thrown when the document syntax is not valid.
    /// </summary>
    public class SyntaxException : PortableDocumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxException"/> class.
        /// </summary>
        public SyntaxException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public SyntaxException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxException"/> class with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception.
        /// </param>
        public SyntaxException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
