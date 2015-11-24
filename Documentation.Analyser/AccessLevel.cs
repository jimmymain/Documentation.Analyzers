// <copyright file="AccessLevel.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    /// <summary>
    /// Describes an element's access level
    /// </summary>
    internal enum AccessLevel
    {
        /// <summary>
        /// No access level specified.
        /// </summary>
        NotSpecified,

        /// <summary>
        /// Private access.
        /// </summary>
        Private,

        /// <summary>
        /// Protected access.
        /// </summary>
        Protected,

        /// <summary>
        /// Protected internal access.
        /// </summary>
        ProtectedInternal,

        /// <summary>
        /// Internal access.
        /// </summary>
        Internal,

        /// <summary>
        /// Public access.
        /// </summary>
        Public
    }
}