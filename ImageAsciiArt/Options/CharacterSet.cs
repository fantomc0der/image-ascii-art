namespace ImageAsciiArt.Options;

/// <summary>
/// Character set presets for classic ASCII rendering.
/// </summary>
public enum CharacterSet
{
    /// <summary>
    /// Standard 10-character ramp.
    /// </summary>
    Standard,

    /// <summary>
    /// Extended 70-character ramp for finer gradation.
    /// </summary>
    Extended,

    /// <summary>
    /// Simple 5-character ramp for basic output.
    /// </summary>
    Simple,

    /// <summary>
    /// Block characters only (░▒▓█).
    /// </summary>
    Blocks,

    /// <summary>
    /// User-provided custom character set.
    /// </summary>
    Custom
}
