component Layout {
    return <html>
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <title>My CSX App</title>
            <link rel="stylesheet" href="/app.css" />
            <link rel="stylesheet" href="/css/site.css" />
        </head>
        <body>
            <nav>
                <div class="container">
                    <a href="/" class="brand">CSX App</a>
                    <div class="links">
                        <a href="https://github.com/fierst-llc/csx" target="_blank">Documentation</a>
                        <a href="https://github.com" target="_blank">GitHub</a>
                    </div>
                </div>
            </nav>

            <main class="container">
                <Slot />
            </main>

            <footer>
                <div class="container">
                    Built with <strong>CSX</strong>. Fast, simple, reliable.
                </div>
            </footer>
        </body>
    </html>;
}
