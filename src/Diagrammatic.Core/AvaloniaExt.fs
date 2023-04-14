module AvaloniaExtensions

open Avalonia
open Avalonia.Media

type Media.EllipseGeometry with
  static member FromRect (rect: Rect) =
    let out = EllipseGeometry(rect)
    out.Center <- Point(rect.Left + rect.Width / 2.0, rect.Top + rect.Height / 2.0)
    out.RadiusX <- rect.Width / 2.0
    out.RadiusY <- rect.Height / 2.0
    
    out

type Media.BezierSegment with
  static member FromPoints p1 p2 p3 =
    let out = BezierSegment()
    out.Point1 <- p1
    out.Point2 <- p2
    out.Point3 <- p3

    out

type Media.PathGeometry with
  static member BezierFromPoints p0 p1 p2 p3 =
    let out = PathGeometry()

    let fig = PathFigure()
    fig.StartPoint <- p0
    fig.Segments <- PathSegments()
    fig.Segments.Add (BezierSegment.FromPoints p1 p2 p3)
    fig.IsClosed <- false

    out.Figures <- PathFigures()
    out.Figures.Add fig
    
    out
