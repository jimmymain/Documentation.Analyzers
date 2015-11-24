// <copyright file="IAccessLevelService.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// service to provide access level
    /// details for syntax nodes.
    /// </summary>
    public interface IAccessLevelService
    {
        /// <summary>
        /// Return true if the setter is public.
        /// </summary>
        /// <param name="propertyNode">true if the property setter is public.</param>
        /// <returns>true if the setter is public.</returns>
        bool IsPropertySetterPublic(PropertyDeclarationSyntax propertyNode);

        /// <summary>
        /// Return true if the supplied property is a boolean property.
        /// </summary>
        /// <param name="propertyNode">the property node.</param>
        /// <returns>true if the property is a boolean property.</returns>
        bool IsPropertyBoolean(PropertyDeclarationSyntax propertyNode);
    }
}