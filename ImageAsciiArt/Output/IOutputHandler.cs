using ImageAsciiArt.Options;

namespace ImageAsciiArt.Output;

/// <summary>
/// Interface for handling ASCII art output.
/// </summary>
public interface IOutputHandler
{
    /// <summary>
    /// Writes the rendered ASCII art to the output destination.
    /// </summary>
    /// <param name="content">The rendered ASCII art content.</param>
    /// <param name="options">The render options used.</param>
    void Write(string content, RenderOptions options);
}
