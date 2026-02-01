# Duality VS Code Extension

This directory contains the source code for the official Duality VS Code Extension (`duality-support`).

## Features

*   **Syntax Highlighting**: Provides full syntax highlighting for CSX files, including:
    *   **React Components**: Teal/Green highlighting for `<MyComponent />` tags.
    *   **HTML Tags**: Blue highlighting for standard HTML tags.
    *   **Attributes**: Correct highlighting for props and attributes.
*   **C# Integration**: Aliases `.csx` files to the `csharp` language ID, enabling:
    *   **IntelliSense**: Full C# autocompletion via the Microsoft C# Dev Kit.
    *   **Go to Definition**: Navigation support for all C# symbols.
    *   **Refactoring**: Standard C# rename and refactoring tools.

## Architecture

The extension uses a **Hybrid Injection Strategy**:

1.  **Language Configuration**: `.csx` extensions are mapped to `csharp` in `package.json`. This ensures the file is treated as C# by default.
2.  **Grammar Injection**: The `csx.tmLanguage.json` grammar is registered as an *injection grammar* targeting `source.cs`.
    *   It uses a "Left Priority" selector (`L:source.cs`) to ensure JSX tags are identified *before* standard C# comparisons (like `<` operators).
    *   This effectively "patches" the C# grammar to understand JSX syntax without breaking existing C# features.

## Installation

### From Source
1.  Navigate to `ide/vscode/`:
    ```bash
    cd ide/vscode
    ```
2.  Install the packages:
    ```bash
    npm install
    ```
3.  Build the VSIX package:
    ```bash
    npx vsce package
    ```
4.  Install the generated `.vsix` file:
    ```bash
    code --install-extension csx-support-0.1.2.vsix
    ```

## Version History

### v0.1.2 (Current Production)
*   **Strategy**: C# Alias + Injection.
*   **Benefits**: Full IntelliSense + JSX Highlighting.
*   **Recommendation**: Use this version for all development.

### v0.1.1 (Archived)
*   **Strategy**: Custom Language ID (`csx`).
*   **Location**: `ide/vscode/archive/v0.1.1/`
*   **Note**: This version establishes `csx` as a distinct language. It does *not* support standard C# IntelliSense but is preserved for potential future development of a dedicated CSX Language Server.
