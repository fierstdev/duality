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
            <main>
                <Slot />
            </main>
        </body>
    </html>;
}
