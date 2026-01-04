namespace ImageAsciiArt.Options;

/// <summary>
/// Output format for the rendered ASCII art.
/// </summary>
public enum OutputFormat
{
    /// <summary>
    /// Output to console with ANSI colors.
    /// </summary>
    Console,

    /// <summary>
    /// Output to text file (with optional ANSI codes).
    /// </summary>
    Text,

    /// <summary>
    /// Output to HTML file for web viewing.
    /// </summary>
    Html
}
