// <copyright file="MethodReturnValueFixTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Method
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// return value fixes.
    /// </summary>
    public class MethodReturnValueFixTests : CodeFixVerifier
    {
        /// <summary>
        /// test that void return values don't raise diagnostics.
        /// </summary>
        [Fact]
        public void TestThatVoidReturnDoesNotRaiseDiagnostic()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.       
        /// </summary>
        public void PerformAFunction()
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// test that a simple return value type is documented correctly.
        /// </summary>
        [Fact]
        public void TestNamedMethodWithStringReturnValue()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        public int PerformAFunction()
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: missing return value documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 9, 20) }
            };

            new DocumentationMethodCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        /// <returns>an int containing the perform a function result.</returns>
        public int PerformAFunction()
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that a simple return value type is documented correctly.
        /// </summary>
        [Fact]
        public void TestStringReturnValueDocumentation()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        public string PerformAFunction()
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: missing return value documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 9, 23) }
            };

            new DocumentationMethodCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        /// <returns>a string containing the perform a function result.</returns>
        public string PerformAFunction()
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
