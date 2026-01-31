#nullable enable
using CSX.Runtime;
using static CSX.Runtime.Hooks;

// Generated CSX Component: DefaultPage
public static partial class DefaultPage_Impl
{
    public static void Render(RenderContext ctx)
    {
        var RouteParams = ctx.RouteParams;
        var el_434d4643 = NativeDom.CreateElement("div");
        var el_edbff36f = NativeDom.CreateElement("h1");
        NativeDom.AppendChild(el_434d4643, el_edbff36f);
        NativeDom.SetText(el_edbff36f, @"Server Side Rendered");
        var el_09f08fbd = NativeDom.CreateElement("p");
        NativeDom.AppendChild(el_434d4643, el_09f08fbd);
        NativeDom.SetText(el_09f08fbd, @"This HTML was generated on the server!");
        var el_3510231d = NativeDom.CreateElement("button");
        NativeDom.AppendChild(el_434d4643, el_3510231d);
        NativeDom.SetText(el_3510231d, @"Interact");
    }
}
