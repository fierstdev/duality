# Duality Architecture Overview

## Solution Structure
The solution includes three primary projects designed to separate the compiler logic from the runtime library.

### 1. Duality.Compiler (formerly CSX.Generator)
- **Path**: `Duality.Compiler/Duality.Compiler.csproj`
- **Framework**: `netstandard2.0` (Required for Roslyn compatibility)
- **Role**: The Source Generator (Compiler).
- **Responsibilities**:
  - Reading `.csx` files passed as `AdditionalFiles`.
  - Parsing the custom CSX syntax (Components, XML tags).
  - Generating standard C# source code that is added to the compilation.
- **Dependencies**: `Microsoft.CodeAnalysis.CSharp`, `Microsoft.CodeAnalysis.Analyzers`.

### 2. Duality.Core (formerly CSX.Runtime)
- **Path**: `Duality.Core/Duality.Core.csproj`
- **Framework**: `net8.0`
- **Role**: The Runtime Library.
- **Responsibilities**:
  - Provides the base types and primitives used by the generated code.
  - Implements `Signal<T>`, `Server Actions`, etc.
  - Handles DOM interop (via `System.Runtime.InteropServices.JavaScript`).

### 3. Duality.Playground
- **Path**: `Duality.Playground/Duality.Playground.csproj` (or similar)
- **Framework**: `net8.0` (Console App)
- **Role**: Testbed and Demo Application.
- **Responsibilities**:
  - Consumes both the Compiler (as an Analyzer) and the Core (as a Library).
  - Contains `.csx` files to verify compilation.
