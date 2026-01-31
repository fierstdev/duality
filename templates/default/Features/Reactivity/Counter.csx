namespace Features.Reactivity;

component Counter {
    static int Count = 0;

    [Server]
    public static void Increment() {
        Count++;
    }

    return 
    <div class={Css.Counter}>
        <div class={Css.Value}>
            Count: {Count}
        </div>
        <button class={Css.Button} onClick={Increment}>
            Increment
        </button>
    </div>;
}
