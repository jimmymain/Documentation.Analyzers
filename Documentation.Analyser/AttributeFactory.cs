// <copyright file="AttributeFactory.cs" company="Palantir (Pty) Ltd">
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
    /// a factory to add new attributes to a class.
    /// </summary>
    public class AttributeFactory : IAttributeFactory
    {
        /// <summary>
        /// create data member attributes for each property in the class.
        /// </summary>
        /// <param name="classDeclaration">the class declaration.</param>
        /// <returns>the updated class declaration.</returns>
        public ClassDeclarationSyntax CreateDataMemberAttributes(ClassDeclarationSyntax classDeclaration)
        {
            PropertyDeclarationSyntax syntax;
            PropertyDeclarationSyntax QueryPropertyDeclaration() =>
                classDeclaration.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .Where(_ => _.Modifiers.Any(o => o.Kind() == SyntaxKind.PublicKeyword))
                    .FirstOrDefault(_ => !_.AttributeLists.Any(o => o.Attributes.Any(n => n.Name.GetIdentifierName() == "DataMember")));

            while ((syntax = QueryPropertyDeclaration()) != null)
                classDeclaration = classDeclaration.ReplaceNode(syntax, this.CreateDataMemberAttribute(syntax));
            return classDeclaration;
        }

        /// <summary>
        /// create a data member attribute, and set the property name.
        /// </summary>
        /// <param name="propertyDeclarationSyntax">the syntax.</param>
        /// <returns>the new class.</returns>
        private PropertyDeclarationSyntax CreateDataMemberAttribute(PropertyDeclarationSyntax propertyDeclarationSyntax)
        {
            var name = propertyDeclarationSyntax.Identifier.Text;
            var lower = char.ToLowerInvariant(name[0]) + name.Substring(1);
            var parameter = SyntaxFactory.ParseAttributeArgumentList($@"(Name = ""{lower}"")");
            var addTrivia = !propertyDeclarationSyntax.AttributeLists.Any();
            var attribute = SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("DataMember"), parameter)));
            if (addTrivia)
                attribute = attribute.WithLeadingTrivia(propertyDeclarationSyntax.GetLeadingTrivia());
            var attributes = propertyDeclarationSyntax.AttributeLists.Add(attribute);
            return propertyDeclarationSyntax
                .WithoutLeadingTrivia()
                .WithAttributeLists(attributes);
        }
    }
}