using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ImageAsciiArt.Options;
using ImageAsciiArt.Output;
using ImageAsciiArt.Rendering;

namespace ImageAsciiArt;

/// <summary>
/// Handles image loading, processing, and ASCII art generation.
/// </summary>
public sealed class ImageProcessor
{
    private static readonly string[] SupportedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".tiff", ".tif"];

    /// <summary>
    /// Processes an image and generates ASCII art.
    /// </summary>
    public void Process(RenderOptions options)
    {
        ValidateInput(options);

        using var image = LoadAndResizeImage(options);
        var renderer = CreateRenderer(options);
        var output = CreateOutputHandler(options);

        var asciiArt = renderer.Render(image, options);
        output.Write(asciiArt, options);
    }

    /// <summary>
    /// Gets the target dimensions for rendering.
    /// </summary>
    public static (int Width, int Height) GetTargetDimensions(RenderOptions options)
    {
        int terminalWidth, terminalHeight;

        if (options.Width.HasValue && options.Height.HasValue)
        {
            return (options.Width.Value, options.Height.Value);
        }

        try
        {
            terminalWidth = Console.WindowWidth - 1;
            terminalHeight = Console.WindowHeight - 1;
        }
        catch
        {
            terminalWidth = 120;
            terminalHeight = 40;
        }

        return (
            options.Width ?? terminalWidth,
            options.Height ?? terminalHeight
        );
    }

    /// <summary>
    /// Validates the input options.
    /// </summary>
    private static void ValidateInput(RenderOptions options)
    {
        if (!File.Exists(options.ImagePath))
        {
            throw new FileNotFoundException($"Image file not found: {options.ImagePath}");
        }

        var extension = Path.GetExtension(options.ImagePath).ToLowerInvariant();
        if (!SupportedExtensions.Contains(extension))
        {
            throw new NotSupportedException(
                $"Unsupported image format: {extension}. Supported formats: {string.Join(", ", SupportedExtensions)}");
        }
    }

    /// <summary>
    /// Loads and resizes the image to fit the target dimensions.
    /// </summary>
    private static Image<Rgba32> LoadAndResizeImage(RenderOptions options)
    {
        var image = Image.Load<Rgba32>(options.ImagePath);
        var (targetWidth, targetHeight) = CalculateTargetSize(image, options);

        // For half-block mode, we need double the height since each character represents 2 pixels
        var resizeHeight = options.Mode == RenderMode.HalfBlock ? targetHeight * 2 : targetHeight;

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(targetWidth, resizeHeight),
            Sampler = KnownResamplers.Lanczos3,
            Mode = ResizeMode.Max
        }));

        return image;
    }

    /// <summary>
    /// Calculates the target size maintaining aspect ratio.
    /// </summary>
    private static (int Width, int Height) CalculateTargetSize(Image<Rgba32> image, RenderOptions options)
    {
        var (maxWidth, maxHeight) = GetTargetDimensions(options);

        // Characters are typically ~2x taller than wide
        const double charAspectRatio = 2.0;
        double imageAspect = (double)image.Width / image.Height;

        // Adjust max height for character aspect ratio
        int adjustedMaxHeight = (int)(maxHeight * charAspectRatio);

        int targetWidth, targetHeight;

        if (image.Width * adjustedMaxHeight > image.Height * maxWidth)
        {
            // Width is the limiting factor
            targetWidth = maxWidth;
            targetHeight = (int)(maxWidth / imageAspect / charAspectRatio);
        }
        else
        {
            // Height is the limiting factor
            targetHeight = maxHeight;
            targetWidth = (int)(maxHeight * imageAspect * charAspectRatio);
        }

        return (Math.Max(1, targetWidth), Math.Max(1, targetHeight));
    }

    /// <summary>
    /// Creates the appropriate renderer based on options.
    /// </summary>
    private static IRenderer CreateRenderer(RenderOptions options)
    {
        return options.Mode switch
        {
            RenderMode.HalfBlock => new HalfBlockRenderer(),
            RenderMode.Classic => new ClassicAsciiRenderer(),
            _ => new HalfBlockRenderer()
        };
    }

    /// <summary>
    /// Creates the appropriate output handler based on options.
    /// </summary>
    private static IOutputHandler CreateOutputHandler(RenderOptions options)
    {
        return options.OutputFormat switch
        {
            OutputFormat.Console => new ConsoleOutputHandler(),
            OutputFormat.Text => new FileOutputHandler(),
            OutputFormat.Html => new HtmlOutputHandler(),
            _ => new ConsoleOutputHandler()
        };
    }
}
