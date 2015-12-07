// <copyright file="CommentNewLineTests.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Property
{
    using Documentation.Analyser.Test.Helpers;
    using Documentation.Analyser.Test.Verifiers;

    using Microsoft.CodeAnalysis;

    using Xunit;

    /// <summary>
    /// test that the articles are correctly placed, and that
    /// new lines occur correctly.
    /// </summary>
    public class CommentNewLineTests
    {
        /// <summary>
        /// test that the article is correctly removed.
        /// the  'Return' should be replaced by 'Gets / Gets or sets'
        /// </summary>
        [Fact]
        public void TestThatAMultiLineCommentStaysOnMultipleLines()
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
        /// First Line
        /// Second line containing text.
        /// </summary>
        public string PacketSettings { get; }
    }
}";
            var expected = new DiagnosticResult
                               {
                                   Id = "SA1623D",
                                   Message = $"Properties must be correctly documented",
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations =
                                       new[] { new DiagnosticResultLocation("Test0.cs", 17, 23) }
                               };

            new DocumentationPropertyCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

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
        /// Gets the First Line
        /// Second line containing text.
        /// </summary>
        public string PacketSettings { get; }
    }
}";
            new DocumentationPropertyCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// test that the article is correctly removed.
        /// the  'Return' should be replaced by 'Gets / Gets or sets'
        /// </summary>
        [Fact]
        public void TestThatArticlesAreCorrectlyRemoved()
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
        /// Return the settings that the socket must use to read
        /// the message length.
        /// </summary>
        public string Settings { get; }
    }
}";
            var expected = new DiagnosticResult
                               {
                                   Id = "SA1623D",
                                   Message = $"Properties must be correctly documented",
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations =
                                       new[] { new DiagnosticResultLocation("Test0.cs", 17, 23) }
                               };

            new DocumentationPropertyCodeFixVerifier().VerifyCSharpDiagnostic(test, expected);

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
        /// Gets the settings that the socket must use to read
        /// the message length.
        /// </summary>
        public string Settings { get; }
    }
}";
            new DocumentationPropertyCodeFixVerifier().VerifyCSharpFix(test, fixtest);
        }
    }
}
