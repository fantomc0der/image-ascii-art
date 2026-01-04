# Claude Instructions for Image ASCII Art

## Project Overview

This is a .NET 10 console application that converts images to colored ASCII art in the terminal.

## Testing Guidelines

When testing the application, always use the following parameters:

- **Input image**: `sample.jpg` (located in the repository root)
- **HTML output**: `output.html` (located in the repository root)
- **Text output**: `output.txt` (located in the repository root)

### Example Test Commands

```bash
# Test half-block mode (default, highest quality)
dotnet run --project ImageAsciiArt -- sample.jpg --width 80 --height 40

# Test classic ASCII mode
dotnet run --project ImageAsciiArt -- sample.jpg --mode classic --width 80 --height 40

# Test HTML output
dotnet run --project ImageAsciiArt -- sample.jpg --output output.html --html

# Test text file output
dotnet run --project ImageAsciiArt -- sample.jpg --output output.txt

# Test grayscale mode
dotnet run --project ImageAsciiArt -- sample.jpg --no-color --width 60 --height 20
```

## Build Commands

```bash
# Build the project
dotnet build ImageAsciiArt

# Run with arguments
dotnet run --project ImageAsciiArt -- <image-path> [options]
```

## Project Structure

- `ImageAsciiArt/Program.cs` - CLI entry point
- `ImageAsciiArt/ImageProcessor.cs` - Image loading and processing
- `ImageAsciiArt/Options/` - Configuration options and enums
- `ImageAsciiArt/Rendering/` - Renderer implementations
- `ImageAsciiArt/Output/` - Output handler implementations

## Documentation Guidelines

When updating this file or other documentation:
- List namespaces/folders and their purpose, not individual classes
- Individual class listings fall out of sync and add maintenance burden
- Let the code be the source of truth for class-level details
