using Widgets;

component Layout {
    return <html lang="en">
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <title>My CSX App</title>
            <meta name="description" content="A blazing fast reactive web app built with CSX." />
            <link rel="stylesheet" href="/app.css" />
            <link rel="stylesheet" href="/css/site.css" />
            <script src="https://unpkg.com/htmx.org@1.9.10"></script>
        </head>
        <body>
            <Navbar />

            <main class="container">
                <Slot />
            </main>

            <Footer />
        </body>
    </html>;
}
