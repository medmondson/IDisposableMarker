using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IDisposableAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    // ReSharper disable once InconsistentNaming
    public class IDisposableAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "IDisposableAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSymbolAction(AnalyzeObjectCreation, SymbolKind.NamedType);
            context.RegisterSyntaxNodeAction(c=> AnalyzeObjectCreation(c), SyntaxKind.ObjectCreationExpression);
            //Still identifying class declarations and NOT the instantiations
            //Strongly suspect it's the symbol action being registered is of the wrong type
            //Checkout http://stackoverflow.com/questions/33433487/where-can-i-find-what-symbol-types-are-under-different-symbol-kinds-in-roslyn/33435449
        }

        private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
        {
            //MAKING PROGRESS - PICK UP FROM https://msdn.microsoft.com/en-us/library/mt297717.aspx

            ObjectCreationExpressionSyntax objectCreation = (ObjectCreationExpressionSyntax) context.Node;
            //objectCreation.
            
            //SymbolInfo symbolInfo = context.SemanticModel.GetDeclaredSymbol()

            //var symbol = symbolInfo.Symbol as ITypeSymbol;
            //if (symbol == null)
            //    return;
            
            //var interfaces = symbol.Interfaces;

            //if (!interfaces.Any(i => i.Name == "IDisposable"))
            //    return;

            //var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name);
            //context.ReportDiagnostic(diagnostic);

        }
    }
}
