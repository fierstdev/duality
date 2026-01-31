#nullable enable
using CSX.Runtime;
using static CSX.Runtime.Hooks;

// Generated CSX Component: RpcTest
public static partial class RpcTest_Impl
{
    // Proxy for SaveData
    public static async void SaveData(string input)
    {
        await CSX.Runtime.RpcClient.CallAsync("RpcTest", "SaveData", input);
    }
    public static void Render(RenderContext ctx)
    {
        var RouteParams = ctx.RouteParams;
        var el_fd452ffc = NativeDom.CreateElement("button");
        NativeDom.SetText(el_fd452ffc, @"Save");
    }
}
