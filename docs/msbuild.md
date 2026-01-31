# MSBuild Configuration

To make the Roslyn Source Generator "see" our custom `.csx` files, we utilize specific MSBuild configurations.

## 1. AdditionalFiles
Roslyn Source Generators do not automatically see non-C# files. We must explicitly register `.csx` files as `AdditionalFiles`.

In `CSX.Playground.csproj`:
```xml
<ItemGroup>
  <None Remove="**/*.csx" />
  <AdditionalFiles Include="**/*.csx" />
</ItemGroup>
```
- `None Remove`: Prevents them from being treated as standard miscellaneous files.
- `AdditionalFiles Include`: Passes them to the generator's `context.AdditionalFiles` collection.

## 2. Generator Reference
The generator is referenced differently than a standard library.

```xml
<ProjectReference Include="..\CSX.Generator\CSX.Generator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
```
- `OutputItemType="Analyzer"`: Tells MSBuild to load the DLL into the Roslyn compiler process.
- `ReferenceOutputAssembly="false"`: Prevents the DLL from being added as a runtime reference (we don't need the generator types at runtime, only its output).

## 3. Generator Project Settings
The `CSX.Generator` project requires specific settings to function as a Roslyn component.

In `CSX.Generator.csproj`:
```xml
<PropertyGroup>
  <TargetFramework>netstandard2.0</TargetFramework>
  <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
  <IsRoslynComponent>true</IsRoslynComponent>
</PropertyGroup>
```
- `netstandard2.0`: Required for Roslyn generators.
- `EnforceExtendedAnalyzerRules`: Enables specific diagnostics for generator authors.
