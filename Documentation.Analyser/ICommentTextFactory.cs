// <copyright file="ICommentTextFactory.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// factory to produce comment text based
    /// on the compiler context.
    /// </summary>
    public interface ICommentTextFactory
    {
        /// <summary>
        /// build summary text for a class declaration.
        /// </summary>
        /// <param name="classDeclaration">the class declaration.</param>
        /// <returns>the summary text.</returns>
        string BuildSummaryTextForClass(ClassDeclarationSyntax classDeclaration);

        /// <summary>
        /// build up the summary text for a constructor declaration.
        /// </summary>
        /// <param name="constructorDeclaration">the constructor declaration.</param>
        /// <returns>a string containing the summary text.</returns>
        string BuildSummaryTextForConstructor(ConstructorDeclarationSyntax constructorDeclaration);

        /// <summary>
        /// Build the summary text for a method declaration.
        /// </summary>
        /// <param name="methodDeclaration">the method node.</param>
        /// <returns>a string containing the summary text.</returns>
        string BuildSummaryTextForMethod(MethodDeclarationSyntax methodDeclaration);

        /// <summary>
        /// build the summary text appropriate for a parameter.
        /// </summary>
        /// <param name="parameterSyntax">the parameter.</param>
        /// <returns>a string containing the short description.</returns>
        string BuildSummaryTextForParameter(ParameterSyntax parameterSyntax);

        /// <summary>
        /// build the summary text for a property based on the
        /// supplied synax propertyDeclaration.
        /// </summary>
        /// <param name="propertyDeclaration">the syntax propertyDeclaration.</param>
        /// <returns>the summary text.</returns>
        string BuildSummaryTextForProperty(PropertyDeclarationSyntax propertyDeclaration);

        /// <summary>
        /// build the summary text for a property based on the supplied
        /// text.
        /// </summary>
        /// <param name="propertyDeclaration">the property declaration.</param>
        /// <param name="text">the existing text.</param>
        /// <returns>a string containing the corrected documentation.</returns>
        string[] BuildSummaryTextForProperty(PropertyDeclarationSyntax propertyDeclaration, string[] text);

        /// <summary>
        /// build the summary prefix text.
        /// </summary>
        /// <param name="propertyNode">the property propertyDeclaration.</param>
        /// <returns>a string containing the summary text.</returns>
        string BuildSummaryTextPrefixForProperty(PropertyDeclarationSyntax propertyNode);

        /// <summary>
        /// build the summary text for the supplied member.
        /// </summary>
        /// <param name="variableDeclaratorSyntax">the variable declarator syntax.</param>
        /// <param name="returnType">the member return type.</param>
        /// <returns>a string containing the text.</returns>
        string BuildSummaryTextForMemberVariable(VariableDeclaratorSyntax variableDeclaratorSyntax, VariableDeclarationSyntax returnType);

        /// <summary>
        /// build the summary text for a return value.
        /// </summary>
        /// <param name="methodDeclaration">the return type for the method.</param>
        /// <returns>a string containing the return type documentation.</returns>
        string BuildSummaryTextForReturnValue(MethodDeclarationSyntax methodDeclaration);
    }
}