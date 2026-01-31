component DefaultPage {
    return <div class="container">
        <h1>Server Side Rendered</h1>
        <p>This HTML was generated on the server!</p>
        <button onclick={ Console.WriteLine("Clicked!"); }>Interact</button>
    </div>;
}
