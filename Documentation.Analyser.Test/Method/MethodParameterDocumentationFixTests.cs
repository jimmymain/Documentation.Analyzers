// <copyright file="MethodParameterDocumentationFixTests.cs" company="Palantir (Pty) Ltd">
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
    public class MethodParameterDocumentationFixTests : CodeFixVerifier
    {
        /// <summary>
        /// test that the 'Get' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestStaticMainMethodWithASingleParameter()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.       
        /// </summary>
        /// <param name=""args""></param>
        public static void Main(string[] args)
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: missing 'args'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 10, 28) }
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
        /// <param name=""args"">the args.</param>
        public static void Main(string[] args)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that a single unsupplied parameter is correctly added
        /// without replacing the existing documentation.
        /// </summary>
        [Fact]
        public void MethodParameterDocumentationAddSingleParameterTest()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// a description has been provided.
        /// line 2 of the description.
        /// </summary>
        /// <param name=""parameterOne"">there is some documentation</param>
        /// <param name=""parameterTwo""></param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: missing 'parameterItemTwo'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 12, 21) }
            };

            new DocumentationMethodCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// a description has been provided.
        /// line 2 of the description.
        /// </summary>
        /// <param name=""parameterOne"">there is some documentation</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that the 'Get' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestFixParametersForMethodMissingParameters()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.       
        /// </summary>
        /// <param name=""parameterOne"">parameter one</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: missing 'parameterItemTwo'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 10, 21) }
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
        /// <param name=""parameterOne"">parameter one</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// text that a documented method that is not the first item
        /// in the class has a space before.
        /// </summary>
        [Fact]
        public void TestSpaceBeforeCorrectedDocumentation()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        /// <param name=""parameterOne"">parameter one</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }

        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
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
                    new[] { new DiagnosticResultLocation("Test0.cs", 15, 21) }
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
        /// <param name=""parameterOne"">parameter one</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }

        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        /// <param name=""parameterOne"">the parameter one.</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test space before is corrected for existing documentation.
        /// </summary>
        [Fact]
        public void TestSpaceBeforeIsCorrectedForExistingDocumentation()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        /// <param name=""parameterOne"">parameter one</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }

        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: missing 'parameterOne', 'parameterItemTwo'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 18, 21) }
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
        /// <param name=""parameterOne"">parameter one</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }

        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        /// <param name=""parameterOne"">the parameter one.</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that additional parameters are documented.
        /// </summary>
        [Fact]
        public void TestAdditionalParameters()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        /// <param name=""parameterOne"">parameter one</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne)
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: additional 'parameterItemTwo'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 11, 21) }
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
        /// <param name=""parameterOne"">parameter one</param>
        public void BuildVogonConstructorFleet(string parameterOne)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that additional parameters are documented.
        /// </summary>
        [Fact]
        public void TestThatLargeParameterTypeNameTrumpsVariableName()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        public void BuildVogonConstructorFleet(ITestParameterType parameterOne)
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
                    new[] { new DiagnosticResultLocation("Test0.cs", 6, 21) }
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
        /// <param name=""parameterOne"">the test parameter type.</param>
        public void BuildVogonConstructorFleet(ITestParameterType parameterOne)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}