// <copyright file="CommentNodeAdjustmentExtensions.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// extensions to alter existing comment trivia
    /// </summary>
    public static class CommentNodeAdjustmentExtensions
    {
        /// <summary>
        /// Adjust the leading and trailing trivia associated with <see cref="SyntaxKind.XmlTextLiteralNewLineToken"/>
        /// tokens to ensure the formatter properly indents the exterior trivia.
        /// </summary>
        /// <typeparam name="T">The type of syntax node.</typeparam>
        /// <param name="node">The syntax node to adjust tokens.</param>
        /// <returns>A <see cref="SyntaxNode"/> equivalent to the input <paramref name="node"/>, adjusted by moving any
        /// trailing trivia from <see cref="SyntaxKind.XmlTextLiteralNewLineToken"/> tokens to be leading trivia of the
        /// following token.</returns>
        public static T AdjustDocumentationCommentNewLineTrivia<T>(this T node)
            where T : SyntaxNode
        {
            var tokensForAdjustment =
                from token in node.DescendantTokens()
                where token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken)
                where token.HasTrailingTrivia
                let next = token.GetNextToken(includeZeroWidth: true, includeSkipped: true, includeDirectives: true, includeDocumentationComments: true)
                where !next.IsMissingOrDefault()
                select new KeyValuePair<SyntaxToken, SyntaxToken>(token, next);

            Dictionary<SyntaxToken, SyntaxToken> replacements = new Dictionary<SyntaxToken, SyntaxToken>();
            foreach (var pair in tokensForAdjustment)
            {
                replacements[pair.Key] = pair.Key.WithTrailingTrivia();
                replacements[pair.Value] = pair.Value.WithLeadingTrivia(pair.Value.LeadingTrivia.InsertRange(0, pair.Key.TrailingTrivia));
            }

            return node.ReplaceTokens(replacements.Keys, (originalToken, rewrittenToken) => replacements[originalToken]);
        }

        /// <summary>
        /// add new line trivia from the supplied leading trivia.
        /// </summary>
        /// <param name="commentTrivia">the existing comment trivia.</param>
        /// <param name="leading">the leading trivia from the syntax node.</param>
        /// <returns>the trivia list, with the added new line characters.</returns>
        public static DocumentationCommentTriviaSyntax AddLeadingEndOfLineTriviaFrom(
            this DocumentationCommentTriviaSyntax commentTrivia,
            SyntaxTriviaList leading)
        {
            if (leading.Any(_ => _.Kind() == SyntaxKind.EndOfLineTrivia))
                commentTrivia.InsertTriviaBefore(
                    commentTrivia.GetLeadingTrivia().First(),
                    new[] { SyntaxFactory.EndOfLine("\r\n") });
            return commentTrivia;
        }

        /// <summary>
        /// Removes the leading and trailing trivia associated with a syntax token.
        /// </summary>
        /// <param name="token">The syntax token to remove trivia from.</param>
        /// <returns>A copy of the input syntax token with leading and trailing trivia removed.</returns>
        public static SyntaxToken WithoutTrivia(this SyntaxToken token)
        {
            return token.WithLeadingTrivia(default(SyntaxTriviaList)).WithTrailingTrivia(default(SyntaxTriviaList));
        }

        /// <summary>
        /// calculate whether or not the supplied token is missing.
        /// </summary>
        /// <param name="token">the token.</param>
        /// <returns>true if the token is 'Missing' or 'None'</returns>
        private static bool IsMissingOrDefault(this SyntaxToken token)
        {
            return token.IsKind(SyntaxKind.None)
                || token.IsMissing;
        }
    }
}