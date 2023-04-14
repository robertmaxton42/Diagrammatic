namespace Diagrammatic.Core.Views

open Avalonia
open Avalonia.Controls
open Avalonia.Input
open Avalonia.Markup.Xaml

open Diagrammatic.Core
open Diagrammatic.Core.ViewModels

open FSharp.FGL.Directed

type TermViewer () as this = 
    inherit Canvas ()

    do //this.InitializeComponent()
        AvaloniaXamlLoader.Load(this)
        this.DataContext <- TermViewerViewModel()

    //member private this.InitializeComponent() =
    //    AvaloniaXamlLoader.Load(this)
    //    this.DataContext <- TermViewerViewModel()

    //member val Graph 
    //    = (this.DataContext :?> TermViewerViewModel).graph
    //    with get

    member this.ViewModel = (this.DataContext :?> TermViewerViewModel)
    member val Dragging: bool = false with get, set
    member val DraggedNode: NodeControl option = None with get, set

    //member private this.AddNodeControl (node: LabeledNode<_,_,_>) =
    //    this.ViewModel.AddNode node
    //    

    member private this.OnNodePointerPressed(sender: obj, e: PointerPressedEventArgs) =
        let nodeControl = sender :?> NodeControl
        this.Dragging <- true
        this.DraggedNode <- Some(nodeControl) //.DataContext :?> LabeledNodeWrapper)
        e.Handled <- true

    member this.OnNodePointerMoved(sender: obj, e: PointerEventArgs) =
        if this.Dragging then
            match this.DraggedNode with
            | Some nodeControl ->
                let position = e.GetPosition(this)
                let node = (nodeControl.DataContext :?> LabeledNodeWrapper)
                node.Label <- { node.Label with x = position.X; y = position.Y }
                nodeControl.InvalidateVisual()
                e.Handled <- true
            | None -> ()

    member this.OnNodePointerReleased(sender: obj, e: PointerReleasedEventArgs) =
        if this.Dragging then
            this.Dragging <- false
            this.DraggedNode <- None

            e.Handled <- true

    member this.OnCanvasPointerPressed(sender: obj, e: PointerPressedEventArgs) =
        if not this.Dragging then
            let position = e.GetPosition(this)
            this.ViewModel.AddNode (LabeledNode(
                (this.ViewModel.Templates.Item 0).coinNode (this.ViewModel.GetUnusedID()),
                ref {x = position.X; y = position.Y; z = 0; is_selected = false}
            ))
            this.InvalidateVisual()
            e.Handled <- true

