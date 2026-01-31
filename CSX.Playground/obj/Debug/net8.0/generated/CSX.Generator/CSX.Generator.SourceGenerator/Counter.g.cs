using CSX.Runtime;
using static CSX.Runtime.Hooks;

// Generated CSX Component: Counter
public static partial class Counter_Impl
{
    public static void Render(RenderContext ctx)
    {
        // User Code

    var (count, setCount) = UseState(0);

    
        var el_f992a6b2 = NativeDom.CreateElement("button");
        var env_Counter_Handler_075109 = new Counter_Closure_Counter_Handler_075109();
        env_Counter_Handler_075109.setCount = setCount;
        env_Counter_Handler_075109.count = count;
        NativeDom.SetAttribute(el_f992a6b2, "onclick-csx", "Counter_Handler_075109");
        HandlerRegistry.Register("Counter_Handler_075109", (s) => Counter_Handler_075109(s));
        NativeDom.SetText(el_f992a6b2, @"
        Count is ");
        Reactivity.Bind(count, val => NativeDom.SetText(el_f992a6b2, val));
    }

    public class Counter_Closure_Counter_Handler_075109
    {
        public dynamic setCount;
        public dynamic count;
    }

    public static void Counter_Handler_075109(object state)
    {
        var env = (Counter_Closure_Counter_Handler_075109)state;
        env.setCount(env.count.Value + 1);
    }
}
