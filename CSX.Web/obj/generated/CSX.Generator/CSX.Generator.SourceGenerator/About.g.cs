#nullable enable
using CSX.Runtime;
using static CSX.Runtime.Hooks;
using System.Text;

// Generated CSX Component: About
public static partial class About_Impl
{
    public static void Render(StringBuilder sb, System.Collections.Generic.Dictionary<string, object>? routeParams = null, System.Action<StringBuilder>? childContent = null)
    {
        var RouteParams = routeParams ?? new System.Collections.Generic.Dictionary<string, object>();
        sb.Append("<div");
        sb.Append(">");
        sb.Append(@"
        ");
        sb.Append("<h1");
        sb.Append(">");
        sb.Append(@"About CSX");
        sb.Append("</h1>");
        sb.Append(@"
        ");
        sb.Append("<p");
        sb.Append(">");
        sb.Append(@"This is the About page.");
        sb.Append("</p>");
        sb.Append(@"
        ");
        sb.Append("<a");
        sb.Append(" href=\"/\"");
        sb.Append(">");
        sb.Append(@"Go Home");
        sb.Append("</a>");
        sb.Append(@"
    ");
        sb.Append("</div>");
    }
}
