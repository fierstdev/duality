using System;
using System.Collections.Generic;

namespace CSX.Runtime
{
    public static class HandlerRegistry
    {
        private static readonly Dictionary<string, Action<object>> _handlers = new Dictionary<string, Action<object>>();

        public static void Register(string id, Action<object> handler)
        {
            _handlers[id] = handler;
        }

        public static void Dispatch(string id, object state)
        {
            if (_handlers.TryGetValue(id, out var handler))
            {
                handler(state);
            }
            else
            {
                Console.WriteLine($"[Error] Handler not found: {id}");
            }
        }
    }
}
