using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GodotSharp.SourceGenerators.AutoEnumExtensions;

internal class AutoEnumDataModel : ClassDataModel
{
    public string IdentityProperty { get; } = "";
    public string[] EnumMembers { get; }

    public string DeclarationKeyword { get; }

    public AutoEnumDataModel(INamedTypeSymbol symbol, string identityProperty = "")
        : base(symbol)
    {
        IdentityProperty = identityProperty;

        EnumMembers = symbol
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f =>
                f.IsStatic &&
                f.IsReadOnly &&
                SymbolEqualityComparer.Default.Equals(f.Type, symbol))
            .Select(f => f.Name)
            .ToArray();

        DeclarationKeyword = symbol.DeclaringSyntaxReferences
            .Select(r => r.GetSyntax())
            .OfType<TypeDeclarationSyntax>()
            .Select(s => s.Kind() switch
            {
                SyntaxKind.RecordDeclaration => "record",
                SyntaxKind.StructDeclaration => "struct",
                _ => "class"
            }).FirstOrDefault() ?? "class";
    }

    protected override string Str()
    {
        return string.Join("\n", EnumMembers.Select(x => $" - Member: {x}"))
             + $"\nIdentityProperty: {IdentityProperty}"
             + $"\nDeclarationKeyword: {DeclarationKeyword}";
    }
}
