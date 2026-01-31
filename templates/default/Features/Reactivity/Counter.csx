namespace Features.Reactivity;

component Counter {
    var (count, setCount) = UseState(0);

    void Increment() {
        setCount(count.Value + 1);
    }

    return 
    <div class={Css.Counter}>
        <div class={Css.Value}>
            Count: {count}
        </div>
        <button class={Css.Button} onclick={Increment}>
            Increment
        </button>
    </div>;
}
