namespace Diagrammatic.Core.ViewModels

open FSharp.FGL
open Diagrammatic.Core
open Avalonia.Controls.Templates
open Avalonia.Collections

type NodeViewData = { x: float; y: float; z: int }
type PortViewData = { θ: float }
type EdgeViewData = { foo: int } //placeholder
type NodeView = Node<PortViewData, EdgeViewData>
//type MContextView = MContext<NodeView, NodeViewData, Edge>

type LabeledNodeWrapper(node: Node<PortViewData, EdgeViewData>, label: NodeViewData) =
  member val Node = node with get
  member val Label = label with get

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
        

    let defaultTerm =
      let node1 = basicNode2 0 "Z"
      let node2 = basicNode2 0 "Z"      
      let ln1 = LabeledNode(node1, {x = 50; y = 50; z=1})
      let ln2 = LabeledNode(node2, {x = 150; y = 50; z=0})

      ([ln1; ln2], Graph.empty 
      |> Vertices.addMany (Seq.toList (seq {
            for n in [node1; node2] do
              for plist in n.ports do
                for p in plist -> p
          })))
    
    member val graph: Term<NodeViewData, PortViewData, EdgeViewData> = defaultTerm with get
    member val nodes = 
      let nodeseq = seq {for LabeledNode(node, label) in fst defaultTerm -> new LabeledNodeWrapper(node, label)}
      AvaloniaList<LabeledNodeWrapper>(nodeseq)



    //new (saved graph) 
    
    //member addNode(Node)
    //member addEdge(Node, Node, Edge)
