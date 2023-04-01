namespace Diagrammatic.Core.Views

open Avalonia.Controls
open Avalonia.Controls.Shapes
open Avalonia.Markup.Xaml
open Avalonia.Media

open Diagrammatic.Core
open Diagrammatic.Core.ViewModels


type NodeControl() as this =
    inherit UserControl()

    let mutable nodeShape: Path = new Path() //Empty path

    let onDataContextChanged () =
       if this.DataContext <> null then
            this.RenderNodeShape()
            this.RenderPorts()

    do this.InitializeComponent()
    
    member this.Node with get() = this.DataContext :?> LabeledNodeWrapper
    member this.Ports with get(): Port<PortViewData, EdgeViewData> list = ()

    override this.OnApplyTemplate(e) =
        base.OnApplyTemplate(e)
        nodeShape <- this.FindControl<Path>("NodeShape")

        onDataContextChanged()
        this.DataContextChanged.Add(fun args -> onDataContextChanged())


    member private this.RenderNodeShape() = 
        match this.Node.Node.kind with 
        | SimpleNode "Z" -> 
            let geometry = EllipseGeometry(Avalonia.Rect(0.0, 0.0, 20.0, 20.0)) 
            nodeShape.Data <- geometry 
            nodeShape.Fill <- SolidColorBrush(Colors.Green) 
        | _ -> 
            nodeShape.Data <- PathGeometry.Parse("") // Empty path 
            nodeShape.Fill <- SolidColorBrush(Colors.Transparent)

    member private this.RenderPorts() =
        let bands = match this.Node.Node.kind with
            | SimpleNode "Z" -> 
        ()

    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)