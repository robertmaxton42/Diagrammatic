namespace Diagrammatic.Core.Views

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml

open Diagrammatic.Core.ViewModels

type TermViewer () as this = 
    inherit Canvas ()

    do this.InitializeComponent()

    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)
        this.DataContext <- TermViewerViewModel()
