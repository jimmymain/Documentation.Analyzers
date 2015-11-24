// <copyright file="ICommentTextFactory.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// factory to produce comment text based
    /// on the compiler context.
    /// </summary>
    public interface ICommentTextFactory
    {
        /// <summary>
        /// build the summary prefix text.
        /// </summary>
        /// <param name="propertyNode">the property propertyDeclaration.</param>
        /// <returns>a string containing the summary text.</returns>
        string BuildSummaryTextPrefixForProperty(PropertyDeclarationSyntax propertyNode);

        /// <summary>
        /// build the summary text for a property based on the
        /// supplied synax propertyDeclaration.
        /// </summary>
        /// <param name="propertyNode">the syntax propertyDeclaration.</param>
        /// <returns>the summary text.</returns>
        string BuildSummaryTextForProperty(SyntaxNode propertyNode);

        /// <summary>
        /// Build the summary text for a method declaration.
        /// </summary>
        /// <param name="methodNode">the method node.</param>
        /// <returns>a string containing the summary text.</returns>
        string BuildSummaryTextForMethod(SyntaxNode methodNode);

        /// <summary>
        /// build the summary text appropriate for a parameter.
        /// </summary>
        /// <param name="parameterSyntax">the parameter.</param>
        /// <returns>a string containing the short description.</returns>
        string BuildSummaryTextForParameter(ParameterSyntax parameterSyntax);
    }
}