// <copyright file="CommentNodeInterrogationExtensions.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// extension methods to interrogate comment trivia.
    /// </summary>
    public static class CommentNodeInterrogationExtensions
    {
        /// <summary>
        /// Obtain the comment syntax trivia.
        /// </summary>
        /// <param name="node">the starting node.</param>
        /// <returns>the document comment trivia</returns>
        public static DocumentationCommentTriviaSyntax GetDocumentationCommentTriviaSyntax(this SyntaxNode node)
        {
            if (node == null)
            {
                return null;
            }

            foreach (var leadingTrivia in node.GetLeadingTrivia())
            {
                var structure = leadingTrivia.GetStructure() as DocumentationCommentTriviaSyntax;

                if (structure != null)
                {
                    return structure;
                }
            }

            return null;
        }

        /// <summary>
        /// Return the existing summary documentation.
        /// </summary>
        /// <param name="existingComment">the existing document comment syntax.</param>
        /// <returns>the existing documentation.</returns>
        public static string[] GetExistingSummaryCommentDocumentation(
            this DocumentationCommentTriviaSyntax existingComment)
        {
            var text = existingComment?.Content
                .OfType<XmlElementSyntax>()
                .Where(_ => _.StartTag.Name.LocalName.Text == "summary")
                .SelectMany(_ => _.GetXmlTextSyntaxLines());
            var comments = text?.ToArray();
            return comments;
        }

        /// <summary>
        /// return all the xml documentation elements that are parameter(s).
        /// </summary>
        /// <param name="commentSyntax">the full comment syntax.</param>
        /// <returns>the list of parameters.</returns>
        internal static XmlElementSyntax GetReturnDocumentationElement(
            this DocumentationCommentTriviaSyntax commentSyntax)
        {
            var syntax = from node in commentSyntax.DescendantNodes()
                where node.Kind() == SyntaxKind.XmlElement
                from child in node.ChildNodes()
                where child.Kind() == SyntaxKind.XmlElementStartTag
                where ((XmlElementStartTagSyntax)child).Name.LocalName.Text == "returns"
                select (XmlElementSyntax)node;
            return syntax.FirstOrDefault();
        }

        /// <summary>
        /// return all the xml documentation elements that are parameter(s).
        /// </summary>
        /// <param name="commentSyntax">the full comment syntax.</param>
        /// <returns>the list of parameters.</returns>
        internal static XmlElementSyntax[] GetParameterDocumentationElements(
            this DocumentationCommentTriviaSyntax commentSyntax)
        {
            var syntax = from node in commentSyntax.DescendantNodes()
                where node.Kind() == SyntaxKind.XmlElement
                from child in node.ChildNodes()
                where child.Kind() == SyntaxKind.XmlElementStartTag
                where ((XmlElementStartTagSyntax)child).Name.LocalName.Text == "param"
                select (XmlElementSyntax)node;
            return syntax.ToArray();
        }

        /// <summary>
        /// Return the names of each of the supplied parameter nodes.
        /// </summary>
        /// <param name="parameterXmlElements">the parameter xml elements.</param>
        /// <returns>a string array containing the names.</returns>
        internal static string[] GetParameterNames(this XmlElementSyntax[] parameterXmlElements)
        {
            var names = from node in parameterXmlElements
                from name in node.DescendantNodes()
                where name.Kind() == SyntaxKind.XmlNameAttribute
                let xmlName = (XmlNameAttributeSyntax)name
                select xmlName.Identifier.Identifier.Text;
            var result = names.ToArray();
            return result;
        }

        /// <summary>
        /// Return the lines of text in the supplied xml element.
        /// </summary>
        /// <param name="xmlElementSyntax">the xml syntax node.</param>
        /// <returns>a list of strings containing the documentation text.</returns>
        internal static string[] GetXmlTextSyntaxLines(this XmlElementSyntax xmlElementSyntax)
        {
            if (xmlElementSyntax == null)
                throw new ArgumentNullException(nameof(xmlElementSyntax));

            var xmlText = xmlElementSyntax
                .Content
                .OfType<XmlTextSyntax>()
                .SelectMany(_ => _.TextTokens)
                .Where(_ => _.Kind() == SyntaxKind.XmlTextLiteralToken)
                .Select(_ => _.Text.Trim())
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .ToArray();
            return xmlText;
        }

        /// <summary>
        /// return the xml text syntax, which can be checked for compliance.
        /// </summary>
        /// <param name="xmlComment">the xml comment node.</param>
        /// <returns>the xml text node, or null</returns>
        internal static string GetXmlTextSyntax(this DocumentationCommentTriviaSyntax xmlComment)
        {
            if (xmlComment == null)
                return null;

            var summary = xmlComment
                .Content
                .Where(_ => _.Kind() == SyntaxKind.XmlElement)
                .OfType<XmlElementSyntax>()
                .ToArray();

            var text = summary
                .SelectMany(_ => _.Content)
                .Where(_ => _.Kind() == SyntaxKind.XmlText)
                .OfType<XmlTextSyntax>()
                .SelectMany(_ => _.TextTokens)
                .FirstOrDefault(_ => _.Kind() == SyntaxKind.XmlTextLiteralToken);
            return text.Text;
        }

        /// <summary>
        /// Return true if the document comment text contains a summary element.
        /// </summary>
        /// <param name="node">the comment node.</param>
        /// <returns>true if the summary element is missing.</returns>
        internal static bool HasSummary(this SyntaxNode node)
        {
            var documentComment = node.GetDocumentationCommentTriviaSyntax();
            var summaryElement = documentComment?.Content
                .OfType<XmlElementSyntax>()
                .FirstOrDefault(_ => _.StartTag.Name.LocalName.Text == "summary");
            return summaryElement != null;
        }

        /// <summary>
        /// Checks if a specific SyntaxNode has documentation in it's leading trivia.
        /// </summary>
        /// <param name="node">The syntax node that should be checked.</param>
        /// <returns>true if the node has documentation, false otherwise.</returns>
        internal static bool HasDocumentation(this SyntaxNode node)
        {
            var commentTrivia = node.GetDocumentationCommentTriviaSyntax();
            return commentTrivia != null && !IsMissingOrEmpty(commentTrivia.ParentTrivia);
        }

        /// <summary>
        /// Checks if a SyntaxTrivia contains a DocumentationCommentTriviaSyntax and returns true if it is considered empty
        /// </summary>
        /// <param name="commentTrivia">A SyntaxTrivia containing possible documentation</param>
        /// <returns>true if commentTrivia does not have documentation in it or the documentation in SyntaxTriviais considered empty. False otherwise.</returns>
        internal static bool IsMissingOrEmpty(SyntaxTrivia commentTrivia)
        {
            if (!commentTrivia.HasStructure)
                return true;

            var structuredTrivia = commentTrivia.GetStructure() as DocumentationCommentTriviaSyntax;
            return structuredTrivia == null || IsEmpty(structuredTrivia);
        }

        /// <summary>
        /// Check if the return type of the supplied method is 'void'
        /// </summary>
        /// <param name="method">the method declaration.</param>
        /// <returns>true if the method has a 'void' return type.</returns>
        internal static bool HasVoidReturnType(this MethodDeclarationSyntax method)
        {
            return (method.ReturnType as PredefinedTypeSyntax)?.Keyword.Kind() == SyntaxKind.VoidKeyword;
        }

        /// <summary>
        /// Check if the return type of the supplied method is 'void'
        /// </summary>
        /// <param name="method">the method declaration.</param>
        /// <returns>true if the method has a 'void' return type.</returns>
        internal static SyntaxKind? GetReturnTypeKind(this MethodDeclarationSyntax method)
        {
            return (method.ReturnType as PredefinedTypeSyntax)?.Keyword.Kind();
        }

        /// <summary>
        /// This helper is used by documentation diagnostics to check if a XML comment should be considered empty.
        /// A comment is empty if
        /// - it is null
        /// - it does not have any text in any XML element and it does not have an empty XML element in it.
        /// </summary>
        /// <param name="xmlComment">The xmlComment that should be checked</param>
        /// <returns>true, if the comment should be considered empty, false otherwise.</returns>
        internal static bool IsEmpty(DocumentationCommentTriviaSyntax xmlComment)
        {
            if (xmlComment == null)
                return true;

            return xmlComment.Content.All(IsEmpty);
        }

        /// <summary>
        /// return the identifier name embedded in the supplied type information.
        /// </summary>
        /// <param name="typeSyntax">the type syntax.</param>
        /// <returns>a string containing the identifier name.</returns>
        internal static string GetIdentifierName(this TypeSyntax typeSyntax)
        {
            var name = typeSyntax as IdentifierNameSyntax;
            return name == null ? null : name.Identifier.Text;
        }

        /// <summary>
        /// This helper is used by documentation diagnostics to check if a XML comment should be considered empty.
        /// A comment is empty if it does not have any text in any XML element and it does not have an empty XML element in it.
        /// </summary>
        /// <param name="xmlSyntax">The xmlSyntax that should be checked</param>
        /// <returns>true, if the comment should be considered empty, false otherwise.</returns>
        internal static bool IsEmpty(XmlNodeSyntax xmlSyntax)
        {
            var text = xmlSyntax as XmlTextSyntax;
            if (text != null)
                return text.TextTokens.All(token => string.IsNullOrWhiteSpace(token.ToString()));

            var element = xmlSyntax as XmlElementSyntax;
            if (element != null)
                return element.Content.All(IsEmpty);

            var cdataElement = xmlSyntax as XmlCDataSectionSyntax;
            if (cdataElement != null)
                return cdataElement.TextTokens.All(token => string.IsNullOrWhiteSpace(token.ToString()));

            var emptyElement = xmlSyntax as XmlEmptyElementSyntax;
            if (emptyElement != null)
            {
                // This includes <inheritdoc/>
                return false;
            }

            var processingElement = xmlSyntax as XmlProcessingInstructionSyntax;
            return processingElement == null;
        }

        /// <summary>
        /// prepend the supplied element.
        /// </summary>
        /// <typeparam name="T">the type of enumerable.</typeparam>
        /// <param name="source">the source elements.</param>
        /// <param name="element">the element to prepend.</param>
        /// <returns>the resulting list.</returns>
        internal static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, Func<T> element)
        {
            return new T[] { element() }.Concat(source);
        }

        /// <summary>
        /// append the supplied element, and return the resulting array.
        /// </summary>
        /// <typeparam name="T">the type of enumerable.</typeparam>
        /// <param name="source">the source elements.</param>
        /// <param name="element">the element to append.</param>
        /// <returns>the resulting list.</returns>
        internal static IEnumerable<T> Append<T>(this IEnumerable<T> source, Func<T> element)
        {
            return source.Concat(new T[] { element() });
        }

        /// <summary>
        /// intersperse the supplied elements.
        /// </summary>
        /// <typeparam name="T">the type of enumerable.</typeparam>
        /// <param name="source">the source elements.</param>
        /// <param name="element">the element to intersperse with.</param>
        /// <returns>the resulting list.</returns>
        internal static IEnumerable<T> Intersperse<T>(this IEnumerable<T> source, Func<T> element)
        {
            bool first = true;
            foreach (var value in source)
            {
                if (!first)
                    yield return element();
                yield return value;
                first = false;
            }
        }

        /// <summary>
        /// Return true if the code is generated.
        /// </summary>
        /// <param name="tree">the syntax tree root.</param>
        /// <param name="cancellationToken">the cancellation token.</param>
        /// <returns>true if the code has been generated.</returns>
        internal static bool IsGeneratedCode(this SyntaxTree tree, CancellationToken cancellationToken)
        {
            var root = tree.GetRoot(cancellationToken);
            var firstToken = root.GetFirstToken();
            SyntaxTriviaList trivia;
            if (firstToken == default(SyntaxToken))
            {
                var token = ((CompilationUnitSyntax)root).EndOfFileToken;
                if (!token.HasLeadingTrivia)
                    return false;
                trivia = token.LeadingTrivia;
            }
            else
            {
                if (!firstToken.HasLeadingTrivia)
                    return false;
                trivia = firstToken.LeadingTrivia;
            }

            var comments =
                trivia.Where(
                    t => t.IsKind(SyntaxKind.SingleLineCommentTrivia) || t.IsKind(SyntaxKind.MultiLineCommentTrivia));
            return comments.Any(t =>
            {
                var commentText = Convert.ToString(t);
                return commentText.Contains("<auto-generated");
            });
        }

        /// <summary>
        /// Gets the effective <see cref="DocumentationMode"/> used when parsing the <see cref="SyntaxTree"/> containing
        /// the specified context.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        /// <returns>
        /// <para>The <see cref="DocumentationMode"/> of the <see cref="SyntaxTree"/> containing the context.</para>
        /// <para>-or-</para>
        /// <para><see cref="DocumentationMode.Diagnose"/>, if the documentation mode could not be determined.</para>
        /// </returns>
        internal static DocumentationMode GetSyntaxDocumentationMode(this SyntaxNodeAnalysisContext context)
        {
            return context.Node.SyntaxTree?.Options.DocumentationMode ?? DocumentationMode.Diagnose;
        }

        /// <summary>
        /// Return true if the documentation mode is on.
        /// </summary>
        /// <param name="context">the context.</param>
        /// <returns>true if the documentation mode is on.</returns>
        internal static bool IsDocumentationModeOn(this SyntaxNodeAnalysisContext context)
        {
            return context.Node.SyntaxTree?.Options.DocumentationMode != DocumentationMode.None;
        }
    }
}