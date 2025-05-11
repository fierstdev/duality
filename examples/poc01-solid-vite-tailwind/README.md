# Duality PoC 1: Basic Deno/Hono + Vite + SolidJS + Tailwind Setup

This Proof of Concept (PoC 1) demonstrates the foundational integration of a Deno/Hono backend with a Vite-powered SolidJS frontend, utilizing Tailwind CSS for styling. It establishes the basic development workflow where the Deno backend serves an initial HTML shell, and the Vite development server serves the dynamic SolidJS frontend application.

This PoC is the first step in Phase 0 of the Duality framework development, focusing on the new SolidJS-based frontend architecture.

## Technologies Demonstrated

* **Backend:**
    * Deno Runtime
    * Hono Web Framework
* **Frontend:**
    * SolidJS (UI Framework)
    * Vite (Build Tool & Dev Server)
    * Tailwind CSS (Utility-First CSS Framework)
* **Integration:**
    * Deno serving an HTML shell that loads assets from the Vite dev server.
    * Frontend tooling (Vite, SolidJS plugins, Tailwind) managed and run via Deno tasks using `npm:` specifiers.

## Directory Structure (within this PoC)

```
poc01-solid-vite-tailwind/
├── backend/                # Deno/Hono backend
│   └── main.ts             # Hono server serving the initial HTML shell
├── frontend/               # SolidJS/Vite/Tailwind frontend
│   ├── public/
│   │   └── index.html      # Vite's template HTML (for standalone Vite dev)
│   ├── src/
│   │   ├── App.tsx         # Main SolidJS root component
│   │   ├── index.css       # Tailwind CSS directives and base styles
│   │   └── index.tsx       # Client-side entry point for SolidJS app
│   ├── postcss.config.cjs  # PostCSS configuration for Tailwind
│   ├── tailwind.config.cjs # Tailwind CSS configuration
│   └── vite.config.ts      # Vite configuration
└── deno.jsonc              # Deno configuration for this PoC (tasks, imports)
```

## Prerequisites

* Deno (latest version recommended)
* Git (for version control, if cloning the main Duality repo)

## Setup & Running the PoC

These commands should be run from the `duality/examples/poc01-solid-vite-tailwind/` directory.

1. **Cache Deno Dependencies & Initialize `node_modules`:**
   (This step ensures Deno downloads necessary `npm:` packages and sets up the `node_modules` directory as specified by `nodeModulesDir: true` in `deno.jsonc`.)
   ```bash
   deno cache backend/main.ts frontend/vite.config.ts frontend/src/index.tsx
   ```
   Alternatively, running one of the Deno tasks that uses these npm packages (like `deno task dev-frontend`) will also trigger the necessary downloads and `node_modules` creation if `nodeModulesDir: true` is set.

2. **Start the Frontend Development Server (Vite via Deno):**
   Open a terminal in the `duality/examples/poc01-solid-vite-tailwind/` directory and run:
   ```bash
   deno task dev:frontend
   ```
    * This will start the Vite development server, typically on `http://localhost:5173`.
    * It provides Hot Module Replacement (HMR) for the SolidJS application.

3. **Start the Backend Development Server (Deno/Hono):**
   Open a *new* terminal window. Navigate to the `duality/examples/poc01-solid-vite-tailwind/` directory and run:
   ```bash
   deno task start:backend
   ```
    * This will start the Hono server on `http://localhost:8000`.

4. **Test in Browser:**
    * Open your web browser and navigate to `http://localhost:8000`.
    * You should see the SolidJS application rendered. The initial HTML page is served by the Deno/Hono backend, but this page then fetches the SolidJS application bundle (JavaScript, CSS, HMR client) from the Vite development server (running on port 5173).

## Expected Outcome

* The browser displays a page with the title "Duality PoC (SolidJS + Vite + Deno)".
* The content is styled with Tailwind CSS, featuring a gradient background and a counter button.
* The message "SolidJS App Loaded! (Served by Vite, initial HTML by Deno/Hono)" appears after a short delay.
* The counter button should be interactive, incrementing its value on click.
* Changes made to the frontend code (e.g., in `frontend/src/App.tsx`) should trigger Hot Module Replacement (HMR) in the browser, updating the view without a full page reload.

This PoC validates the basic interoperability of the chosen backend and frontend technologies in a development setting.
