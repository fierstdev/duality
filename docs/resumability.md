# Resumability & Static Extraction

Duality achieves instant startup by not running component logic on the client until interaction occurs.

## Architecture

1.  **Static Extraction**: The compiler (Duality.Compiler) identifies event handlers (like `onclick`) and "hoists" them into `static` methods.
2.  **HTML Generation**: The initial HTML contains `onclick-csx="HandlerID"`. Standard `onclick` is blocked.
3.  **Automatic Closure Capturing**:
    - The compiler analyzes the handler code.
    - It identifies local variables captured from the component scope (e.g., `count`).
    - It generates a `Closure` class to hold these values.
    - It rewrites the handler to use this closure object.
    
    **User Code**:
    ```csharp
    return <button onclick={ setCount(count.Value + 1) }>...</button>
    ```

    **Generated Code**:
    ```csharp
    public class Closure_XYZ { public Signal<int> count; public Action<int> setCount; }
    
    // In Render
    var env = new Closure_XYZ { count = count, setCount = setCount };
    
    // Handler
    public static void Handler_XYZ(object state) {
        var env = (Closure_XYZ)state;
        env.setCount(env.count.Value + 1);
    }
    ```

4.  **Bootloader (`bootloader.js`)**: A tiny script (<1kb) listens to global events.
