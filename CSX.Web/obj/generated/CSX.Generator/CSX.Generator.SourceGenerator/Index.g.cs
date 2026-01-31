#nullable enable
using CSX.Runtime;
using static CSX.Runtime.Hooks;
using System.Text;

// Generated CSX Component: Index
public static partial class Index_Impl
{
    // Class Members

    [Server]
    public static void Save(string input) {
        Console.WriteLine($"Server Action: Save '{input}' Executed!");
    }


    public static void Render(StringBuilder sb, System.Collections.Generic.Dictionary<string, object>? routeParams = null, System.Action<StringBuilder>? childContent = null)
    {
        var RouteParams = routeParams ?? new System.Collections.Generic.Dictionary<string, object>();
        sb.Append("<div");
        sb.Append(" class=\"container\"");
        sb.Append(">");
        sb.Append(@"
        ");
        sb.Append("<h1");
        sb.Append(">");
        sb.Append(@"Server Side Rendered");
        sb.Append("</h1>");
        sb.Append(@"
        ");
        sb.Append("<p");
        sb.Append(">");
        sb.Append(@"This HTML was generated on the server!");
        sb.Append("</p>");
        sb.Append(@"
        ");
        sb.Append("<button");
        sb.Append(" onclick-csx=\"Index_Handler_0\"");
        sb.Append(">");
        sb.Append(@"Interact");
        sb.Append("</button>");
        sb.Append(@"
    ");
        sb.Append("</div>");
    }
}
