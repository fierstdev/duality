component RpcTest {
    [Server]
    public static void SaveData(string input) {
        Console.WriteLine("This should NOT be in client bundle!");
    }

    return <button onclick={SaveData("test")}>Save</button>;
}
