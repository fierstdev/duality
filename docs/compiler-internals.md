# Compiler Internals

The `CSX.Generator` functions as a Roslyn `IIncrementalGenerator`. It transforms `.csx` source text into C# source code through a three-stage pipeline.

## 1. Lexer (`Lexer.cs`)
The Lexer converts the raw input string into a stream of tokens. It operates in two simplified modes (conceptual):
- **C# Mode**: Scans for standard code and component declarations.
- **JSX Mode**: Triggered by `<Tag`, it scans for tags, attributes, and switches back to C# mode inside `{}` blocks.

current implementation supports:
- `component` keyword.
- `<tag>` and `</tag>` (strict parsing).
- Self-closing tags `/>`.
- Attributes (basic parsing).

## 2. Parser (`Parser.cs`)
The Parser consumes tokens and builds an Abstract Syntax Tree (AST).
- **Nodes**:
  - `ComponentNode`: Represents a `component Name { ... }`.
  - `ElementNode`: Represents an HTML tag.
  - `AttributeNode`: Represents `key="value"`.
  - `TextNode`: Represents text content.

## 3. Emitter (`Emitter.cs`)
The Emitter walks the AST and generates valid C# code.
- **Output Strategy**: usage of `CSX.Runtime.NativeDom`.
- **Logic**:
  - Creates variable names for elements (`var el_123...`).
  - Calls `NativeDom.CreateElement`.
  - Recursively appends children via `NativeDom.AppendChild`.

### Example Output
Input:
```csx
component Foo { return <div />; }
```

Output:
```csharp
public static partial class Foo_Impl {
    public static void Render(RenderContext ctx) {
        var el_xyz = NativeDom.CreateElement("div");
    }
}
```
