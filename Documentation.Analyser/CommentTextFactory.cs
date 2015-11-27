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
        /// <param name="propertyDeclaration">the syntax propertyDeclaration.</param>
        /// <returns>the summary text.</returns>
        public string BuildSummaryTextForProperty(PropertyDeclarationSyntax propertyDeclaration)
        {
            var name = propertyDeclaration.Identifier.Text;
            var sentence = this.SplitCamelCaseWords(name);
            var prefix = this.BuildSummaryTextPrefixForProperty(propertyDeclaration);
            return $"{prefix} {string.Join(" ", sentence)}.";
        }

        /// <summary>
        /// Build summary text for a property based on the supplied text.
        /// The text is corrected to start with the correct prefix.
        /// </summary>
        /// <param name="propertyDeclaration">the property declaration.</param>
        /// <param name="text">the summary text.</param>
        /// <returns>a string containing the summary text.</returns>
        public string[] BuildSummaryTextForProperty(PropertyDeclarationSyntax propertyDeclaration, string[] text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var startingText = this.RemoveArticles(text.First());
            var strings = text.Skip(1).Prepend(() => startingText).ToArray();
            var prefix = this.BuildSummaryTextPrefixForProperty(propertyDeclaration);
            return new[] { $"{prefix} {strings.First()}" }.Concat(strings.Skip(1)).ToArray();
        }

        /// <summary>
        /// build up the summary text for a constructor declaration.
        /// </summary>
        /// <param name="constructorDeclaration">the constructor declaration.</param>
        /// <returns>a string containing the summary text.</returns>
        public string BuildSummaryTextForClass(ConstructorDeclarationSyntax constructorDeclaration)
        {
            var name = constructorDeclaration.Identifier.Text;
            var sentence = $"Initializes a new instance of the <see cref=\"{name}\" /> class.";
            return sentence;
        }

        /// <summary>
        /// Build the summary text for a method declaration.
        /// </summary>
        /// <param name="methodDeclaration">the method node.</param>
        /// <returns>a string containing the summary text.</returns>
        public string BuildSummaryTextForMethod(MethodDeclarationSyntax methodDeclaration)
        {
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

        /// <summary>
        /// Remove articles from the Get / Set text.
        /// which should clean the sentence up a little before
        /// the correct prefix is added.
        /// </summary>
        /// <param name="sentence">a string containing the text.</param>
        /// <returns>the string without leading articles</returns>
        private string RemoveArticles(string sentence)
        {
            string[] articles = new[] { "a", "an", "the", "return", "returns", "gets", "or", "sets" };
            var words = sentence.Split(' ').ToArray();
            while (articles.Any(_ => string.Compare(words.First(), _, StringComparison.CurrentCultureIgnoreCase) == 0))
                words = words.Skip(1).ToArray();
            return string.Join(" ", words);
        }
    }
}