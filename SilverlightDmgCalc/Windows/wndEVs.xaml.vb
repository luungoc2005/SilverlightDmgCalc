Partial Public Class wndEVs
    Inherits ChildWindow

    Dim pkData As Pokemon

    Public Property PokeData As Pokemon
        Get
            Return pkData
        End Get
        Set(ByVal value As Pokemon)
            pkData = value
            With value
                Slider1.Value = .EVs.HP
                Slider2.Value = .EVs.Atk
                Slider3.Value = .EVs.Def
                Slider4.Value = .EVs.SpAtk
                Slider5.Value = .EVs.SpDef
                Slider6.Value = .EVs.Speed
            End With
        End Set
    End Property

    Public Sub New()
        InitializeComponent()
        AddHandler Slider1.ValueChanged, AddressOf OnSliderChange
        AddHandler Slider2.ValueChanged, AddressOf OnSliderChange
        AddHandler Slider3.ValueChanged, AddressOf OnSliderChange
        AddHandler Slider4.ValueChanged, AddressOf OnSliderChange
        AddHandler Slider5.ValueChanged, AddressOf OnSliderChange
        AddHandler Slider6.ValueChanged, AddressOf OnSliderChange
    End Sub

    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles OKButton.Click
        With pkData
            .EVs.HP = Slider1.Value
            .EVs.Atk = Slider2.Value
            .EVs.Def = Slider3.Value
            .EVs.SpAtk = Slider4.Value
            .EVs.SpDef = Slider5.Value
            .EVs.Speed = Slider6.Value
        End With
        Me.DialogResult = True
    End Sub

    Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles CancelButton.Click
        Me.DialogResult = False
    End Sub

    Private Sub OnSliderChange(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double))
        Dim bEVs As Byte = Math.Round(e.NewValue)
        With CType(sender, Slider)
            If .Value <> bEVs Then .Value = bEVs : Exit Sub
            Dim intTotal As Integer = Slider1.Value + Slider2.Value + Slider3.Value + Slider4.Value + Slider5.Value + Slider6.Value
            ProgressBar1.Value = IIf(intTotal > 510, 510, intTotal)
            Label7.Text = ProgressBar1.Value
        End With
    End Sub

    Private Function FixEV(Optional ByVal CurrentValue As Byte = 0) As Byte
        Dim intTotal As Integer = Slider1.Value + Slider2.Value + Slider3.Value + Slider4.Value + Slider5.Value + Slider6.Value
        intTotal = 510 - intTotal + CurrentValue
        Return Math.Floor(IIf(intTotal > 255, 255, IIf(intTotal < 0, 0, intTotal)) / 4) * 4
    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        Slider1.Value = FixEV(Slider1.Value)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        Slider2.Value = FixEV(Slider2.Value)
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button3.Click
        Slider3.Value = FixEV(Slider3.Value)
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button4.Click
        Slider4.Value = FixEV(Slider4.Value)
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button5.Click
        Slider5.Value = FixEV(Slider5.Value)
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button6.Click
        Slider6.Value = FixEV(Slider6.Value)
    End Sub

    Private Sub btnReset_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnReset.Click
        Slider1.Value = 0
        Slider2.Value = 0
        Slider3.Value = 0
        Slider4.Value = 0
        Slider5.Value = 0
        Slider6.Value = 0
    End Sub
End Class
