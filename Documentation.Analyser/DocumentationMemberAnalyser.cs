// <copyright file="DocumentationMemberAnalyser.cs" company="Palantir (Pty) Ltd">
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
    public class DocumentationMemberAnalyser : DiagnosticAnalyzer
    {
        /// <summary>
        /// the text factory.
        /// </summary>
        private readonly ICommentTextFactory _commentTextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationMemberAnalyser"/> class.
        /// </summary>
        public DocumentationMemberAnalyser()
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
                    "SA1600D",
                    "members must be correctly documented.",
                    "members must be correctly documented.",
                    "Documentation Rules",
                    DiagnosticSeverity.Warning,
                    true,
                    "A C# code element is missing documentation.",
                    "https://github.com/jimmymain/documentation.analyzers/blob/master/SA1600D.md");
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
                this.HandleMemberDeclaration,
                SyntaxKind.FieldDeclaration);
        }

        /// <summary>
        /// handle the case of a constructor, and associated parameters.
        /// </summary>
        /// <param name="context">the constructor declaration context.</param>
        private void HandleMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!context.IsDocumentationModeOn())
                return;

            var declaration = (FieldDeclarationSyntax)context.Node;
            if (declaration.SyntaxTree.IsGeneratedCode(context.CancellationToken))
                return;

            var variable = declaration
                .DescendantNodesAndSelf()
                .OfType<VariableDeclaratorSyntax>()
                .FirstOrDefault();
            if (variable != null && !declaration.HasDocumentation())
            {
                var diagnostic = Diagnostic.Create(this.Descriptor, variable.Identifier.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}