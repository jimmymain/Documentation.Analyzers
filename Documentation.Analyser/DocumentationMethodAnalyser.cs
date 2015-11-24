// <copyright file="DocumentationMethodAnalyser.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// the analyser to determine whether or not a documentation
    /// quick fix is needed.
    /// Reference: http://roslynquoter.azurewebsites.net/
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DocumentationMethodAnalyser : DiagnosticAnalyzer
    {
        /// <summary>
        /// the text factory.
        /// </summary>
        private readonly ICommentTextFactory _commentTextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationMethodAnalyser"/> class.
        /// </summary>
        public DocumentationMethodAnalyser()
        {
            this._commentTextFactory = new CommentTextFactory(new AccessLevelService());
        }

        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(this.Descriptor);
            }
        }

        /// <summary>
        /// the analysis descriptor.
        /// </summary>
        private DiagnosticDescriptor Descriptor
        {
            get
            {
                return new DiagnosticDescriptor(
                    "SA1612",
                    "Methods must be documented",
                    "Methods must be documented",
                    "Documentation Rules",
                    DiagnosticSeverity.Warning,
                    true,
                    "A C# code element is missing documentation.",
                    "https://github.com/jimmymain/documentation.analyzers/blob/master/SA1612.md");
            }
        }

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <param name="context">the analysis context</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                this.HandleMethodDeclaration,
                SyntaxKind.MethodDeclaration);
        }

        /// <summary>
        /// Handle the property declaration, adding a diagnostic for properties
        /// that are not documented.
        /// </summary>
        /// <param name="context">the analysis context.</param>
        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (MethodDeclarationSyntax)context.Node;
            {
                if (declaration.SyntaxTree.IsGeneratedCode(context.CancellationToken))
                    return;

                if (!declaration.HasDocumentation() || !this.ValidDocumentation(declaration))
                {
                    var diagnostic = Diagnostic.Create(this.Descriptor, declaration.Identifier.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        /// <summary>
        /// Check if the existing documentation is invalid.
        /// </summary>
        /// <param name="declaration">the declaration.</param>
        /// <returns>true if the documentation is invalid.</returns>
        private bool ValidDocumentation(MethodDeclarationSyntax declaration)
        {
            var commentSyntax = declaration.GetDocumentationCommentTriviaSyntax();
            var parameters = declaration
                .ParameterList
                .Parameters
                .Select(_ => _.Identifier.Text);
            var documentedParameter = commentSyntax
                .GetParameterDocumentationElements()
                .Where(_ => _.GetXmlTextSyntaxLines().Any())
                .ToArray()
                .GetParameterNames();

            // not certain this is the best way, I will tweak it later.
            return parameters.SequenceEqual(documentedParameter);
        }
    }
}