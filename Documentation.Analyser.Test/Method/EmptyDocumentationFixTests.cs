// <copyright file="EmptyDocumentationFixTests.cs" company="Palantir (Pty) Ltd">
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
    public class EmptyDocumentationFixTests : CodeFixVerifier
    {
        /// <summary>
        /// test that the 'Get' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestMethodBuildsCorrectMethodSummaryAfterFix()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class TypeName
    {
        public void BuildVogonConstructorFleet()
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612",
                Message = $"methods must be correctly documented.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 13, 21) }
            };

            new DocumentationMethodCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        public void BuildVogonConstructorFleet()
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}