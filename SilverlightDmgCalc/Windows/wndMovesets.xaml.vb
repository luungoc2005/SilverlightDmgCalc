Partial Public Class wndMovesets
    Inherits ChildWindow

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles OKButton.Click
        Me.DialogResult = True
    End Sub

    Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.DialogResult = False
    End Sub

    Private Sub cmbPokeName_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbPokeName.SelectionChanged
        If cmbPokeName.SelectedItem Is Nothing Then lstMovesets.ClearValue(ItemsControl.ItemsSourceProperty) : Exit Sub
        lstMovesets.ItemsSource = MovesetsManager.LoadMovesets(cmbPokeName.Text)
    End Sub

    Private Sub wndMovesets_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        cmbPokeName.ValueMemberPath = "Name"
        cmbPokeName.ItemsSource = _Xpokedata.Values
        lstMovesets.DisplayMemberPath = "MovesetName"
    End Sub

    Private Sub lstMovesets_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstMovesets.SelectionChanged
        If Not lstMovesets.SelectedItem Is Nothing Then
            txtExport.Text = CType(lstMovesets.SelectedItem, MovesetData).ToText
        End If
    End Sub

    Private Sub btnRemove_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnRemove.Click
        If Not lstMovesets.SelectedItem Is Nothing Then
            Dim strPokeName As String = CType(cmbPokeName.SelectedItem, PokeData).Name
            Dim lstTemp As List(Of MovesetData) = MovesetsManager.LoadMovesets(strPokeName)
            lstTemp.Remove(lstMovesets.SelectedItem)
            MovesetsManager.SaveMovesets(strPokeName, lstTemp)
            MovesetsManager.SaveChanges()
            lstMovesets.ItemsSource = MovesetsManager.LoadMovesets(strPokeName)
        End If
    End Sub

    Private Sub btnImport_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnImport.Click
        Try
            Dim objImport As New wndImport
            AddHandler objImport.Closed, Sub(s, a)
                                             If objImport.DialogResult Then
                                                 If objImport.txtMain.Text = vbNullString Then Exit Sub
                                                 Dim strMovesets() As String = Split(objImport.txtMain.Text, vbCr & vbCr)
                                                 For Each strTemp As String In strMovesets
                                                     Dim objMoveset As New MovesetData
                                                     objMoveset.FromText(strTemp, True)
                                                     Dim lstTemp As List(Of MovesetData) = MovesetsManager.LoadMovesets(objMoveset.PokeName)
                                                     lstTemp.Add(objMoveset)
                                                     MovesetsManager.SaveMovesets(objMoveset.PokeName, lstTemp)
                                                 Next
                                                 MovesetsManager.SaveChanges()
                                             End If
                                             Dim objMovesets As New wndMovesets
                                             objMovesets.Show()
                                         End Sub
            Me.Close()
            objImport.Show()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnRemoveAll_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnRemoveAll.Click
        MovesetsManager.ClearAll()
        lstMovesets.ClearValue(ItemsControl.ItemsSourceProperty)
        txtExport.Text = ""
    End Sub
End Class
