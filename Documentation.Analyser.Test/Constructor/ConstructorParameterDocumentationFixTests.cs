// <copyright file="ConstructorParameterDocumentationFixTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Constructor
{
    using Documentation.Analyser.Test.Helpers;
    using Documentation.Analyser.Test.Verifiers;

    using Microsoft.CodeAnalysis;

    using Xunit;

    /// <summary>
    /// Test that constructor arguments are correctly documented.
    /// </summary>
    public class ConstructorParameterDocumentationFixTests
    {
        /// <summary>
        /// test that a single unsupplied parameter is correctly added
        /// without replacing the existing documentation.
        /// </summary>
        [Fact]
        public void ConstructorParameterDocumentationAddSingleParameterTest()
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
    public class TypeName
    {
        /// <summary>
        /// a description has been provided.
        /// line 2 of the description.
        /// </summary>
        /// <param name=""parameterOne"">there is some documentation</param>
        /// <param name=""parameterTwo""></param>
        public TypeName(
            string parameterOne,
            int parameterItemTwo,
            string parameterThree)
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1612",
                Message = $"Method and Constructor parameters must be documented",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 19, 16) }
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
    public class TypeName
    {
        /// <summary>
        /// a description has been provided.
        /// line 2 of the description.
        /// </summary>
        /// <param name=""parameterOne"">there is some documentation</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        /// <param name=""parameterThree"">the parameter three.</param>
        public TypeName(
            string parameterOne,
            int parameterItemTwo,
            string parameterThree)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
