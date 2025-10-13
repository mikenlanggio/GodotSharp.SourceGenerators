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



    protected override (string GeneratedCode, DiagnosticDetail Error, OutputType outputType) GenerateCodeEx(
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

        return (output, null, data.OutputType);

        Godot.AutoEnumAttribute ReconstructAttribute() =>
            new((string)attribute.ConstructorArguments[0].Value ?? "", (OutputType) attribute.ConstructorArguments[1].Value);
    }
}
