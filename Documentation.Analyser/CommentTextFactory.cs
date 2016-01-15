// <copyright file="CommentTextFactory.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
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
        private readonly char[] _invalidCharacters = { '_', '$' };

        /// <summary>
        /// invalid words.
        /// </summary>
        private readonly string[] _invalidWords = { "i" };

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
        /// build summary text for a class declaration.
        /// </summary>
        /// <param name="classDeclaration">the class declaration.</param>
        /// <returns>the summary text.</returns>
        public string BuildSummaryTextForClass(ClassDeclarationSyntax classDeclaration)
        {
            var name = classDeclaration.Identifier.Text;
            var sentence = this.SplitCamelCaseWords(name);
            return $"{string.Join(" ", sentence)}.";
        }

        /// <summary>
        /// build up the summary text for a constructor declaration.
        /// </summary>
        /// <param name="constructorDeclaration">the constructor declaration.</param>
        /// <returns>a string containing the summary text.</returns>
        public string BuildSummaryTextForConstructor(ConstructorDeclarationSyntax constructorDeclaration)
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
            var fromName = this.SplitCamelCaseWords(name) ?? new string[] { };
            var fromType = this.CleanVariables(this.SplitCamelCaseWords(parameterSyntax.Type.GetIdentifierName()));
            var sentence = fromName.Length > fromType.Length
                ? fromName
                : fromType;
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
        /// <param name="returnType">the return type for the member variable.</param>
        /// <returns>a string containing the text.</returns>
        public string BuildSummaryTextForMemberVariable(VariableDeclaratorSyntax variableDeclaratorSyntax, VariableDeclarationSyntax returnType)
        {
            if (variableDeclaratorSyntax == null)
                throw new ArgumentNullException(nameof(variableDeclaratorSyntax));
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            var id = returnType.Type.GetIdentifierName();
            var word = variableDeclaratorSyntax.Identifier.Text.Length > (id ?? string.Empty).Length
                ? variableDeclaratorSyntax.Identifier.Text
                : id;
            var words = this.SplitCamelCaseWords(word);
            var text = $"the {string.Join(" ", this.RemoveInvalidPrefix(this.RemoveNonPrintables(words)))}.";
            return text;
        }

        /// <summary>
        /// build the summary text for a return value.
        /// </summary>
        /// <param name="methodDeclaration">the return type for the method.</param>
        /// <returns>a string containing the return type documentation.</returns>
        public string BuildSummaryTextForReturnValue(MethodDeclarationSyntax methodDeclaration)
        {
            var id = methodDeclaration.ReturnType.GetIdentifierName();
            if (id == null)
                return this.BuildSummaryTextForSimpleReturnValue(methodDeclaration);

            var words = this.SplitCamelCaseWords(id);
            var text = $"the {string.Join(" ", this.CleanReturnValueText(words))}.";
            return text;
        }

        /// <summary>
        /// build summary text for a simple return value type.
        /// 'int', 'string' etc.
        /// </summary>
        /// <param name="methodDeclaration">the return type.</param>
        /// <returns>a string containing the simple return value text.</returns>
        public string BuildSummaryTextForSimpleReturnValue(MethodDeclarationSyntax methodDeclaration)
        {
            var name = methodDeclaration.Identifier.Text;
            var sentence = this.SplitCamelCaseWords(name);
            var words = sentence.Count() == 1
                ? this.SplitFirstParameterName(methodDeclaration)
                : sentence;

            // booleans have special text.
            if (methodDeclaration.GetReturnTypeKind() == SyntaxKind.BoolKeyword)
                return $"true if the {string.Join(" ", this.RemoveArticles(words.ToArray()))}, otherwise false.";

            var prefix = this.PrefixAnA(Convert.ToString(methodDeclaration.ReturnType));
            return $"{prefix} {this.CleanReturnType(methodDeclaration.ReturnType)} containing the {string.Join(" ", words)}.";
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
        /// clean the return value text.
        /// </summary>
        /// <param name="words">the set of words</param>
        /// <returns>the cleaned tokens.</returns>
        private IEnumerable<string> CleanReturnValueText(string[] words)
        {
            return this.RemoveInvalidPrefix(this.RemoveNonPrintables(this.ReplaceAngleBraces(words)));
        }

        /// <summary>
        /// Clean variables and invalid characters from the supplied split string.
        /// </summary>
        /// <param name="splitCamelCaseWords">the set of split words.</param>
        /// <returns>the cleaned up string.</returns>
        private string[] CleanVariables(string[] splitCamelCaseWords)
        {
            if (splitCamelCaseWords == null)
                return new string[] { };
            var y = splitCamelCaseWords;
            var z = this.RemoveInvalidPrefix(y);
            var w = this.RemoveNonPrintables(z);
            return w;
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
            var words = sentence.Split(' ').ToArray();
            return this.RemoveArticles(words);
        }

        /// <summary>
        /// Remove articles from the Get / Set text.
        /// which should clean the sentence up a little before
        /// the correct prefix is added.
        /// </summary>
        /// <param name="words">the set of words.</param>
        /// <returns>the string without leading articles</returns>
        private string RemoveArticles(string[] words)
        {
            string[] articles = { "a", "an", "the", "return", "returns", "gets", "or", "sets", "has", "is" };
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
            if (name == null)
                return null;
            var sentence = Regex
                .Split(name, "(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])")
                .Select(_ => _.ToLower())
                .ToArray();
            return sentence;
        }

        /// <summary>
        /// remove invalid prefix words.
        /// </summary>
        /// <param name="words">words like 'I'</param>
        /// <returns>the remaining words.</returns>
        private string[] RemoveInvalidPrefix(string[] words)
        {
            var query = from word in words
                where !this._invalidWords.Contains(word)
                select word;
            var result = query.ToArray();
            return result;
        }

        /// <summary>
        /// Remove non printable characters.
        /// </summary>
        /// <param name="words">the set of words.</param>
        /// <returns>the strings.</returns>
        private string[] RemoveNonPrintables(string[] words)
        {
            var query = from word in words
                        select word.Trim(this._invalidCharacters);
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

        /// <summary>
        /// prefix the supplied word with 'a' or 'an'.
        /// </summary>
        /// <param name="word">a string containing the word to prefix.</param>
        /// <returns>the prefix word.</returns>
        private string PrefixAnA(string word)
        {
            if (string.IsNullOrEmpty(word))
                return "a";
            return new[] { 'a', 'e', 'i', 'o', 'u', 'h' }.Contains(word[0])
                ? "an"
                : "a";
        }

        /// <summary>
        /// replace the angle braces in the return value documentation.
        /// </summary>
        /// <param name="words">the words.</param>
        /// <returns>the new tokens.</returns>
        private string[] ReplaceAngleBraces(string[] words)
        {
            return words
                .Select(_ => _.Replace("<", "{").Replace(">", "}"))
                .ToArray();
        }

        /// <summary>
        /// use full names of types.
        /// </summary>
        /// <param name="returnType">the return type.</param>
        /// <returns>a string containing the type text.</returns>
        private string GetTypeName(TypeSyntax returnType)
        {
            var typeString = Convert.ToString(returnType);
            switch (typeString)
            {
                case "int":
                    return "integer";
                case "bool":
                    return "boolean";
                default:
                    return typeString;
            }
        }

        /// <summary>
        /// clean the return type.
        /// </summary>
        /// <param name="returnType">the return type.</param>
        /// <returns>a string containing the cleaned text.</returns>
        private string CleanReturnType(TypeSyntax returnType)
        {
            var braces = Convert.ToString(returnType)
                .Replace("<", "{").Replace(">", "}");
            if (braces.Contains("{") && braces.Contains("}"))
                return $"<see cref=\"{braces}\"/>";
            return braces;
        }
    }
}