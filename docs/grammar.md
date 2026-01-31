# CSX Grammar Specification

## Overview
CSX (C# Syntax Extension) files allow embedding XML-like syntax directly into C# code. The compiler transforms these `.csx` files into standard C# source code.

## File Structure
A `.csx` file is treated as a collection of component definitions.

```csharp
component MyComponent {
    // Standard C# code
    var name = "World";
    
    // CSX Expression
    return <div>Hello, {name}!</div>;
}
```

## Grammar Rules

### 1. Component Definition
A `component` keyword introduces a functional component.
- **Syntax**: `component Name { ... }`
- **Output**: The compiler generates a static method: `public static void Name(RenderContext ctx) { ... }`

### 2. XML Elements
HTML tags are supported as first-class expressions.
- **Self-Closing**: `<div />`
- **Container**: `<div>...</div>`
- **Attributes**: `id="main"`, `class="p-4"`.
    - String literals: `class="foo"`
    - Expressions: `class={variable}`

### 3. Code Blocks
Curly braces `{ ... }` switch context back to raw C#.
- **Inside Content**: `<div>Count: {count}</div>`
- **Inside Attributes**: `<button onclick={handleClick} />`

### 4. Directives (Planned)
- `UseState<T>(value)`: Returns `(Signal<T>, Action<T>)`.
- `UseMemo<T>(func)`: Returns `Signal<T>`.

## Restrictions
- Tags must be balanced.
- Only one root element per return (or use `<Fragment>`).
- No arbitrary C# statements inside XML element lists (must use `{}` for logic).
