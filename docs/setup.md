# Getting Started & Setup

## Prerequisites
- .NET 8.0 SDK

## Building the Project
To build the entire solution, run:

```bash
dotnet build
```

This will:
1. Build the generator (`CSX.Generator`).
2. Build the runtime (`CSX.Runtime`).
3. specific: The `CSX.Playground` project will run the `CSX.Generator` during its build process.

## Build Configuration
We use a `Directory.Build.props` file in the root to ensure consistent settings across all projects:
- **ImplicitUsings**: enabled
- **Nullable**: enabled
- **TreatWarningsAsErrors**: true

## Adding CSX to a New Project
To use CSX in a new project (like `CSX.Playground`), you must add references to both the Runtime and the Generator. The Generator must be referenced as an **Analyzer**.

```xml
<ItemGroup>
  <ProjectReference Include="..\CSX.Runtime\CSX.Runtime.csproj" />
  <ProjectReference Include="..\CSX.Generator\CSX.Generator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
</ItemGroup>
```
