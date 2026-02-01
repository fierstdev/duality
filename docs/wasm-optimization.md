# Wasm & NativeAOT optimization

The `Duality.Wasm` project is configured for **NativeAOT** targeting `browser-wasm` with the experimental `wasm-gc` instruction set.

## Project Configuration (`.csproj`)

```xml
<PropertyGroup>
    <RuntimeIdentifier>browser-wasm</RuntimeIdentifier>
    <PublishAot>true</PublishAot>
    <IlcInstructionSet>wasm-gc</IlcInstructionSet>
</PropertyGroup>
```

- **RuntimeIdentifier**: Targets the Browser Wasm environment.
- **PublishAot**: Enables the NativeAOT compiler (ILC).
- **IlcInstructionSet**: Tells the AOT compiler to use WasmGC instructions (struct types, array types) instead of linear memory emulation for managed objects.

## Optimization Flags

To achieve the "Zero-Overhead" goal, we aggressively strip features:

| Flag | Effect |
|------|--------|
| `<InvariantGlobalization>true</InvariantGlobalization>` | Removes ICU/Timezone data (~1-2MB savings). |
| `<StackTraceSupport>false</StackTraceSupport>` | Removes stack trace strings. |
| `<DebuggerSupport>false</DebuggerSupport>` | Removes debugging symbols. |
| `<EventSourceSupport>false</EventSourceSupport>` | Removes diagnostic eventing. |

## Building (User Action)
Since this requires the `wasm-tools` workload:

```bash
dotnet workload install wasm-tools
dotnet publish -c Release -r browser-wasm
```

The output `publish/` folder will contain the highly optimized `.wasm` file.
