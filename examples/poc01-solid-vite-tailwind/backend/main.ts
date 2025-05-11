import { Hono, Context } from '@hono/hono';

const app = new Hono();

app.get('/', (c: Context) => {
	const viteDevServerUrl = 'http://localhost:5173'; // Default Vite port

	return c.html(`
    <!DOCTYPE html>
    <html lang="en">
      <head>
        <meta charset="UTF-8" />
        <link rel="icon" type="image/svg+xml" href="${viteDevServerUrl}/@vite/client" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>Duality PoC (SolidJS + Vite + Deno)</title>
      </head>
      <body>
        <div id="root"></div>
        <script type="module" src="${viteDevServerUrl}/src/index.tsx"></script>
        <script type="module" src="${viteDevServerUrl}/@vite/client"></script>
      </body>
    </html>
  `);
});

Deno.serve({ port: 8000 }, app.fetch);
console.log("Duality PoC Backend (Hono) running on http://localhost:8000");
console.log("Ensure the Frontend (Vite + SolidJS) is running separately via 'deno task dev-frontend', typically on http://localhost:5173");
