using System;

namespace Duality.Core
{
    public static class NativeDom
    {
        public static object CreateElement(string tagName)
        {
#if WASM
            return Interop.CreateElement(tagName);
#else
            Console.WriteLine($"[Stub] CreateElement: {tagName}");
            return new object();
#endif
        }

        public static void SetAttribute(object element, string name, string value)
        {
#if WASM
            Interop.SetAttribute((System.Runtime.InteropServices.JavaScript.JSObject)element, name, value);
#else
            Console.WriteLine($"[Stub] SetAttribute: {name}={value}");
#endif
        }

        public static void SetText(object element, string text)
        {
#if WASM
            Interop.SetText((System.Runtime.InteropServices.JavaScript.JSObject)element, text);
#else
            Console.WriteLine($"[Stub] SetText: {text}");
#endif
        }
        
        public static void AppendChild(object parent, object child)
        {
#if WASM
             Interop.AppendChild((System.Runtime.InteropServices.JavaScript.JSObject)parent, (System.Runtime.InteropServices.JavaScript.JSObject)child);
#else
             Console.WriteLine($"[Stub] AppendChild");
#endif
        }
    }

#if WASM
    internal static partial class Interop
    {
        [System.Runtime.InteropServices.JavaScript.JSImport("globalThis.document.createElement")]
        public static partial System.Runtime.InteropServices.JavaScript.JSObject CreateElement(string tagName);

        [System.Runtime.InteropServices.JavaScript.JSImport("globalThis.document.setAttribute")]
        public static partial void SetAttribute(System.Runtime.InteropServices.JavaScript.JSObject element, string name, string value);
        
        // Note: Real DOM API SetText is explicitly setting textContent property, usually done via property setter helper
        [System.Runtime.InteropServices.JavaScript.JSImport("globalThis.InternalSetText", "main.js")] 
        public static partial void SetText(System.Runtime.InteropServices.JavaScript.JSObject element, string text);

        [System.Runtime.InteropServices.JavaScript.JSImport("globalThis.InternalAppendChild", "main.js")]
        public static partial void AppendChild(System.Runtime.InteropServices.JavaScript.JSObject parent, System.Runtime.InteropServices.JavaScript.JSObject child);
    }
#endif
}
