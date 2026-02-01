# Duality File-System Routing

Duality implements a file-system based routing engine inspired by Next.js, leveraging the power of C# Source Generators.

## Features

### Static Routes
Any `.csx` file placed in `Duality.Web/Pages` becomes a route.
* `Pages/Index.csx` -> `/`
* `Pages/About.csx` -> `/about`

### Dynamic Routes (`[param]`)
Files using optional bracket syntax `[param].csx` are converted to path parameters.
* `Pages/User/[id].csx` -> `/user/{id}`

Parameters are available in the component via `RouteParams`:
```csharp
component UserProfile {
    var id = RouteParams["id"];
    return <h1>User: {id}</h1>;
}
```

### Component Parameters
The build system automatically extracts C# local variables defined at the top of your component execution block. You can mix standard C# logic with JSX rendering seamlessly.

```csharp
component Product {
    var productId = RouteParams["id"];
    var db = new Database();
    var product = db.GetProduct(productId);
    
    return <div>
        <h1>{product.Name}</h1>
        <p>{product.Price}</p>
    </div>;
}
```

### Nested Layouts (`_layout.csx`)
Define a `_layout.csx` component in your `Pages` directory to wrap all pages.
Use the `<Slot />` component to indicate where the page content should be injected.

```csharp
component Layout {
    return <html>
        <body>
            <nav class="navbar">My App</nav>
            <main>
                <Slot />
            </main>
        </body>
    </html>;
}
```

## How It Works
The `CSX.Generator` scans the `Pages` directory and generates an `AppRouter` class that maps endpoints using ASP.NET Core Minimal APIs.
- Layouts are applied by wrapping the Page's render call: `Layout_Impl.Render(sb, params, (s) => Page_Impl.Render(s, ...))`.
- C# statements (`var x = ...`) are extracted and placed before the render logic in the generated `_Impl` class.
