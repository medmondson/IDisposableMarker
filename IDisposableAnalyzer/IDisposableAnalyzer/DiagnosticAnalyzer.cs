using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using InvocationExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax;

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
            context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
            //Still identifying class declarations and NOT the instantiations
            //Strongly suspect it's the symbol action being registered is of the wrong type
            //Checkout http://stackoverflow.com/questions/33433487/where-can-i-find-what-symbol-types-are-under-different-symbol-kinds-in-roslyn/33435449
        }

        private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
        {
            ObjectCreationExpressionSyntax objectCreation = (ObjectCreationExpressionSyntax) context.Node;

            SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(context.Node);

            var symbol = symbolInfo.Symbol as IMethodSymbol;
            
            if (symbol == null)
                return;

            var interfaces = symbol.ContainingType.Interfaces;

            if (!interfaces.Any(i => i.Name == "IDisposable"))
                return;

            Location location = objectCreation.GetLocation();

            var diagnostic = Diagnostic.Create(Rule, location, symbol.ReceiverType.Name);
            context.ReportDiagnostic(diagnostic);

        }
    }
}
