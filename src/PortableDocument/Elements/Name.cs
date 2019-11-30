using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortableDocument.Elements
{
    /// <summary>
    /// Represents a name object.
    /// </summary>
    public class Name : Object
    {
        /// <summary>
        /// The <see cref="System.Text.Encoding"/> instance for converting between <see cref="string"/> and the
        /// identifier.
        /// </summary>
        public static readonly Encoding Encoding = new UTF8Encoding(false, true);

        /// <summary>
        /// Initializes a new instance of the <see cref="Name"/> class with the raw identifier.
        /// </summary>
        /// <param name="id">
        /// The identifier of the name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="id"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="id"/> is contains one or more values with zero.
        /// </exception>
        public Name(byte[] id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (id.Any(b => b == 0))
            {
                throw new ArgumentException("The value contains one ore more zero byte.", nameof(id));
            }

            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Name"/> class with identifier constructed from
        /// <see cref="string"/>
        /// </summary>
        /// <param name="id">
        /// The value to construct identifier.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="id"/> is <c>null</c>.
        /// </exception>
        public Name(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Id = Encoding.GetBytes(id);
        }

        /// <summary>
        /// Gets the identifier of this name.
        /// </summary>
        /// <value>
        /// The identifier of this name.
        /// </value>
        /// <remarks>
        /// The bytes sequence will never contains an entry with zero.
        /// </remarks>
        public byte[] Id { get; }

        public override Task WriteAsync(ISyntaxWriter writer, CancellationToken cancellationToken)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            return writer.WriteNameAsync(Id, cancellationToken);
        }
    }
}
