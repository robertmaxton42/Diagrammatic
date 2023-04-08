namespace Diagrammatic.Core.ViewModels

open FSharp.FGL
type MainWindowViewModel() =
    inherit ViewModelBase()

    let termviewer = TermViewerViewModel()

    member this.graph with get () = termviewer.Graph
    //member this.nodeLabels with get () = [for (_, l) in Vertices.toVertexList termviewer.graph -> l]
    //member this.edgeLabels with get () = [for (_, _, e) in Directed.Edges.toEdgeList termviewer.graph -> e]