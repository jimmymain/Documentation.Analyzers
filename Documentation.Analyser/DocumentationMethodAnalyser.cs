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
                    "SA1612D",
                    "methods must be correctly documented.",
                    "method documentation: {0}.",
                    "Documentation Rules",
                    DiagnosticSeverity.Warning,
                    true,
                    "A C# code element is missing documentation.",
                    "https://github.com/jimmymain/documentation.analyzers/blob/master/SA1612D.md");
            }
        }

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <param name="context">the analysis context</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(this.HandleCompilationStart);
        }

        /// <summary>
        /// Documentation analysis is done only on compilation start, to delay
        /// </summary>
        /// <param name="context">the compilation start action.</param>
        private void HandleCompilationStart(CompilationStartAnalysisContext context)
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
            if (!context.IsDocumentationModeOn())
                return;

            var declaration = (MethodDeclarationSyntax)context.Node;
            if (declaration.SyntaxTree.IsGeneratedCode(context.CancellationToken))
                return;

            var hasDocumentation = declaration.HasDocumentation();
            if (!hasDocumentation || !this.ValidateParameters(declaration) || !this.ValidateReturnValue(declaration))
            {
                var description = hasDocumentation
                    ? this.GetUndocumentedDescription(declaration)
                    : "no documentation";
                var diagnostic = Diagnostic.Create(this.Descriptor, declaration.Identifier.GetLocation(), description);
                context.ReportDiagnostic(diagnostic);
            }
        }

        /// <summary>
        /// Validate that the return value is present if necessary.
        /// </summary>
        /// <param name="declaration">the method declaration syntax.</param>
        /// <returns>true if the return value is correctly documented.</returns>
        private bool ValidateReturnValue(MethodDeclarationSyntax declaration)
        {
            if (declaration.HasVoidReturnType())
                return true; // void is valid regardless,
            return declaration
                .GetDocumentationCommentTriviaSyntax()
                .GetReturnDocumentationElement() != null;
        }

        /// <summary>
        /// Check if the existing documentation is invalid.
        /// </summary>
        /// <param name="declaration">the declaration.</param>
        /// <returns>true if the documentation is invalid.</returns>
        private bool ValidateParameters(MethodDeclarationSyntax declaration)
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

        /// <summary>
        /// return the list of undocumented parameters.
        /// </summary>
        /// <param name="declaration">the method declaration.</param>
        /// <returns>a string containing the missing parameters.</returns>
        private string GetUndocumentedDescription(MethodDeclarationSyntax declaration)
        {
            var commentSyntax = declaration.GetDocumentationCommentTriviaSyntax();
            var parameters = declaration
                .ParameterList
                .Parameters
                .Select(_ => _.Identifier.Text)
                .ToArray();
            var documentedParameter = commentSyntax
                .GetParameterDocumentationElements()
                .Where(_ => _.GetXmlTextSyntaxLines().Any())
                .ToArray()
                .GetParameterNames();

            // check missing parameters.
            var missing = parameters
                .Except(documentedParameter)
                .Select(_ => $"'{_}'")
                .ToArray();
            if (missing.Any())
                return $"missing {string.Join(", ", missing)}";

            // check extra parameters.
            var extra = documentedParameter
                .Except(parameters)
                .Select(_ => $"'{_}'")
                .ToArray();

            if (extra.Any())
                return $"additional {string.Join(", ", extra)}";

            if (!declaration.HasVoidReturnType() && commentSyntax.GetReturnDocumentationElement() == null)
                return "missing return value documentation";

            return "invalid";
        }
    }
}