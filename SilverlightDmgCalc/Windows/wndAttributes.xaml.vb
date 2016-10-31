Partial Public Class wndAttributes
    Inherits ChildWindow

    Dim pkPoke As Pokemon

    Public Sub PopupForm(PokeData As Pokemon)
        On Error Resume Next
        pkPoke = PokeData
        With pkPoke
            Label1.Text = .Level
            Select Case .Gender
                Case _Gender.Male
                    optMale.IsChecked = True
                Case _Gender.Female
                    optFemale.IsChecked = True
                Case Else
                    optGenderless.IsChecked = True
            End Select
            cmbStatus.SelectedIndex = .Status
            txtWeight.Text = .Data.Weight
            Label2.Text = .Happiness
            Label3.Text = .HPPercent
            cmbType1.SelectedIndex = .Data.Type1
            cmbType2.SelectedIndex = .Data.Type2
        End With
    End Sub

    Public Sub SaveData()
        With pkPoke
            .Level = sldLevel.Value
            .Gender = IIf(optMale.IsChecked, _Gender.Male, IIf(optFemale.IsChecked, _Gender.Female, _Gender.Genderless))
            .Status = cmbStatus.SelectedIndex
            .Data.Weight = Val(txtWeight.Text)
            .Happiness = sldHappiness.Value
            .HPPercent = sldHP.Value
            .Data.Type1 = IIf(cmbType1.SelectedIndex = -1, 0, cmbType1.SelectedIndex)
            .Data.Type2 = IIf(cmbType2.SelectedIndex = -1, 0, cmbType2.SelectedIndex)
        End With
    End Sub

    Public Sub New()
        InitializeComponent()
        cmbType1.ItemsSource = arrTypes
        cmbType2.ItemsSource = arrTypes
    End Sub

    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles OKButton.Click
        SaveData()
        Me.DialogResult = True
    End Sub

    Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles CancelButton.Click
        Me.DialogResult = False
    End Sub

    Private Sub sldLevel_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles sldLevel.ValueChanged
        sldLevel.Value = Math.Round(sldLevel.Value)
    End Sub

    Private Sub sldHappiness_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles sldHappiness.ValueChanged
        sldHappiness.Value = Math.Round(sldHappiness.Value)
    End Sub

    Private Sub sldHP_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles sldHP.ValueChanged
        sldHP.Value = Math.Round(sldHP.Value)
    End Sub

    Private Sub wndAttributes_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Closed
        Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, True)
    End Sub
End Class
