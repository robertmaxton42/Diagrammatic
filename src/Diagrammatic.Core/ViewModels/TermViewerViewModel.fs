namespace Diagrammatic.Core.ViewModels

open Diagrammatic.Core

open System.Reactive.Linq
open System.ComponentModel

open FSharp.FGL
open ReactiveUI
open Avalonia.Controls.Templates
open Avalonia.Collections

type NodeViewData = { x: float; y: float; z: int }
type Segment = 
  Segment of id: int | Subsegment of group: int * id: int
  
type PortViewData = { segment: Segment; t: float }
type EdgeViewData = { foo: int } //placeholder
type NodeView = Node<PortViewData, EdgeViewData>
//type MContextView = MContext<NodeView, NodeViewData, Edge>

type LabeledNodeWrapper(node: Node<PortViewData, EdgeViewData>, label: NodeViewData ref) =
  let label = label
  let propertyChanged = new Event<_,_>()
  member val Node = node

  interface INotifyPropertyChanged with
    [<CLIEvent>]
    member this.PropertyChanged = propertyChanged.Publish

  member this.Label 
    with get() = label.Value
    and set label' =
      if not (label.Value = label') then
        label.Value <- label'
        propertyChanged.Trigger(this, PropertyChangedEventArgs("Label"))

type TermViewerViewModel() as this = 
    inherit ViewModelBase()

    let basicNode2 = 
      let portview1 = ref { segment = Segment 0; t = 0 }
      let portview2 = ref { segment = Segment 0; t = System.Math.PI }
      NodeTemplate(SimpleNode("Z"), seq { 
        MutableGroup [
          Port({parent = None; group = 0; id = 0}, portview1)
          Port({parent = None; group = 0; id = 1}, portview2)
        ]
      })
      
    let defaultTerm =
      let node1 = basicNode2.coinNode(0)
      let node2 = basicNode2.coinNode(1)
      let ln1 = LabeledNode(node1, ref {x = 50; y = 50; z=1})
      let ln2 = LabeledNode(node2, ref {x = 150; y = 50; z=1})

      Term([ln1; ln2], Graph.empty 
      |> Vertices.addMany (Seq.toList (seq {
            for n in [node1; node2] do
              for plist in n.ports do
                for p in plist -> p
          })))
    
    let mutable term: Term<NodeViewData, PortViewData, EdgeViewData> = defaultTerm
    let nodes = 
      let get_nodes (term: Term<_,_,_>) =
        let nodeseq = seq { for LabeledNode(node, label) in term.Nodes -> new LabeledNodeWrapper(node, label) }
        List.ofSeq nodeseq
      let obs = (this.WhenAnyValue (fun tvvm -> tvvm.Term)).Select(get_nodes)
      obs.ToProperty(this, "Nodes", List.empty, true)
    
    member val Templates = [basicNode2] with get, set
    member this.Term 
      with get() = term 
      and private set term' = 
        this.RaiseAndSetIfChanged (&term, term', "Term") |> ignore
   

    member this.Nodes with get() = nodes.Value

    //member this.Nodes with get() = 
    //  let nodeseq = seq { for LabeledNode(node, label) in term.Nodes -> new LabeledNodeWrapper(node, label) }
    //  AvaloniaList<LabeledNodeWrapper>(nodeseq)
    member this.NodeIDs with get() = seq { for LabeledNode(node, _) in term.Nodes -> node.id }

    member this.AddNode node = this.Term <- this.Term.addNode node
    member this.RemoveNode node = this.Term <- this.Term.removeNode node
    member this.GetUnusedID() = (Seq.max this.NodeIDs) + 1



    //new (saved graph) 
    
    //member addNode(Node)
    //member addEdge(Node, Node, Edge)
