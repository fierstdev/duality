#nullable enable
using CSX.Runtime;
using static CSX.Runtime.Hooks;
using System.Text;

// Generated CSX Component: StyleTest
public static partial class StyleTest_Impl
{
    // Class Members
    public static class Css
    {
        public static string RedBox => "red-box_StyleTest_460d32";
        public static string TitleText => "title-text_StyleTest_460d32";
    }


    public static void Render(StringBuilder sb, System.Collections.Generic.Dictionary<string, object>? routeParams = null, System.Action<StringBuilder>? childContent = null)
    {
        var RouteParams = routeParams ?? new System.Collections.Generic.Dictionary<string, object>();
        sb.Append("<div");
        sb.Append(" class=\"");
        sb.Append(Css.RedBox);
        sb.Append("\"");
        sb.Append(">");
        sb.Append(@"
        ");
        sb.Append("<h1");
        sb.Append(" class=\"");
        sb.Append(Css.TitleText);
        sb.Append("\"");
        sb.Append(">");
        sb.Append(@"Scoped Styles");
        sb.Append("</h1>");
        sb.Append(@"
    ");
        sb.Append("</div>");
    }
}
