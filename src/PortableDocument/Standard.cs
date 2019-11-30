using System;

namespace PortableDocument
{
    /// <summary>
    /// Contains methods to help with ISO 32000-1:2008 compliance.
    /// </summary>
    public static class Standard
    {
        /// <summary>
        /// Get the EOL marker at the specified index in the byte array.
        /// </summary>
        /// <param name="raw">
        /// Data to get EOL marker.
        /// </param>
        /// <param name="position">
        /// The index in the <paramref name="raw"/> to start checking.
        /// </param>
        /// <returns>
        /// The value of <see cref="EndOfLine"/> if the specified position is EOL marker; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="raw"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="position"/> is not a valid index in <paramref name="raw"/>.
        /// </exception>
        public static EndOfLine? GetEndOfLineMarker(byte[] raw, int position)
        {
            if (raw == null)
            {
                throw new ArgumentNullException(nameof(raw));
            }

            if (position < 0 || position >= raw.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position), position, "The value is not valid index.");
            }

            switch (raw[position])
            {
                case 0x0D:
                    if (++position == raw.Length)
                    {
                        return EndOfLine.CarriageReturn;
                    }
                    return (raw[position] == 0x0A) ? EndOfLine.Both : EndOfLine.CarriageReturn;
                case 0x0A:
                    return EndOfLine.LineFeed;
                default:
                    return null;
            }
        }
    }
}
