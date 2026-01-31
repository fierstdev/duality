using CSX.Runtime;
using static CSX.Runtime.Hooks;

// Generated CSX Component: Counter {
public static partial class Counter {_Impl
{
    public static void Render(RenderContext ctx)
    {
        // User Code

    var (count, setCount) = UseState(0);
    
        var el_a80f0148 = NativeDom.CreateElement("button");
        NativeDom.SetText(el_a80f0148, @"
        Count is ");
        Reactivity.Bind(count, val => NativeDom.SetText(el_a80f0148, val));
    }
}
