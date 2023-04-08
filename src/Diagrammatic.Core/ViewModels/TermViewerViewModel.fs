namespace Diagrammatic.Core.ViewModels

open FSharp.FGL
open Diagrammatic.Core
open Avalonia.Controls.Templates
open Avalonia.Collections

type NodeViewData = { x: float; y: float; z: int }
type Segment = 
  Segment of id: int | Subsegment of group: int * id: int
  
type PortViewData = { segment: Segment; t: float }
type EdgeViewData = { foo: int } //placeholder
type NodeView = Node<PortViewData, EdgeViewData>
//type MContextView = MContext<NodeView, NodeViewData, Edge>

type LabeledNodeWrapper(node: Node<PortViewData, EdgeViewData>, label: NodeViewData) =
  member val Node = node
  member val Label = label with get, set

type TermViewerViewModel() = 
    inherit ViewModelBase()

    let basicNode2 id kind =
      let portview1 = { segment = Segment 0; t = 0 }
      let portview2 = { segment = Segment 0; t = System.Math.PI }
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

      Term([ln1; ln2], Graph.empty 
      |> Vertices.addMany (Seq.toList (seq {
            for n in [node1; node2] do
              for plist in n.ports do
                for p in plist -> p
          })))
    
    let mutable term: Term<NodeViewData, PortViewData, EdgeViewData> = defaultTerm
    member this.Term with get() = term and private set g = term <- g
    member this.Nodes with get() = 
      let nodeseq = seq {for LabeledNode(node, label) in term.Nodes -> new LabeledNodeWrapper(node, label)}
      AvaloniaList<LabeledNodeWrapper>(nodeseq)

    member this.AddNode node = term <- term.addNode node
    member this.RemoveNode node = term <- term.removeNode node



    //new (saved graph) 
    
    //member addNode(Node)
    //member addEdge(Node, Node, Edge)
