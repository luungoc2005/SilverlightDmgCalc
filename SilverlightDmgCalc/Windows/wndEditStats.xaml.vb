Partial Public Class wndEditStats
    Inherits ChildWindow
    Dim iTotal As Integer
    Dim iData As Pokemon
    Dim bUpdating As Boolean

    Public Sub PopupForm(ByVal PokeData As Pokemon)
        With PokeData
            bUpdating = True
            iData = PokeData

            TextBox1.Text = .Data.BaseStats.HP
            TextBox2.Text = .Data.BaseStats.Atk
            TextBox3.Text = .Data.BaseStats.Def
            TextBox4.Text = .Data.BaseStats.SpAtk
            TextBox5.Text = .Data.BaseStats.SpDef
            TextBox6.Text = .Data.BaseStats.Speed

            Slider1.Value = .Boosts.HP
            Slider2.Value = .Boosts.Atk
            Slider3.Value = .Boosts.Def
            Slider4.Value = .Boosts.SpAtk
            Slider5.Value = .Boosts.SpDef
            Slider6.Value = .Boosts.Speed
            bUpdating = False
            DisplayStats()
        End With
    End Sub

    Private Sub DisplayStats()
        If bUpdating Then Exit Sub
        With iData
            .Boosts.HP = Slider1.Value
            .Boosts.Atk = Slider2.Value
            .Boosts.Def = Slider3.Value
            .Boosts.SpAtk = Slider4.Value
            .Boosts.SpDef = Slider5.Value
            .Boosts.Speed = Slider6.Value

            .Data.BaseStats.HP = TextBox1.Text
            .Data.BaseStats.Atk = TextBox2.Text
            .Data.BaseStats.Def = TextBox3.Text
            .Data.BaseStats.SpAtk = TextBox4.Text
            .Data.BaseStats.SpDef = TextBox5.Text
            .Data.BaseStats.Speed = TextBox6.Text
        End With
        Dim objStats As StatsData = CalcPokeStats(iData, iData.Ability = 109)
        Label16.Text = objStats.HP
        Label17.Text = objStats.Atk
        Label18.Text = objStats.Def
        Label19.Text = objStats.SpAtk
        Label20.Text = objStats.SpDef
        Label21.Text = objStats.Speed
    End Sub

    Private Sub Slider1_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles Slider1.ValueChanged
        If Slider1.Value <> Math.Round(Slider1.Value) Then Slider1.Value = Math.Round(Slider1.Value) : Exit Sub
        DisplayStats()
    End Sub

    Private Sub Slider2_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles Slider2.ValueChanged
        If Slider2.Value <> Math.Round(Slider2.Value) Then Slider2.Value = Math.Round(Slider2.Value) : Exit Sub
        DisplayStats()
    End Sub

    Private Sub Slider3_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles Slider3.ValueChanged
        If Slider3.Value <> Math.Round(Slider3.Value) Then Slider3.Value = Math.Round(Slider3.Value) : Exit Sub
        DisplayStats()
    End Sub

    Private Sub Slider4_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles Slider4.ValueChanged
        If Slider4.Value <> Math.Round(Slider4.Value) Then Slider4.Value = Math.Round(Slider4.Value) : Exit Sub
        DisplayStats()
    End Sub

    Private Sub Slider5_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles Slider5.ValueChanged
        If Slider5.Value <> Math.Round(Slider5.Value) Then Slider5.Value = Math.Round(Slider5.Value) : Exit Sub
        DisplayStats()
    End Sub

    Private Sub Slider6_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles Slider6.ValueChanged
        If Slider6.Value <> Math.Round(Slider6.Value) Then Slider6.Value = Math.Round(Slider6.Value) : Exit Sub
        DisplayStats()
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox1.TextChanged
        DisplayStats()
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox2.TextChanged
        DisplayStats()
    End Sub

    Private Sub TextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox3.TextChanged
        DisplayStats()
    End Sub

    Private Sub TextBox4_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox4.TextChanged
        DisplayStats()
    End Sub

    Private Sub TextBox5_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox5.TextChanged
        DisplayStats()
    End Sub

    Private Sub TextBox6_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox6.TextChanged
        DisplayStats()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        bUpdating = True
        Slider1.Value = 0
        Slider2.Value = 0
        Slider3.Value = 0
        Slider4.Value = 0
        Slider5.Value = 0
        Slider6.Value = 0
        bUpdating = False
        DisplayStats()
    End Sub

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles OKButton.Click
        Me.DialogResult = True
    End Sub

    Private Sub wndEditStats_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Closed
        Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, True)
    End Sub
End Class
