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
    /// interface documentation fixes.
    /// </summary>
    public class InterfaceDocumentationFixTests
    {
        /// <summary>
        /// test that class documentation creates a valuable summary.
        /// </summary>
        [Fact]
        public void TestInterfaceDocumentationCreatesConstructiveSummary()
        {
            var test = @"
using System;
namespace ConsoleApplication1
{
    public interface ThisIsALongTypeName
    {
        int Test { get; set; };
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1606D",
                Message = $"interface documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 5, 22) }
            };

            new DocumentationClassFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
namespace ConsoleApplication1
{
    /// <summary>
    /// this is a long type name.
    /// </summary>
    public interface ThisIsALongTypeName
    {
        int Test { get; set; };
    }
}";
            new DocumentationClassFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
