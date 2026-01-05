<!-- OPENSPEC:START -->
# OpenSpec Instructions

These instructions are for AI assistants working in this project.

Always open `@/openspec/AGENTS.md` when the request:
- Mentions planning or proposals (words like proposal, spec, change, plan)
- Introduces new capabilities, breaking changes, architecture shifts, or big performance/security work
- Sounds ambiguous and you need the authoritative spec before coding

Use `@/openspec/AGENTS.md` to learn:
- How to create and apply change proposals
- Spec format and conventions
- Project structure and guidelines

Keep this managed block so 'openspec update' can refresh the instructions.

<!-- OPENSPEC:END -->

# Claude Instructions for Image ASCII Art

## Project Overview

This is a .NET 10 console application that converts images to colored ASCII art in the terminal.

## Testing Guidelines

When testing the application, always use the following parameters:

- **Input image**: `sample.jpg` (located in the repository root)
- **HTML output**: `output.html` (located in the repository root)
- **Text output**: `output.txt` (located in the repository root)

### Example Test Commands

```powershell
# Test block mode (default, highest quality)
.\run.ps1 sample.jpg --width 80 --height 40

# Test classic ASCII mode
.\run.ps1 sample.jpg --mode classic --width 80 --height 40

# Test HTML output
.\run.ps1 sample.jpg --output output.html --html

# Test text file output
.\run.ps1 sample.jpg --output output.txt

# Test grayscale mode
.\run.ps1 sample.jpg --no-color --width 60 --height 20

# Show help
.\run.ps1 -?
```

## Build Commands

```powershell
# Build the project
dotnet build ImageAsciiArt

# Run with arguments (use run.ps1 for convenience)
.\run.ps1 <image-path> [options]
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
