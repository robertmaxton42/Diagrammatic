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
