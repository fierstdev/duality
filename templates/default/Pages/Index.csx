using Shared.UI;

component Index {
    return 
    <div>
        <div class="hero">
            <h1 class="title-gradient">Build better with CSX</h1>
            <p class="subtitle">The web framework that feels like writing C#, because it is.</p>
        </div>

        <div class="grid">
            <Card>
                <h3>Structured</h3>
                <p>Edit <code>Pages/Index.csx</code> to see changes instantly using Hot Reload.</p>
            </Card>
            
            <Card>
                <h3>Components</h3>
                <p>Run <code>csx g component Name</code> to generate modular, reusable UI parts with scoped CSS.</p>
            </Card>

            <Card>
                <h3>Type Safe</h3>
                <p>Catch errors at build time. No more `undefined is not a function`.</p>
            </Card>
        </div>
    </div>;
}
