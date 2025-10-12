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
                (SymbolEqualityComparer.Default.Equals(f.Type, symbol)
                    ||
                // HasAutoEnumIncludeAttribute(f)
                InheritsFrom(f.Type, symbol)
                )
                )
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
    private static bool HasAutoEnumIncludeAttribute(IFieldSymbol f)
    {
        return f.GetAttributes().Any(a =>
            a.AttributeClass?.Name is "AutoEnumIncludeAttribute" or "AutoEnumInclude");
    }

    private static bool InheritsFrom(ITypeSymbol type, INamedTypeSymbol baseType)
    {
        if (type is null)
            return false;

        // Nếu cùng type thì true
        if (SymbolEqualityComparer.Default.Equals(type, baseType))
            return true;

        // Kiểm tra chain kế thừa
        var current = type.BaseType;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
                return true;

            current = current.BaseType;
        }

        return false;
    }
    protected override string Str()
    {
        return string.Join("\n", EnumMembers.Select(x => $" - Member: {x}"))
             + $"\nIdentityProperty: {IdentityProperty}"
             + $"\nDeclarationKeyword: {DeclarationKeyword}";
    }
}
