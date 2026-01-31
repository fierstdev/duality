#nullable enable
using CSX.Runtime;
using static CSX.Runtime.Hooks;
using System.Text;

// Generated CSX Component: Layout
public static partial class Layout_Impl
{
    public static void Render(StringBuilder sb, System.Collections.Generic.Dictionary<string, object>? routeParams = null, System.Action<StringBuilder>? childContent = null)
    {
        var RouteParams = routeParams ?? new System.Collections.Generic.Dictionary<string, object>();
        sb.Append("<html");
        sb.Append(">");
        sb.Append(@"
        ");
        sb.Append("<head");
        sb.Append(">");
        sb.Append(@"
            ");
        sb.Append("<link");
        sb.Append(" rel=\"stylesheet\"");
        sb.Append(" href=\"/app.css\"");
        sb.Append(">");
        sb.Append("</link>");
        sb.Append(@"
        ");
        sb.Append("</head>");
        sb.Append(@"
        ");
        sb.Append("<body");
        sb.Append(">");
        sb.Append(@"
           ");
        sb.Append("<nav");
        sb.Append(">");
        sb.Append(@"MY APP NAV");
        sb.Append("</nav>");
        sb.Append(@"
           ");
        childContent?.Invoke(sb);
        sb.Append(@"
           ");
        sb.Append("<footer");
        sb.Append(">");
        sb.Append(@"MY APP FOOTER");
        sb.Append("</footer>");
        sb.Append(@"
        ");
        sb.Append("</body>");
        sb.Append(@"
    ");
        sb.Append("</html>");
    }
}
