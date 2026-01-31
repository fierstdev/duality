#nullable enable
using CSX.Runtime;
using static CSX.Runtime.Hooks;
using System.Text;

// Generated CSX Component: UserProfile
public static partial class UserProfile_Impl
{
    public static void Render(StringBuilder sb, System.Collections.Generic.Dictionary<string, object>? routeParams = null, System.Action<StringBuilder>? childContent = null)
    {
        var RouteParams = routeParams ?? new System.Collections.Generic.Dictionary<string, object>();
        // Render Logic

    var id = RouteParams["id"];


        sb.Append("<h1");
        sb.Append(">");
        sb.Append(@"User: ");
        sb.Append(id);
        sb.Append("</h1>");
    }
}
