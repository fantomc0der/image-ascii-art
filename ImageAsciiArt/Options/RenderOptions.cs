namespace ImageAsciiArt.Options;

/// <summary>
/// Configuration options for ASCII art rendering.
/// </summary>
public sealed class RenderOptions
{
    /// <summary>
    /// Path to the input image file.
    /// </summary>
    public required string ImagePath { get; init; }

    /// <summary>
    /// Rendering mode (Classic ASCII or HalfBlock).
    /// </summary>
    public RenderMode Mode { get; init; } = RenderMode.HalfBlock;

    /// <summary>
    /// Character set to use for classic ASCII mode.
    /// </summary>
    public CharacterSet CharacterSet { get; init; } = CharacterSet.Extended;

    /// <summary>
    /// Custom character string when CharacterSet is Custom.
    /// Characters should be ordered from darkest to lightest.
    /// </summary>
    public string? CustomCharacters { get; init; }

    /// <summary>
    /// Override width in characters. Null means auto-detect from terminal.
    /// </summary>
    public int? Width { get; init; }

    /// <summary>
    /// Override height in characters. Null means auto-detect from terminal.
    /// </summary>
    public int? Height { get; init; }

    /// <summary>
    /// Output format.
    /// </summary>
    public OutputFormat OutputFormat { get; init; } = OutputFormat.Console;

    /// <summary>
    /// Output file path when OutputFormat is Text or Html.
    /// </summary>
    public string? OutputPath { get; init; }

    /// <summary>
    /// Disable colors (grayscale output).
    /// </summary>
    public bool NoColor { get; init; }

    /// <summary>
    /// Invert brightness mapping (for light terminals).
    /// </summary>
    public bool Invert { get; init; }

    /// <summary>
    /// Enable live preview mode that re-renders on terminal resize.
    /// </summary>
    public bool Watch { get; init; }

    /// <summary>
    /// Include ANSI color codes when outputting to text file.
    /// </summary>
    public bool PreserveAnsiInTextOutput { get; init; }

    /// <summary>
    /// Gets the character ramp for the selected character set.
    /// </summary>
    public string GetCharacterRamp()
    {
        var ramp = CharacterSet switch
        {
            CharacterSet.Standard => "@%#*+=-:. ",
            CharacterSet.Extended => "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`'. ",
            CharacterSet.Simple => "@#:. ",
            CharacterSet.Blocks => "█▓▒░ ",
            CharacterSet.Custom => CustomCharacters ?? "@%#*+=-:. ",
            _ => "@%#*+=-:. "
        };

        return Invert ? new string(ramp.Reverse().ToArray()) : ramp;
    }
}
