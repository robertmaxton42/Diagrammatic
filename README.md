# Diagrammatic

> a generalized diagrammatic CAS and proof assistant 

[![top language](https://img.shields.io/github/languages/top/robertmaxton42/Diagrammatic?style=flat-square)](https://github.com/robertmaxton42/Diagrammatic/search?l=f%23)

## What is Diagrammatic?

Diagrammatic is a tool designed to allow users to specify constraints on node-based diagrams, provide valid transformations for those node-based diagrams, and subsequently validate those constraints and transformations.

<!-- TODO: Specific examples of different variations of diagram-based calculus that the tool supports? -->

## Getting Started

1. After cloning the repository, run `dotnet tool restore` in order to use [Paket](http://fsprojects.github.io/Paket/).
2. Run `dotnet paket install` to obtain the project dependencies.
3. This process is expected to fail when installing `FSharp.FGL` due to .NET version mismatches. To fix this, enter the `./paket-files/github.com/CSBiology/FSharp.FGL` directory and run `dotnet tool restore` to use [Fake](https://fake.build/).
4. Run `dotnet fake build` to build `FSharp.FGL`.
5. Return to the root directory and use `dotnet run --project src/Diagrammatic.Core` to start the application.

## Testing

You can use `dotnet run` or `dotnet watch` from the command line.

> dotnet run --project src/Diagrammatic.Tests \
> dotnet watch --project src/Diagrammatic.Tests run

Diagrammatic uses [Expecto](https://github.com/haf/expecto) for testing.