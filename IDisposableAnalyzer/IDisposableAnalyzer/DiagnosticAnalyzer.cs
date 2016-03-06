using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IDisposableAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IDisposableAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "IDisposableAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType); //think this is right as it'll be an object i.e new Disposable
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            ImmutableArray<INamedTypeSymbol> allInterfaces = namedTypeSymbol.AllInterfaces;
            //var typeHaveInterfaces = allInterfaces.Any(x => x.Name == "IDisposable");


            //var typeHaveInterfaces = interfaces.Any(x => x.Interfaces[0].Name == "IDisposable");
            //if (!typeHaveInterfaces)
            //    return;

            foreach (var disposables in allInterfaces.Where(x => x.Name == "IDisposable"))
            {
                var diagnostic = Diagnostic.Create(Rule, disposables.Locations[0], namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic); //Succesfully hit - but message isn't displayed
            }
        }
    }
}
