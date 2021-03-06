﻿// <copyright file="DocumentationConstructorAnalyser.cs" company="Palantir (Pty) Ltd">
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
    public class DocumentationConstructorAnalyser : DiagnosticAnalyzer
    {
        /// <summary>
        /// the text factory.
        /// </summary>
        private readonly ICommentTextFactory _commentTextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationConstructorAnalyser"/> class.
        /// </summary>
        public DocumentationConstructorAnalyser()
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
                    "SA1642D",
                    "constructors must be correctly documented.",
                    "constructors must be correctly documented.",
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
                this.HandleConstructorDeclaration,
                SyntaxKind.ConstructorDeclaration);
        }

        /// <summary>
        /// handle the case of a constructor, and associated parameters.
        /// </summary>
        /// <param name="context">the constructor declaration context.</param>
        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!context.IsDocumentationModeOn())
                return;

            var declaration = (ConstructorDeclarationSyntax)context.Node;
            if (declaration.SyntaxTree.IsGeneratedCode(context.CancellationToken))
                return;

            if (declaration.Modifiers.Any(SyntaxKind.StaticKeyword))
                return;

            if (!declaration.HasDocumentation() || !this.ValidDocumentation(declaration))
            {
                var diagnostic = Diagnostic.Create(this.Descriptor, declaration.Identifier.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        /// <summary>
        /// Check if the existing documentation is valid.
        /// </summary>
        /// <param name="declaration">the constructor declaration.</param>
        /// <returns>true if the constructor already contains valid documentation.</returns>
        private bool ValidDocumentation(ConstructorDeclarationSyntax declaration)
        {
            var commentSyntax = declaration.GetDocumentationCommentTriviaSyntax();
            var lines = commentSyntax.GetXmlTextSyntax();

            var typeDeclaration = declaration.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();
            if (!(typeDeclaration is ClassDeclarationSyntax))
                return true; // if it's not a class, skip it.

            if (lines.IndexOf("Initializes a new instance of the", StringComparison.CurrentCultureIgnoreCase) < 0)
                return false;

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