// <copyright file="TypedParameterMethodFixTests.cs" company="Palantir (Pty) Ltd">
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
    public class TypedParameterMethodFixTests : CodeFixVerifier
    {
        /// <summary>
        /// test that a single unsupplied parameter is correctly added
        /// without replacing the existing documentation.
        /// </summary>
        [Fact]
        public void TypedMethodParameterDocumentationAddSingleParameterTest()
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
        /// <param name=""parameterTwo"">parameter two</param>
        public void BuildVogonConstructorFleet<TVogonType>(string parameterOne, int parameterTwo)
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: missing 'TVogonType'.",
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
        /// <param name=""parameterTwo"">parameter two</param>
        /// <typeparam name=""TVogonType"">a type of vogon type.</typeparam>
        public void BuildVogonConstructorFleet<TVogonType>(string parameterOne, int parameterTwo)
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
        /// <param name=""parameterOne"">a parameter one</param>
        public void BuildVogonConstructorFleet<TVogonType>(string parameterOne)
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: missing 'TVogonType'.",
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
        /// <param name=""parameterOne"">a parameter one</param>
        /// <typeparam name=""TVogonType"">a type of vogon type.</typeparam>
        public void BuildVogonConstructorFleet<TVogonType>(string parameterOne)
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
        public void TestDescriptiveMethodWithTypeParameterIsReasonablyDocumented()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        public Fleet BuildVogonDestroyerFleet<TMunitionsPayload>(int destroyerCount)
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
                    new[] { new DiagnosticResultLocation("Test0.cs", 6, 22) }
            };

            new DocumentationMethodCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon destroyer fleet.
        /// </summary>
        /// <param name=""destroyerCount"">the destroyer count.</param>
        /// <returns>the fleet.</returns>
        /// <typeparam name=""TMunitionsPayload"">a type of munitions payload.</typeparam>
        public Fleet BuildVogonDestroyerFleet<TMunitionsPayload>(int destroyerCount)
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
        public void TestSimpleMethodWithTypeProvidesDocumentation()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.       
        /// </summary>
        public void BuildVogonConstructorFleet<T>()
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: missing 'T'.",
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
        /// <typeparam name=""T"">a type of {T}.</typeparam>
        public void BuildVogonConstructorFleet<T>()
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}