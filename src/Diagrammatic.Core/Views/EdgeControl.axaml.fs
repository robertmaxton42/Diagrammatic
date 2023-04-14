namespace Diagrammatic.Core.Views

open Avalonia
open Avalonia.Collections
open Avalonia.Controls
open Avalonia.Controls.Shapes
open Avalonia.Markup.Xaml
open Avalonia.Media
open AvaloniaExtensions

open Diagrammatic.Core
open Diagrammatic.Core.ViewModels

type EdgeControl() as this =
  inherit UserControl()

  // let mutable path = PathGeometry()
  // static let mutable renderedPathProperty =
  //   AvaloniaProperty.Register<EdgeControl, PathGeometry> "RenderedPath"

  // let onDataContextChanged () =
  //     if this.DataContext <> null then
  //         let edge = this.DataContext :?> LabeledEdgeWrapper
  //         path <- edge.Label.path
  //         this.RenderEdgePath()

  do
    AvaloniaXamlLoader.Load(this)

  // member this.Edge with get() = (this.DataContext :?> LabeledEdgeWrapper)
  // member this.StartPoint with get() = this.Edge.StartPoint
  // member this.EndPoint with get() = this.Edge.EndPoint

  // member private this.RenderEdgePath() =
    