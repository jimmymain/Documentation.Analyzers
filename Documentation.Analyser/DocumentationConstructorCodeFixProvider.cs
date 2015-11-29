// <copyright file="DocumentationConstructorCodeFixProvider.cs" company="Palantir (Pty) Ltd">
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DocumentationConstructorCodeFixProvider)), Shared]
    public class DocumentationConstructorCodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// the comment node factory instance.
        /// </summary>
        private readonly CommentNodeFactory _commentNodeFactory;

        /// <summary>
        /// the text factory used to extract / generate content.
        /// </summary>
        private readonly ICommentTextFactory _commentTextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationConstructorCodeFixProvider"/> class.
        /// </summary>
        public DocumentationConstructorCodeFixProvider()
        {
            this._commentTextFactory = new CommentTextFactory(
                new AccessLevelService());
            this._commentNodeFactory = new CommentNodeFactory(
                new CommentTextFactory(new AccessLevelService()));
        }

        /// <summary>
        /// Diagnostic Ids for which a quick fix is associated.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("SA1612");

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
            var constructorDeclaration = startNode as ConstructorDeclarationSyntax;
            if (constructorDeclaration != null)
                this.RegisterConstructorCodeFix(constructorDeclaration, root, context, diagnostic);
        }

        /// <summary>
        /// register the constructor code fix.
        /// </summary>
        /// <param name="constructorDeclaration">the constructor declaration.</param>
        /// <param name="root">the syntax root.</param>
        /// <param name="context">the code fix context.</param>
        /// <param name="diagnostic">the diagnostic detail.</param>
        private void RegisterConstructorCodeFix(ConstructorDeclarationSyntax constructorDeclaration, SyntaxNode root, CodeFixContext context, Diagnostic diagnostic)
        {
            var documentationStructure = constructorDeclaration.GetDocumentationCommentTriviaSyntax();
            var action = CodeAction.Create(
                "SA1642",
                c => this.AddDocumentationAsync(
                    context,
                    root,
                    constructorDeclaration,
                    documentationStructure),
                "SA1642");
            context.RegisterCodeFix(
                action,
                diagnostic);
        }

        /// <summary>
        /// add documentation for the constructor.
        /// </summary>
        /// <param name="context">the code fix context.</param>
        /// <param name="root">the syntax root.</param>
        /// <param name="constructorDeclaration">the constructor declaration syntax.</param>
        /// <param name="documentComment">the document content.</param>
        /// <returns>the resulting document.</returns>
        private Task<Document> AddDocumentationAsync(CodeFixContext context, SyntaxNode root, ConstructorDeclarationSyntax constructorDeclaration, DocumentationCommentTriviaSyntax documentComment)
        {
            var lines = documentComment.GetExistingSummaryCommentDocumentation() ?? new string[] { };
            var standardCommentText = this._commentNodeFactory.PrependStandardCommentText(constructorDeclaration, lines);

            var parameters = this._commentNodeFactory.CreateParameters(constructorDeclaration, documentComment);

            var summaryPlusParameters = new XmlNodeSyntax[] { standardCommentText }
                .Concat(parameters)
                .ToArray();

            var comment = this._commentNodeFactory
                .CreateDocumentComment(summaryPlusParameters)
                .AddLeadingEndOfLineTriviaFrom(constructorDeclaration.GetLeadingTrivia());

            var trivia = SyntaxFactory.Trivia(comment);
            var result = documentComment != null
                ? root.ReplaceNode(documentComment, comment.AdjustDocumentationCommentNewLineTrivia())
                : root.ReplaceNode(constructorDeclaration, constructorDeclaration.WithLeadingTrivia(trivia));

            var newDocument = context.Document.WithSyntaxRoot(result);
            return Task.FromResult(newDocument);
        }
    }
}