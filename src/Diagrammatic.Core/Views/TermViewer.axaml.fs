namespace Diagrammatic.Core.Views

open Avalonia
open Avalonia.Controls
open Avalonia.Input
open Avalonia.Markup.Xaml

open Diagrammatic.Core.ViewModels

open FSharp.FGL.Directed

type TermViewer () as this = 
    inherit Canvas ()

    let viewmodel = this.DataContext :?> TermViewerViewModel

    do this.InitializeComponent()
    
    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)
        this.DataContext <- TermViewerViewModel()

    //member val Graph 
    //    = (this.DataContext :?> TermViewerViewModel).graph
    //    with get

    member val Dragging: bool = false with get, set
    member val DraggedNode: LabeledNodeWrapper option = None with get, set

    member private this.OnNodePointerPressed(sender: obj, e: PointerPressedEventArgs) =
        let nodeControl = sender :?> NodeControl
        this.Dragging <- true
        this.DraggedNode <- Some(nodeControl.DataContext :?> LabeledNodeWrapper)
        e.Handled <- true

    member this.OnNodePointerMoved(sender: obj, e: PointerEventArgs) =
        if this.Dragging then
            match this.DraggedNode with
            | Some node ->
                let position = e.GetPosition(this)
                node.Label <- { node.Label with x = position.X; y = position.Y }
                e.Handled <- true
            | None -> ()

    member this.OnNodePointerReleased(sender: obj, e: PointerReleasedEventArgs) =
        if this.Dragging then
            this.Dragging <- false
            this.DraggedNode <- None
            e.Handled <- true

    member this.OnCanvasPointerPressed(sender: obj, e: PointerPressedEventArgs) =
        if not this.Dragging then
            viewmodel.AddNode 
            e.Handled <- true

