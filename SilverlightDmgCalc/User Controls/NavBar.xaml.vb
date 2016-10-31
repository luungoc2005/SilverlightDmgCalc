Partial Public Class NavBar
    Inherits UserControl

    Public Event btnAddClicked()
    Public Event btnRemClicked()
    Public Event btnSwitchClicked()
    Public Event btnCalcClicked()

    Public Sub New 
        InitializeComponent()
    End Sub

    Private Sub btnAbout_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnAbout.Click
        Dim wndAbout As New AboutWindow
        wndAbout.Show()
    End Sub

    Private Sub btnMovesets_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnMovesets.Click
        Dim wndMovesets As New wndMovesets
        wndMovesets.Show()
    End Sub

    Private Sub btnAdd_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnAdd.Click
        RaiseEvent btnAddClicked()
    End Sub

    Private Sub btnRem_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnRem.Click
        RaiseEvent btnRemClicked()
    End Sub

    Private Sub btnSwitch_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnSwitch.Click
        RaiseEvent btnSwitchClicked()
    End Sub

    Private Sub btnCalc_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnCalc.Click
        RaiseEvent btnCalcClicked()
    End Sub
End Class
