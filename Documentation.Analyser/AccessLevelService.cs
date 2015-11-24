// <copyright file="AccessLevelService.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// service to provide access level
    /// details for syntax nodes.
    /// </summary>
    public class AccessLevelService : IAccessLevelService
    {
        /// <summary>
        /// Return true if the setter is public.
        /// </summary>
        /// <param name="propertyNode">true if the property setter is public.</param>
        /// <returns>true if the setter is public.</returns>
        public bool IsPropertySetterPublic(PropertyDeclarationSyntax propertyNode)
        {
            if (propertyNode == null)
                throw new ArgumentNullException(nameof(propertyNode));

            if (propertyNode.AccessorList == null)
                return false; // expression bodied.

            var accessor = propertyNode
                .AccessorList
                .Accessors
                .FirstOrDefault(_ => _.Kind() == SyntaxKind.SetAccessorDeclaration);
            if (accessor == null)
                return false; // (Set is missing, so it's assumed private)
            var access = this.GetAccessLevel(accessor.Modifiers);
            return access == AccessLevel.Public || access == AccessLevel.NotSpecified;
        }

        /// <summary>
        /// Return true if the supplied property is a boolean property.
        /// </summary>
        /// <param name="propertyNode">the property node.</param>
        /// <returns>true if the property is a boolean property.</returns>
        public bool IsPropertyBoolean(PropertyDeclarationSyntax propertyNode)
        {
            if (propertyNode.Type.Kind() != SyntaxKind.PredefinedType)
                return false;

            var predefined = propertyNode.Type as PredefinedTypeSyntax;
            return predefined?.Keyword.Kind() == SyntaxKind.BoolKeyword;
        }

        /// <summary>Determines the access level for the given <paramref name="modifiers"/>.</summary>
        /// <param name="modifiers">The modifiers.</param>
        /// <returns>A <see cref="AccessLevel"/> value representing the access level.</returns>
        private AccessLevel GetAccessLevel(SyntaxTokenList modifiers)
        {
            var isProtected = false;
            var isInternal = false;
            foreach (var modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        return AccessLevel.Public;
                    case SyntaxKind.PrivateKeyword:
                        return AccessLevel.Private;
                    case SyntaxKind.InternalKeyword:
                        if (isProtected)
                            return AccessLevel.ProtectedInternal;

                        isInternal = true;
                        break;

                    case SyntaxKind.ProtectedKeyword:
                        if (isInternal)
                            return AccessLevel.ProtectedInternal;

                        isProtected = true;
                        break;
                }
            }

            if (isProtected)
                return AccessLevel.Protected;

            if (isInternal)
                return AccessLevel.Internal;

            return AccessLevel.NotSpecified;
        }
    }
}