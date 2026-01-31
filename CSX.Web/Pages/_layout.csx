component Layout {
    return <html>
        <head>
            <link rel="stylesheet" href="/app.css" />
        </head>
        <body>
           <nav>MY APP NAV</nav>
           <Slot />
           <footer>MY APP FOOTER</footer>
        </body>
    </html>;
}
