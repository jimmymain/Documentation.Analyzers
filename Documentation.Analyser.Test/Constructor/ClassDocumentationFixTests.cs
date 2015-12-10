// <copyright file="ClassDocumentationFixTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Constructor
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// class documentation fixes.
    /// </summary>
    public class ClassDocumentationFixTests
    {
        /// <summary>
        /// test that class documentation creates a valuable summary.
        /// </summary>
        [Fact]
        public void TestClassDocumentationCreatesConstructiveSummary()
        {
            var test = @"
using System;
namespace ConsoleApplication1
{
    public class ThisIsALongTypeName
    {
        private int Test { get; set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1606D",
                Message = $"class documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 5, 18) }
            };

            new DocumentationClassFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
namespace ConsoleApplication1
{
    /// <summary>
    /// this is a long type name.
    /// </summary>
    public class ThisIsALongTypeName
    {
        private int Test { get; set; }
    }
}";
            new DocumentationClassFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
