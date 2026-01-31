component RpcTest {
    [Server]
    public static void MyAction(string name) {
        System.Console.WriteLine($"RPC Received: Hello {name}!");
    }
    
    return <button>RPC Test</button>;
}
