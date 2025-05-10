import { Hono, Context } from '@hono/hono';
import { stream } from '@hono/hono/streaming';
import { serveStatic } from "@hono/hono/deno"

const app = new Hono(); // Removed custom AppEnv, letting Hono infer types

// Handler for PoC 2.1's main.ts (component definitions)
app.get('/examples/poc02-basic-component-definition/main.ts', async (c) => {
	try {
		// Path relative to where `deno run` is executed (assumed project root)
		const filePath = "./examples/poc02-basic-component-definition/main.ts";
		const file = await Deno.open(filePath, { read: true });
		const readable = file.readable;
		c.header('Content-Type', 'application/javascript; charset=utf-8');
		console.log(`Serving ${filePath} with Content-Type: application/javascript`);
		return c.body(readable);
	} catch (e) {
		console.error(`Error serving ${c.req.path}: ${e.message}`);
		return c.notFound();
	}
});

// Handler for PoC 2.1's index.html
app.get('/examples/poc02-basic-component-definition/', async (c) => {
	try {
		const filePath = "./examples/poc02-basic-component-definition/index.html";
		const file = await Deno.open(filePath, { read: true });
		const readable = file.readable;
		c.header('Content-Type', 'text/html; charset=utf-8');
		console.log(`Serving ${filePath} with Content-Type: text/html`);
		return c.body(readable);
	} catch (e) {
		console.error(`Error serving ${c.req.path}: ${e.message}`);
		return c.notFound();
	}
});
app.get('/examples/poc02-basic-component-definition/index.html', async (c) => {
	try {
		const filePath = "./examples/poc02-basic-component-definition/index.html";
		const file = await Deno.open(filePath, { read: true });
		const readable = file.readable;
		c.header('Content-Type', 'text/html; charset=utf-8');
		console.log(`Serving ${filePath} with Content-Type: text/html`);
		return c.body(readable);
	} catch (e) {
		console.error(`Error serving ${c.req.path}: ${e.message}`);
		return c.notFound();
	}
});


// --- General Serve Static Files for other PoCs in /examples ---
// This will now only handle requests under /examples/* that were NOT caught by the specific handlers above.
app.use('/examples/*', serveStatic({
	root: './', // Relative to where Deno is run (project root)
	// This middleware will attempt to determine MIME types for other files.
}));

// --- PoC 1.1: Basic Hono App Setup ---
// Using basic Context type, allowing Hono to infer more.
app.get('/', (c: Context) => {
	return c.html(`
    <!DOCTYPE html>
    <html lang="en">
    <head>
      <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, initial-scale=1.0">
      <title>Duality PoC - Hono App</title>
      <style>
        body { font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; margin: 2rem; background-color: #f0f2f5; color: #1a1b1c; line-height: 1.6; }
        h1 { color: #003366; margin-bottom: 1rem; }
        ul { list-style: none; padding: 0; }
        li { margin-bottom: 0.75rem; }
        a { color: #0066cc; text-decoration: none; }
        a:hover { text-decoration: underline; }
        .container { background-color: #ffffff; padding: 2rem; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); max-width: 700px; margin: auto; }
        .poc-link { display: inline-block; padding: 0.5rem 1rem; background-color: #0077cc; color: white; border-radius: 4px; margin-top: 0.5rem;}
        .poc-link:hover { background-color: #005fa3;}
      </style>
    </head>
    <body>
      <div class="container">
        <h1>Welcome to Duality Framework PoC!</h1>
        <p>This is a basic Hono application running on Deno, demonstrating core server functionality.</p>
        <nav>
          <ul>
            <li><a href="/stream-simple" class="poc-link">View Simple Streaming HTML</a></li>
            <li><a href="/stream-progressive" class="poc-link">View Progressive Streaming HTML (simulated delay)</a></li>
          </ul>
        </nav>
      </div>
    </body>
    </html>
  `);
});

// --- PoC 1.2: Basic Streaming HTML Response ---
// Using basic Context type.
app.get('/stream-simple', (c: Context) => {
	const streamResponse = new ReadableStream({
		start(controller) {
			const encoder = new TextEncoder();
			controller.enqueue(encoder.encode("<!DOCTYPE html>"));
			controller.enqueue(encoder.encode("<html lang='en'><head><meta charset='UTF-8'><title>Simple Stream</title>"));
			controller.enqueue(encoder.encode("<style>body { font-family: sans-serif; margin: 20px; } h1 { color: #228b22; }</style>"));
			controller.enqueue(encoder.encode("</head><body>"));
			controller.enqueue(encoder.encode("<h1>Simple Streamed HTML</h1>"));
			controller.enqueue(encoder.encode("<p>This content is being streamed as a single conceptual block, but delivered via a stream.</p>"));
			controller.enqueue(encoder.encode("<p><a href='/'>Back to Home</a></p>"));
			controller.enqueue(encoder.encode("</body></html>"));
			controller.close();
		}
	});
	return c.body(streamResponse, {
		headers: {
			'Content-Type': 'text/html; charset=UTF-8',
			'X-Content-Type-Options': 'nosniff'
		}
	});
});

// Helper function for manual delay
const sleep = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));

// --- PoC 1.2 (Extended): Progressive Streaming with Simulated Delay ---
// Using the imported standalone stream() helper function.
app.get('/stream-progressive', (c: Context) => { // c is still typed as Context
	return stream( // Call the imported 'stream' function
		c, // Pass the context 'c' as the first argument
		async (stream) => { // The callback receives the StreamingApi instance
			// Hono's stream.write() can accept strings directly and handles encoding.
			await stream.write("<!DOCTYPE html>");
			await stream.write("<html lang='en'><head><meta charset='UTF-8'><title>Progressive Stream</title>");
			await stream.write("<style>");
			await stream.write("body { font-family: 'Arial', sans-serif; margin: 0; background-color: #2c3e50; color: #ecf0f1; display: flex; flex-direction: column; align-items: center; min-height: 100vh; padding: 2rem 1rem; box-sizing: border-box;}");
			await stream.write("header, footer { width: 100%; max-width: 800px; text-align: center; margin-bottom: 1.5rem; }");
			await stream.write("h1 { color: #3498db; margin-bottom: 0.5rem; }");
			await stream.write(".chunk { background-color: #34495e; padding: 1.5rem; margin-bottom: 1rem; border-radius: 6px; box-shadow: 0 2px 5px rgba(0,0,0,0.2); width: 100%; max-width: 800px; transition: opacity 0.5s ease-in-out; opacity: 1; }");
			await stream.write(".chunk h2 { margin-top: 0; color: #9b59b6; }");
			await stream.write(".chunk ul { padding-left: 20px; }");
			await stream.write("footer { margin-top: auto; padding: 1rem; font-size: 0.9rem; color: #bdc3c7;}");
			await stream.write("a { color: #3498db; } a:hover { color: #2980b9; }");
			await stream.write("</style>");
			await stream.write("</head><body>");

			await stream.write("<header><h1>Progressive HTML Streaming with Duality PoC</h1></header>");
			await stream.write("<main style='width:100%; max-width: 800px; display:flex; flex-direction:column; align-items:center;'>");

			await stream.write("<div class='chunk'><h2>Chunk 1: Initial Content</h2><p>This is the first piece of content, delivered immediately. Welcome to the demonstration of Duality's streaming capabilities.</p></div>");
			await sleep(1000); // Replaced streamApi.wait with manual sleep

			await stream.write("<div class='chunk'><h2>Chunk 2: More Content Loaded</h2><p>This content arrives after a short delay. Imagine this is data fetched from a database or a slow API call, being rendered as it becomes available.</p></div>");
			await sleep(1500); // Replaced streamApi.wait with manual sleep

			await stream.write("<div class='chunk'><h2>Chunk 3: Final Content Block</h2><p>And here's the final piece of the main content. The page is progressively built, improving perceived performance.</p><ul><li>List Item A</li><li>List Item B</li><li>List Item C</li></ul></div>");
			await sleep(500); // Replaced streamApi.wait with manual sleep

			await stream.write("<div class='chunk'><p><a href='/'>Back to Home</a></p></div>");

			await stream.write("</main>");
			await stream.write("<footer><p>&copy; Fierst - Duality Framework PoC</p></footer>"); // Updated copyright
			await stream.write("</body></html>");
			// streamApi.close() can be called here if needed, though Hono often handles it with the standalone helper.
		});
});

Deno.serve({ port: 8000, handler: app.fetch });

console.log("Duality PoC server running on http://localhost:8000");
console.log("  Try: /stream-simple or /stream-progressive");
