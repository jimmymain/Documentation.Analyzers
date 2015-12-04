using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Analyser.Test.Member
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// test spacing issues encountered.
    /// </summary>
    public class TestSpacingOnFirstVariables
    {

        /// <summary>
        /// check spacing before property documentation.
        /// </summary>
        [Fact]
        public void TestThatFirstPropertyHasCorrectSpacingBefore()
        {
            var test = @"
namespace Test
{
    public class TestClass
    {
        private readonly int _someVariable;
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SA1600D",
                Message = $"members must be correctly documented.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 6, 30) }
            };

            new DocumentationMemberCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace Test
{
    public class TestClass
    {
        /// <summary>
        /// the some variable.
        /// </summary>
        private readonly int _someVariable;
    }
}";
            new DocumentationMemberCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
