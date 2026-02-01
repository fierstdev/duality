# Duality Server Actions (RPC)

Duality provides a seamless way to trigger server-side logic from the client without writing API endpoints manually. This is powered by **HTMX** and compiler transpilation.

## Concept

Instead of creating a separate API controller, you define `static` methods inside your component annotated with `[Server]`. 
The compiler automatically:
1.  **Generates an Endpoint**: Creates a route at `/_rpc/{Component}/{Method}`.
2.  **Binds the UI**: Transpiles your `onClick` handlers into HTMX attributes (`hx-post`) that call this endpoint.

## Usage

### 1. Define the Action
Mark a static method with `[Server]`. This method runs entirely on the server.

```csharp
component Counter {
    static int Count = 0;

    [Server]
    public static void Increment() {
        Count++;
    }

    return (
        <div>
            Count: {Count}
            <button onClick={Increment}>Increment</button>
        </div>
    );
}
```

### 2. How it Compiles
The CSX compiler transforms the `onClick` handler into declarative HTMX attributes:

**Input (Your Code):**
```csharp
<button onClick={Increment}>Increment</button>
```

**Output (Generated HTML):**
```html
<button hx-post="/_rpc/Counter/Increment" hx-target="closest div" hx-swap="outerHTML">
    Increment
</button>
```

When clicked:
1.  HTMX sends a POST request to `/_rpc/Counter/Increment`.
2.  The server executes `Increment()`.
3.  The server **re-renders** the component with the new state (`Count` is now 1).
4.  The server returns the updated HTML.
5.  HTMX swaps the old component HTML with the new one.

## State Management

Since these actions run on the server, they do not have access to the browser's memory. State must be persisted somewhere accessible to the server:

*   **Static Fields**: Shared global state (simplest for demos/single-tenant apps).
*   **Database**: Persistent data.
*   **Session**: Per-user state (Future feature).

*Note: The `[Server]` attribute currently works best with `static` methods and state.*
