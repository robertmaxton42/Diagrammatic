namespace Diagrammatic.Core.Views

open Avalonia.Controls.Shapes
open Avalonia.Markup.Xaml

type CenteredEllipse() as this=
    inherit Ellipse()
    do AvaloniaXamlLoader.Load(this)
