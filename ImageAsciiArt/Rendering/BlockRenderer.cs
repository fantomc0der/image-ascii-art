using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ImageAsciiArt.Options;

namespace ImageAsciiArt.Rendering;

/// <summary>
/// Renders images using Unicode block characters (▀) with foreground and background colors.
/// This effectively doubles the vertical resolution compared to classic ASCII rendering.
/// </summary>
public sealed class BlockRenderer : IRenderer
{
    /// <summary>
    /// Upper half block character - foreground fills top half, background fills bottom half.
    /// </summary>
    private const char UpperHalfBlock = '▀';

    /// <summary>
    /// Lower half block character - foreground fills bottom half.
    /// </summary>
    private const char LowerHalfBlock = '▄';

    /// <summary>
    /// Full block character - used when both pixels are the same color.
    /// </summary>
    private const char FullBlock = '█';

    /// <summary>
    /// Space character - used when we only need background color.
    /// </summary>
    private const char Space = ' ';

    /// <inheritdoc />
    public string Render(Image<Rgba32> image, RenderOptions options)
    {
        // Each character represents 2 vertical pixels
        int outputHeight = (image.Height + 1) / 2;
        var output = new StringBuilder(image.Width * outputHeight * 40);

        // Reset any previous formatting
        if (!options.NoColor)
        {
            output.Append("\x1b[0m");
        }

        for (int charY = 0; charY < outputHeight; charY++)
        {
            int pixelY1 = charY * 2;        // Top pixel
            int pixelY2 = charY * 2 + 1;    // Bottom pixel

            for (int x = 0; x < image.Width; x++)
            {
                var topPixel = image[x, pixelY1];

                // Handle case where bottom pixel is out of bounds (odd height images)
                var bottomPixel = pixelY2 < image.Height
                    ? image[x, pixelY2]
                    : topPixel;

                if (options.NoColor)
                {
                    // In no-color mode, use brightness to determine character
                    var topBright = CalculateBrightness(topPixel);
                    var bottomBright = CalculateBrightness(bottomPixel);
                    output.Append(GetGrayscaleChar(topBright, bottomBright, options.Invert));
                }
                else
                {
                    // Use upper half block with:
                    // - Foreground color = top pixel
                    // - Background color = bottom pixel
                    // Format: \x1b[38;2;R;G;Bm (foreground) \x1b[48;2;R;G;Bm (background)
                    output.Append($"\x1b[38;2;{topPixel.R};{topPixel.G};{topPixel.B}m");
                    output.Append($"\x1b[48;2;{bottomPixel.R};{bottomPixel.G};{bottomPixel.B}m");
                    output.Append(UpperHalfBlock);
                }
            }

            // Reset colors at end of line
            if (!options.NoColor)
            {
                output.Append("\x1b[0m");
            }

            if (charY < outputHeight - 1)
            {
                output.AppendLine();
            }
        }

        if (!options.NoColor)
        {
            output.Append("\x1b[0m");
        }

        return output.ToString();
    }

    /// <summary>
    /// Calculates pixel brightness using standard luminance formula.
    /// </summary>
    private static double CalculateBrightness(Rgba32 pixel)
    {
        return (0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B) / 255.0;
    }

    /// <summary>
    /// Gets a grayscale character based on top and bottom pixel brightness.
    /// </summary>
    private static char GetGrayscaleChar(double topBright, double bottomBright, bool invert)
    {
        if (invert)
        {
            topBright = 1.0 - topBright;
            bottomBright = 1.0 - bottomBright;
        }

        // Threshold for considering a pixel "light" vs "dark"
        const double threshold = 0.5;

        bool topLight = topBright > threshold;
        bool bottomLight = bottomBright > threshold;

        return (topLight, bottomLight) switch
        {
            (true, true) => ' ',           // Both light
            (false, false) => FullBlock,   // Both dark
            (false, true) => UpperHalfBlock,  // Top dark, bottom light
            (true, false) => LowerHalfBlock,  // Top light, bottom dark
        };
    }
}
