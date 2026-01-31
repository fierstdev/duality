using System;

namespace Duality.Core
{
    public static class Hooks
    {
        // Simple UseState implementation.
        // In a real framework, this would rely on the RenderContext to track state per component instance.
        // For this PoC/Prototype (Stateless components), we just create a new Signal.
        // Note: This won't persist state across re-renders of the component function itself, 
        // BUT the unique architecture of CSX means the component function only runs ONCE.
        // So simple instantiation IS correct!
        
        public static (Signal<T>, Action<T>) UseState<T>(T initialValue)
        {
            var signal = new Signal<T>(initialValue);
            Action<T> setter = (newValue) => signal.Value = newValue;
            return (signal, setter);
        }

        public static Signal<T> UseMemo<T>(Func<T> compute)
        {
            // For a real implementation, we need dependency tracking.
            // For this PoC, we will cheat and assume it's just a derived value that might not update 
            // unless we build a full dependency graph.
            // To make it work with Signals:
            // logic: The 'compute' function likely accesses other signals.
            // We need a global "Tracking Context" to know which signals were accessed.
            
            // Simplified: Just compute it once. Real reactivity requires tracking.
            return new Signal<T>(compute());
        }
    }
}
