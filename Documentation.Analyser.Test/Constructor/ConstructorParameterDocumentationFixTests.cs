// <copyright file="ConstructorParameterDocumentationFixTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Constructor
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
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
                Id = "SA1642D",
                Message = $"constructors must be correctly documented.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 13, 16) }
            };

            new DocumentationConstructorCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
namespace ConsoleApplication1
{
    public class TypeName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""TypeName""/> class.
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
            new DocumentationConstructorCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that the constructor has correct starting text added.
        /// </summary>
        [Fact]
        public void TestThatConstructorContainsCorrectFirstLineOfText()
        {
            var test = @"
using System;
namespace ConsoleApplication1
{
    public class TypeName<T, TR>
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
                Id = "SA1642D",
                Message = $"constructors must be correctly documented.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 13, 16) }
            };

            new DocumentationConstructorCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
namespace ConsoleApplication1
{
    public class TypeName<T, TR>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""TypeName{T, TR}""/> class.
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
            new DocumentationConstructorCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Ensure that, at least for now, structs dont' trigger analysis.
        /// </summary>
        [Fact]
        public void CheckThatStructConstructorDoesNotTriggerAnalysis()
        {
            var test = @"
using System;
namespace ConsoleApplication1
{
    public struct TypeName
    {
        /// <summary>
        /// a description has been provided.
        /// line 2 of the description.
        /// </summary>
        public TypeName()
        {
        }
    }
}";

            new DocumentationConstructorCodeFixVerifier().VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// test that a constructor with correct arguments but incorrect text
        /// triggers the analyser.
        /// </summary>
        [Fact]
        public void TestThatConstructorWithCorrectArgumentsButIncorrectTextTriggers()
        {
            var test = @"
using System;
namespace ConsoleApplication1
{
    public class TypeName<T, TR>
    {
        /// <summary>
        /// Initializes a instance of the <see cref=""TypeName{T, TR}""/> class.
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
            var expected = new DiagnosticResult
            {
                Id = "SA1642D",
                Message = $"constructors must be correctly documented.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 15, 16) }
            };

            new DocumentationConstructorCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
namespace ConsoleApplication1
{
    public class TypeName<T, TR>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""TypeName{T, TR}""/> class.
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
            new DocumentationConstructorCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test single line / single parameter correctly fixes documentation.
        /// </summary>
        [Fact]
        public void TestSingleLineCorrectDocumentationTriggersAndFixesCorrectly()
        {
            var test = @"
using System;
namespace ConsoleApplication1
{
    public class TypeName<T, TR>
    {
        /// <summary>
        /// this is some documentation for type name.
        /// </summary>
        /// <param name=""parameterOne"">there is some documentation</param>
        public TypeName(string parameterOne)
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1642D",
                Message = $"constructors must be correctly documented.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 11, 16) }
            };

            new DocumentationConstructorCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
namespace ConsoleApplication1
{
    public class TypeName<T, TR>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""TypeName{T, TR}""/> class.
        /// this is some documentation for type name.
        /// </summary>
        /// <param name=""parameterOne"">there is some documentation</param>
        public TypeName(string parameterOne)
        {
        }
    }
}";
            new DocumentationConstructorCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
