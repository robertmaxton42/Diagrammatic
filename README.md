# Diagrammatic

> a generalized diagrammatic CAS and proof assistant 

[![top language](https://img.shields.io/github/languages/top/robertmaxton42/Diagrammatic?style=flat-square)](https://github.com/robertmaxton42/Diagrammatic/search?l=f%23)

## What is Diagrammatic?

Diagrammatic is a tool designed for the verification of *diagrammatic calculi* (also known simply as graphical notation): the systematic use of node-based diagrams to perform rigorous calculations in a graphical, two-dimensional way.

Graphical notations of this type share a number of common axioms:

* They take the form of a graph (in the vertices-and-edges sense), where nodes represent objects (vectors, tensors, knots...) and edges relate and combine those objects (usually some sort of inner product or composable map).
* They are composable -- connecting a free edge to the free edge of another diagram ("in series") produces another valid diagram, and simply writing two diagrams next to each other ("in parallel") also produces a valid diagram.
* "Only topology matters" -- graphs can be reorganized as if they were a projection of a real, three-dimensional network of objects connected by strings. "Twisting" two strings around each other sometimes matters, but simply repositioning objects never does.
* As a result of the above, they satisfy the properties of a category -- specifically, a [ribbon category](https://en.wikipedia.org/wiki/Ribbon_category).

Specific examples of such notations include:

* [Trace diagrams](https://en.wikipedia.org/wiki/Trace_diagram), useful in vector calculus and linear algebra
* The [Penrose graphical notation](https://en.wikipedia.org/wiki/Penrose_graphical_notation), widely used in general relativity and implicitly in the design of [quantum circuits](https://en.wikipedia.org/wiki/Quantum_circuit)
* The famous [Feynman diagrams](https://en.wikipedia.org/wiki/Feynman_diagram) used extensively in quantum field theory
* The [ZX calculus](https://en.wikipedia.org/wiki/ZX-calculus), another method of simplifying and interpreting quantum circuits

(No attempt is made to handle notations with higher-dimensional components such as [string diagrams](https://en.wikipedia.org/wiki/String_diagram) as applied to 2-categories. Perhaps in a later version.)

Diagrammatic is heavily inspired by [Quantomatic](https://quantomatic.github.io/), a similar tool designed specifically for the ZX calculus. However, unlike Quantomatic, Diagrammatic is:

* **More general**: Designed from the ground up to handle anything a ribbon category can handle, plus additional operators like sums and the Penrose derivative.
* **Not written in Java**: Diagrammatic is written in F# and Avalonia for cross-platform performance not subject to the limitations of the JVM.
* **Focused on proof verification, not generation**: Diagrammatic's goal is to be a method for putting existing diagrams and proofs into a digital format, validating them in the process. While it may acquire automatic simplification/canonicalization abilities later, this is not its main purpose. This separation allows users who just want to be able to check and share their work to do so without having to spend clock cycles on computationally expensive graph recognition algorithms.

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
