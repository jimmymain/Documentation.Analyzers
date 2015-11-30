namespace Documentation.Analyser.Test.Method
{
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Verifiers;
    using Xunit;

    /// <summary>
    /// test some smaller single line fixes.
    /// </summary>
    public class SingleWordMethodDocumentationTests
    {
        /// <summary>
        /// test that a single word method creates meaningful documentation from
        /// the first parameter name.
        /// </summary>
        [Fact]
        public void TestThatSingleWordMethodDocumentationIncludesFirstParameter()
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
        public void Observe(string vogonConstructorFleet)
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
        /// observe the vogon constructor fleet.
        /// </summary>
        /// <param name=""vogonConstructorFleet"">the vogon constructor fleet.</param>
        public void Observe(string vogonConstructorFleet)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
