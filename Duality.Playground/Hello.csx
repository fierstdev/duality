component Counter {
    var (count, setCount) = UseState(0);

    return <button onclick={setCount(count.Value + 1);}>
        Count is {count}
    </button>;
}
