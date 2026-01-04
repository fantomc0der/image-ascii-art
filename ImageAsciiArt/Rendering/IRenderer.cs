using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ImageAsciiArt.Options;

namespace ImageAsciiArt.Rendering;

/// <summary>
/// Interface for ASCII art renderers.
/// </summary>
public interface IRenderer
{
    /// <summary>
    /// Renders an image to ASCII art.
    /// </summary>
    /// <param name="image">The source image.</param>
    /// <param name="options">Rendering options.</param>
    /// <returns>The rendered ASCII art with ANSI color codes.</returns>
    string Render(Image<Rgba32> image, RenderOptions options);
}
