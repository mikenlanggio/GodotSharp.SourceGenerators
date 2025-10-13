using Godot;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace GodotSharp.SourceGenerators;

public abstract class SourceGeneratorForDeclaredFieldWithAttribute<TAttribute> : SourceGeneratorForDeclaredMemberWithAttribute<TAttribute, FieldDeclarationSyntax>
    where TAttribute : Attribute
{
    protected abstract (string GeneratedCode, DiagnosticDetail Error) GenerateCode(Compilation compilation, SyntaxNode node, IFieldSymbol symbol, AttributeData attribute, AnalyzerConfigOptions options);

    protected virtual (string GeneratedCode, DiagnosticDetail Error, OutputType outputType)
        GenerateCodeEx(Compilation compilation, SyntaxNode node, IFieldSymbol symbol, AttributeData attribute, AnalyzerConfigOptions options)
    {
        var (code, err) = GenerateCode(compilation, node, symbol, attribute, options);
        return (code, err, OutputType.ROSLYN);
    }

    protected sealed override (string GeneratedCode, DiagnosticDetail Error, OutputType outputType) GenerateCode(Compilation compilation, SyntaxNode node, ISymbol symbol, AttributeData attribute, AnalyzerConfigOptions options)
        => GenerateCodeEx(compilation, node, (IFieldSymbol)symbol, attribute, options);

    protected override SyntaxNode Node(FieldDeclarationSyntax node)
        => node.Declaration.Variables.Single();
}
