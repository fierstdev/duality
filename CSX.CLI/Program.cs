using System;
using System.Diagnostics;
using System.IO;

namespace CSX.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return;
            }

            switch (args[0].ToLower())
            {
                case "new":
                    if (args.Length < 2) Console.WriteLine("Usage: csx new [name]");
                    else CreateNewProject(args[1]);
                    break;
                case "build":
                    RunDotnet("build");
                    break;
                case "serve":
                    // For now, just run the playground or serve current folder
                    Console.WriteLine("Starting CSX Dev Server...");
                    RunDotnet("run");
                    break;
                case "help":
                default:
                    PrintUsage();
                    break;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("CSX CLI Tool");
            Console.WriteLine("  csx new [name]  - Create a new project");
            Console.WriteLine("  csx build       - Build the current project");
            Console.WriteLine("  csx serve       - Run the dev server");
        }

        static void CreateNewProject(string name)
        {
            Console.WriteLine($"Creating new CSX project: {name}...");
            // Real implementation: dotnet new csx-app -n name
            // Mock:
            Console.WriteLine("[Mock] Project created.");
        }

        static void RunDotnet(string args)
        {
            var psi = new ProcessStartInfo("dotnet", args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var p = Process.Start(psi);
            if (p != null)
            {
                p.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
                p.ErrorDataReceived += (s, e) => Console.Error.WriteLine(e.Data);
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
            }
        }
    }
}
