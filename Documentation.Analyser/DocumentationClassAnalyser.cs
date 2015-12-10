// <copyright file="DocumentationClassAnalyser.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// the analyser to determine whether or not a documentation
    /// quick fix is needed.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DocumentationClassAnalyser : DiagnosticAnalyzer
    {
        /// <summary>
        /// the text factory.
        /// </summary>
        private readonly ICommentTextFactory _commentTextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationClassAnalyser"/> class.
        /// </summary>
        public DocumentationClassAnalyser()
        {
            this._commentTextFactory = new CommentTextFactory(new AccessLevelService());
        }

        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this.Descriptor);

        /// <summary>
        /// the analysis descriptor.
        /// </summary>
        private DiagnosticDescriptor Descriptor
        {
            get
            {
                return new DiagnosticDescriptor(
                    "SA1606D",
                    "Classes must be correctly documented",
                    "class documentation: no documentation.",
                    "Documentation Rules",
                    DiagnosticSeverity.Warning,
                    true,
                    "A C# code element is missing documentation.",
                    "https://github.com/jimmymain/documentation.analyzers/blob/master/README.md");
            }
        }

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <param name="context">the analysis context</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                this.HandlePropertyDeclaration,
                SyntaxKind.ClassDeclaration);
        }

        /// <summary>
        /// Handle the property declaration, adding a diagnostic for properties
        /// that are not documented.
        /// </summary>
        /// <param name="context">the analysis context.</param>
        private void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (ClassDeclarationSyntax)context.Node;
            {
                if (declaration.SyntaxTree.IsGeneratedCode(context.CancellationToken))
                    return;

                var hasDocumentation = declaration.HasDocumentation();
                if (!hasDocumentation)
                {
                    var diagnostic = Diagnostic.Create(this.Descriptor, declaration.Identifier.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}