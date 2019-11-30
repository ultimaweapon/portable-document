namespace PortableDocument
{
    /// <summary>
    /// End-Of-Line (EOL) mode.
    /// </summary>
    public enum EndOfLine
    {
        /// <summary>
        /// A single LINE FEED character (0Ah).
        /// </summary>
        LineFeed,

        /// <summary>
        /// A single CARRIAGE RETURN character (0Dh).
        /// </summary>
        CarriageReturn,

        /// <summary>
        /// Two characters started with a CARRIAGE RETURN and followed immediately by a LINE FEED.
        /// </summary>
        Both
    }
}
