// <copyright file="DocumentationClassCodeFixProvider.cs" company="Palantir (Pty) Ltd">
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DocumentationPropertyCodeFixProvider))]
    [Shared]
    public class DocumentationClassCodeFixProvider : CodeFixProvider
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
        /// Initializes a new instance of the <see cref="DocumentationClassCodeFixProvider"/> class.
        /// </summary>
        public DocumentationClassCodeFixProvider()
        {
            this._textFactory = new CommentTextFactory(new AccessLevelService());
            this._commentNodeFactory = new CommentNodeFactory(
                new CommentTextFactory(new AccessLevelService()));
        }

        /// <summary>
        /// Gets Diagnostic Ids for which a quick fix is associated.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("SA1606D");

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
            this.RegisterPropertyDocumentationCodeFix(root, context, context.Diagnostics.First());
        }

        /// <summary>
        /// Register the property fix for property documentation.
        /// </summary>
        /// <param name="root">the syntax root node.</param>
        /// <param name="context">the code fix context, containing the location of the fix.</param>
        /// <param name="diagnostic">the diagnostic, where the invalid code was located.</param>
        private void RegisterPropertyDocumentationCodeFix(SyntaxNode root, CodeFixContext context, Diagnostic diagnostic)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan);
            this.TryCorrectClassDocumentation(root, context, diagnostic, node);
            this.TryCorrectInterfaceDocumentation(root, context, diagnostic, node);
        }

        /// <summary>
        /// Correct the interface documentation.
        /// </summary>
        /// <param name="root">the root node.</param>
        /// <param name="context">the correction context.</param>
        /// <param name="diagnostic">the diagnostic being fixed.</param>
        /// <param name="node">the current ndoe.</param>
        private void TryCorrectInterfaceDocumentation(SyntaxNode root, CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
        {
            var interfaceDeclaration = node as InterfaceDeclarationSyntax;
            if (interfaceDeclaration == null)
                return;

            var documentationStructure = interfaceDeclaration.GetDocumentationCommentTriviaSyntax();
            var action = CodeAction.Create(
                "Generate class documentation.",
                c => this.AddDocumentationAsync(context, root, interfaceDeclaration, documentationStructure),
                "SA1623D");
            context.RegisterCodeFix(
                action,
                diagnostic);
        }

        /// <summary>
        /// Correct the class documentation.
        /// </summary>
        /// <param name="root">the root node.</param>
        /// <param name="context">the correction context.</param>
        /// <param name="diagnostic">the diagnostic being fixed.</param>
        /// <param name="node">the current ndoe.</param>
        private void TryCorrectClassDocumentation(SyntaxNode root, CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
        {
            var classDeclaration = node as ClassDeclarationSyntax;
            if (classDeclaration == null)
                return;

            var documentationStructure = classDeclaration.GetDocumentationCommentTriviaSyntax();
            var action = CodeAction.Create(
                "Generate class documentation.",
                c => this.AddDocumentationAsync(context, root, classDeclaration, documentationStructure),
                "SA1623D");
            context.RegisterCodeFix(
                action,
                diagnostic);
        }

        /// <summary>
        /// Add documentation for the property.
        /// </summary>
        /// <param name="context">the code fix context.</param>
        /// <param name="root">the root syntax node.</param>
        /// <param name="typeDeclaration">the property declaration containing invalid documentation.</param>
        /// <param name="documentComment">the existing comment.</param>
        /// <returns>the correct code.</returns>
        private Task<Document> AddDocumentationAsync(
            CodeFixContext context,
            SyntaxNode root,
            TypeDeclarationSyntax typeDeclaration,
            DocumentationCommentTriviaSyntax documentComment)
        {
            var summary = this._commentNodeFactory.CreateCommentSummaryText(typeDeclaration);
            var typeParameters = this._commentNodeFactory
                .CreateTypeParameters(typeDeclaration, documentComment)
                .ToArray();
            var all = typeParameters != null && typeParameters.Any()
                ? new XmlNodeSyntax[] { summary }.Concat(typeParameters).ToArray()
                : new XmlNodeSyntax[] { summary };

            var comment = this._commentNodeFactory
                .CreateDocumentComment(all)
                .WithAdditionalAnnotations(Formatter.Annotation);

            var trivia = SyntaxFactory.Trivia(comment);
            var replacement = typeDeclaration.WithLeadingTrivia(trivia);
            var result = root.ReplaceNode(typeDeclaration, replacement);

            var newDocument = context.Document.WithSyntaxRoot(result);
            return Task.FromResult(newDocument);
        }
    }
}