namespace Diagrammatic.Core.ViewModels

open Avalonia
open Avalonia.Media

open MathNet.Numerics


module PathLength =

    let getLineLength (line: LineSegment) (length: float) (lastPoint: Point) =
        let dx = line.Point.X - lastPoint.X
        let dy = line.Point.Y - lastPoint.Y
        length + sqrt (dx * dx + dy * dy), line.Point

    let ellipticIntegralInc2 z m =
        SpecialFunctions.GeneralizedHypergeometric 

    let getArcLength (arc: ArcSegment) (length: float) (lastPoint: Point) =
        let sdx = (arc.Point.X - lastPoint.X) / arc.Size.Width
        let sdy = (arc.Point.Y - lastPoint.Y) / arc.Size.Height
        let chord = sqrt (sdx * sdx + sdy + sdy)
        let arc = 2.0 * asin (chord / 2.0)

        length + sqrt (dx * dx + dy * dy), arc.Point

    let getFigureLength (figure: PathFigure) =
        Seq.fold (fun (length, lastPoint) (segment: PathSegment) ->
                    match segment with
                    | :? LineSegment as line -> getLineLength line length lastPoint
                    | :? ArcSegment as arc -> getArcLength arc length lastPoint

            ) (0, figure.StartPoint) figure.Segments |> fst

    let getTotalLength (pathGeometry: PathGeometry) =
        let mutable totalLength = 0.0
        for figure in pathGeometry.Figures do
            let mutable lastPoint = figure.StartPoint
            for segment in figure.Segments do
                match segment with
                | :? LineSegment as lineSegment ->
                    

                    
                // Handle other types of segments here, e.g., BezierSegment, QuadraticBezierSegment, etc.
                | _ -> ()
        totalLength

    let getPointAtPosition (pathGeometry: PathGeometry) (normalizedPosition: float) =
        let targetLength = getTotalLength pathGeometry * normalizedPosition
        let mutable currentLength = 0.0
        let mutable foundPoint = Avalonia.Point(0.0, 0.0)

        for figure in pathGeometry.Figures do
            let mutable lastPoint = figure.StartPoint
            for segment in figure.Segments do
                match segment with
                | :? LineSegment as lineSegment ->
                    let dx = lineSegment.Point.X - lastPoint.X
                    let dy = lineSegment.Point.Y - lastPoint.Y
                    let segmentLength = Math.Sqrt(dx * dx + dy * dy)

                    if currentLength + segmentLength >= targetLength then
                        let t = (targetLength - currentLength) / segmentLength
                        foundPoint <- Avalonia.Point(lastPoint.X + t * dx, lastPoint.Y + t * dy)
                        break

                    currentLength <- currentLength + segmentLength
                    lastPoint <- lineSegment.Point
                // Handle other types of segments here, e.g., BezierSegment, QuadraticBezierSegment, etc.
                | _ -> ()

            if currentLength >= targetLength then
                break

        foundPoint
