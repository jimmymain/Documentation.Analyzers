// <copyright file="SerializationClassCodeFixProvider.cs" company="Palantir (Pty) Ltd">
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
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Code Fix Provider for Serialization
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SerializationClassCodeFixProvider)), Shared]
    public class SerializationClassCodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// the attribute factory.
        /// </summary>
        private readonly IAttributeFactory _attributeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationClassCodeFixProvider"/> class.
        /// </summary>
        public SerializationClassCodeFixProvider()
        {
            this._attributeFactory = new AttributeFactory();
        }

        /// <summary>
        /// Diagnostic Ids for which a quick fix is associated.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("SERI001");

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
            this.RegisterClassSerializationFix(root, context, context.Diagnostics.First());
        }

        /// <summary>
        /// Register the property fix for property documentation.
        /// </summary>
        /// <param name="root">the syntax root node.</param>
        /// <param name="context">the code fix context, containing the location of the fix.</param>
        /// <param name="diagnostic">the diagnostic, where the invalid code was located.</param>
        private void RegisterClassSerializationFix(SyntaxNode root, CodeFixContext context, Diagnostic diagnostic)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan);
            this.TryCorrectClassSerialization(root, context, diagnostic, node);
        }

        /// <summary>
        /// Correct the class documentation.
        /// </summary>
        /// <param name="root">the root node.</param>
        /// <param name="context">the correction context.</param>
        /// <param name="diagnostic">the diagnostic being fixed.</param>
        /// <param name="node">the current ndoe.</param>
        private void TryCorrectClassSerialization(SyntaxNode root, CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
        {
            switch (node)
            {
                case ClassDeclarationSyntax classDeclaration:
                    var action = CodeAction.Create(
                        "Generate Serialization",
                        c => this.AddMemberSerializationAttributes(
                            context,
                            root,
                            classDeclaration),
                        "SERI001");
                    context.RegisterCodeFix(
                        action,
                        diagnostic);
                    break;
            }
        }

        /// <summary>
        /// add the member serialization attributes, and return them.
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="root">the root syntax node.</param>
        /// <param name="classDeclaration">the class declaration</param>
        /// <returns>the complete file.</returns>
        private Task<Document> AddMemberSerializationAttributes(CodeFixContext context, SyntaxNode root, ClassDeclarationSyntax classDeclaration)
        {
            var replacement = this._attributeFactory.CreateDataMemberAttributes(classDeclaration);
            var result = root.ReplaceNode(classDeclaration, replacement);
            var newDocument = context.Document.WithSyntaxRoot(result);
            return Task.FromResult(newDocument);
        }
    }
}