// <copyright file="EmptyDocumentationDetectionTests.cs" company="Palantir (Pty) Ltd">
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
    public class ClassContainsPocoDetectionTests : CodeFixVerifier
    {
        /// <summary>
        /// test that the 'Set' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestClassContaingMethodsReturnsFalse()
        {
            var test = @"
namespace ConsoleApplication1
{
    class NonPocoClass
    {
        /// <summary>
        /// build the vogon constructor fleet.       
        /// </summary>
        public void PerformAFunction()
        {
        }
    }
}";
            new ClassSerializationFixVerifier().VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// test that the 'Set' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestClassContaingNonDataContractAttribute()
        {
            var test = @"
namespace ConsoleApplication1
{
    [DataSomething]
    class NonPocoClass
    {
        /// <summary>
        /// Gets or sets the fleet...
        /// </summary>
        public string ConstructorFleet { get; set; }
    }
}";
            new ClassSerializationFixVerifier().VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// test that the 'Set' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestClassContaingDataContractAttribute()
        {
            var test = @"
namespace ConsoleApplication1
{
    [DataSomething]
    [DataContract]
    class NonPocoClass
    {
        /// <summary>
        /// Gets or sets the fleet...
        /// </summary>
        public string ConstructorFleet { get; set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SERI001",
                Message = $"Generate Serialization",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 6, 11) }
            };
            new ClassSerializationFixVerifier().VerifyCSharpDiagnostic(test, expected);
        }
    }
}