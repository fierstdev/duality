# CSX Server Actions (RPC)

CSX enables type-safe Remote Procedure Calls (RPC) between Client components and Server logic using the `[Server]` attribute.

## Concept
Instead of manually creating API endpoints and fetch requests, you define static C# methods inside your component annotated with `[Server]`.
The compiler generates:
1. **Server-Side:** An API endpoint at `/_rpc/{Component}/{Method}`.
2. **Client-Side:** A generated proxy method that calls this endpoint securely using `HttpClient`.

## Usage

```csharp
component Counter {
    [Server]
    public static void LogCount(int count) {
        // This runs ON THE SERVER
        System.Console.WriteLine($"Count is now: {count}");
    }

    // Client-side code calls it like a normal function
    return <button onclick={() => LogCount(5)}>Log It</button>;
}
```

## Architecture

### Compilation Split
* **Server Build:** The method body is compiled as-is.
* **Client Build:** The method body is **removed** and replaced with a proxy call:
  ```csharp
  public static async void LogCount(int count) {
      await RpcClient.CallAsync("Counter", "LogCount", count);
  }
  ```

### Type Safety
Since the exact same C# file is used for both Server and Client compilation, arguments and return types are checked at compile time. You cannot invoke a Server Action with incorrect parameters.

### Serialization
Arguments are serialized to JSON using `System.Text.Json`.

## Best Practices
* **Security:** Always validate inputs in your `[Server]` methods. Do not trust the client.
* **Async:** Server actions should ideally be `async Task` if they perform I/O.
* **Static:** Currently, Server Actions must be `static` as they don't have access to Client Component state (only passed arguments).
