#nullable enable
using CSX.Runtime;
using static CSX.Runtime.Hooks;
using System.Text;

// Generated CSX Component: RpcTest
public static partial class RpcTest_Impl
{
    // Class Members

    [Server]
    public static void MyAction(string name) {
        System.Console.WriteLine($"RPC Received: Hello {name}!");
    }


    public static void Render(StringBuilder sb, System.Collections.Generic.Dictionary<string, object>? routeParams = null, System.Action<StringBuilder>? childContent = null)
    {
        var RouteParams = routeParams ?? new System.Collections.Generic.Dictionary<string, object>();
        sb.Append("<button");
        sb.Append(">");
        sb.Append(@"RPC Test");
        sb.Append("</button>");
    }
}
