namespace FSharp.FGL.Directed

open FSharp.FGL

module Edges =
    let undirect2 g =
        Graph.empty 
        |> Vertices.addMany (Vertices.toVertexList g)
        |> Undirected.Edges.addMany (Edges.toEdgeList g)
