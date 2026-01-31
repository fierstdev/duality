namespace Widgets;

component Navbar {
    return 
    <nav class={Css.Nav}>
        <div class={Css.Container}>
            <a href="/" class={Css.Brand}>CSX App</a>
            <div class={Css.Links}>
                <a href="https://github.com/fierst-llc/csx" target="_blank">Documentation</a>
                <a href="https://github.com" target="_blank">GitHub</a>
            </div>
        </div>
    </nav>;
}
