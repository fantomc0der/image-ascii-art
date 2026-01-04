using ImageAsciiArt.Options;

namespace ImageAsciiArt.Output;

/// <summary>
/// Outputs ASCII art to the console.
/// </summary>
public sealed class ConsoleOutputHandler : IOutputHandler
{
    /// <inheritdoc />
    public void Write(string content, RenderOptions options)
    {
        Console.Write(content);
        Console.WriteLine();
    }
}
