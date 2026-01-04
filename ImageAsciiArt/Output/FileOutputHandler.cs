using System.Text.RegularExpressions;
using ImageAsciiArt.Options;

namespace ImageAsciiArt.Output;

/// <summary>
/// Outputs ASCII art to a text file.
/// </summary>
public sealed partial class FileOutputHandler : IOutputHandler
{
    [GeneratedRegex(@"\x1b\[[0-9;]*m")]
    private static partial Regex AnsiEscapeRegex();

    /// <summary>
    /// Removes ANSI escape codes from the content.
    /// </summary>
    private static string StripAnsiCodes(string input) => AnsiEscapeRegex().Replace(input, string.Empty);

    /// <inheritdoc />
    public void Write(string content, RenderOptions options)
    {
        if (string.IsNullOrEmpty(options.OutputPath))
        {
            throw new InvalidOperationException("Output path is required for file output.");
        }

        var outputContent = options.PreserveAnsiInTextOutput
            ? content
            : StripAnsiCodes(content);

        File.WriteAllText(options.OutputPath, outputContent);
        Console.WriteLine($"ASCII art saved to: {options.OutputPath}");
    }
}
