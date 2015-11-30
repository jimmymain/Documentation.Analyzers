// <copyright file="MethodParameterDocumentationFixTests.cs" company="Palantir (Pty) Ltd">
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
    public class MethodParameterDocumentationFixTests : CodeFixVerifier
    {
        /// <summary>
        /// test that the 'Get' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestStaticMainMethodWithASingleParameter()
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
        /// <summary>
        /// build the vogon constructor fleet.       
        /// </summary>
        /// <param name=""args""></param>
        public static void Main(string[] args)
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
                    new[] { new DiagnosticResultLocation("Test0.cs", 17, 28) }
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
        /// <param name=""args"">the args.</param>
        public static void Main(string[] args)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that a single unsupplied parameter is correctly added
        /// without replacing the existing documentation.
        /// </summary>
        [Fact]
        public void MethodParameterDocumentationAddSingleParameterTest()
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
        /// <summary>
        /// a description has been provided.
        /// line 2 of the description.
        /// </summary>
        /// <param name=""parameterOne"">there is some documentation</param>
        /// <param name=""parameterTwo""></param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
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
                    new[] { new DiagnosticResultLocation("Test0.cs", 19, 21) }
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
        /// a description has been provided.
        /// line 2 of the description.
        /// </summary>
        /// <param name=""parameterOne"">there is some documentation</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that the 'Get' property has correct documentation.
        /// </summary>
        [Fact]
        public void TestFixParametersForMethodMissingParameters()
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
        /// <summary>
        /// build the vogon constructor fleet.       
        /// </summary>
        /// <param name=""parameterOne"">parameter one</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
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
                    new[] { new DiagnosticResultLocation("Test0.cs", 17, 21) }
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
        /// <param name=""parameterOne"">parameter one</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// text that a documented method that is not the first item
        /// in the class has a space before.
        /// </summary>
        [Fact]
        public void TestSpaceBeforeCorrectedDocumentation()
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
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        /// <param name=""parameterOne"">parameter one</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }

        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
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
                    new[] { new DiagnosticResultLocation("Test0.cs", 22, 21) }
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
        /// <param name=""parameterOne"">parameter one</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }

        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        /// <param name=""parameterOne"">the parameter one.</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test space before is corrected for existing documentation.
        /// </summary>
        [Fact]
        public void TestSpaceBeforeIsCorrectedForExistingDocumentation()
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
        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        /// <param name=""parameterOne"">parameter one</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }

        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
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
                    new[] { new DiagnosticResultLocation("Test0.cs", 25, 21) }
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
        /// <param name=""parameterOne"">parameter one</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }

        /// <summary>
        /// build the vogon constructor fleet.
        /// </summary>
        /// <param name=""parameterOne"">the parameter one.</param>
        /// <param name=""parameterItemTwo"">the parameter item two.</param>
        public void BuildVogonConstructorFleet(string parameterOne, int parameterItemTwo)
        {
        }
    }
}";
            new DocumentationMethodCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}