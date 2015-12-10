// <copyright file="PropertyWithoutCorrectStartingText.cs" company="Palantir (Pty) Ltd">
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
    public class PropertyWithoutCorrectStartingText
    {
        /// <summary>
        /// test that the 'Get' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestPropertyWithoutCorrectText()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// Gets or sets the test.
        /// </summary>
        public string SomeProperty { get; private set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1623D",
                Message = $"property documentation: does not start with 'Gets the'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 9, 23) }
            };

            new DocumentationPropertyCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// Gets the test.
        /// </summary>
        public string SomeProperty { get; private set; }
    }
}";
            new DocumentationPropertyCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Test that if a property contains some existing text, just the
        /// prefix is corrected.
        /// </summary>
        [Fact]
        public void TestThatPropertyFixDoesNotEraseExistingCommentText()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// returns an interresting property value.
        /// </summary>
        public string SomeProperty { get; set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1623D",
                Message = $"property documentation: does not start with 'Gets or sets the'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 9, 23) }
            };

            new DocumentationPropertyCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// Gets or sets the interresting property value.
        /// </summary>
        public string SomeProperty { get; set; }
    }
}";
            new DocumentationPropertyCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
