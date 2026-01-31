using System;

namespace Duality.Core
{
    public static class Reactivity
    {
        public static void Bind(object valueOrSignal, Action<string> domUpdater)
        {
            if (valueOrSignal is ISignal signal)
            {
                signal.Subscribe(val => domUpdater(val?.ToString() ?? ""));
            }
            else
            {
                domUpdater(valueOrSignal?.ToString() ?? "");
            }
        }
    }
}
