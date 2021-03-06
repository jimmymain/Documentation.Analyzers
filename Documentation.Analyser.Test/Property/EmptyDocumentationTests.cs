﻿// <copyright file="EmptyDocumentationTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Property
{
    using Analyser;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// Documentation quick fix unit tests.
    /// </summary>
    public class EmptyDocumentationTests : CodeFixVerifier
    {
        /// <summary>
        /// verify the diagnostic finds violation.
        /// </summary>
        [Fact]
        public void VerifyEmptyDocumentationDiagnostic()
        {
            var test = string.Empty;

            this.VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// verify the quick fix.
        /// </summary>
        [Fact]
        public void VerifyEmptyQuickFixReplacesDocumentation()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// </summary>
        public string TestProperty { get; set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1623D",
                Message = $"property documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 8, 23) }
            };

            this.VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// Gets or sets the test property.
        /// </summary>
        public string TestProperty { get; set; }
    }
}";
            this.VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Returns the codefix being tested (C#) - to be implemented in non-abstract class
        /// </summary>
        /// <returns>The CodeFixProvider to be used for CSharp code</returns>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DocumentationPropertyCodeFixProvider();
        }

        /// <summary>
        /// Get the CSharp analyzer being tested - to be implemented in non-abstract class
        /// </summary>
        /// <returns>the diagnostic analyzer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DocumentationPropertyAnalyser();
        }
    }
}