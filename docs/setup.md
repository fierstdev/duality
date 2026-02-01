# Getting Started & Setup

## Prerequisites
- .NET 8.0 SDK

## Building the Project
To build the entire solution, run:

```bash
dotnet build
```

This will:
1. Build the generator (`Duality.Compiler`).
2. Build the runtime (`Duality.Core`).
3. specific: The `Duality.Playground` project will run the `Duality.Compiler` during its build process.

## Build Configuration
We use a `Directory.Build.props` file in the root to ensure consistent settings across all projects:
- **ImplicitUsings**: enabled
- **Nullable**: enabled
- **TreatWarningsAsErrors**: true

## Adding Duality to a New Project
To use Duality in a new project (like `Duality.Playground`), you must add references to both the Runtime and the Generator. The Generator must be referenced as an **Analyzer**.

```xml
<ItemGroup>
  <ProjectReference Include="..\Duality.Core\Duality.Core.csproj" />
  <ProjectReference Include="..\Duality.Compiler\Duality.Compiler.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
</ItemGroup>
```
