Partial Public Class GUIContainer
    Inherits UserControl

    Public _GUIContained As PokeGUI
    Dim _MainGUI As PokeGUI
    Dim _IsAttacker As Boolean

    Dim Percent1, Percent2 As Byte
    Public UsePercent2 As Boolean

    Public Property IsAttacker As Boolean
        Get
            Return _IsAttacker
        End Get
        Set(ByVal value As Boolean)
            _IsAttacker = value
            _GUIContained.IsAttacker = value
        End Set
    End Property

    Public Sub New(ByVal MainGUI As PokeGUI)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _GUIContained = New PokeGUI
        With _GUIContained
            .VerticalAlignment = Windows.VerticalAlignment.Stretch
            .HorizontalAlignment = Windows.HorizontalAlignment.Stretch
        End With
        gridMain.Children.Add(_GUIContained)

        _MainGUI = MainGUI
    End Sub

    Public Sub PrgTimerTick()
        Percent1 = IIf(Percent1 > 100, 100, Percent1)
        Percent2 = IIf(Percent2 > 100, 100, Percent2)
        prgHP.Value = IIf(UsePercent2, Percent2, Percent1)

        If prgHP.Value >= 50 Then
            prgHP.Foreground = New SolidColorBrush(Colors.Green)
        ElseIf prgHP.Value >= 33 Then
            prgHP.Foreground = New SolidColorBrush(Colors.Orange)
        Else
            prgHP.Foreground = New SolidColorBrush(Colors.Red)
        End If
    End Sub

    Public Property MainGUI As PokeGUI
        Get
            Return _MainGUI
        End Get
        Set(ByVal value As PokeGUI)
            _MainGUI = value
        End Set
    End Property

    Private Function GetAttacker() As Pokemon
        Return IIf(_IsAttacker, _GUIContained.PokeData, _MainGUI.PokeData)
    End Function

    Private Function GetDefender() As Pokemon
        Return IIf(_IsAttacker, _MainGUI.PokeData, _GUIContained.PokeData)
    End Function

    Private Function GetHazardsDmg(ByVal NormalHP As Integer) As Integer
        Dim iHazards As Integer
        Dim pkDefender As Pokemon = GetDefender()
        If pkDefender.Ability = 98 Then Return 0

        With pkDefender.fOptions.Hazards
            If .StealthRock Then
                iHazards += Math.Floor(NormalHP / 100 * 12.5 * _
                                       Eff(pkDefender.Data.Type1, 6) * Eff(pkDefender.Data.Type2, 6))
            End If
            If (Not ((Eff(pkDefender.Data.Type1, 5) * Eff(pkDefender.Data.Type2, 5) = 0) Or _
                pkDefender.Ability = 26 Or pkDefender.Item = "Air Balloon")) _
                Or pkDefender.Item = "Iron Ball" Then

                Select Case .Spikes
                    Case 1
                        iHazards += Math.Floor(NormalHP / 8)
                    Case 2
                        iHazards += Math.Floor(NormalHP / 6)
                    Case 3
                        iHazards += Math.Floor(NormalHP / 4)
                    Case Else
                        iHazards += 0
                End Select
            End If
        End With
        Return iHazards
    End Function

    Private Function GetBoostString(ByVal Boost As Integer) As String
        Return IIf(Boost > 0, "+" & Boost & " ", IIf(Boost < 0, Boost & " ", vbNullString))
    End Function

    Public Sub Button1_Click() Handles Button1.Click
        Try
            _pGetMainPageData()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

        Dim pAttacker As Pokemon = GetAttacker()
        Dim pDefender As Pokemon = GetDefender()

        If pAttacker Is Nothing Or pDefender Is Nothing Then Exit Sub

        Dim iDefHP As Integer = CalcPokeStats(pDefender).HP
        Dim iLeftovers As Integer = IIf(pDefender.Item = "Leftovers" Or _
                                        (pDefender.Item = "Black Sludge" And (pDefender.Data.Type1 = 4 Or pDefender.Data.Type2 = 4)), _
                                        Math.Ceiling(iDefHP / 16), 0)
        Dim iHazards As Integer = GetHazardsDmg(iDefHP)

        If Not CType(TabControl1.SelectedItem, TabItem).Header = "Reverse" Then
            'Main
            Dim colDamage As Collection = New Collection
            Dim colDamage2 As Collection = New Collection

            If pDefender.Ability = 136 And iHazards > 0 Then pDefender.Ability = 0

            colDamage2 = CalcAll_Damage(pAttacker, pDefender, pAttacker.CurMove, _MainGUI.PokeData.fOptions.Weather)
            colDamage = CalcAll_Damage_New(pAttacker, pDefender, pAttacker.CurMove, _MainGUI.PokeData.fOptions.Weather)

            'For i As Byte = 85 To 100
            '    colDamage2.Add(Calc_Damage(pAttacker, pDefender, pAttacker.CurMove, i, _MainGUI.PokeData.fOptions.Weather))
            'Next
            'For i As Single = 15 To 0 Step -1
            '    colDamage.Add(Calc_Damage_New(pAttacker, pDefender, pAttacker.CurMove, i, _MainGUI.PokeData.fOptions.Weather))
            'Next

            'Show results
            lblPokeName.Text = pDefender.Data.Name
            lblHPDamage.Text = colDamage.Item(1) & " - " & colDamage.Item(colDamage.Count) & " HP Damage"
            'Percentage
            Dim iPercent1, iPercent2 As Double
            If iDefHP <> 0 Then
                iPercent1 = Math.Round(colDamage.Item(1) / iDefHP * 100, 2)
                iPercent2 = Math.Round(colDamage.Item(colDamage.Count) / iDefHP * 100, 2)
            End If

            lblPerHPDmg.Text = iPercent1 & "% - " & iPercent2 & "%"

            Percent1 = Math.Min(100 - iPercent1, 100)
            Percent2 = Math.Min(100 - iPercent2, 100)

            'Details
            Dim strDResult As String = "Detailed Result: " & vbCrLf
            Dim strCResult As String = vbNullString

            If pAttacker.CurMove.DmgType <> _DmgType.Status And _
                pAttacker.CurMove.Name <> vbNullString And _
                pAttacker.Data.Name <> vbNullString Then
                Dim strBoosts1 As String, strNature1 As String, strBoosts2 As String, strNature2 As String
                If pAttacker.CurMove.DmgType = _DmgType.Physical Then
                    strBoosts1 = pAttacker.EVs.Atk & " " & GetBoostString(pAttacker.Boosts.Atk) & "Atk "
                    strNature1 = IIf(pAttacker.Nature.Atk = 2, " (+Atk) ", IIf(pAttacker.Nature.Atk = 1, " (-Atk) ", ""))
                Else
                    strBoosts1 = pAttacker.EVs.SpAtk & " " & GetBoostString(pAttacker.Boosts.SpAtk) & "SpAtk "
                    strNature1 = IIf(pAttacker.Nature.SpAtk = 2, " (+SpAtk) ", IIf(pAttacker.Nature.SpAtk = 1, " (-SpAtk) ", ""))
                End If
                If GetDmgType(pAttacker.CurMove, GetFOptions(pAttacker, pDefender)) = _DmgType.Physical Then
                    strBoosts2 = pDefender.EVs.Def & " " & GetBoostString(pDefender.Boosts.Def) & "Def "
                    strNature2 = IIf(pDefender.Nature.Def = 2, " (+Def) ", IIf(pDefender.Nature.Def = 1, " (-Def) ", ""))
                Else
                    strBoosts2 = pDefender.EVs.SpDef & " " & GetBoostString(pDefender.Boosts.SpDef) & " SpDef "
                    strNature2 = IIf(pDefender.Nature.SpDef = 2, " (+SpDef) ", IIf(pDefender.Nature.SpDef = 1, " (-SpDef) ", ""))
                End If
                strCResult += strBoosts1 _
                            & IIf(pAttacker.ItemEff, pAttacker.Item & " ", vbNullString) & _
                            IIf(pAttacker.AbilityEff, arrAbilities(pAttacker.Ability) & " ", vbNullString) & _
                            pAttacker.Data.Name & " " & _
                            strNature1 & _
                            pAttacker.CurMove.Name & " vs " & _
                            pDefender.EVs.HP & " HP/" & _
                            strBoosts2 _
                            & IIf(pDefender.ItemEff, pDefender.Item & " ", vbNullString) & _
                            IIf(pDefender.AbilityEff, arrAbilities(pDefender.Ability) & " ", vbNullString) & _
                            pDefender.Data.Name & _
                            strNature2 & ": " & _
                            iPercent1 & "% - " & iPercent2 & "%"

                strCResult = System.Text.RegularExpressions.Regex.Replace(strCResult, "\s{2,}", " ")

                strDResult += strCResult & vbCrLf

                If iHazards > 0 Then
                    Dim iHazardsDmg1 As Integer = colDamage(1) + iHazards
                    Dim iHazardsDmg2 As Integer = colDamage(colDamage.Count) + iHazards
                    strDResult += "Entry hazards damage: " & iHazards
                    strDResult += vbCrLf & "After entry hazards: " & iHazardsDmg1 & " - " & iHazardsDmg2 _
                        & " (" & Math.Round(iHazardsDmg1 / iDefHP * 100, 2) & "% - " & _
                        Math.Round(iHazardsDmg2 / iDefHP * 100, 2) & "%)" & vbCrLf
                End If
            End If

            'N0 hit KO
            iDefHP -= iHazards
            Dim strHTK As String

            If colDamage(colDamage.Count) >= iDefHP Then
                Dim iPercentKO As Double
                For i = 1 To colDamage.Count
                    If colDamage(i) >= iDefHP Then iPercentKO += 6.25
                Next
                'iPercentKO = IIf(iPercentKO > 99.98, 100, iPercentKO)
                strHTK = IIf(iPercentKO < 100, iPercentKO & "% chance to OHKO", "Guaranteed OHKO")
                txtHitsToKO.Text = "OHKO (" & iPercentKO & "%)"
                strDResult += strHTK
            Else
                Dim iHKOTemp As Integer = iDefHP
                Dim iMinHKO As Integer
                Dim iMaxHKO As Integer

                If iLeftovers < colDamage(1) Then
                    Do Until iHKOTemp <= 0
                        iMinHKO += 1
                        iHKOTemp -= colDamage(1)
                        If iHKOTemp <= 0 Then Exit Do
                        iHKOTemp += IIf(pDefender.Item = "Leftovers", Math.Ceiling(iDefHP / 16), 0)
                    Loop
                End If

                iHKOTemp = iDefHP

                If iLeftovers < colDamage(colDamage.Count) Then
                    Do Until iHKOTemp <= 0
                        iMaxHKO += 1
                        iHKOTemp -= colDamage(colDamage.Count)
                        If iHKOTemp <= 0 Then Exit Do
                        iHKOTemp += IIf(pDefender.Item = "Leftovers", Math.Ceiling(iDefHP / 16), 0)
                    Loop
                End If

                If iMinHKO > 0 And iMaxHKO > 0 Then
                    strHTK = IIf(iMaxHKO <> iMinHKO, iMaxHKO & "-" & iMinHKO, iMaxHKO) & " hits to KO"
                    txtHitsToKO.Text = IIf(iMaxHKO <> iMinHKO, iMaxHKO & "-" & iMinHKO, iMaxHKO) & " hits to KO"
                    strDResult += strHTK

                ElseIf iMaxHKO > 0 Then
                    strHTK = iMaxHKO & " hits to KO at best"
                    txtHitsToKO.Text = iMaxHKO & " hits to KO"
                    strDResult += strHTK
                Else
                    strHTK = "Cannot KO"
                    txtHitsToKO.Text = "Cannot KO"
                    strDResult += strHTK
                End If
                strDResult += IIf(iLeftovers > 0, " (with " & pDefender.Item & ")", vbNullString)
            End If

            'strDResult += strHTK
            strCResult += " (" & strHTK & ")"
            ttCopy.Content = strCResult

            strDResult += vbCrLf & vbCrLf & "Possible HP Damage:" & vbCrLf
            For i = 1 To colDamage.Count
                strDResult += colDamage(i) & IIf(i <> colDamage.Count, ", ", vbNullString)
            Next

            strDResult += vbCrLf & vbCrLf & "Possible HP Damage (Old Formula):" & vbCrLf
            For i = 1 To colDamage2.Count
                strDResult += colDamage2(i) & IIf(i <> colDamage2.Count, ", ", vbNullString)
            Next

            txtCalcDetails.Text = strDResult

        ElseIf pAttacker.CurMove.DmgType <> _DmgType.Status Then

            'Reverse Calc 
            Dim iRnd As Byte = IIf(cmbMinMax.SelectedIndex = 0, 15, 0)
            Dim iRevHP As Integer = Int(IIf(cmbHP.SelectedIndex = 1, iDefHP / 100 * Val(txtHPRev.Text), txtHPRev.Text))
            Dim strFailMsg As String = "Cannot " & IIf(cmbTarget.SelectedIndex = 0, "deal", "take") & " " & iRevHP & " HP Damage"

            lblRevResult.Text = "Result: "
            Dim AtkDmgType As _DmgType = pAttacker.CurMove.DmgType
            Dim DefDmgType As _DmgType = GetDmgType(pAttacker.CurMove, GetFOptions(pAttacker, pDefender))
            Dim bCanKO As Boolean = False

            Select Case cmbCalcTarget.SelectedIndex
                Case 0 'Move Power
                    Dim iBP As Integer = pAttacker.CurMove.Power
                    Dim strMoveName As String = pAttacker.CurMove.Name
                    pAttacker.CurMove.Name = vbNullString

                    'Dim iTest As Integer
                    'For iTest = 0 To 9999
                    '    pAttacker.CurMove.Power = iTest
                    '    If Calc_Damage(pAttacker, pDefender, pAttacker.CurMove, _
                    '                   iRnd, _MainGUI.PokeData.fOptions.Weather) >= iRevHP Then
                    '        bCanKO = True
                    '        Exit For
                    '    End If
                    'Next
                    Dim iResult As Integer
                    iResult = FindVariable(iRevHP, 0, 9999, Function(a As Integer) As Integer
                                                                pAttacker.CurMove.Power = a
                                                                Return Calc_Damage_New(pAttacker, pDefender, pAttacker.CurMove, _
                                                                        iRnd, _MainGUI.PokeData.fOptions.Weather)
                                                            End Function, CompareFunc.MoreOrEquals)
                    If iResult > -1 Then bCanKO = True
                    lblRevResult.Text += IIf(Not bCanKO, strFailMsg, iResult & " Base Power")

                    pAttacker.CurMove.Power = iBP
                    pAttacker.CurMove.Name = strMoveName
                Case 1 'EVs
                    Dim iEV As Byte = IIf(cmbTarget.SelectedIndex = 0, _
                                            IIf(AtkDmgType = _DmgType.Physical, _
                                                pAttacker.EVs.Atk, _
                                            pAttacker.EVs.SpAtk), _
                                        IIf(DefDmgType = _DmgType.Physical, _
                                            pDefender.EVs.Def, _
                                            pDefender.EVs.SpDef))
                    'Dim iTest As Integer
                    'For iTest = 0 To 252
                    '    If cmbTarget.SelectedIndex = 0 Then
                    '        If AtkDmgType = _DmgType.Physical Then
                    '            pAttacker.EVs.Atk = iTest
                    '        Else
                    '            pAttacker.EVs.SpAtk = iTest
                    '        End If
                    '        If Calc_Damage(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                    '                        _MainGUI.PokeData.fOptions.Weather) >= iRevHP Then
                    '            bCanKO = True
                    '            Exit For
                    '        End If
                    '    Else
                    '        If DefDmgType = _DmgType.Physical Then
                    '            pDefender.EVs.Def = iTest
                    '        Else
                    '            pDefender.EVs.SpDef = iTest
                    '        End If
                    '        If Calc_Damage(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                    '            _MainGUI.PokeData.fOptions.Weather) <= iRevHP Then
                    '            bCanKO = True
                    '            Exit For
                    '        End If
                    '    End If
                    'Next
                    Dim iResult As Integer
                    iResult = _
                        FindVariable(iRevHP, 0, 252, _
                                     Function(a As Integer)
                                         If cmbTarget.SelectedIndex = 0 Then
                                             If AtkDmgType = _DmgType.Physical Then
                                                 pAttacker.EVs.Atk = a
                                             Else
                                                 pAttacker.EVs.SpAtk = a
                                             End If
                                             Return Calc_Damage_New(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                                                             _MainGUI.PokeData.fOptions.Weather)
                                         Else
                                             If DefDmgType = _DmgType.Physical Then
                                                 pDefender.EVs.Def = a
                                             Else
                                                 pDefender.EVs.SpDef = a
                                             End If
                                             Return Calc_Damage_New(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                                                 _MainGUI.PokeData.fOptions.Weather)
                                         End If
                                     End Function, IIf(cmbTarget.SelectedIndex = 0, False, True), _
                                     IIf(cmbTarget.SelectedIndex = 0, CompareFunc.MoreOrEquals, CompareFunc.LessOrEquals))
                    iResult = Math.Floor(iResult / 4) * 4
                    If iResult > -1 Then bCanKO = True
                    'Restore
                    If cmbTarget.SelectedIndex = 0 Then
                        If AtkDmgType = _DmgType.Physical Then
                            pAttacker.EVs.Atk = iEV
                        Else
                            pAttacker.EVs.SpAtk = iEV
                        End If
                    Else
                        If DefDmgType = _DmgType.Physical Then
                            pDefender.EVs.Def = iEV
                        Else
                            pDefender.EVs.SpDef = iEV
                        End If
                    End If
                    lblRevResult.Text += IIf(Not bCanKO, strFailMsg, iResult & _
                                                IIf(cmbTarget.SelectedIndex = 0, _
                                            IIf(AtkDmgType = _DmgType.Physical, _
                                                " Atk EVs", _
                                            " SpAtk EVs"), _
                                        IIf(DefDmgType = _DmgType.Physical, _
                                            " Def EVs", _
                                            " SpDef EVs")))
                Case 2 'IVs
                    Dim iIV As Byte = IIf(cmbTarget.SelectedIndex = 0, _
                        IIf(AtkDmgType = _DmgType.Physical, _
                            pAttacker.IVs.Atk, _
                        pAttacker.IVs.SpAtk), _
                    IIf(DefDmgType = _DmgType.Physical, _
                        pDefender.IVs.Def, _
                        pDefender.IVs.SpDef))
                    'Dim iTest As Integer
                    'For iTest = 0 To 31
                    '    If cmbTarget.SelectedIndex = 0 Then
                    '        If AtkDmgType = _DmgType.Physical Then
                    '            pAttacker.IVs.Atk = iTest
                    '        Else
                    '            pAttacker.IVs.SpAtk = iTest
                    '        End If
                    '        If Calc_Damage(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                    '                        _MainGUI.PokeData.fOptions.Weather) >= iRevHP Then
                    '            bCanKO = True
                    '            Exit For
                    '        End If
                    '    Else
                    '        If DefDmgType = _DmgType.Physical Then
                    '            pDefender.IVs.Def = iTest
                    '        Else
                    '            pDefender.IVs.SpDef = iTest
                    '        End If
                    '        If Calc_Damage(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                    '            _MainGUI.PokeData.fOptions.Weather) <= iRevHP Then
                    '            bCanKO = True
                    '            Exit For
                    '        End If
                    '    End If
                    'Next
                    Dim iResult As Integer
                    iResult = _
                        FindVariable(iRevHP, 0, 31, _
                                           Function(a As Integer)
                                               If cmbTarget.SelectedIndex = 0 Then
                                                   If AtkDmgType = _DmgType.Physical Then
                                                       pAttacker.IVs.Atk = a
                                                   Else
                                                       pAttacker.IVs.SpAtk = a
                                                   End If
                                                   Return Calc_Damage_New(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                                                                   _MainGUI.PokeData.fOptions.Weather)
                                               Else
                                                   If DefDmgType = _DmgType.Physical Then
                                                       pDefender.IVs.Def = a
                                                   Else
                                                       pDefender.IVs.SpDef = a
                                                   End If
                                                   Return Calc_Damage_New(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                                                       _MainGUI.PokeData.fOptions.Weather)
                                               End If
                                           End Function, _
                                            IIf(cmbTarget.SelectedIndex = 0, False, True), _
                                            IIf(cmbTarget.SelectedIndex = 0, CompareFunc.MoreOrEquals, CompareFunc.LessOrEquals))
                    If iResult > -1 Then bCanKO = True
                    'Restore
                    If cmbTarget.SelectedIndex = 0 Then
                        If AtkDmgType = _DmgType.Physical Then
                            pAttacker.IVs.Atk = iIV
                        Else
                            pAttacker.IVs.SpAtk = iIV
                        End If
                    Else
                        If DefDmgType = _DmgType.Physical Then
                            pDefender.IVs.Def = iIV
                        Else
                            pDefender.IVs.SpDef = iIV
                        End If
                    End If
                    lblRevResult.Text += IIf(Not bCanKO, strFailMsg, iResult & _
                                                IIf(cmbTarget.SelectedIndex = 0, _
                                            IIf(AtkDmgType = _DmgType.Physical, _
                                                " Atk IVs", _
                                            " SpAtk IVs"), _
                                        IIf(DefDmgType = _DmgType.Physical, _
                                            " Def IVs", _
                                            " SpDef IVs")))

                Case 3 'Stats
                    'Dim iTest As Integer
                    'For iTest = 1 To 9999
                    '    If cmbTarget.SelectedIndex = 0 Then
                    '        If Calc_Damage(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                    '                        _MainGUI.PokeData.fOptions.Weather, iTest) >= iRevHP Then
                    '            bCanKO = True
                    '            Exit For
                    '        End If
                    '    Else
                    '        If Calc_Damage(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                    '            _MainGUI.PokeData.fOptions.Weather, 0, iTest) <= iRevHP Then
                    '            bCanKO = True
                    '            Exit For
                    '        End If
                    '    End If
                    'Next
                    Dim iResult As Integer
                    iResult = _
                        FindVariable(iRevHP, 1, 9999, _
                                     Function(a As Integer)
                                         If cmbTarget.SelectedIndex = 0 Then
                                             Return Calc_Damage_New(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                                             _MainGUI.PokeData.fOptions.Weather, a)
                                         Else
                                             Return Calc_Damage_New(pAttacker, pDefender, pAttacker.CurMove, iRnd, _
                                             _MainGUI.PokeData.fOptions.Weather, 0, a)
                                         End If
                                     End Function, _
                                     IIf(cmbTarget.SelectedIndex = 0, False, True), _
                                     IIf(cmbTarget.SelectedIndex = 0, CompareFunc.MoreOrEquals, CompareFunc.LessOrEquals))
                    If iResult > -1 Then bCanKO = True
                    lblRevResult.Text += IIf(Not bCanKO, strFailMsg, iResult & _
                                                IIf(cmbTarget.SelectedIndex = 0, _
                                            IIf(AtkDmgType = _DmgType.Physical, _
                                                " Atk Stat", _
                                            " SpAtk Stat"), _
                                        IIf(DefDmgType = _DmgType.Physical, _
                                            " Def Stat", _
                                            " SpDef Stat")))
            End Select
            If bCanKO Then lblRevResult.Text += " (" & iRevHP & " HP Damage)"
        Else
            lblRevResult.Text = "Result"
            'Do nothing
        End If
    End Sub

    Private Sub txtHPRev_PreviewTextInput(ByVal sender As Object, ByVal e As System.Windows.Input.TextCompositionEventArgs) Handles txtHPRev.TextInputStart
        Dim strAllowed As String = "0123456789."
        If InStr(strAllowed, e.Text) = 0 Then e.Handled = True
    End Sub

    Private Sub cmbCalcTarget_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbCalcTarget.SelectionChanged
        On Error Resume Next
        cmbTarget.IsEnabled = Not (cmbCalcTarget.SelectedIndex = 0)
    End Sub

    Private Sub btnCopy_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnCopy.Click
        Try
            If Not ttCopy.Content Is Nothing Then
                Clipboard.SetText(ttCopy.Content)
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
