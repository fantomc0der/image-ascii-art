using System.Text;
using System.Text.RegularExpressions;

namespace ImageAsciiArt.Output;

/// <summary>
/// Outputs ASCII art to an HTML file for web viewing and sharing.
/// </summary>
public sealed partial class HtmlOutputHandler : IOutputHandler
{
    /// <inheritdoc />
    public void Write(string content, RenderOptions options)
    {
        if (string.IsNullOrEmpty(options.OutputPath))
        {
            throw new InvalidOperationException("Output path is required for HTML output.");
        }

        var html = GenerateHtml(content, options);
        File.WriteAllText(options.OutputPath, html);
        Console.WriteLine($"HTML file saved to: {options.OutputPath}");
    }

    /// <summary>
    /// Generates a complete HTML document from the ASCII art content.
    /// </summary>
    private static string GenerateHtml(string content, RenderOptions options)
    {
        var imageName = Path.GetFileName(options.ImagePath);
        var htmlContent = ConvertAnsiToHtml(content);

        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>ASCII Art - {{HtmlEncode(imageName)}}</title>
                <style>
                    body {
                        background-color: #1a1a1a;
                        margin: 0;
                        padding: 20px;
                        display: flex;
                        justify-content: center;
                        align-items: flex-start;
                        min-height: 100vh;
                    }
                    pre {
                        font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
                        font-size: 10px;
                        line-height: 1.0;
                        letter-spacing: 0;
                        margin: 0;
                        white-space: pre;
                    }
                    .container {
                        background-color: #0d0d0d;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
                        overflow: auto;
                        max-width: 100%;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <pre>{{htmlContent}}</pre>
                </div>
            </body>
            </html>
            """;
    }

    /// <summary>
    /// Converts ANSI color codes to HTML span elements with inline styles.
    /// </summary>
    private static string ConvertAnsiToHtml(string input)
    {
        var output = new StringBuilder();
        var currentFg = (R: 255, G: 255, B: 255);
        var currentBg = (R: 0, G: 0, B: 0);
        bool hasFg = false;
        bool hasBg = false;
        bool inSpan = false;

        int i = 0;
        while (i < input.Length)
        {
            // Check for ANSI escape sequence
            if (i < input.Length - 1 && input[i] == '\x1b' && input[i + 1] == '[')
            {
                int endIndex = input.IndexOf('m', i);
                if (endIndex != -1)
                {
                    var sequence = input.Substring(i + 2, endIndex - i - 2);

                    if (sequence == "0")
                    {
                        // Reset
                        if (inSpan)
                        {
                            output.Append("</span>");
                            inSpan = false;
                        }
                        hasFg = false;
                        hasBg = false;
                    }
                    else if (sequence.StartsWith("38;2;"))
                    {
                        // Foreground color
                        var parts = sequence.Split(';');
                        if (parts.Length >= 5)
                        {
                            currentFg = (
                                int.Parse(parts[2]),
                                int.Parse(parts[3]),
                                int.Parse(parts[4])
                            );
                            hasFg = true;
                        }
                    }
                    else if (sequence.StartsWith("48;2;"))
                    {
                        // Background color
                        var parts = sequence.Split(';');
                        if (parts.Length >= 5)
                        {
                            currentBg = (
                                int.Parse(parts[2]),
                                int.Parse(parts[3]),
                                int.Parse(parts[4])
                            );
                            hasBg = true;
                        }
                    }

                    i = endIndex + 1;
                    continue;
                }
            }

            // Regular character
            char c = input[i];

            if (c == '\n' || c == '\r')
            {
                if (inSpan)
                {
                    output.Append("</span>");
                    inSpan = false;
                }
                output.Append(c);
            }
            else
            {
                // Start a new span if we have colors
                if ((hasFg || hasBg) && !inSpan)
                {
                    output.Append("<span style=\"");
                    if (hasFg)
                    {
                        output.Append($"color:rgb({currentFg.R},{currentFg.G},{currentFg.B});");
                    }
                    if (hasBg)
                    {
                        output.Append($"background-color:rgb({currentBg.R},{currentBg.G},{currentBg.B});");
                    }
                    output.Append("\">");
                    inSpan = true;
                }

                // HTML encode special characters
                output.Append(c switch
                {
                    '<' => "&lt;",
                    '>' => "&gt;",
                    '&' => "&amp;",
                    '"' => "&quot;",
                    _ => c.ToString()
                });
            }

            i++;
        }

        if (inSpan)
        {
            output.Append("</span>");
        }

        return output.ToString();
    }

    /// <summary>
    /// HTML encodes a string.
    /// </summary>
    private static string HtmlEncode(string input)
    {
        return input
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;");
    }
}
