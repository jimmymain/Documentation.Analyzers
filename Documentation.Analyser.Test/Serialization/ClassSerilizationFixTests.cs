// <copyright file="ClassSerilizationFixTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Serialization
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// class serialization fix tests.
    /// </summary>
    public class ClassSerilizationFixTests : CodeFixVerifier
    {
        /// <summary>
        /// test that the 'Set' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestClassContaingDataContractAttribute()
        {
            var test = @"
using System.Runtime.Serialization;

namespace ConsoleApplication1
{
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
                    new[] { new DiagnosticResultLocation("Test0.cs", 7, 11) }
            };
            new ClassSerializationFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System.Runtime.Serialization;

namespace ConsoleApplication1
{
    [DataContract]
    class NonPocoClass
    {
        /// <summary>
        /// Gets or sets the fleet...
        /// </summary>
        [DataMember(Name = ""constructorFleet"")]
        public string ConstructorFleet { get; set; }
    }
}";

            new ClassSerializationFixVerifier().VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        /// <summary>
        /// test that the 'Set' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestDataMemberWhereExistingAttributesPresent()
        {
            var test = @"
using System.Runtime.Serialization;

namespace ConsoleApplication1
{
    [DataContract]
    class NonPocoClass
    {
        /// <summary>
        /// Gets or sets the fleet...
        /// </summary>
        [NotNull]
        public string ConstructorFleet { get; set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SERI001",
                Message = $"Generate Serialization",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 7, 11) }
            };
            new ClassSerializationFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System.Runtime.Serialization;

namespace ConsoleApplication1
{
    [DataContract]
    class NonPocoClass
    {
        /// <summary>
        /// Gets or sets the fleet...
        /// </summary>
        [NotNull]
        [DataMember(Name = ""constructorFleet"")]
        public string ConstructorFleet { get; set; }
    }
}";

            new ClassSerializationFixVerifier().VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        /// <summary>
        /// test that the 'Set' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestThatAdditonalAttributesAreNotAddedWhenTheyExist()
        {
            var test = @"
using System.Runtime.Serialization;

namespace ConsoleApplication1
{
    [DataContract]
    class NonPocoClass
    {
        /// <summary>
        /// Gets or sets the fleet...
        /// </summary>
        [DataMember(Name = ""constructorFleet"")]
        public string ConstructorFleet { get; set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SERI001",
                Message = $"Generate Serialization",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 7, 11) }
            };
            new ClassSerializationFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System.Runtime.Serialization;

namespace ConsoleApplication1
{
    [DataContract]
    class NonPocoClass
    {
        /// <summary>
        /// Gets or sets the fleet...
        /// </summary>
        [DataMember(Name = ""constructorFleet"")]
        public string ConstructorFleet { get; set; }
    }
}";

            new ClassSerializationFixVerifier().VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        /// <summary>
        /// test that the 'Set' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestThatClassWithTwoPropertiesFixesBoth()
        {
            var test = @"
using System.Runtime.Serialization;

namespace ConsoleApplication1
{
    [DataContract]
    class NonPocoClass
    {
        /// <summary>
        /// Gets or sets the fleet...
        /// </summary>
        public string ConstructorFleet { get; set; }

        /// <summary>
        /// Gets or sets the destroyer...
        /// </summary>
        public string DestroyerArmour { get; set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SERI001",
                Message = $"Generate Serialization",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 7, 11) }
            };
            new ClassSerializationFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System.Runtime.Serialization;

namespace ConsoleApplication1
{
    [DataContract]
    class NonPocoClass
    {
        /// <summary>
        /// Gets or sets the fleet...
        /// </summary>
        [DataMember(Name = ""constructorFleet"")]
        public string ConstructorFleet { get; set; }

        /// <summary>
        /// Gets or sets the destroyer...
        /// </summary>
        [DataMember(Name = ""destroyerArmour"")]
        public string DestroyerArmour { get; set; }
    }
}";

            new ClassSerializationFixVerifier().VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }
    }
}