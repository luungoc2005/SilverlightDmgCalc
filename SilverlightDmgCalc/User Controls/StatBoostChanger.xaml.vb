Partial Public Class StatBoostChanger
    Inherits UserControl

    Public Property Value As Integer
        Get
            Return Slider1.Value
        End Get
        Set(ByVal value As Integer)
            Slider1.Value = value
        End Set
    End Property

    Public Sub New 
        InitializeComponent()
        AddHandler Slider1.ValueChanged, AddressOf OnSliderChange
    End Sub


    Private Sub OnSliderChange(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double))
        CType(sender, Slider).Value = Math.Round(e.NewValue)
    End Sub

    Private Sub btnPlus_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnPlus.Click
        Slider1.Value = IIf(Slider1.Value >= 6, 6, Slider1.Value + 1)
    End Sub

    Private Sub btnMinus_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnMinus.Click
        Slider1.Value = IIf(Slider1.Value <= -6, -6, Slider1.Value - 1)
    End Sub
End Class
