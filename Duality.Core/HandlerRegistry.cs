using System;
using System.Collections.Generic;

namespace Duality.Core
{
    public static class HandlerRegistry
    {
        private static readonly Dictionary<string, Action> _handlers = new Dictionary<string, Action>();

        public static void Register(string id, Action handler)
        {
            _handlers[id] = handler;
        }

        public static void Dispatch(string id)
        {
            if (_handlers.TryGetValue(id, out var handler))
            {
                handler();
            }
            else
            {
                Console.WriteLine($"[Error] Handler not found: {id}");
            }
        }
    }
}
