﻿// <copyright file="CommentTextFactory.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// factory to produce comment text based
    /// on the compiler context.
    /// </summary>
    public class CommentTextFactory : ICommentTextFactory
    {
        /// <summary>
        /// leading characters removed from documentation.
        /// </summary>
        private readonly char[] _invalid = { '_', '$' };

        /// <summary>
        /// the access level service.
        /// </summary>
        private readonly IAccessLevelService _accessService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentTextFactory" /> class.
        /// </summary>
        /// <param name="accessService">the access level service.</param>
        public CommentTextFactory(IAccessLevelService accessService)
        {
            this._accessService = accessService;
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
            if (sentence.Count() == 1)
                return $"{sentence.First()} the {string.Join(" ", this.SplitFirstParameterName(methodDeclaration))}.";
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
        /// build the summary text for the supplied member.
        /// </summary>
        /// <param name="variableDeclaratorSyntax">the variable declarator syntax.</param>
        /// <returns>a string containing the text.</returns>
        public string BuildSummaryTextForProperty(VariableDeclaratorSyntax variableDeclaratorSyntax)
        {
            if (variableDeclaratorSyntax == null)
                throw new ArgumentNullException(nameof(variableDeclaratorSyntax));
            var words = this.SplitCamelCaseWords(variableDeclaratorSyntax.Identifier.Text);
            var text = $"the {string.Join(" ", this.RemoveNonPrintables(words))}.";
            return text;
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
        /// Remove articles from the Get / Set text.
        /// which should clean the sentence up a little before
        /// the correct prefix is added.
        /// </summary>
        /// <param name="sentence">a string containing the text.</param>
        /// <returns>the string without leading articles</returns>
        private string RemoveArticles(string sentence)
        {
            string[] articles = { "a", "an", "the", "return", "returns", "gets", "or", "sets" };
            var words = sentence.Split(' ').ToArray();
            while (articles.Any(_ => string.Compare(words.First(), _, StringComparison.CurrentCultureIgnoreCase) == 0))
                words = words.Skip(1).ToArray();
            return string.Join(" ", words);
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
        /// Remove non printable characters.
        /// </summary>
        /// <param name="words">the set of words.</param>
        /// <returns>the strings.</returns>
        private string[] RemoveNonPrintables(string[] words)
        {
            var query = from word in words
                        select word.Trim(this._invalid);
            var result = query.ToArray();
            return result;
        }

        /// <summary>
        /// Split the first parameter name, and return the articles.
        /// </summary>
        /// <param name="methodDeclaration">the method declaration.</param>
        /// <returns>the set of strings.</returns>
        private IEnumerable<string> SplitFirstParameterName(MethodDeclarationSyntax methodDeclaration)
        {
            var parameter = methodDeclaration
                .ParameterList
                .Parameters
                .FirstOrDefault();
            if (parameter == null)
                return new[] { string.Empty };
            return this
                .SplitCamelCaseWords(parameter.Identifier.Text)
                .ToArray();
        }
    }
}