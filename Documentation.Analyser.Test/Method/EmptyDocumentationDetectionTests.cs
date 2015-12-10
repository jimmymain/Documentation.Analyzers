// <copyright file="EmptyDocumentationDetectionTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Method
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// Documentation quick fix unit tests.
    /// </summary>
    public class EmptyDocumentationDetectionTests : CodeFixVerifier
    {
        /// <summary>
        /// test that the 'Set' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestSetPropertyHasCorrectLeadingText()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class TypeName
    {
        public string VogonConstructorFleet()
        {
            return string.Empty;
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 13, 23) }
            };

            new DocumentationMethodCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);
        }
    }
}