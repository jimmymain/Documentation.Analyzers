// <copyright file="DocumentationMethodCodeFixProvider.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Code fix provider for all documentation.
    /// http://roslynquoter.azurewebsites.net/
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DocumentationMethodCodeFixProvider)), Shared]
    public class DocumentationMethodCodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// the comment node factory instance.
        /// </summary>
        private readonly CommentNodeFactory _commentNodeFactory;

        /// <summary>
        /// a text factory to create comment text based
        /// on the context.
        /// </summary>
        private readonly ICommentTextFactory _textFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationMethodCodeFixProvider"/> class.
        /// </summary>
        public DocumentationMethodCodeFixProvider()
        {
            this._textFactory = new CommentTextFactory(
                new AccessLevelService());
            this._commentNodeFactory = new CommentNodeFactory(
                new CommentTextFactory(new AccessLevelService()));
        }

        /// <summary>
        /// Diagnostic Ids for which a quick fix is associated.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("SA1612D");

        /// <summary>
        /// Return the registered provider of quick fixes.
        /// </summary>
        /// <returns>the provider instance.</returns>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <summary>
        /// Register all the code fixes supplied by this library.
        /// </summary>
        /// <param name="context">the code fix context.</param>
        /// <returns>the registered code fixes.</returns>
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            this.RegisterMethodDocumentationCodeFix(root, context, context.Diagnostics.First());
        }

        /// <summary>
        /// Register the property fix for property documentation.
        /// </summary>
        /// <param name="root">the syntax root node.</param>
        /// <param name="context">the code fix context, containing the location of the fix.</param>
        /// <param name="diagnostic">the diagnostic, where the invalid code was located.</param>
        private void RegisterMethodDocumentationCodeFix(SyntaxNode root, CodeFixContext context, Diagnostic diagnostic)
        {
            var startNode = root.FindNode(diagnostic.Location.SourceSpan);
            var methodDeclarationSyntax = startNode as MethodDeclarationSyntax;
            if (methodDeclarationSyntax != null)
                this.RegisterMethodCodeFix(methodDeclarationSyntax, root, context, diagnostic);
        }

        private void RegisterMethodCodeFix(MethodDeclarationSyntax methodDeclarationSyntax, SyntaxNode root, CodeFixContext context, Diagnostic diagnostic)
        {
            var documentationStructure = methodDeclarationSyntax.GetDocumentationCommentTriviaSyntax();
            var action = CodeAction.Create(
                "Generate method documentation.",
                c => this.AddDocumentationAsync(
                    context,
                    root,
                    methodDeclarationSyntax,
                    documentationStructure),
                "SA1612D");
            context.RegisterCodeFix(
                action,
                diagnostic);
        }

        /// <summary>
        /// Add documentation for the property.
        /// </summary>
        /// <param name="context">the code fix context.</param>
        /// <param name="root">the root syntax node.</param>
        /// <param name="methodDeclaration">the property declaration containing invalid documentation.</param>
        /// <param name="documentComment">the existing comment.</param>
        /// <returns>the correct code.</returns>
        private Task<Document> AddDocumentationAsync(
            CodeFixContext context,
            SyntaxNode root,
            MethodDeclarationSyntax methodDeclaration,
            DocumentationCommentTriviaSyntax documentComment)
        {
            var summary = this._commentNodeFactory.GetExistingSummaryCommentText(documentComment)
                          ?? this._commentNodeFactory.CreateCommentSummaryText(methodDeclaration);
            var @class = methodDeclaration.Parent as ClassDeclarationSyntax;
            var first = @class?.DescendantNodes().FirstOrDefault() == methodDeclaration;

            var parameters = this._commentNodeFactory.CreateParameters(methodDeclaration, documentComment);

            var summaryPlusParameters = new XmlNodeSyntax[] { summary }
                .Concat(parameters)
                .ToArray();

            var comment = this._commentNodeFactory
                .CreateDocumentComment(summaryPlusParameters)
                .AddLeadingEndOfLineTriviaFrom(methodDeclaration.GetLeadingTrivia());

            var trivia = SyntaxFactory.Trivia(comment);
            var methodTrivia = first
                   ? methodDeclaration.WithLeadingTrivia(trivia)
                   : methodDeclaration.WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed, trivia);
            var result = documentComment != null
                ? root.ReplaceNode(documentComment, comment.AdjustDocumentationCommentNewLineTrivia())
                : root.ReplaceNode(methodDeclaration, methodTrivia);

            var newDocument = context.Document.WithSyntaxRoot(result);
            return Task.FromResult(newDocument);
        }
    }
}