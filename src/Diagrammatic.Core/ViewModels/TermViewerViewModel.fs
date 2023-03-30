namespace Diagrammatic.Core.ViewModels

open FSharp.FGL
open Diagrammatic.Core
open Avalonia.Controls.Templates

type NodeViewData = { x: float; y: float; z: int }
type PortViewData = { θ: float }
type EdgeViewData = int //placeholder
type NodeView = Node<PortViewData, EdgeViewData>
//type MContextView = MContext<NodeView, NodeViewData, Edge>

type TermViewerViewModel() = 
    inherit ViewModelBase()

    let basicNode2 id kind =
      let portview1 = { θ = 0 }
      let portview2 = { θ = 180 }
      let rec out = Node(id, SimpleNode(kind), 
                      seq { seq {
                          Port({parent = out; group = 0; id = 0}, portview1); 
                          Port({parent = out; group = 0; id = 1}, portview2)
                      }}
                    )
      out
        

    let defaultTerm = //:Graph<Node<NodeViewData>, NodeViewData, Edge> = 
      let node1 = basicNode2 0 ""
      let node2 = basicNode2 0 ""      
      let ln1 = LabeledNode(node1, {x = 50; y = 50; z=1})
      let ln2 = LabeledNode(node2, {x = 150; y = 50; z=0})

      ([ln1; ln2], Graph.empty 
      |> Vertices.addMany (Seq.toList (seq {
            for n in [node1; node2] do
              for plist in n.ports do
                for p in plist -> p
          })))
    
    member val graph: Term<NodeViewData, PortViewData, EdgeViewData> = defaultTerm with get

    //new (saved graph) 
    
    //member addNode(Node)
    //member addEdge(Node, Node, Edge)
