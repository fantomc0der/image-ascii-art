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
    public static void Process(RenderOptions options)
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

        // For block mode, we need double the pixel height since each character represents 2 pixels
        var pixelHeight = options.Mode == RenderMode.Block ? targetHeight * 2 : targetHeight;

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(targetWidth, pixelHeight),
            Sampler = KnownResamplers.Lanczos3,
            Mode = ResizeMode.Stretch // Use exact dimensions we calculated
        }));

        return image;
    }

    /// <summary>
    /// Calculates the target size in characters, maintaining aspect ratio.
    /// Both modes should produce the same character dimensions for the same terminal size.
    /// </summary>
    private static (int Width, int Height) CalculateTargetSize(Image<Rgba32> image, RenderOptions options)
    {
        var (maxWidth, maxHeight) = GetTargetDimensions(options);

        // Characters are typically ~2x taller than wide, so we need to compensate
        const double charAspectRatio = 2.0;
        double imageAspect = (double)image.Width / image.Height;

        // Calculate dimensions that fit within terminal while preserving visual aspect ratio
        // The "visual" width in character cells = charWidth
        // The "visual" height in character cells = charHeight
        // We want: (charWidth) / (charHeight * charAspectRatio) ≈ imageAspect
        // So: charWidth = charHeight * charAspectRatio * imageAspect

        int charWidth, charHeight;

        // Try fitting to max width first
        charWidth = maxWidth;
        charHeight = (int)(charWidth / imageAspect / charAspectRatio);

        // If height exceeds max, fit to height instead
        if (charHeight > maxHeight)
        {
            charHeight = maxHeight;
            charWidth = (int)(charHeight * imageAspect * charAspectRatio);
        }

        return (Math.Max(1, charWidth), Math.Max(1, charHeight));
    }

    /// <summary>
    /// Creates the appropriate renderer based on options.
    /// </summary>
    private static IRenderer CreateRenderer(RenderOptions options) => options.Mode switch
    {
        RenderMode.Block => new BlockRenderer(),
        RenderMode.Classic => new ClassicAsciiRenderer(),
        _ => new BlockRenderer()
    };

    /// <summary>
    /// Creates the appropriate output handler based on options.
    /// </summary>
    private static IOutputHandler CreateOutputHandler(RenderOptions options) => options.OutputFormat switch
    {
        OutputFormat.Console => new ConsoleOutputHandler(),
        OutputFormat.Text => new FileOutputHandler(),
        OutputFormat.Html => new HtmlOutputHandler(),
        _ => new ConsoleOutputHandler()
    };
}
