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

            var command = args[0].ToLower();
            try
            {
                switch (command)
                {
                    case "new":
                        string? projName = null;
                        string? template = "default";

                        if (args.Length >= 2) projName = args[1];
                        else 
                        {
                            Console.Write("What is your project name? ");
                            projName = Console.ReadLine();
                        }
                        
                        if (args.Length >= 3)
                        {
                            // Ignore template arg or warn, for now just use default
                        }

                        if (!string.IsNullOrWhiteSpace(projName)) CreateNewProject(projName, template);
                        else Console.WriteLine("Project name is required.");
                        break;

                    case "generate":
                    case "g":
                        string? type = null;
                        string? name = null;

                        if (args.Length >= 2) type = args[1];
                        else
                        {
                            Console.Write("What do you want to generate? (component/page): ");
                            type = Console.ReadLine();
                        }

                        if (args.Length >= 3) name = args[2];
                        else
                        {
                            Console.Write("What is the name? ");
                            name = Console.ReadLine();
                        }

                        if (!string.IsNullOrWhiteSpace(type) && !string.IsNullOrWhiteSpace(name)) 
                        {
                            GenerateItem(type, name);
                        }
                        else
                        {
                             Console.WriteLine("Type and Name are required.");
                        }
                        break;

                    case "build":
                        RunDotnet("build");
                        break;
                    case "dev":
                    case "serve":
                        Console.WriteLine("Starting CSX Dev Server...");
                        RunDotnet("watch run");
                        break;
                    case "help":
                    default:
                        PrintUsage();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("CSX CLI Tool");
            Console.WriteLine("  csx new [name]               - Create a new project");
            Console.WriteLine("  csx g component [name]       - Generate a component");
            Console.WriteLine("  csx g page [name]            - Generate a page");
            Console.WriteLine("  csx dev                      - Run dev server (dotnet watch)");
            Console.WriteLine("  csx build                    - Build project");
        }

        static void CreateNewProject(string name, string template = "default")
        {
            var destination = Path.Combine(Directory.GetCurrentDirectory(), name);
            if (Directory.Exists(destination))
            {
                Console.WriteLine($"Error: Directory '{name}' already exists.");
                return;
            }

            // Locate Template
            var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var templatePath = FindTemplatesDir(Path.GetDirectoryName(assemblyPath)!);
            if (string.IsNullOrEmpty(templatePath))
            {
                Console.WriteLine("Error: Could not find 'templates' directory.");
                return;
            }
            
            var source = Path.Combine(templatePath, template);
            if (!Directory.Exists(source)) 
            {
                Console.WriteLine($"Error: Template '{template}' not found at {source}");
                Console.WriteLine($"Available templates in {templatePath}: " + string.Join(", ", Directory.GetDirectories(templatePath).Select(Path.GetFileName)));
                return;
            }

            Console.WriteLine($"Creating '{name}' using '{template}' template...");
            CopyDirectory(source, destination, name);
            Console.WriteLine("Done.");
            Console.WriteLine($"\ncd {name}\ncsx dev");
        }

        static string? FindTemplatesDir(string startPath)
        {
            var current = new DirectoryInfo(startPath);
            while (current != null)
            {
                var templates = Path.Combine(current.FullName, "templates");
                if (Directory.Exists(templates)) return templates;
                current = current.Parent;
            }
            return null;
        }

        static void CopyDirectory(string sourceDir, string destDir, string projectName)
        {
            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destDir, Path.GetFileName(file));
                // Rename csproj
                if (destFile.EndsWith(".csproj"))
                {
                    destFile = Path.Combine(destDir, $"{projectName}.csproj");
                }
                
                // Copy & Replace placeholder if needed (csproj replacement?)
                // The blank csproj is generic enough or needs replacement?
                // It references ../CSX.Runtime. We should keep that for this repo.
                File.Copy(file, destFile);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var destSubDir = Path.Combine(destDir, new DirectoryInfo(dir).Name);
                CopyDirectory(dir, destSubDir, projectName);
            }
        }

        static void GenerateItem(string type, string name)
        {
            if (type.StartsWith("comp")) 
            {
                // Prompts for Layer
                Console.Write("Which layer? (shared/features/widgets): ");
                var layer = Console.ReadLine()?.ToLower().Trim();
                
                string targetDir = "Shared/UI"; // default
                if (layer == "features" || layer == "f")
                {
                     Console.Write("Feature Name? ");
                     var feature = Console.ReadLine()?.Trim();
                     if (!string.IsNullOrEmpty(feature))
                     {
                         targetDir = Path.Combine("Features", feature);
                     }
                     else
                     {
                         Console.WriteLine("Feature name required. Aborting.");
                         return;
                     }
                }
                else if (layer == "widgets" || layer == "w")
                {
                    targetDir = "Widgets";
                }
                else if (layer == "shared" || layer == "s")
                {
                    targetDir = "Shared/UI";
                }

                GenerateComponent(name, targetDir); 
            }
            else if (type.StartsWith("p")) 
            {
                GenerateComponent(name, "Pages");
            } 
            else Console.WriteLine("Unknown generator type.");
        }

        static void GenerateComponent(string name, string directory)
        {
             if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
             
             var cssPath = Path.Combine(directory, $"{name}.css");
             var csxPath = Path.Combine(directory, $"{name}.csx");

             if (File.Exists(csxPath)) 
             {
                 Console.WriteLine($"Error: {csxPath} already exists.");
                 return;
             }

             File.WriteAllText(cssPath, $".container {{\n    /* Styles for {name} */\n}}");
             File.WriteAllText(csxPath, $@"component {name} {{
    return 
    <div class={{Css.Container}}>
        <h1>{name}</h1>
    </div>;
}}");
             Console.WriteLine($"Created {csxPath}");
             Console.WriteLine($"Created {cssPath}");
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
                p.OutputDataReceived += (s, e) => { if(e.Data != null) Console.WriteLine(e.Data); };
                p.ErrorDataReceived += (s, e) => { if(e.Data != null) Console.Error.WriteLine(e.Data); };
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
            }
        }
    }
}
