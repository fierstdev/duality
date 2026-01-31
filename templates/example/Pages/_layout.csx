component Layout {
    return <html>
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <title>CSX Todos</title>
            <link rel="stylesheet" href="/app.css" />
            <link rel="stylesheet" href="/css/site.css" />
        </head>
        <body>
            <nav>
                <div class="container">
                    <a href="/" class="brand">CSX Todos</a>
                </div>
            </nav>

            <main class="container">
                <Slot />
            </main>
        </body>
    </html>;
}
