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

type NodeControl() as this =
    inherit UserControl()

    let mutable nodeShape: Path = new Path() //Empty path
    static let mutable renderedPortsProperty =
        AvaloniaProperty.Register<NodeControl, AvaloniaList<Point>> "RenderedPorts"

    let onDataContextChanged () =
       if this.DataContext <> null then
            this.RenderNodeShape()
            this.RenderPorts()

    let deltaVector (p1: Point) (p2: Point) =
        let δ = (p2 - p1)
        Vector(δ.X, δ.Y)

    let rotateVector (v: Vector) θ =
        Vector(
            v.X * cos θ + v.Y * sin θ,
            v.Y * cos θ - v.X * sin θ
        )

    let getPortPoint (portData: PortViewData) =
        let getLinePoint t (s: Point, e: Point) = s * (1.0 - t) + e * t
        let getPolyPoint n t (poly: PolylineGeometry) = getLinePoint t (poly.Points.Item n, poly.Points.Item (n+1))
        let getArcPoint t (start: Point) (arc: ArcSegment) = //(center: Point, (rx, ry), (s: Point, e: Point), φ) = 
            let canonicalize vec =
                let rot = rotateVector vec -arc.RotationAngle
                Vector(rot.X / arc.Size.Width, rot.Y / arc.Size.Height)
            let revert (vec: Vector) =
                let unscale = Vector(vec.X * arc.Size.Width, vec.Y * arc.Size.Height)
                rotateVector unscale arc.RotationAngle
            
            let center = 0.5 * (revert Vector.UnitX + revert -Vector.UnitX)
            let canon_start = canonicalize ((Point.op_Implicit start) - center)

            let chord = canonicalize (deltaVector start arc.Point)
            let modarc = (2.0 * asin (chord.Length / 2.0))
            let arc = if arc.IsLargeArc then System.Math.Tau - modarc else modarc
            let θ = t * arc

            let canon_point = rotateVector canon_start θ
            Vector.op_Explicit (center + revert canon_point)
        let getQuadBezierPoint (p0: Point) (p1: Point) (p2: Point) t =
            p1 + (1.0 - t) * (1.0 - t) * (p0 - p1) + t * t * (p2 - p1)
        let getCubicBezierPoint p0 p1 p2 p3 t =
            (1.0 - t) * getQuadBezierPoint p0 p1 p2 t + t * getQuadBezierPoint p1 p2 p3 t

        let (| SegmentID |) = function
            | Segment id -> id
            | Subsegment (_, id) -> id
        match nodeShape.Data with
        | :? EllipseGeometry as ellipse -> 
            ellipse.Center + Vector(ellipse.RadiusX * (sin portData.t), ellipse.RadiusY * (cos portData.t))
        | :? LineGeometry as line -> getLinePoint portData.t (line.StartPoint, line.EndPoint)
        | :? RectangleGeometry as rect -> 
            let (SegmentID n) = portData.segment
            getPolyPoint n portData.t (
                new PolylineGeometry([
                    rect.Rect.TopLeft
                    rect.Rect.TopRight
                    rect.Rect.BottomRight
                    rect.Rect.BottomLeft
                    ], true))
        | :? PolylineGeometry as poly -> 
            let (SegmentID n) = portData.segment
            getPolyPoint n portData.t poly
        | :? PathGeometry as path -> 
            let (Subsegment (n, k)) = portData.segment
            let fig = path.Figures.Item n
            let start = 
                match fig.Segments.Item (k - 1) with 
                | :? LineSegment as line -> line.Point
                | :? ArcSegment as arc -> arc.Point
                | :? QuadraticBezierSegment as quadbezier -> quadbezier.Point2
                | :? BezierSegment as cubicbezier -> cubicbezier.Point3
                | _ -> fig.StartPoint
            match fig.Segments.Item k with
                | :? LineSegment as line -> getLinePoint portData.t (start, line.Point)
                | :? ArcSegment as arc -> getArcPoint portData.t start arc
                | :? QuadraticBezierSegment as quadbezier -> 
                    getQuadBezierPoint start quadbezier.Point1 quadbezier.Point2 portData.t
                | :? BezierSegment as cubicbezier -> 
                    getCubicBezierPoint start cubicbezier.Point1 cubicbezier.Point2 cubicbezier.Point3 portData.t
                | _ -> fig.StartPoint


    do this.InitializeComponent()
    
    member this.Node with get() = this.DataContext :?> LabeledNodeWrapper
    member this.Ports with get(): LabeledPort<PortViewData, EdgeViewData> seq = 
        seq {
            for group in this.Node.Node.ports do
                for port in group -> port
        }

    member this.RenderedPorts
        with get() = this.GetValue(renderedPortsProperty) 
        and set p = this.SetValue(renderedPortsProperty, p) |> ignore

    override this.OnApplyTemplate(e) =
        base.OnApplyTemplate(e)
        nodeShape <- this.FindControl<Path>("NodeShape")

        onDataContextChanged()
        this.DataContextChanged.Add(fun args -> onDataContextChanged())


    member private this.RenderNodeShape() = 
        match this.Node.Node.kind with 
        | SimpleNode "Z" -> 
            let geometry = EllipseGeometry.FromRect(Rect(0.0, 0.0, 20.0, 20.0)) 
            nodeShape.Data <- geometry
            nodeShape.Height <- geometry.RadiusY * 2.0
            nodeShape.Width <- geometry.RadiusX * 2.0  
            nodeShape.Fill <- SolidColorBrush(Colors.Green) //Later this will be set by the NodeViewData itself (and themeable)
        | _ -> 
            nodeShape.Data <- PathGeometry.Parse("") // Empty path 
            nodeShape.Fill <- SolidColorBrush(Colors.Transparent)

    member private this.RenderPorts() =
        this.RenderedPorts <- AvaloniaList(
            Seq.map 
                (fun (port: LabeledPort<_,_>) -> getPortPoint (snd port).Value) 
                this.Ports
            )

    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)