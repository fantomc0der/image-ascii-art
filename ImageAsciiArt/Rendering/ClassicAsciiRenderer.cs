using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ImageAsciiArt.Options;

namespace ImageAsciiArt.Rendering;

/// <summary>
/// Renders images using classic ASCII characters with foreground colors.
/// </summary>
public sealed class ClassicAsciiRenderer : IRenderer
{
    /// <inheritdoc />
    public string Render(Image<Rgba32> image, RenderOptions options)
    {
        var characterRamp = options.GetCharacterRamp();
        var output = new StringBuilder(image.Width * image.Height * 20);

        // Reset any previous formatting
        if (!options.NoColor)
        {
            output.Append("\x1b[0m");
        }

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var pixel = image[x, y];
                var brightness = CalculateBrightness(pixel);
                var charIndex = (int)(brightness * (characterRamp.Length - 1));
                charIndex = Math.Clamp(charIndex, 0, characterRamp.Length - 1);

                // Light areas get sparse characters (end of ramp)
                var asciiChar = characterRamp[characterRamp.Length - 1 - charIndex];

                if (options.NoColor)
                {
                    output.Append(asciiChar);
                }
                else
                {
                    // ANSI 24-bit true color: \x1b[38;2;R;G;Bm
                    output.Append($"\x1b[38;2;{pixel.R};{pixel.G};{pixel.B}m{asciiChar}");
                }
            }

            if (!options.NoColor)
            {
                output.Append("\x1b[0m");
            }

            if (y < image.Height - 1)
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
    private static double CalculateBrightness(Rgba32 pixel) => (0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B) / 255.0;
}
