// <copyright file="PublicGetDocumentationTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Property
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// Test the get property documentation.
    /// </summary>
    public class PublicGetDocumentationTests
    {
        /// <summary>
        /// test that the 'Get' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestGetPropertyHasCorrectLeadingText()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        public string VogonConstructorFleet { get; set; }
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
        /// Gets or sets the vogon constructor fleet.
        /// </summary>
        public string VogonConstructorFleet { get; set; }
    }
}";
            new DocumentationPropertyCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that the 'Get' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestExpressionBodiedProperties()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        private IAuthenticationManager AuthenticationManager => this.HttpContext.GetOwinContext().Authentication;
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1623D",
                Message = $"property documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 6, 40) }
            };

            new DocumentationPropertyCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// Gets the authentication manager.
        /// </summary>
        private IAuthenticationManager AuthenticationManager => this.HttpContext.GetOwinContext().Authentication;
    }
}";
            new DocumentationPropertyCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
