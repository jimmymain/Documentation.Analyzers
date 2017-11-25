// <copyright file="IAttributeFactory.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// a factory to add new attributes to a class.
    /// </summary>
    public interface IAttributeFactory
    {
        /// <summary>
        /// create data member attributes for each property in the class.
        /// </summary>
        /// <param name="classDeclaration">the class declaration.</param>
        /// <returns>the updated class declaration.</returns>
        ClassDeclarationSyntax CreateDataMemberAttributes(ClassDeclarationSyntax classDeclaration);
    }
}