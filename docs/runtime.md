# Duality Runtime & Reactivity

The Duality Runtime (`Duality.Core`) provides the primitives for fine-grained reactivity and DOM manipulation.

## 1. The Signal Primitive
`Signal<T>` is the core unit of state.
- **Value**: Getting the value records a dependency (in a tracking context) - *Pending implementation of tracking*.
- **Set**: Setting the value notifies all subscribers.
- **Subscribe**: `Subscribe(Action<T>)` registers a callback invoked immediately and on updates.

## 2. Server Actions
Instead of client-side hooks, Duality primarily uses Server Actions for interactivity and state management.
- **`[Server]` Attribute**: Marks static methods that run on the server.
- **RPC Integration**: Automatically creates API endpoints and binds UI events using HTMX.
- **See**: [Server Actions (RPC)](rpc.md) for detailed usage.

## 3. Compiler-Driven Subscriptions
The Compiler detects `{expression}` blocks in the CSX.
- **Text Content**: If `{signal}` is found, it emits `Reactivity.Bind(signal, val => NativeDom.SetText(el, val))`.
- **Logic**: `Reactivity.Bind` detects if the object implements `ISignal`. If so, it subscribes. If not (static value), it runs once.

## 4. DOM Interop
`NativeDom.cs` handles browser interaction.
- **Console Mode**: Prints operations to StdOut (Stub).
- **Wasm Mode**: Uses `[JSImport]` to call browser APIs via `System.Runtime.InteropServices.JavaScript`.
