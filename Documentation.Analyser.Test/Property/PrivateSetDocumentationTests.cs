// <copyright file="PrivateSetDocumentationTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Property
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// Test the set property documentation.
    /// </summary>
    public class PrivateSetDocumentationTests
    {
        /// <summary>
        /// test that the 'Set' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestSetPropertyHasCorrectLeadingText()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        public string VogonConstructorFleet { get; private set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1623D",
                Message = $"property documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 23) }
            };

            new DocumentationPropertyCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// Gets the vogon constructor fleet.
        /// </summary>
        public string VogonConstructorFleet { get; private set; }
    }
}";
            new DocumentationPropertyCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that the 'Set' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestUnspecifiedSetPropertyHasCorrectComment()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        public string VogonConstructorFleet { get; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1623D",
                Message = $"property documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 6, 23) }
            };

            new DocumentationPropertyCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// Gets the vogon constructor fleet.
        /// </summary>
        public string VogonConstructorFleet { get; }
    }
}";
            new DocumentationPropertyCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}