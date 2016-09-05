// <copyright file="ClassWithTemplateDocumentationFixTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Constructor
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// class documentation fixes.
    /// </summary>
    public class ClassWithTemplateDocumentationFixTests
    {
        /// <summary>
        /// test that class documentation with a type parameter creates a valuable summary.
        /// </summary>
        [Fact]
        public void TestClassDocumentationWithTemplateCreatesConstructiveSummary()
        {
            var test = @"
using System;
namespace ConsoleApplication1
{
    public class ThisIsALongTypeName<TTypePayload>
    {
        private int Test { get; set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1606D",
                Message = $"class documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 5, 18) }
            };

            new DocumentationClassFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
namespace ConsoleApplication1
{
    /// <summary>
    /// this is a long type name.
    /// </summary>
    /// <typeparam name=""TTypePayload"">a type of type payload.</typeparam>
    public class ThisIsALongTypeName<TTypePayload>
    {
        private int Test { get; set; }
    }
}";
            new DocumentationClassFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that class documentation with a type parameter creates a valuable summary.
        /// </summary>
        [Fact]
        public void TestThatExistingTypeParametersAreNotOverridden()
        {
            var test = @"
using System;
namespace ConsoleApplication1
{
    /// <typeparam name=""TTypePayload"">existing documentation must not change</typeparam>
    public class ThisIsALongTypeName<TTypePayload, TOther>
    {
        private int Test { get; set; }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1606D",
                Message = $"class documentation: no documentation.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 6, 18) }
            };

            new DocumentationClassFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
namespace ConsoleApplication1
{
    /// <summary>
    /// this is a long type name.
    /// </summary>
    /// <typeparam name=""TTypePayload"">existing documentation must not change</typeparam>
    /// <typeparam name=""TOther"">a type of other.</typeparam>
    public class ThisIsALongTypeName<TTypePayload, TOther>
    {
        private int Test { get; set; }
    }
}";
            new DocumentationClassFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
