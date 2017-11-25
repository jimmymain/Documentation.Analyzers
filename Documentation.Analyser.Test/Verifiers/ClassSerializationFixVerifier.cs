// <copyright file="ClassSerializationFixVerifier.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Verifiers
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// constructor code fix verifier.
    /// </summary>
    public class ClassSerializationFixVerifier : CodeFixVerifier
    {
        /// <summary>
        /// Returns the codefix being tested (C#) - to be implemented in non-abstract class
        /// </summary>
        /// <returns>The CodeFixProvider to be used for CSharp code</returns>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SerializationClassCodeFixProvider();
        }

        /// <summary>
        /// Get the CSharp analyzer being tested - to be implemented in non-abstract class
        /// </summary>
        /// <returns>the diagnostic analyzer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SerializationClassAnalyser();
        }
    }
}
