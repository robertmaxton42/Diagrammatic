namespace Diagrammatic.Core.Views

open Avalonia
open Avalonia.Data.Converters

type PositionConverter() =
    interface IValueConverter with
        member this.Convert(value, targetType, parameter, culture) =
            let center = unbox<float> value
            let factor = unbox<string> parameter
            let size = center * (float)factor
            box(size)

        member this.ConvertBack(value, targetType, parameter, culture) =
            let center = unbox<float> value
            let factor = unbox<string> parameter
            let size = center / (float)factor
            box(size)

    static member Instance = PositionConverter()
