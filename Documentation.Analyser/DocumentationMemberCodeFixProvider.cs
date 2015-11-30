// <copyright file="DocumentationMemberCodeFixProvider.cs" company="Palantir (Pty) Ltd">
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
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// Code fix provider for all documentation.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DocumentationPropertyCodeFixProvider)), Shared]
    public class DocumentationMemberCodeFixProvider : CodeFixProvider
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
        /// Initializes a new instance of the <see cref="DocumentationMemberCodeFixProvider"/> class.
        /// </summary>
        public DocumentationMemberCodeFixProvider()
        {
            this._textFactory = new CommentTextFactory(new AccessLevelService());
            this._commentNodeFactory = new CommentNodeFactory(
                new CommentTextFactory(new AccessLevelService()));
        }

        /// <summary>
        /// Diagnostic Ids for which a quick fix is associated.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("SA1600");

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
            this.RegisterMemberDocumentationCodeFix(root, context, context.Diagnostics.First());
        }

        /// <summary>
        /// Register the property fix for property documentation.
        /// </summary>
        /// <param name="root">the syntax root node.</param>
        /// <param name="context">the code fix context, containing the location of the fix.</param>
        /// <param name="diagnostic">the diagnostic, where the invalid code was located.</param>
        private void RegisterMemberDocumentationCodeFix(SyntaxNode root, CodeFixContext context, Diagnostic diagnostic)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan);
            var fieldDeclarationSyntax = node.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();

            var documentationStructure = fieldDeclarationSyntax.GetDocumentationCommentTriviaSyntax();
            var action = CodeAction.Create(
                "SA1600",
                c => this.AddDocumentationAsync(context, root, fieldDeclarationSyntax, documentationStructure),
                "SA1600");
            context.RegisterCodeFix(
                action,
                diagnostic);
        }

        /// <summary>
        /// Add documentation for the property.
        /// </summary>
        /// <param name="context">the code fix context.</param>
        /// <param name="root">the root syntax node.</param>
        /// <param name="fieldDeclaration">the property declaration containing invalid documentation.</param>
        /// <param name="documentComment">the existing comment.</param>
        /// <returns>the correct code.</returns>
        private Task<Document> AddDocumentationAsync(
            CodeFixContext context,
            SyntaxNode root,
            FieldDeclarationSyntax fieldDeclaration,
            DocumentationCommentTriviaSyntax documentComment)
        {
            var @class = fieldDeclaration.Parent as ClassDeclarationSyntax;
            var first = @class?.DescendantNodes().FirstOrDefault() == fieldDeclaration;

            var summary = this._commentNodeFactory.CreateCommentSummaryText(fieldDeclaration);

            var comment = this._commentNodeFactory
                .CreateDocumentComment(summary)
                .WithAdditionalAnnotations(Formatter.Annotation);

            var trivia = SyntaxFactory.Trivia(comment);
            var pd = first
                ? fieldDeclaration.WithLeadingTrivia(trivia)
                : fieldDeclaration.WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed, trivia);
            var result = documentComment != null
                ? root.ReplaceNode(documentComment, comment.AdjustDocumentationCommentNewLineTrivia())
                : root.ReplaceNode(fieldDeclaration, pd);

            var newDocument = context.Document.WithSyntaxRoot(result);
            return Task.FromResult(newDocument);
        }
    }
}