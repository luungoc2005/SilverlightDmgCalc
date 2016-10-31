Partial Public Class wndCoverage
    Inherits ChildWindow
    Dim lstResult As New ObservableCollection(Of String)

    Public Sub New()
        InitializeComponent()

        ComboBox1.ItemsSource = arrTypes
        ComboBox2.ItemsSource = arrTypes
        ComboBox3.ItemsSource = arrTypes
        ComboBox4.ItemsSource = arrTypes

        ComboBox1.SelectedIndex = 0
        ComboBox2.SelectedIndex = 0
        ComboBox3.SelectedIndex = 0
        ComboBox4.SelectedIndex = 0

        ListBox1.ItemsSource = lstResult
    End Sub

    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles OKButton.Click
        Calculate()
    End Sub

    Dim Attacker As New Pokemon
    Dim AtkMove As New Move

    Private Function Calc_TypeMatch(ByVal Type As Byte, ByVal Defender As Pokemon, ByRef AbilityEff As Boolean) As Boolean
        AbilityEff = False
        Defender.AbilityEff = False
        If Type = 0 Or Type = -1 Then Return True

        If CheckBox1.IsChecked Then
            Attacker.Ability = 104
        Else
            Attacker.Ability = 0
        End If
        AtkMove.Type = Type

        If GetTypeMatch(Attacker, AtkMove, Defender, New Field_Effects, True) < 1 Then
            If Defender.AbilityEff Then AbilityEff = True
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub Calculate()
        lstResult.Clear()
        lstResult.HaltUpdate = True

        For Each datTemp As PokeData In _Xpokedata.Values
            Dim objDefender As New Pokemon
            objDefender.Data = datTemp
            Dim Abi1 As Boolean, Abi2 As Boolean, Abi3 As Boolean
            Abi1 = False : Abi2 = False : Abi3 = False

            If objDefender.Data.Ability1 <> 0 Then
                objDefender.Ability = objDefender.Data.Ability1
                Dim Ability1 As Boolean
                If CalcMoves(objDefender, Ability1) Then
                    Abi1 = True
                    'Dim strItem As String
                    'strItem = objDefender.Data.Name & IIf(Ability1, " (" & arrAbilities(objDefender.Ability) & ")", vbNullString)
                    'If Not lstResult.Contains(strItem) Then lstResult.Add(strItem)
                End If
            End If

            If objDefender.Data.Ability2 <> 0 Then
                objDefender.Ability = objDefender.Data.Ability2
                Dim Ability2 As Boolean
                If CalcMoves(objDefender, Ability2) Then
                    Abi2 = True
                    'Dim strItem As String
                    'strItem = objDefender.Data.Name & IIf(Ability2, " (" & arrAbilities(objDefender.Ability) & ")", vbNullString)
                    'If Not lstResult.Contains(strItem) Then lstResult.Add(strItem)
                End If
            End If

            If objDefender.Data.Ability3 <> 0 Then
                objDefender.Ability = objDefender.Data.Ability3
                Dim Ability3 As Boolean
                If CalcMoves(objDefender, Ability3) Then
                    Abi3 = True
                    'Dim strItem As String
                    'strItem = objDefender.Data.Name & IIf(Ability3, " (" & arrAbilities(objDefender.Ability) & ")", vbNullString)
                    'If Not lstResult.Contains(strItem) Then lstResult.Add(strItem)
                End If
            End If

            Dim strItem As String = vbNullString
            If IIf(objDefender.Data.Ability2 <> 0, Abi1 = Abi2, True) And IIf(objDefender.Data.Ability3 <> 0, Abi1 = Abi3, True) And Abi1 = True Then
                strItem = objDefender.Data.Name
                If Not (strItem = vbNullString Or lstResult.Contains(strItem)) Then lstResult.Add(strItem)
            Else
                If Abi1 = True Then
                    strItem = objDefender.Data.Name & IIf(Abi1, " (" & arrAbilities(objDefender.Data.Ability1) & ")", vbNullString)
                    If Not (strItem = vbNullString Or lstResult.Contains(strItem)) Then lstResult.Add(strItem)
                End If
                If Abi2 = True Then
                    strItem = objDefender.Data.Name & IIf(Abi2, " (" & arrAbilities(objDefender.Data.Ability2) & ")", vbNullString)
                    If Not (strItem = vbNullString Or lstResult.Contains(strItem)) Then lstResult.Add(strItem)
                End If
                If Abi3 = True Then
                    strItem = objDefender.Data.Name & IIf(Abi3, " (" & arrAbilities(objDefender.Data.Ability3) & ")", vbNullString)
                    If Not (strItem = vbNullString Or lstResult.Contains(strItem)) Then lstResult.Add(strItem)
                End If
            End If
        Next

        lstResult.HaltUpdate = False
        lstResult.UpdateCollection()
        ListBox1.ItemsSource = lstResult.OrderBy(Function(s As String) As String
                                                     Return s
                                                 End Function)
        ListBox1.UpdateLayout()

        If lstResult.Count > 0 Then ListBox1.ScrollIntoView(ListBox1.Items(0))
    End Sub

    Private Function CalcMoves(ByVal objDefender As Pokemon, ByRef AbilityEff As Boolean) As Boolean
        AbilityEff = False

        Dim IdvAbilityEff As Boolean

        Dim Move1 As Boolean = Calc_TypeMatch(ComboBox1.SelectedIndex, objDefender, IdvAbilityEff)
        If IdvAbilityEff Then AbilityEff = True

        Dim Move2 As Boolean = Calc_TypeMatch(ComboBox2.SelectedIndex, objDefender, IdvAbilityEff)
        If IdvAbilityEff Then AbilityEff = True

        Dim Move3 As Boolean = Calc_TypeMatch(ComboBox3.SelectedIndex, objDefender, IdvAbilityEff)
        If IdvAbilityEff Then AbilityEff = True

        Dim Move4 As Boolean = Calc_TypeMatch(ComboBox4.SelectedIndex, objDefender, IdvAbilityEff)
        If IdvAbilityEff Then AbilityEff = True

        If Move1 And Move2 And Move3 And Move4 Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
