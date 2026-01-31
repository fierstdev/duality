# CSX Runtime & Reactivity

The CSX Runtime (`CSX.Runtime`) provides the primitives for fine-grained reactivity and DOM manipulation.

## 1. The Signal Primitive
`Signal<T>` is the core unit of state.
- **Value**: Getting the value records a dependency (in a tracking context) - *Pending implementation of tracking*.
- **Set**: Setting the value notifies all subscribers.
- **Subscribe**: `Subscribe(Action<T>)` registers a callback invoked immediately and on updates.

## 2. Hooks API
We mimic React's API but backing it with Signals.
- `UseState<T>(initial)`: Returns `(Signal<T> signal, Action<T> setter)`.
  - Unlike React, `setter` updates the signal, it does NOT re-run the component function.
- `UseMemo<T>`: Derived signals.

## 3. Compiler-Driven Subscriptions
The Generator detects `{expression}` blocks in the CSX.
- **Text Content**: If `{signal}` is found, it emits `Reactivity.Bind(signal, val => NativeDom.SetText(el, val))`.
- **Logic**: `Reactivity.Bind` detects if the object implements `ISignal`. If so, it subscribes. If not (static value), it runs once.

## 4. DOM Interop
`NativeDom.cs` handles browser interaction.
- **Console Mode**: Prints operations to StdOut (Stub).
- **Wasm Mode**: Uses `[JSImport]` to call browser APIs via `System.Runtime.InteropServices.JavaScript`.
