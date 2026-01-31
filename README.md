# CSX Framework

**The "TSX for C#" WebAssembly Framework.**

CSX is a zero-overhead, fine-grained reactive framework for building web applications using C# and WebAssembly. It combines the developer experience of JSX (CSX) with the performance of .NET 9 NativeAOT and WasmGC.

## Key Features

- **.csx File Format**: Write C# components with HTML-like syntax.
- **Zero-Virtual DOM**: Compiles directly to imperative `NativeDom` calls.
- **Fine-Grained Reactivity**: Signals (`Signal<T>`) drive granular updates.
- **Instant Resumability**: Static extraction of event handlers enables "Lazy Interaction".
- **Zero-Cost Abstractions**: Components vanish at compile time; only the render logic remains.
- **NativeAOT + WasmGC**: Targets the smallest possible Wasm footprint.

## Project Structure

- **CSX.Generator**: The Roslyn Source Generator (Compiler).
- **CSX.Runtime**: The core library (Signals, Hooks, DOM Interop).
- **CSX.Wasm**: The WebAssembly application host.
- **CSX.CLI**: Command-line tool for managing projects.
- **CSX.Playground**: Console verification tool.

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- `wasm-tools` workload (`dotnet workload install wasm-tools`)

### Running the Example

1. **Navigate to the Wasm project**:
   ```bash
   cd CSX.Wasm
   ```

2. **Build and Run**:
   ```bash
   dotnet publish -c Release
   # The output in /bin/Release/net9.0/browser-wasm/publish/wwwroot can be served.
   ```

   *Note: Ensure you include the generated `bootloader.js` in your HTML index.*

## Architecture

CSX uses a **Compile-Time** approach. The `.csx` files are parsed by a Source Generator, which emits optimized C# code.

```csharp
// Source (.csx)
return <div onclick={ setCount(count.Value + 1) }>{count}</div>;

// Generated Output (Simplfied)
var el = NativeDom.CreateElement("div");
Reactivity.Bind(count, v => NativeDom.SetText(el, v)); // Dynamic text
// Static Handler (no closures!)
public static void Handler_123(object state) { ... }
```

## Contributing

See `docs/` for detailed architecture notes.::contentReference[oaicite:0]{index=0}::
