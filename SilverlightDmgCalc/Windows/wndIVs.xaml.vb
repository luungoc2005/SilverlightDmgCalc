Partial Public Class wndIVs
    Inherits ChildWindow

    Dim pkData As Pokemon

    Public Property PokeData As Pokemon
        Get
            Return pkData
        End Get
        Set(ByVal value As Pokemon)
            pkData = value
            With pkData
                Slider1.Value = .IVs.HP
                Slider2.Value = .IVs.Atk
                Slider3.Value = .IVs.Def
                Slider4.Value = .IVs.SpAtk
                Slider5.Value = .IVs.SpDef
                Slider6.Value = .IVs.Speed
            End With
        End Set
    End Property

    Public Sub New()
        InitializeComponent()
        AddHandler Slider1.ValueChanged, AddressOf ShowHPResult
        AddHandler Slider2.ValueChanged, AddressOf ShowHPResult
        AddHandler Slider3.ValueChanged, AddressOf ShowHPResult
        AddHandler Slider4.ValueChanged, AddressOf ShowHPResult
        AddHandler Slider5.ValueChanged, AddressOf ShowHPResult
        AddHandler Slider6.ValueChanged, AddressOf ShowHPResult
    End Sub

    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles OKButton.Click
        With pkData
            .IVs.HP = Slider1.Value
            .IVs.Atk = Slider2.Value
            .IVs.Def = Slider3.Value
            .IVs.SpAtk = Slider4.Value
            .IVs.SpDef = Slider5.Value
            .IVs.Speed = Slider6.Value
        End With
        Me.DialogResult = True
    End Sub

    Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles CancelButton.Click
        Me.DialogResult = False
    End Sub

    Private Sub ShowHPResult(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double))
        Dim bIV As Byte = Math.Round(e.NewValue)
        With CType(sender, Slider)
            If .Value <> bIV Then .Value = bIV : Exit Sub
        End With
        Dim objMove As Move = CalcHP(Slider1.Value, Slider2.Value, Slider3.Value, Slider6.Value, Slider4.Value, Slider5.Value)
        lblHPower.Text = objMove.Name & ", " & objMove.Power
    End Sub
End Class
