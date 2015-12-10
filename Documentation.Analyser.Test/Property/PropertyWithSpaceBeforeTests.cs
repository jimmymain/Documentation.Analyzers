// <copyright file="PropertyWithSpaceBeforeTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Property
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// Test that the spacing before the property is maintained in the fix.
    /// </summary>
    public class PropertyWithSpaceBeforeTests
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
        /// <summary>
        /// Gets or sets the test.
        /// </summary>
        public string Test { get; set; }

        /// <summary>
        /// </summary>
        public string VogonConstructorFleet { get; set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1623D",
                Message = $"property documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 13, 23) }
            };

            new DocumentationPropertyCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// Gets or sets the test.
        /// </summary>
        public string Test { get; set; }

        /// <summary>
        /// Gets or sets the vogon constructor fleet.
        /// </summary>
        public string VogonConstructorFleet { get; set; }
    }
}";
            new DocumentationPropertyCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
