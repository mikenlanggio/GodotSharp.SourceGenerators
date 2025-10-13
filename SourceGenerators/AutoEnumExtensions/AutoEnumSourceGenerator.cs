using System.Text;
using Godot;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Scriban;

namespace GodotSharp.SourceGenerators.AutoEnumExtensions;

[Generator]
internal class AutoEnumSourceGenerator : SourceGeneratorForDeclaredTypeWithAttribute<Godot.AutoEnumAttribute>
{
    private static Template AutoEnumTemplate => field ??= Template.Parse(Resources.AutoEnumTemplate);



    protected override (string GeneratedCode, DiagnosticDetail Error) GenerateCode(
        Compilation compilation,
        SyntaxNode node,
        INamedTypeSymbol symbol,
        AttributeData attribute,
        AnalyzerConfigOptions options)
    {
        var data = ReconstructAttribute();
        var model = new AutoEnumDataModel(symbol, data.IdentityProperty);
        Log.Debug($"--- MODEL ---\n{model}\n");

        var output = AutoEnumTemplate.Render(model, member => member.Name);
        Log.Debug($"--- OUTPUT ---\n{output}<END>\n");


        if (data.OutputType == OutputType.REAL)
        {
            var fileName = GenerateFilename(symbol);
            try
            {
                var sourceFilePath = node.SyntaxTree.FilePath;
                var sourceDir = Path.GetDirectoryName(sourceFilePath);
                var targetDir = Path.Combine(sourceDir, data.OutputDir);
                Directory.CreateDirectory(targetDir);

                var physicalPath = Path.Combine(targetDir, fileName);
                File.WriteAllText(physicalPath, output, Encoding.UTF8);
                Log.Debug($"[AutoEnum] Wrote physical enum file: {physicalPath}");
            }
            catch (Exception ex)
            {
                Log.Debug($"[AutoEnum] Failed to write physical file: {ex}");
            }
            return (null, null);
        } else
        {
            return (output, null);
        }

        Godot.AutoEnumAttribute ReconstructAttribute() =>
            new((string)attribute.ConstructorArguments[0].Value ?? "", (OutputType) attribute.ConstructorArguments[1].Value, (string)attribute.ConstructorArguments[2].Value ?? "");
    }
}
