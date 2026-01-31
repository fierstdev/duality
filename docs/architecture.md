# CSX Architecture Overview

## Solution Structure
The solution includes three primary projects designed to separate the compiler logic from the runtime library.

### 1. CSX.Generator
- **Path**: `CSX.Generator/CSX.Generator.csproj`
- **Framework**: `netstandard2.0` (Required for Roslyn compatibility)
- **Role**: The Source Generator (Compiler).
- **Responsibilities**:
  - Reading `.csx` files passed as `AdditionalFiles`.
  - Parsing the custom CSX syntax (Components, XML tags).
  - Generating standard C# source code that is added to the compilation.
- **Dependencies**: `Microsoft.CodeAnalysis.CSharp`, `Microsoft.CodeAnalysis.Analyzers`.

### 2. CSX.Runtime
- **Path**: `CSX.Runtime/CSX.Runtime.csproj`
- **Framework**: `net8.0`
- **Role**: The Runtime Library.
- **Responsibilities**:
  - Provides the base types and primitives used by the generated code.
  - Implements `Signal<T>`, `UseState`, `UseMemo`, etc.
  - Handles DOM interop (via `System.Runtime.InteropServices.JavaScript`).

### 3. CSX.Playground
- **Path**: `CSX.Playground/CSX.Playground.csproj`
- **Framework**: `net8.0` (Console App)
- **Role**: Testbed and Demo Application.
- **Responsibilities**:
  - Consumes both the Generator (as an Analyzer) and the Runtime (as a Library).
  - Contains `.csx` files to verify compilation.
