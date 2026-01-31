# Getting Started with Duality

Welcome to Duality! This guide will walk you through building your first website, even if you've never written a line of C# before. Think of Duality as a way to write web apps with the simplicity of HTML/JS but the power and reliability of .NET.

## 1. Prerequisites

Just like you need Node.js for React, you need the **.NET SDK** for Duality.

- [Download .NET 9 SDK](https://dotnet.microsoft.com/download)
- Verify installation: Open your terminal and run `dotnet --version`.

## 2. Install the Duality CLI

Install the global `dlty` command from [nuget.org](https://www.nuget.org).

```bash
dotnet tool install -g Duality.CLI --prerelease
```

_Note: If `dlty` command is not found, verify `~/.dotnet/tools` is in your PATH._

## 3. Create Your Project

Let's build a website for a fictional client, "Bean Cafe".

Run the creation command:

```bash
dlty new BeanCafe
```

This creates a folder `BeanCafe` with a professional **Feature-Sliced Design (FSD)** structure:

- `Pages/`: Your routes (`index.csx`).
- `Widgets/`: Major UI blocks like `Navbar`, `Footer`.
- `Shared/`: Reusable primitives like `Button`, `Card`.
- `Features/`: Business logic components (e.g., `TodoList`).

## 4. Run the Development Server

Navigate to your folder and start the app:

```bash
cd BeanCafe
dlty dev
```

Open `http://localhost:5000` in your browser. You'll see the "Welcome to Duality" page.
_Note: `dlty dev` supports "Hot Reload" — changes you make will update the running app instantly._

## 5. Your First Change

Let's update the homepage. Open `Pages/Index.csx`.
You'll see code that looks very similar to HTML (we call it **CSX**).

```csharp
component Index {
    return
    <div>
        <h1>Welcome to Bean Cafe</h1>
        <p>Best coffee in town!</p>
    </div>;
}
```

Save the file. The browser updates automatically!

## 6. Creating Components

Let's make a **Navbar** for the site. Instead of creating files manually, use the generator:

```bash
dlty g component Navbar
```

The CLI will ask where to put it. Since a Navbar is a major UI block used for layout, choose **Widgets**.

```text
Which layer? (shared/features/widgets): w
```

This creates:

- `Widgets/Navbar.csx`: The structure.
- `Widgets/Navbar.css`: The styling (scoped to this component!).

### Styling (CSS Modules)

Open `Widgets/Navbar.css`:

```css
.nav {
  display: flex;
  background: #333;
  color: white;
  padding: 1rem;
  gap: 1rem;
}
```

### Logic

Open `Widgets/Navbar.csx`. Notice how we use `Css.Nav` to apply the class safely.

```csharp
component Navbar {
    return
    <nav class={Css.Nav}>
        <a href="/">Home</a>
        <a href="/about">About</a>
    </nav>;
}
```

## 7. Using Your Component

Now, let's add the Navbar to every page by editing the **Layout**.
Open `Pages/_layout.csx`. This file wraps all your pages.

```csharp
using BeanCafe.Widgets;

component Layout {
    return
    <html>
        <head>
            <title>Bean Cafe</title>
            <link rel="stylesheet" href="/app.css" />
            <link rel="stylesheet" href="/css/site.css" />
        </head>
        <body>
            <Navbar />

            <main>
                <Slot /> {/* Page content goes here */}
            </main>

            <Footer />
        </body>
    </html>;
}
```

## 8. Adding a New Page

Let's create an "About" page.

```bash
dlty g page About
```

This creates `Pages/About.csx`. It automatically becomes available at `/about`.

```csharp
component About {
    return
    <div>
        <h1>About Us</h1>
        <p>We have been brewing since 2024.</p>
    </div>;
}
```

Navigate to `http://localhost:5000/about` to see it.

## Summary

1.  **`dlty new`**: Start fresh.
2.  **`dlty dev`**: Run and watch for changes.
3.  **`dlty g component`**: specific pieces of UI.
4.  **`dlty g page`**: New routes.

You're now building with Duality!

## Next Steps

- Read [Server Actions (RPC)](rpc.md) to add interactivity to your app.
- Read [Deployment Guide](deployment.md) to learn how to host your app.
