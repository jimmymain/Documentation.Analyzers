# Documentation Quick Fixes (Roslyn .NET)

This repository contains an implementation of the SA1600 quick fixes for property and method documentation
For a full implementation of all the stylecop rules, please see [Stylecop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)

Stylecop.Analyzers do not contain any form of automatic documentation generation.
This project aims to fill that gap by providing minimal documentation generation for properties / methods and method parameters to save time.

## Using Documentation.Analyzers

The preferable way to use the analyzers is to add the nuget package [Documentation.Analyser](http://www.nuget.org/packages/Documentation.Analyser/)
to the project where you want to provide SA1600 quick fixes.

The severity of individual rules may be configured using [rule set files](https://msdn.microsoft.com/en-us/library/dd264996.aspx)

## Installation

Documentation.Analyser can be installed using the NuGet Package Manager in Visual Studio 2015.

## Examples

#### Method Declaration

From this bare declaration.
```csharp
public void BuildVogonConstructorFleets(int fleetCount)
{
}
```

To a well documented method.
```csharp
/// <summary>
/// build the vogon constructor fleets.
/// </summary>
/// <param name="fleetCount">the fleet count.</param>
public void BuildVogonConstructorFleets(int fleetCount)
{
}
```

#### Property Declaration

From this bare property
```csharp
public Fleet[] VogonConstructorFleet { get; set; }
```

To a well documented property.
```csharp
/// <summary>
/// Gets or sets the vogon constructor fleet.
/// </summary>
public Fleet[] VogonConstructorFleet { get; set; }
```

## Team Considerations

If you use older versions of Visual Studio in addition to Visual Studio 2015, you may still install these analyzers. They will be automatically disabled when you open the project back up in Visual Studio 2013 or earlier.

## Should Documentation Be Generated?

Opinions vary, but my feeling is that if the generated documentation does not read correctly and precisely, then you have fallen prey to one of the two 'hard things to get right' in software engineering.
* Naming Things.
* Cache Invalidation.
* Off by One Errors.

<small>(Phil Carlton)</small>

## The Road Ahead

* Constructor Support has been added.
* Property documentation now edits existing documentation to be compliant, rather than replacing the existing documentation.
* Return values are not currently documented.
* Generic type parameters are not documented.
