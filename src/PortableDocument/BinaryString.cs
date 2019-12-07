using System;
using System.Linq;

namespace PortableDocument
{
    /// <summary>
    /// Represents a binary string.
    /// </summary>
    public struct BinaryString : IEquatable<BinaryString>
    {
        /// <summary>
        /// Represents a null instance.
        /// </summary>
        public static readonly BinaryString Null;

        readonly byte[] data;

        /// <summary>
        /// Initializes the <see cref="BinaryString"/>.
        /// </summary>
        /// <param name="data">
        /// The binary to initialize the instance.
        /// </param>
        public BinaryString(byte[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Gets the value to indicated this object represent a null value.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is null; otherwise <c>false</c>.
        /// </value>
        public bool IsNull => this.data == null;

        public bool Equals(BinaryString other)
        {
            if (IsNull != other.IsNull)
            {
                return false;
            }

            return IsNull ? true : this.data.SequenceEqual(other.data);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((BinaryString)obj);
        }

        /// <summary>
        /// Search for EOL marker starting at specified position.
        /// </summary>
        /// <param name="start">
        /// Index to start searching.
        /// </param>
        /// <returns>
        /// The index of the first EOL marker and the type of marker or <c>-1</c> for the index if no marker was found.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="start"/> is not a valid start position.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The object is null.
        /// </exception>
        public (int location, EndOfLine type) FindEndOfLine(int start = 0)
        {
            if (IsNull)
            {
                throw new InvalidOperationException("The object is null.");
            }

            if (start < 0 || start >= this.data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(start), start, "The value is not a valid start position.");
            }

            for (int i = start; i < this.data.Length; i++)
            {
                var type = IsEndOfLine(i);
                if (type.HasValue)
                {
                    return (i, type.Value);
                }
            }

            return (-1, default(EndOfLine));
        }

        public override int GetHashCode()
        {
            return IsNull ? 0 : this.data.Aggregate(17, (previous, next) => previous * (29 + next));
        }

        /// <summary>
        /// Check the specified position to see if it is EOL marker.
        /// </summary>
        /// <param name="position">
        /// The index to check.
        /// </param>
        /// <returns>
        /// One of <see cref="EndOfLine"/> if the specified position is EOL marker; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="position"/> is not a valid index.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The object is null.
        /// </exception>
        public EndOfLine? IsEndOfLine(int position)
        {
            if (IsNull)
            {
                throw new InvalidOperationException("The object is null.");
            }

            if (position < 0 || position >= this.data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position), position, "The value is not a valid position.");
            }

            switch (this.data[position])
            {
                case Character.CarriageReturn:
                    if (++position == this.data.Length)
                    {
                        return EndOfLine.CarriageReturn;
                    }
                    return (this.data[position] == Character.LineFeed) ? EndOfLine.Both : EndOfLine.CarriageReturn;
                case Character.LineFeed:
                    return EndOfLine.LineFeed;
                default:
                    return null;
            }
        }
    }
}
