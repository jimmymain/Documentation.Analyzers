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
        /// <returns>an int containing the perform a function.</returns>
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
        /// <returns>a string containing the perform a function.</returns>
        public string PerformAFunction()
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test a boolean return value is documented correctly.
        /// </summary>
        [Fact]
        public void TestBooleanReturnValueDocumentation()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        public bool IsTheFugglyThingUgly()
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
                    new[] { new DiagnosticResultLocation("Test0.cs", 9, 21) }
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
        /// <returns>true if the fuggly thing ugly, otherwise false.</returns>
        public bool IsTheFugglyThingUgly()
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test a has boolean return value is documented correctly.
        /// </summary>
        [Fact]
        public void TestBooleanHasReturnValueDocumentation()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        public bool HasDocumentationNodes()
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
                    new[] { new DiagnosticResultLocation("Test0.cs", 9, 21) }
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
        /// <returns>true if the documentation nodes, otherwise false.</returns>
        public bool HasDocumentationNodes()
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test complex return value(s) with generic types.
        /// </summary>
        [Fact]
        public void TestReturnValuesWithGenericTypesUseCorrectBraces()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        public List<Function> PerformAFunction()
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 6, 31) }
            };

            new DocumentationMethodCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// perform the a function.
        /// </summary>
        /// <returns>a List{Function} containing the perform a function.</returns>
        public List<Function> PerformAFunction()
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

    }
}