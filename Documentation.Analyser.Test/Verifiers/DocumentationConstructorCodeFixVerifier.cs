namespace Documentation.Analyser.Test.Verifiers
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// constructor code fix verifier.
    /// </summary>
    public class DocumentationConstructorCodeFixVerifier : CodeFixVerifier
    {
        /// <summary>
        /// Returns the codefix being tested (C#) - to be implemented in non-abstract class
        /// </summary>
        /// <returns>The CodeFixProvider to be used for CSharp code</returns>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DocumentationConstructorCodeFixProvider();
        }

        /// <summary>
        /// Get the CSharp analyzer being tested - to be implemented in non-abstract class
        /// </summary>
        /// <returns>the diagnostic analyzer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DocumentationConstructorAnalyser();
        }
    }
}
