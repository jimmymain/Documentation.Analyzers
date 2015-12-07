// <copyright file="TestThatPrivateMemberVariableIsDocumented.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Member
{
    using Documentation.Analyser.Test.Helpers;
    using Documentation.Analyser.Test.Verifiers;

    using Microsoft.CodeAnalysis;

    using Xunit;

    /// <summary>
    /// test that a member variable is correctly documented.
    /// </summary>
    public class TestThatPrivateMemberVariableIsDocumented
    {
        /// <summary>
        /// test documenting a simple member variable.
        /// </summary>
        [Fact]
        public void TestDocumentSimpleMemberVariable()
        {
            var test = @"
using System;
namespace ConsoleApplication1
{
    class TypeName
    {
        private Fleet[] _vogonConstructorFleet;
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1600D",
                Message = $"members must be correctly documented.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                                       new[] { new DiagnosticResultLocation("Test0.cs", 7, 25) }
            };

            new DocumentationMemberCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// the vogon constructor fleet.
        /// </summary>
        private Fleet[] _vogonConstructorFleet;
    }
}";
            new DocumentationMemberCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// check the spacing.
        /// </summary>
        [Fact]
        public void TestThatSpaceBeforeIsCorrectForSubsequentProperties()
        {
            var test = @"
using System;
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// some documentation.
        /// </summary>
        private int firstVariable;
        private Fleet[] _vogonConstructorFleet;
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1600D",
                Message = $"members must be correctly documented.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                                       new[] { new DiagnosticResultLocation("Test0.cs", 11, 25) }
            };

            new DocumentationMemberCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// some documentation.
        /// </summary>
        private int firstVariable;

        /// <summary>
        /// the vogon constructor fleet.
        /// </summary>
        private Fleet[] _vogonConstructorFleet;
    }
}";
            new DocumentationMemberCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Space before first property.
        /// </summary>
        [Fact]
        public void TestSpaceBeforeForFirstProperty()
        {
            var test = @"
using System;
namespace ConsoleApplication1
{
    class TypeName
    {
        private int firstVariable;

        /// <summary>
        /// the vogon constructor fleet.
        /// </summary>
        private Fleet[] _vogonConstructorFleet;
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1600D",
                Message = $"members must be correctly documented.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 21) }
            };

            new DocumentationMemberCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// the first variable.
        /// </summary>
        private int firstVariable;

        /// <summary>
        /// the vogon constructor fleet.
        /// </summary>
        private Fleet[] _vogonConstructorFleet;
    }
}";
            new DocumentationMemberCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// verify that if a long type name is specified as part of a property
        /// that the longer type name is used for documentation.
        /// </summary>
        [Fact]
        public void VerifyThatALongerTypeNameIsDocumentedCorrectly()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    class TypeName
    {
        private readonly IReturnTypeWithProperName _properName;
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1600D",
                Message = $"members must be correctly documented.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 52) }
            };

            new DocumentationMemberCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;

namespace ConsoleApplication1
{
    class TypeName
    {
        /// <summary>
        /// the return type with proper name.
        /// </summary>
        private readonly IReturnTypeWithProperName _properName;
    }
}";
             new DocumentationMemberCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
