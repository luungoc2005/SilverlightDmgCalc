Imports System.Collections.ObjectModel

Partial Public Class wndEditMove
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
        With pkPoke.CurMove
            If .Power > 0 Then cmbName.SelectedItem = pkPoke.CurMove
            txtBP.Text = .Power
            cmbDmgType.SelectedIndex = CType(.DmgType, _DmgType)
            cmbType.SelectedIndex = .Type
        End With
    End Sub

    Public Sub SaveData()
        With pkPoke.CurMove
            If cmbName.Text <> vbNullString Then .Name = CType(cmbName.SelectedItem, Move).Name
            .Power = Integer.Parse(txtBP.Text)
            .Type = Byte.Parse(cmbType.SelectedIndex)
            .DmgType = CType(cmbDmgType.SelectedIndex, _DmgType)
        End With
    End Sub

    Public Sub New()
        InitializeComponent()

        cmbName.ItemsSource = AtkMovesSource
        cmbName.ValueMemberPath = "Name"
        cmbType.ItemsSource = arrTypes
    End Sub

    Public ReadOnly Property AtkMovesSource As System.Collections.Generic.IEnumerable(Of Move)
        Get
            Return _Xmovedata.Values.Where(Function(a As Move) As Boolean
                                               If a.Power > 0 Then
                                                   Return True
                                               Else
                                                   Return False
                                               End If
                                           End Function)
        End Get
    End Property

    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles OKButton.Click
        SaveData()
        Me.DialogResult = True
    End Sub

    Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles CancelButton.Click
        Me.DialogResult = False
    End Sub

    Private Sub cmbName_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbName.SelectionChanged
        With CType(cmbName.SelectedItem, Move)
            txtBP.Text = .Power
            cmbDmgType.SelectedIndex = .DmgType
            cmbType.SelectedIndex = .Type
        End With
    End Sub

    Private Sub txtBP_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtBP.TextChanged
        If Integer.Parse(txtBP.Text) <> CType(cmbName.SelectedItem, Move).Power Then cmbName.Text = vbNullString
    End Sub

    Private Sub cmbType_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbType.SelectionChanged
        If cmbType.SelectedIndex <> CType(cmbName.SelectedItem, Move).Type Then cmbName.Text = vbNullString
    End Sub

    Private Sub cmbDmgType_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbDmgType.SelectionChanged
        If cmbDmgType.SelectedIndex <> CType(cmbName.SelectedItem, Move).DmgType Then cmbName.Text = vbNullString
    End Sub

    Private Sub wndEditMove_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Closed
        Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, True)
    End Sub
End Class
