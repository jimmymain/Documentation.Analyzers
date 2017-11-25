// <copyright file="ClassNodeInterrogationExtensions.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// class node interrogation extensions.
    /// </summary>
    public static class ClassNodeInterrogationExtensions
    {
        /// <summary>
        /// return true if the class has any methods.
        /// we're anlysing strictly POCO's
        /// </summary>
        /// <param name="cls">the class</param>
        /// <returns>true if the class has a method.</returns>
        public static bool HasMethods(this ClassDeclarationSyntax cls)
        {
            return cls.Members.OfType<MethodDeclarationSyntax>().Any();
        }

        /// <summary>
        /// return true if the class declaration has a 'DataContract' attribute.
        /// </summary>
        /// <param name="cls">the data contract attribute.</param>
        /// <returns>the class</returns>
        public static bool HasDataContextAttribute(this ClassDeclarationSyntax cls)
        {
            var dataContextAttribute = cls.AttributeLists
                .SelectMany(_ => _.Attributes)
                .Select(_ => _.Name)
                .OfType<IdentifierNameSyntax>()
                .Any(_ => _.Identifier.Text == "DataContract");
            return dataContextAttribute;
        }
    }
}