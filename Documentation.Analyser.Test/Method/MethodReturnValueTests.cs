// <copyright file="MethodReturnValueTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Method
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// test that return values are not replaced.
    /// </summary>
    public class MethodReturnValueTests : CodeFixVerifier
    {
        /// <summary>
        /// test that return values are not replaced.
        /// </summary>
        [Fact]
        public void TestThatReturnValuesAreNotReplaced()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.       
        /// </summary>
        /// <returns>the result of the application</returns>
        public static int Main<T>(string[] arguments)
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612D",
                Message = $"method documentation: missing 'arguments'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 10, 27) }
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
        /// <param name=""arguments"">the arguments.</param>
        /// <returns>the result of the application</returns>
        /// <typeparam name=""T"">a type of {T}.</typeparam>
        public static int Main<T>(string[] arguments)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
