// <copyright file="MethodAndPropertySpacingTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Property
{
    using Documentation.Analyser.Test.Helpers;
    using Documentation.Analyser.Test.Verifiers;

    using Microsoft.CodeAnalysis;

    using Xunit;

    /// <summary>
    /// the method and property spacing tests.
    /// </summary>
    public class MethodAndPropertySpacingTests
    {
        /// <summary>
        /// test spacing of methods and properties.
        /// </summary>
        [Fact]
        public void TestSpacingOfMethodAndProperty()
        {
            var test = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// main method.
        /// </summary>
        /// <param name=""args"">the arguments.</param>
        public static void Main(string args)
        {
        }

        public string VogonConstructorFleet { get; set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1623D",
                Message = $"property documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 14, 23) }
            };

            new DocumentationPropertyCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// main method.
        /// </summary>
        /// <param name=""args"">the arguments.</param>
        public static void Main(string args)
        {
        }

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