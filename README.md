# Image ASCII Art

A .NET 10 console application that converts images to high-quality colored ASCII art in the terminal.

## Features

- **Half-block rendering** - Uses Unicode half-block characters (▀) for double vertical resolution
- **Classic ASCII rendering** - Traditional ASCII character density mapping
- **24-bit true color** - Full RGB color support using ANSI escape sequences
- **Multiple character sets** - Standard, extended, simple, blocks, or custom
- **Automatic scaling** - Fits output to current terminal dimensions
- **Aspect ratio preservation** - Compensates for character height/width ratio
- **HTML export** - Generate shareable HTML files
- **Text file export** - Save ASCII art to text files
- **Live preview mode** - Re-renders automatically when terminal is resized
- **Grayscale mode** - Disable colors for terminals without color support
- **Invert mode** - For light-background terminals

## Requirements

- .NET 10 SDK
- A terminal that supports Unicode and ANSI 24-bit color (Windows Terminal, PowerShell, most Linux/macOS terminals)

## Installation

```bash
dotnet build ImageAsciiArt
```

## Usage

### Basic Usage

```bash
# Default: Half-block mode with auto-detected terminal size
dotnet run --project ImageAsciiArt -- <image-path>

# Example
dotnet run --project ImageAsciiArt -- C:\photos\cat.jpg
```

### Rendering Modes

```bash
# Half-block mode (default, highest quality)
dotnet run --project ImageAsciiArt -- image.jpg --mode halfblock

# Classic ASCII mode
dotnet run --project ImageAsciiArt -- image.jpg --mode classic
```

### Character Sets (Classic Mode)

```bash
# Extended character set (70 characters, finest gradation)
dotnet run --project ImageAsciiArt -- image.jpg -m classic -c extended

# Standard character set (10 characters)
dotnet run --project ImageAsciiArt -- image.jpg -m classic -c standard

# Simple character set (5 characters)
dotnet run --project ImageAsciiArt -- image.jpg -m classic -c simple

# Block characters only
dotnet run --project ImageAsciiArt -- image.jpg -m classic -c blocks

# Custom character ramp (dark to light)
dotnet run --project ImageAsciiArt -- image.jpg -m classic --chars "@#$%^. "
```

### Output Options

```bash
# Save to text file (without ANSI codes)
dotnet run --project ImageAsciiArt -- image.jpg -o output.txt

# Save to text file with ANSI codes preserved
dotnet run --project ImageAsciiArt -- image.jpg -o output.txt --preserve-ansi

# Generate HTML file
dotnet run --project ImageAsciiArt -- image.jpg -o output.html --html
```

### Display Options

```bash
# Custom dimensions
dotnet run --project ImageAsciiArt -- image.jpg --width 80 --height 40

# Grayscale output (no colors)
dotnet run --project ImageAsciiArt -- image.jpg --no-color

# Invert brightness (for light terminals)
dotnet run --project ImageAsciiArt -- image.jpg --invert

# Live preview mode (re-renders on terminal resize)
dotnet run --project ImageAsciiArt -- image.jpg --watch
```

## Command-Line Options

| Option | Short | Description |
|--------|-------|-------------|
| `--mode` | `-m` | Rendering mode: `halfblock` (default) or `classic` |
| `--charset` | `-c` | Character set: `standard`, `extended`, `simple`, `blocks` |
| `--chars` | | Custom character ramp (dark to light) |
| `--width` | `-w` | Override output width in characters |
| `--height` | | Override output height in characters |
| `--output` | `-o` | Output to file instead of console |
| `--html` | | Output as HTML file (requires `--output`) |
| `--no-color` | | Disable colors (grayscale output) |
| `--invert` | `-i` | Invert brightness (for light terminals) |
| `--watch` | | Live preview mode with auto re-render |
| `--preserve-ansi` | | Keep ANSI codes in text file output |

## Supported Image Formats

- JPG/JPEG
- PNG
- GIF
- BMP
- WebP
- TIFF

## How It Works

### Half-Block Mode (Default)
1. Loads the image using ImageSharp with Lanczos3 resampling
2. Scales to fit terminal, accounting for character aspect ratio
3. Pairs vertical pixels: top pixel = foreground color, bottom pixel = background color
4. Uses the upper half-block character (▀) with dual colors
5. Achieves double vertical resolution compared to classic mode

### Classic Mode
1. Loads and scales the image
2. Maps each pixel's brightness to an ASCII character from the selected ramp
3. Applies the pixel's RGB color as ANSI foreground color
4. Denser characters represent darker areas

## Examples

```bash
# High-quality half-block rendering
dotnet run --project ImageAsciiArt -- photo.png

# Classic ASCII with extended character set
dotnet run --project ImageAsciiArt -- photo.png -m classic -c extended

# Generate HTML for sharing
dotnet run --project ImageAsciiArt -- photo.png -o art.html --html

# Watch mode for presentations
dotnet run --project ImageAsciiArt -- photo.png --watch

# Retro look with block characters
dotnet run --project ImageAsciiArt -- photo.png -m classic -c blocks
```
