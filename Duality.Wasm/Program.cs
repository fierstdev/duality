using Duality.Core;
using System.Runtime.InteropServices.JavaScript;

// Keep the Render method alive for the JS bootloader
[assembly: System.Runtime.Versioning.SupportedOSPlatform("browser")]

public partial class Program
{
    public static void Main()
    {
        // Entry point is managed by JS import usually, 
        // but we can kick off the render here.
        Console.WriteLine("CSX Wasm Booting...");
        
        // In a real app, 'document.body' or a specific ID would be passed
        Counter_Impl.Render(new RenderContext());
        
        Console.WriteLine("CSX Wasm Rendered.");
    }

    [JSExport]
    public static void Boot()
    {
        Main();
    }
}
