// <copyright file="CommentTextFactory.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// factory to produce comment text based
    /// on the compiler context.
    /// </summary>
    public class CommentTextFactory : ICommentTextFactory
    {
        /// <summary>
        /// the access level service.
        /// </summary>
        private readonly IAccessLevelService _accessService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentTextFactory"/> class.
        /// </summary>
        /// <param name="accessService">the access level service.</param>
        public CommentTextFactory(IAccessLevelService accessService)
        {
            this._accessService = accessService;
        }

        /// <summary>
        /// build the summary text for a property based on the
        /// supplied synax propertyDeclaration.
        /// </summary>
        /// <param name="propertyNode">the syntax propertyDeclaration.</param>
        /// <returns>the summary text.</returns>
        public string BuildSummaryTextForProperty(SyntaxNode propertyNode)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)propertyNode;
            if (propertyDeclaration == null)
                throw new InvalidOperationException($"property declaration expected {propertyNode.ToFullString()}");

            var name = propertyDeclaration.Identifier.Text;
            var sentence = this.SplitCamelCaseWords(name);
            var prefix = this.BuildSummaryTextPrefixForProperty(propertyDeclaration);
            return $"{prefix} {string.Join(" ", sentence)}.";
        }

        /// <summary>
        /// Build the summary text for a method declaration.
        /// </summary>
        /// <param name="methodNode">the method node.</param>
        /// <returns>a string containing the summary text.</returns>
        public string BuildSummaryTextForMethod(SyntaxNode methodNode)
        {
            var methodDeclaration = (MethodDeclarationSyntax)methodNode;
            if (methodDeclaration == null)
                throw new InvalidOperationException($"property declaration expected {methodNode.ToFullString()}");

            var name = methodDeclaration.Identifier.Text;
            var sentence = this.SplitCamelCaseWords(name);
            return $"{sentence.First()} the {string.Join(" ", sentence.Skip(1))}.";
        }

        /// <summary>
        /// build the summary text appropriate for a parameter.
        /// </summary>
        /// <param name="parameterSyntax">the parameter.</param>
        /// <returns>a string containing the short description.</returns>
        public string BuildSummaryTextForParameter(ParameterSyntax parameterSyntax)
        {
            var name = parameterSyntax.Identifier.Text;
            var sentence = this.SplitCamelCaseWords(name);
            return $"the {string.Join(" ", sentence)}.";
        }

        /// <summary>
        /// build the summary prefix text.
        /// </summary>
        /// <param name="propertyNode">the property propertyDeclaration.</param>
        /// <returns>a string containing the summary text.</returns>
        public string BuildSummaryTextPrefixForProperty(PropertyDeclarationSyntax propertyNode)
        {
            var set = this._accessService.IsPropertySetterPublic(propertyNode);
            var isBoolean = this._accessService.IsPropertyBoolean(propertyNode);
            var setterText = set ? " or sets" : string.Empty;
            return isBoolean
                ? $"Gets{setterText} a value indicating whether"
                : $"Gets{setterText} the";
        }

        /// <summary>
        /// split camel case words.
        /// </summary>
        /// <param name="name">the identifier name.</param>
        /// <returns>the set of words.</returns>
        private string[] SplitCamelCaseWords(string name)
        {
            var sentence = Regex
                .Split(name, "(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])")
                .Select(_ => _.ToLower())
                .ToArray();
            return sentence;
        }
    }
}