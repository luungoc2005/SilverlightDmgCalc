Partial Public Class wndBoosts
    Inherits ChildWindow
    Dim pkPoke As Pokemon

    Public Property PokeData As Pokemon
        Get
            Return pkPoke
        End Get
        Set(ByVal value As Pokemon)
            pkPoke = value
        End Set
    End Property

    Public Sub LoadData()
        With pkPoke.Boosts
            StatBoostChanger2.Value = .Atk
            StatBoostChanger3.Value = .Def
            StatBoostChanger4.Value = .SpAtk
            StatBoostChanger5.Value = .SpDef
            StatBoostChanger6.Value = .Speed
        End With
    End Sub

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles OKButton.Click
        With pkPoke.Boosts
            .Atk = StatBoostChanger2.Value
            .Def = StatBoostChanger3.Value
            .SpAtk = StatBoostChanger4.Value
            .SpDef = StatBoostChanger5.Value
            .Speed = StatBoostChanger6.Value
        End With
        Me.DialogResult = True
    End Sub

    Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles CancelButton.Click
        Me.DialogResult = False
    End Sub

End Class
