// <copyright file="CommentNodeFactory.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// a factory for creating comment nodes.
    /// </summary>
    public class CommentNodeFactory
    {
        /// <summary>
        /// the new line characters to use (replace with lookup later).
        /// </summary>
        private const string NewLine = "\r\n";

        /// <summary>
        /// the comment text factory.
        /// </summary>
        private readonly ICommentTextFactory _commentTextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentNodeFactory" /> class.
        /// </summary>
        /// <param name="commentTextFactory">the comment text factory.</param>
        public CommentNodeFactory(ICommentTextFactory commentTextFactory)
        {
            this._commentTextFactory = commentTextFactory;
        }

        /// <summary>
        /// create a comment summary text field for the supplied class declaration.
        /// </summary>
        /// <param name="classDeclaration">the declaration.</param>
        /// <returns>the xml element syntax.</returns>
        public XmlElementSyntax CreateCommentSummaryText(ClassDeclarationSyntax classDeclaration)
        {
            var sentence = this._commentTextFactory.BuildSummaryTextForClass(classDeclaration);
            return this.CreateCommentTextElementForSentence(sentence);
        }

        /// <summary>
        /// create a comment summary text field for the supplied method declaration.
        /// </summary>
        /// <param name="methodDeclaration">the method declaration</param>
        /// <returns>the xml nodes containing the summary.</returns>
        public XmlElementSyntax CreateCommentSummaryText(MethodDeclarationSyntax methodDeclaration)
        {
            var sentence = this._commentTextFactory.BuildSummaryTextForMethod(methodDeclaration);
            return this.CreateCommentTextElementForSentence(sentence);
        }

        /// <summary>
        /// Create the comment summary text for a constructor.
        /// </summary>
        /// <param name="constructorDeclaration">the constructor declaration</param>
        /// <returns>the xml node syntax for the constructor.</returns>
        public XmlNodeSyntax CreateCommentSummaryText(ConstructorDeclarationSyntax constructorDeclaration)
        {
            var sentence = this._commentTextFactory.BuildSummaryTextForConstructor(constructorDeclaration);
            return this.CreateCommentTextElementForSentence(sentence);
        }

        /// <summary>
        /// create the comment summary text for a property.
        /// </summary>
        /// <param name="propertyDeclaration">the property declaration.</param>
        /// <returns>the summary comment node.</returns>
        public XmlNodeSyntax CreateCommentSummaryText(PropertyDeclarationSyntax propertyDeclaration)
        {
            var sentence = this._commentTextFactory.BuildSummaryTextForProperty(propertyDeclaration);
            return this.CreateCommentTextElementForSentence(sentence);
        }

        /// <summary>
        /// Create summary text for the supplied field declaration syntax.
        /// </summary>
        /// <param name="fieldDeclaration">the field declaration.</param>
        /// <returns>the xml node syntax.</returns>
        public XmlNodeSyntax CreateCommentSummaryText(FieldDeclarationSyntax fieldDeclaration)
        {
            var identifier = fieldDeclaration
                .DescendantNodesAndSelf()
                .OfType<VariableDeclaratorSyntax>()
                .FirstOrDefault();
            var returnType = fieldDeclaration
                .DescendantNodesAndSelf()
                .OfType<VariableDeclarationSyntax>()
                .FirstOrDefault();
            var sentence = this._commentTextFactory.BuildSummaryTextForMemberVariable(identifier, returnType);
            return this.CreateCommentTextElementForSentence(sentence);
        }

        /// <summary>
        /// take the comment text for an existing property, and correct it.
        /// </summary>
        /// <param name="propertyDeclaration">the property documentation.</param>
        /// <param name="documentComment">the document comment text.</param>
        /// <returns>the summary node.</returns>
        public XmlNodeSyntax CreateCommentSummaryTextFromExistingProperty(
            PropertyDeclarationSyntax propertyDeclaration,
            DocumentationCommentTriviaSyntax documentComment)
        {
            var text = documentComment.GetExistingSummaryCommentDocumentation();
            if (text == null || !text.Any())
                return null;

            var sentence = this._commentTextFactory.BuildSummaryTextForProperty(propertyDeclaration, text);
            return this.CreateCommentTextElementForSentence(sentence);
        }

        /// <summary>
        /// Create documentation syntax for the supplied content.
        /// </summary>
        /// <param name="content">the xml nodes containing the comment content.</param>
        /// <returns>the documentation comment syntax.</returns>
        public DocumentationCommentTriviaSyntax CreateDocumentComment(params XmlNodeSyntax[] content)
        {
            return SyntaxFactory.DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxFactory.List(content))
                .WithLeadingTrivia(
                    SyntaxFactory.DocumentationCommentExterior("/// "))
                .WithTrailingTrivia(SyntaxFactory.EndOfLine(NewLine))
                .WithAdditionalAnnotations(Formatter.Annotation);
        }

        /// <summary>
        /// Create a list of parameter elements.
        /// </summary>
        /// <param name="constructorDeclaration">the constructor declaration</param>
        /// <param name="documentComment">the set of comments.</param>
        /// <returns>the set of parameter documentation elements.</returns>
        public IEnumerable<XmlNodeSyntax> CreateParameters(
            ConstructorDeclarationSyntax constructorDeclaration,
            DocumentationCommentTriviaSyntax documentComment)
        {
            var query = constructorDeclaration
                .ParameterList
                .Parameters
                .Select(
                    _ =>
                        this.CreateParameter(
                            _,
                            this.GetExistingParameterDocumentation(_.Identifier.Text, documentComment)));
            var results = query.ToArray();
            return results;
        }

        /// <summary>
        /// create a list of parameter elements.
        /// </summary>
        /// <param name="methodDeclaration">the method declaration.</param>
        /// <param name="documentComment">the document comment.</param>
        /// <returns>the list of correctly placed parameters.</returns>
        public XmlElementSyntax[] CreateParameters(
            MethodDeclarationSyntax methodDeclaration,
            DocumentationCommentTriviaSyntax documentComment)
        {
            var query = methodDeclaration
                .ParameterList
                .Parameters
                .Select(
                    _ =>
                        this.CreateParameter(
                            _,
                            this.GetExistingParameterDocumentation(_.Identifier.Text, documentComment)));
            var results = query.ToArray();
            return results;
        }

        /// <summary>
        /// create the documentation for a return value.
        /// </summary>
        /// <param name="methodDeclaration">the method declaration.</param>
        /// <returns>the return value documentation.</returns>
        public XmlElementSyntax CreateReturnValueDocumentation(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.HasVoidReturnType())
                return null;

            var text = this._commentTextFactory.BuildSummaryTextForReturnValue(methodDeclaration);
            if (text == null)
                return null;

            var sentences = from line in new[] { text }
                select SyntaxFactory.XmlText(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.XmlTextLiteral(
                            SyntaxFactory.TriviaList(),
                            line,
                            line,
                            SyntaxFactory.TriviaList())));

            var withNewLines = sentences
                .Intersperse(this.CreateNewLine);

            var summary = SyntaxFactory.XmlElement(
                SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("returns")),
                SyntaxFactory.List<XmlNodeSyntax>(withNewLines),
                SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("returns")))
                .WithLeadingTrivia(
                    SyntaxFactory.EndOfLine(NewLine),
                    SyntaxFactory.DocumentationCommentExterior("/// "));
            return summary;
        }

        /// <summary>
        /// return the existing documentation comments
        /// </summary>
        /// <param name="documentComment">the document comment.</param>
        /// <returns>a string containing the existing comment.</returns>
        public XmlElementSyntax GetExistingSummaryCommentText(DocumentationCommentTriviaSyntax documentComment)
        {
            var summaryElement = documentComment?.Content
                .OfType<XmlElementSyntax>()
                .FirstOrDefault(_ => _.StartTag.Name.LocalName.Text == "summary");
            if (summaryElement == null)
                return null;

            var rawText = summaryElement.GetXmlTextSyntaxLines();
            if (!rawText.Any())
                return null; // there is no documentation.
            var xmlText = this.CreateLinesOfCommentTextFromStrings(rawText);

            var summary = SyntaxFactory.XmlElement(
                SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
                SyntaxFactory.List(xmlText),
                SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));
            return summary;
        }

        /// <summary>
        /// prepend the standard comment text to the constructor description.
        /// </summary>
        /// <param name="constructorDeclaration">the constructor declaration.</param>
        /// <param name="lines">the lines of comment text.</param>
        /// <returns>the set of xml nodes.</returns>
        public XmlElementSyntax PrependStandardCommentText(
            ConstructorDeclarationSyntax constructorDeclaration,
            string[] lines)
        {
            return this.BuildStandardConstructorCommentText(
                constructorDeclaration,
                this.StripExistingInitializationComment(lines).ToArray());
        }

        /// <summary>
        /// build standard constructor declaration text.
        /// </summary>
        /// <param name="constructorDeclaration">the constructor declaration.</param>
        /// <param name="lines">existing lines of text (if any)</param>
        /// <returns>the xml element syntax.</returns>
        private XmlElementSyntax BuildStandardConstructorCommentText(
            ConstructorDeclarationSyntax constructorDeclaration,
            string[] lines)
        {
            var typeDeclaration = constructorDeclaration.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();
            var classDeclaration = typeDeclaration as ClassDeclarationSyntax;
            var additional = lines
                .Select(this.CreateXmlTextNode)
                .Intersperse(this.CreateNewLine);
            if (additional.Any())
                additional = additional.Prepend(this.CreateNewLine);
            var nodes = this.BuildStandardText(
                typeDeclaration.Identifier,
                classDeclaration.TypeParameterList,
                "Initializes a new instance of the ",
                " class.")
                .Prepend(this.CreateNewLine)
                .Concat(additional)
                .Append(this.CreateNewLine);

            var summary = SyntaxFactory.XmlElement(
                SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
                SyntaxFactory.List(nodes),
                SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));
            return summary;
        }

        /// <summary>
        /// Build the standard constructor text.
        /// </summary>
        /// <param name="identifier">the identifer for the constructor.</param>
        /// <param name="typeParameters">the set of type parameters.</param>
        /// <param name="preText">the pre-text.</param>
        /// <param name="postText">the post-text.</param>
        /// <returns>the xml node syntax.</returns>
        private XmlNodeSyntax[] BuildStandardText(
            SyntaxToken identifier,
            TypeParameterListSyntax typeParameters,
            string preText,
            string postText)
        {
            TypeSyntax identifierName;

            // Get a TypeSyntax representing the class name with its type parameters
            if (typeParameters == null || !typeParameters.Parameters.Any())
                identifierName = SyntaxFactory.IdentifierName(identifier.Text);
            else
                identifierName = SyntaxFactory.GenericName(
                    identifier.WithoutTrivia(),
                    this.ParameterToArgumentListSyntax(typeParameters));

            var cred = SyntaxFactory.TypeCref(identifierName);
            var xmlRef = SyntaxFactory.XmlCrefAttribute(
                SyntaxFactory.XmlName("cref"),
                SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken),
                cred.ReplaceTokens(cred.DescendantTokens(), this.ReplaceBraceTokens),
                SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken))
                .WithLeadingTrivia(SyntaxFactory.Whitespace(" "));
            var see = SyntaxFactory.XmlEmptyElement(SyntaxFactory.XmlName("see"))
                .AddAttributes(xmlRef);

            return new XmlNodeSyntax[]
            {
                this.CreateXmlTextNode(preText),
                see,
                this.CreateXmlTextNode(postText)
            };
        }

        /// <summary>
        /// Create a multi line comment based on the contents
        /// of the supplied strings.
        /// </summary>
        /// <param name="linesOfText">the text lines.</param>
        /// <returns>a text xml element containing the resulting documentation.</returns>
        private XmlElementSyntax CreateCommentTextElementForSentence(params string[] linesOfText)
        {
            var sentences = from line in linesOfText
                select SyntaxFactory.XmlText(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.XmlTextLiteral(
                            SyntaxFactory.TriviaList(),
                            line,
                            line,
                            SyntaxFactory.TriviaList())));

            var withNewLines = sentences
                .Intersperse(this.CreateNewLine)
                .Prepend(this.CreateNewLine)
                .Append(this.CreateNewLine);

            var summary = SyntaxFactory.XmlElement(
                SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
                SyntaxFactory.List<XmlNodeSyntax>(withNewLines),
                SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));
            return summary;
        }

        /// <summary>
        /// create a single line of comment text delimited by new lines.
        /// </summary>
        /// <param name="text">the documentation text.</param>
        /// <returns>the xml text node.</returns>
        private SyntaxList<XmlNodeSyntax> CreateLinesOfCommentTextFromStrings(params string[] text)
        {
            var lines = text
                .Select(this.CreateXmlTextNode)
                .Intersperse(this.CreateNewLine)
                .Prepend(this.CreateNewLine)
                .Append(this.CreateNewLine);

            var delimitedText = SyntaxFactory.List<XmlNodeSyntax>(lines);
            return delimitedText;
        }

        /// <summary>
        /// Create a new line, suitable for inclusion in EOL summary comments.
        /// </summary>
        /// <returns>the xml containing the new line</returns>
        private XmlTextSyntax CreateNewLine()
        {
            var token = SyntaxFactory.XmlTextNewLine(
                SyntaxFactory.TriviaList(),
                NewLine,
                NewLine,
                SyntaxFactory.TriviaList());
            var commentNewLine = token.WithTrailingTrivia(SyntaxFactory.DocumentationCommentExterior("/// "));
            return SyntaxFactory.XmlText(SyntaxFactory.TokenList(commentNewLine));
        }

        /// <summary>
        /// Create a parameter element for the supplied parameter syntax.
        /// </summary>
        /// <param name="parameterSyntax">the parameter syntax.</param>
        /// <param name="existingDocumentation">the existing parameter documentation.</param>
        /// <returns>the corresponding parameter XML entry.</returns>
        private XmlElementSyntax CreateParameter(ParameterSyntax parameterSyntax, string[] existingDocumentation)
        {
            var identifier = SyntaxFactory.Identifier(parameterSyntax.Identifier.Text);
            var description = this._commentTextFactory.BuildSummaryTextForParameter(parameterSyntax);

            var attribute = SyntaxFactory.XmlNameAttribute(
                SyntaxFactory.XmlName("name"),
                SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken),
                SyntaxFactory.IdentifierName(identifier),
                SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken))
                .WithLeadingTrivia(SyntaxFactory.Space);

            var startTag = SyntaxFactory
                .XmlElementStartTag(SyntaxFactory.XmlName("param"))
                .WithAttributes(
                    default(SyntaxList<XmlAttributeSyntax>)
                        .Add(attribute));

            var endTag = SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("param"));

            var documentation = existingDocumentation != null && existingDocumentation.Any()
                ? existingDocumentation
                : new[] { description };

            var xmlText = documentation.Select(this.CreateXmlTextNode);
            var delimitedText = SyntaxFactory.List<XmlNodeSyntax>(xmlText);

            return SyntaxFactory.XmlElement(
                startTag,
                delimitedText,
                endTag)
                .WithLeadingTrivia(
                    SyntaxFactory.EndOfLine(NewLine),
                    SyntaxFactory.DocumentationCommentExterior("/// "));
        }

        /// <summary>
        /// Create an xml text node from the supplied text.
        /// the resulting node is lean, and does not contain leading new lines or spaces.
        /// </summary>
        /// <param name="text">the documentation text.</param>
        /// <returns>the xml node.</returns>
        private XmlTextSyntax CreateXmlTextNode(string text)
        {
            var xml = SyntaxFactory.XmlText(
                SyntaxFactory.TokenList(
                    SyntaxFactory.XmlTextLiteral(
                        SyntaxFactory.TriviaList(),
                        text,
                        text,
                        SyntaxFactory.TriviaList())));
            return xml;
        }

        /// <summary>
        /// return the lines of text in the existing parameter documentation.
        /// </summary>
        /// <param name="parameterName">the parameter name.</param>
        /// <param name="documentComment">the document comment.</param>
        /// <returns>the lines of documentation.</returns>
        private string[] GetExistingParameterDocumentation(
            string parameterName,
            DocumentationCommentTriviaSyntax documentComment)
        {
            var parameter = documentComment?.Content
                .OfType<XmlElementSyntax>()
                .Where(_ => _.StartTag.Name.LocalName.Text == "param")
                .FirstOrDefault(
                    _ => _.StartTag
                        .Attributes
                        .OfType<XmlNameAttributeSyntax>()
                        .Any(name => name.Identifier.Identifier.Text == parameterName));

            var lines = parameter?.GetXmlTextSyntaxLines();
            return lines;
        }

        /// <summary>
        /// Create a type argument list from the supplied type parameters.
        /// </summary>
        /// <param name="typeParameters">the type parameters.</param>
        /// <returns>the type argument list.</returns>
        private TypeArgumentListSyntax ParameterToArgumentListSyntax(TypeParameterListSyntax typeParameters)
        {
            var list = SyntaxFactory.SeparatedList<TypeSyntax>();
            list =
                list.AddRange(
                    typeParameters.Parameters.Select(p => SyntaxFactory.ParseName(p.ToString()).WithTriviaFrom(p)));

            for (var i = 0; i < list.SeparatorCount; i++)
            {
                // Make sure the parameter list looks nice
                var separator = list.GetSeparator(i);
                list = list.ReplaceSeparator(separator, separator.WithTrailingTrivia(SyntaxFactory.Space));
            }

            return SyntaxFactory.TypeArgumentList(list);
        }

        /// <summary>
        /// Replace the brace tokens with curly braces.
        /// </summary>
        /// <param name="originalToken">the original token.</param>
        /// <param name="rewrittenToken">the rewritten token.</param>
        /// <returns>the replaced token.</returns>
        private SyntaxToken ReplaceBraceTokens(SyntaxToken originalToken, SyntaxToken rewrittenToken)
        {
            if (rewrittenToken.IsKind(SyntaxKind.LessThanToken)
                && string.Equals("<", rewrittenToken.Text, StringComparison.Ordinal))
                return SyntaxFactory.Token(
                    rewrittenToken.LeadingTrivia,
                    SyntaxKind.LessThanToken,
                    "{",
                    rewrittenToken.ValueText,
                    rewrittenToken.TrailingTrivia);

            if (rewrittenToken.IsKind(SyntaxKind.GreaterThanToken)
                && string.Equals(">", rewrittenToken.Text, StringComparison.Ordinal))
                return SyntaxFactory.Token(
                    rewrittenToken.LeadingTrivia,
                    SyntaxKind.GreaterThanToken,
                    "}",
                    rewrittenToken.ValueText,
                    rewrittenToken.TrailingTrivia);

            return rewrittenToken;
        }

        /// <summary>
        /// strip out the existing initializes an instance of.
        /// </summary>
        /// <param name="lines">the lines of text.</param>
        /// <returns>the stripped comment text.</returns>
        private IEnumerable<string> StripExistingInitializationComment(IEnumerable<string> lines)
        {
            var search = new[] { "initializes a", "class." };
            var query = from line in lines
                where search.All(_ => line.IndexOf(_, StringComparison.CurrentCultureIgnoreCase) < 0)
                select line;
            return query;
        }
    }
}