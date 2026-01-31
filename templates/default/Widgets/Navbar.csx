namespace Widgets;

component Navbar {
    return 
    <nav class={Css.Nav}>
        <div class={Css.Container}>
            <a href="/" class={Css.Brand}>CSX App</a>
            <div class={Css.Links}>
                <a href="https://github.com/fierstdev/csx-framework" target="_blank">Documentation</a>
                <a href="https://github.com/fierstdev" target="_blank">GitHub</a>
            </div>
        </div>
    </nav>;
}
