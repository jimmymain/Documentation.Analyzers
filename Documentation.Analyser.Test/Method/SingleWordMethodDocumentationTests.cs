// <copyright file="SingleWordMethodDocumentationTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Method
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// test some smaller single line fixes.
    /// </summary>
    public class SingleWordMethodDocumentationTests
    {
        /// <summary>
        /// test that a single word method creates meaningful documentation from
        /// the first parameter name.
        /// </summary>
        [Fact]
        public void TestThatSingleWordMethodDocumentationIncludesFirstParameter()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        public void Observe(string vogonConstructorFleet)
        {
        }
    }
}";
            var expected = new DiagnosticResult
                               {
                                   Id = "SA1612D",
                                   Message = $"methods must be correctly documented.",
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations =
                                       new[] { new DiagnosticResultLocation("Test0.cs", 6, 21) }
                               };

            new DocumentationMethodCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// observe the vogon constructor fleet.
        /// </summary>
        /// <param name=""vogonConstructorFleet"">the vogon constructor fleet.</param>
        public void Observe(string vogonConstructorFleet)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that a single word method creates meaningful documentation from
        /// the first parameter name.
        /// </summary>
        [Fact]
        public void TestThatReturnTypeDocumentationContainsCorrectReturnDocumentation()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        public ITestAnInterfaceTypeReturnValue Observe(string vogonConstructorFleet)
        {
        }
    }
}";
            var expected = new DiagnosticResult
                               {
                                   Id = "SA1612D",
                                   Message = $"methods must be correctly documented.",
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 48) }
                               };

            new DocumentationMethodCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// observe the vogon constructor fleet.
        /// </summary>
        /// <param name=""vogonConstructorFleet"">the vogon constructor fleet.</param>
        /// <returns>the test an interface type return value.</returns>
        public ITestAnInterfaceTypeReturnValue Observe(string vogonConstructorFleet)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}