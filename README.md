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
- PowerShell (for run.ps1 convenience script)
- A terminal that supports Unicode and ANSI 24-bit color (Windows Terminal, PowerShell, most Linux/macOS terminals)

## Installation

```powershell
dotnet build ImageAsciiArt
```

## Usage

### Basic Usage

```powershell
# Show help
.\run.ps1 -?

# Default: Half-block mode with auto-detected terminal size
.\run.ps1 <image-path>

# Example
.\run.ps1 C:\photos\cat.jpg
```

### Rendering Modes

```powershell
# Half-block mode (default, highest quality)
.\run.ps1 image.jpg --mode halfblock

# Classic ASCII mode
.\run.ps1 image.jpg --mode classic
```

### Character Sets (Classic Mode)

```powershell
# Extended character set (70 characters, finest gradation)
.\run.ps1 image.jpg -m classic -c extended

# Standard character set (10 characters)
.\run.ps1 image.jpg -m classic -c standard

# Simple character set (5 characters)
.\run.ps1 image.jpg -m classic -c simple

# Block characters only
.\run.ps1 image.jpg -m classic -c blocks

# Custom character ramp (dark to light)
.\run.ps1 image.jpg -m classic --chars "@#$%^. "
```

### Output Options

```powershell
# Save to text file (without ANSI codes)
.\run.ps1 image.jpg -o output.txt

# Save to text file with ANSI codes preserved
.\run.ps1 image.jpg -o output.txt --preserve-ansi

# Generate HTML file
.\run.ps1 image.jpg -o output.html --html
```

### Display Options

```powershell
# Custom dimensions
.\run.ps1 image.jpg --width 80 --height 40

# Grayscale output (no colors)
.\run.ps1 image.jpg --no-color

# Invert brightness (for light terminals)
.\run.ps1 image.jpg --invert

# Live preview mode (re-renders on terminal resize)
.\run.ps1 image.jpg --watch
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
| `--help` | `-?`, `-h` | Show help and usage information |

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

```powershell
# High-quality half-block rendering
.\run.ps1 photo.png

# Classic ASCII with extended character set
.\run.ps1 photo.png -m classic -c extended

# Generate HTML for sharing
.\run.ps1 photo.png -o art.html --html

# Watch mode for presentations
.\run.ps1 photo.png --watch

# Retro look with block characters
.\run.ps1 photo.png -m classic -c blocks
```
