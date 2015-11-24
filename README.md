# Documentation Quick Fixes (Roslyn .NET)

This repository contains an implementation of the SA1600 quick fixes for property and method documentation
For a full implementation of all the stylecop rules, please see [Stylecop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)

Stylecop.Analyzers do not contain any form of automatic documentation generation.
This project aims to fill that gap by providing minimal documentation generation for properties / methods and method parameters to save time.

## Using Documentation.Analyzers

The preferable way to use the analyzers is to add the nuget package [Documentation.Analyzers](http://www.nuget.org/packages/Documentation.Analyzers/)
to the project where you want to provide SA1600 quick fixes.

The severity of individual rules may be configured using [rule set files](https://msdn.microsoft.com/en-us/library/dd264996.aspx)

## Installation

Documentation.Analyzers can be installed using the NuGet Package Manager in Visual Studio 2015.

## Team Considerations

If you use older versions of Visual Studio in addition to Visual Studio 2015, you may still install these analyzers. They will be automatically disabled when you open the project back up in Visual Studio 2013 or earlier.
