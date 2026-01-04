using System.CommandLine;
using ImageAsciiArt;
using ImageAsciiArt.Options;

var imageArgument = new Argument<FileInfo>(
    name: "image",
    description: "Path to the image file to convert")
{
    Arity = ArgumentArity.ExactlyOne
};

var modeOption = new Option<RenderMode>(
    aliases: ["--mode", "-m"],
    getDefaultValue: () => RenderMode.HalfBlock,
    description: "Rendering mode: 'halfblock' (high quality) or 'classic' (ASCII characters)");

var charsetOption = new Option<CharacterSet>(
    aliases: ["--charset", "-c"],
    getDefaultValue: () => CharacterSet.Extended,
    description: "Character set for classic mode: standard, extended, simple, blocks");

var customCharsOption = new Option<string?>(
    aliases: ["--chars"],
    description: "Custom character ramp (dark to light) for classic mode");

var widthOption = new Option<int?>(
    aliases: ["--width", "-w"],
    description: "Override output width in characters");

var heightOption = new Option<int?>(
    aliases: ["--height"],
    description: "Override output height in characters");

var outputOption = new Option<string?>(
    aliases: ["--output", "-o"],
    description: "Output to file instead of console");

var htmlOption = new Option<bool>(
    aliases: ["--html"],
    getDefaultValue: () => false,
    description: "Output as HTML file (requires --output)");

var noColorOption = new Option<bool>(
    aliases: ["--no-color"],
    getDefaultValue: () => false,
    description: "Disable colors (grayscale output)");

var invertOption = new Option<bool>(
    aliases: ["--invert", "-i"],
    getDefaultValue: () => false,
    description: "Invert brightness (for light terminals)");

var watchOption = new Option<bool>(
    aliases: ["--watch"],
    getDefaultValue: () => false,
    description: "Live preview mode - re-render on terminal resize");

var preserveAnsiOption = new Option<bool>(
    aliases: ["--preserve-ansi"],
    getDefaultValue: () => false,
    description: "Preserve ANSI codes in text file output");

var rootCommand = new RootCommand("Convert images to colored ASCII art in the terminal")
{
    imageArgument,
    modeOption,
    charsetOption,
    customCharsOption,
    widthOption,
    heightOption,
    outputOption,
    htmlOption,
    noColorOption,
    invertOption,
    watchOption,
    preserveAnsiOption
};

rootCommand.SetHandler(async (context) =>
{
    var image = context.ParseResult.GetValueForArgument(imageArgument);
    var mode = context.ParseResult.GetValueForOption(modeOption);
    var charset = context.ParseResult.GetValueForOption(charsetOption);
    var customChars = context.ParseResult.GetValueForOption(customCharsOption);
    var width = context.ParseResult.GetValueForOption(widthOption);
    var height = context.ParseResult.GetValueForOption(heightOption);
    var output = context.ParseResult.GetValueForOption(outputOption);
    var html = context.ParseResult.GetValueForOption(htmlOption);
    var noColor = context.ParseResult.GetValueForOption(noColorOption);
    var invert = context.ParseResult.GetValueForOption(invertOption);
    var watch = context.ParseResult.GetValueForOption(watchOption);
    var preserveAnsi = context.ParseResult.GetValueForOption(preserveAnsiOption);

    // Determine output format
    var outputFormat = OutputFormat.Console;
    if (!string.IsNullOrEmpty(output))
    {
        outputFormat = html ? OutputFormat.Html : OutputFormat.Text;
    }

    // Validate HTML requires output path
    if (html && string.IsNullOrEmpty(output))
    {
        Console.Error.WriteLine("Error: --html requires --output to specify the output file path.");
        context.ExitCode = 1;
        return;
    }

    // If custom chars provided, use custom charset
    if (!string.IsNullOrEmpty(customChars))
    {
        charset = CharacterSet.Custom;
    }

    var options = new RenderOptions
    {
        ImagePath = image.FullName,
        Mode = mode,
        CharacterSet = charset,
        CustomCharacters = customChars,
        Width = width,
        Height = height,
        OutputFormat = outputFormat,
        OutputPath = output,
        NoColor = noColor,
        Invert = invert,
        Watch = watch,
        PreserveAnsiInTextOutput = preserveAnsi
    };

    try
    {
        var processor = new ImageProcessor();

        if (watch && outputFormat == OutputFormat.Console)
        {
            await RunWatchMode(processor, options, context.GetCancellationToken());
        }
        else
        {
            processor.Process(options);
        }

        context.ExitCode = 0;
    }
    catch (FileNotFoundException ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        context.ExitCode = 1;
    }
    catch (NotSupportedException ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        context.ExitCode = 1;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error processing image: {ex.Message}");
        context.ExitCode = 1;
    }
});

return await rootCommand.InvokeAsync(args);

/// <summary>
/// Runs the application in watch mode, re-rendering on terminal resize.
/// </summary>
static async Task RunWatchMode(ImageProcessor processor, RenderOptions options, CancellationToken cancellationToken)
{
    int lastWidth = 0;
    int lastHeight = 0;

    Console.CursorVisible = false;
    Console.Clear();

    Console.WriteLine("Watch mode: Press Ctrl+C to exit. Resize terminal to re-render.");
    await Task.Delay(1500, cancellationToken);

    try
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            int currentWidth, currentHeight;
            try
            {
                currentWidth = Console.WindowWidth;
                currentHeight = Console.WindowHeight;
            }
            catch
            {
                currentWidth = 120;
                currentHeight = 40;
            }

            if (currentWidth != lastWidth || currentHeight != lastHeight)
            {
                lastWidth = currentWidth;
                lastHeight = currentHeight;

                Console.Clear();
                Console.SetCursorPosition(0, 0);

                // Create new options with current dimensions
                var currentOptions = new RenderOptions
                {
                    ImagePath = options.ImagePath,
                    Mode = options.Mode,
                    CharacterSet = options.CharacterSet,
                    CustomCharacters = options.CustomCharacters,
                    Width = currentWidth - 1,
                    Height = currentHeight - 1,
                    OutputFormat = options.OutputFormat,
                    OutputPath = options.OutputPath,
                    NoColor = options.NoColor,
                    Invert = options.Invert,
                    Watch = options.Watch,
                    PreserveAnsiInTextOutput = options.PreserveAnsiInTextOutput
                };

                processor.Process(currentOptions);
            }

            await Task.Delay(100, cancellationToken);
        }
    }
    catch (OperationCanceledException)
    {
        // Expected when Ctrl+C is pressed
    }
    finally
    {
        Console.CursorVisible = true;
        Console.WriteLine();
        Console.WriteLine("Watch mode ended.");
    }
}
