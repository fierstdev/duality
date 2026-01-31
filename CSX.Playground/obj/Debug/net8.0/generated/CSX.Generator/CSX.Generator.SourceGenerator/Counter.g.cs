#nullable enable
using CSX.Runtime;
using static CSX.Runtime.Hooks;

// Generated CSX Component: Counter
public static partial class Counter_Impl
{
    public static void Render(RenderContext ctx)
    {
        var RouteParams = ctx.RouteParams;
        // Render Logic

    var (count, setCount) = UseState(0);


        var el_429f378e = NativeDom.CreateElement("button");
        NativeDom.SetText(el_429f378e, @"
        Count is ");
        Reactivity.Bind(count, val => NativeDom.SetText(el_429f378e, val));
    }
}
