# CSS Modules in CSX

CSX supports **CSS Modules** out of the box, providing scoped styles and type-safe class names.

## Usage

### 1. Create a CSS File
Create a `.css` file next to your component with the same name.

**`Pages/Hero.css`**
```css
.container {
    background-color: #f0f0f0;
    padding: 2rem;
}

.title-text {
    font-size: 3rem;
    color: #333;
}
```

### 2. Use Scoped Classes
In your component, use the generated `Css` class to access your styles. The class names are automatically converted to `PascalCase`.

**`Pages/Hero.csx`**
```csharp
component Hero {
    // .container -> Css.Container
    // .title-text -> Css.TitleText

    return <div class={Css.Container}>
        <h1 class={Css.TitleText}>Welcome to CSX</h1>
    </div>;
}
```

## How It Works

1. **Scoping**: The compiler renames your CSS classes to be unique per component (e.g., `.container` becomes `.container_Hero_x82z91`).
2. **Bundling**: All referenced CSS files are aggregated into a single bundle served at `/app.css`.
3. **Type Safety**: If you mistype a class name (e.g., `Css.TitlText`), the C# compiler will throw an error.

## Global Styles
For global styles (like resets or base styles), simply include them in `wwwroot/css/site.css` (or equivalent) and reference them manually in your Layout header, as standard HTML. CSS Modules are intended for component-scoped styles.
