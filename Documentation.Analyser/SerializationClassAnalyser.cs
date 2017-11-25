// <copyright file="SerializationClassAnalyser.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// serialization class analyser.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SerializationClassAnalyser : DiagnosticAnalyzer
    {
        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(this.ClassDescriptor);

        /// <summary>
        /// the analysis descriptor.
        /// </summary>
        private DiagnosticDescriptor ClassDescriptor => new DiagnosticDescriptor(
            "SERI001",
            "Generate Serialization",
            "Generate Serialization",
            "Serialization Rules",
            DiagnosticSeverity.Info,
            true,
            "Generate Poco Class Serialization",
            "https://github.com/jimmymain/documentation.analyzers/blob/master/README.md");

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <param name="context">the analysis context</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(this.HandleCompilationStart);
        }

        /// <summary>
        /// analyse the content of the 'Class' declaration.
        /// </summary>
        /// <param name="context">the context.</param>
        private void AnalyseClassSerialisation(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case ClassDeclarationSyntax declaration:
                    if (declaration.SyntaxTree.IsGeneratedCode(context.CancellationToken))
                        return;

                    var hasMethods = declaration.HasMethods();
                    if (hasMethods)
                        return;

                    var hasDataContract = declaration.HasDataContextAttribute();
                    if (!hasDataContract)
                        return;

                    var diagnostic = Diagnostic.Create(this.ClassDescriptor, declaration.Identifier.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                    break;
            }
        }

        /// <summary>
        /// Handle the property declaration, adding a diagnostic for properties
        /// that are not documented.
        /// </summary>
        /// <param name="context">the analysis context.</param>
        private void HandleClassSerialisation(SyntaxNodeAnalysisContext context)
        {
            this.AnalyseClassSerialisation(context);
        }

        /// <summary>
        /// Documentation analysis is done only on compilation start, to delay
        /// </summary>
        /// <param name="context">the compilation start action.</param>
        private void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                this.HandleClassSerialisation,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.ClassDeclaration);
        }
    }
}