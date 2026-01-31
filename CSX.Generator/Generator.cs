using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.IO;

namespace CSX.Generator
{
    [Generator]
    public class SourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
             var csxFiles = context.AdditionalTextsProvider
                .Where(file => file.Path.EndsWith(".csx"));
            
            context.RegisterSourceOutput(csxFiles, (spc, file) =>
            {
                var sourceCodeMaybe = file.GetText(spc.CancellationToken)?.ToString();
                if (sourceCodeMaybe is not { } sourceCode || string.IsNullOrWhiteSpace(sourceCode)) return;

                // 1. Lex
                var lexer = new Lexer(sourceCode);
                var tokens = lexer.Tokenize();

                // 2. Parse
                var parser = new Parser(tokens);
                var component = parser.ParseComponent();

                if (component == null) return; // specific handle error

                // 3. Emit
                var emitter = new Emitter();
                var csharp = emitter.Emit(component);

                spc.AddSource($"{component.Name}.g.cs", SourceText.From(csharp, Encoding.UTF8));
            });
        }
    }
}
