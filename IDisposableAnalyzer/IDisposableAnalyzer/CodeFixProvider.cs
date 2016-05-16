using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDisposableAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IDisposableAnalyzerCodeFixProvider)), Shared]
    // ReSharper disable once InconsistentNaming
    public class IDisposableAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Encapsuate expression in using statement";
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(IDisposableAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;               

            // Find the type declaration identified by the diagnostic.
            LocalDeclarationStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().SingleOrDefault();

            var codeAction = CodeAction.Create(Title, c => PlaceInUsing(context.Document, declaration, c), Title);

            context.RegisterCodeFix(
                codeAction, diagnostic
                );
        }
        
        private static async Task<Document> PlaceInUsing(Document document, LocalDeclarationStatementSyntax typeDecl, CancellationToken cancellationToken)
        {
            IEnumerable<SyntaxNode> oldNode = typeDecl.DescendantNodes().OfType<VariableDeclarationSyntax>();
      
            //https://blogs.msdn.microsoft.com/csharpfaq/2012/02/06/implementing-a-code-action-using-roslyn/

            // Replace the old local declaration with the new local declaration.
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            LocalDeclarationStatementSyntax originalNode = (LocalDeclarationStatementSyntax) oldNode.Single().Parent;

            string variableName = originalNode.Declaration.Variables[0].Identifier.Text;

            IdentifierNameSyntax identifier = (IdentifierNameSyntax)((ObjectCreationExpressionSyntax) originalNode.Declaration.Variables[0].Initializer.Value).Type;
            String typeName = identifier.Identifier.Text;
      
            SyntaxNode syntax = SyntaxFactory.UsingStatement(SyntaxFactory.Block())
                    .WithDeclaration(SyntaxFactory
                        .VariableDeclaration(SyntaxFactory.IdentifierName("var"))
                        .WithVariables(SyntaxFactory.SingletonSeparatedList(SyntaxFactory
                            .VariableDeclarator(SyntaxFactory.Identifier(variableName))
                            .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory
                                .ObjectCreationExpression(SyntaxFactory.IdentifierName(typeName))
                                    .WithArgumentList(SyntaxFactory.ArgumentList()))))));

            SyntaxNode newRoot = oldRoot.ReplaceNode(originalNode, new List<SyntaxNode> { syntax });

            // Return document with transformed tree.
            return document.WithSyntaxRoot(newRoot);

        }
    }
}