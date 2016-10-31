Imports System.Xml.Linq
Imports System.Windows.Resources

Partial Public Class PokeGUI
    Inherits UserControl

    Public Property IsMainPoke As Boolean

    Dim _PokeData As New Pokemon
    Dim _IsAttacker = True

    'Public Property _Movesets As New ObservableCollection(Of MovesetData)
    Public Property _Abilities As New ObservableCollection(Of String)
    Public Property _Moves As New ObservableCollection(Of Move)
    Public Property _Movesets As New ObservableCollection(Of MovesetData)

    Dim _defSets As List(Of MovesetData)

    Public ReadOnly Property _DefaultSets As List(Of MovesetData)
        Get
            If cmbPokeName.SelectedItem Is Nothing Then Return Nothing
            Dim strName As String = CType(cmbPokeName.SelectedItem, PokeData).Name
            If _defSets Is Nothing Then
                _defSets = New List(Of MovesetData)
                With _defSets
                    .Add(New MovesetData(strName, "Physically Defensive", "Leftovers", "Impish", 252, , 252))
                    .Add(New MovesetData(strName, "Specially Defensive", "Leftovers", "Calm", 252, , , , 252))
                    .Add(New MovesetData(strName, "Physical Attacker", "Life Orb", "Naive", , 252, , , , 252))
                    .Add(New MovesetData(strName, "Special Attacker", "Life Orb", "Naive", , , , 252, , 252))
                    .Add(New MovesetData(strName, "Choice Band", "Choice Band", "Adamant", , 252, , , , 252))
                    .Add(New MovesetData(strName, "Choice Specs", "Choice Specs", "Modest", , , , 252, , 252))
                End With
            Else
                For Each objData As MovesetData In _defSets
                    objData.PokeName = strName
                Next
            End If
            Return _defSets
        End Get
    End Property

    Public Property IsAttacker As Boolean
        Get
            Return _IsAttacker
        End Get
        Set(ByVal value As Boolean)
            _IsAttacker = value
            If value Then
                txtAtkDef.Text = "Attacker"
            Else
                txtAtkDef.Text = "Defender"
            End If
        End Set
    End Property

    Public Sub New()
        InitializeComponent()
        AddHandler _Moves.CollectionChanged, AddressOf UpdateChkMoveFilter

        pUpdateStats()
        'AddHandler txtEV.TextInputStart, AddressOf KeyFilter
        'AddHandler txtIV.TextInputStart, AddressOf KeyFilter
    End Sub

    Private Function GetWeather() As _Weather
        Try
            Dim objRoot As MainPage = CType(App.Current.RootVisual, MainPage)
            Return CType(objRoot.cmbWeather.SelectedIndex, _Weather)
        Catch ex As Exception
            Return _Weather.None
        End Try
    End Function

    Public WriteOnly Property Import As String
        Set(ByVal value As String)
            On Error Resume Next
            Dim objMoveset As New MovesetData("")
            objMoveset.FromText(value, True)
            ApplyMoveset(objMoveset, True)
        End Set
    End Property

    Public ReadOnly Property PokeStats As StatsData
        Get
            SaveData()
            Return CalcPokeStats(_PokeData, _PokeData.Ability = 109)
        End Get
    End Property

    Public ReadOnly Property PokeData As Pokemon
        Get
            SaveData()
            Return _PokeData
        End Get
    End Property

    Public Sub UpdateData()
        'Activity1.Visibility = Windows.Visibility.Collapsed
        Try
            cmbAbility.ItemsSource = arrAbilities
            cmbNature.ItemsSource = arrNatures
            cmbItem.ItemsSource = colItems
            cmbPokeName.ValueMemberPath = "Name"
            cmbPokeName.ItemsSource = _Xpokedata.Values
            cmbMove.ValueMemberPath = "Name"
            cmbMove.ItemsSource = _Xmovedata.Values
            cmbMove2.DisplayMemberPath = "Name"
            cmbMove2.ItemsSource = _Moves
            cmbAbility2.ItemsSource = _Abilities
            'cmbMoveset.ItemsSource = _Movesets
            cmbMoveset.DisplayMemberPath = "MovesetName"

            UpdateChkMoveFilter()
        Catch ex As Exception
            MessageBox.Show("Error while updating data:" & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub cmbPokeName_LostFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmbPokeName.LostFocus
        If CType(cmbPokeName.SelectedItem, PokeData) Is Nothing Then Exit Sub
        With CType(cmbPokeName.SelectedItem, PokeData)
            _PokeData = New Pokemon

            _Abilities.Clear()
            If .Ability1 <> 0 Then _Abilities.Add(arrAbilities(.Ability1))
            If .Ability2 <> 0 Then _Abilities.Add(arrAbilities(.Ability2))
            If .Ability3 <> 0 Then _Abilities.Add(arrAbilities(.Ability3))

            cmbAbility2.ItemsSource = _Abilities

            If _Abilities.Count > 0 Then
                cmbAbility2.SelectedIndex = 0
            Else
                cmbAbility2.SelectedIndex = -1
            End If

            _PokeData.Data = cmbPokeName.SelectedItem

            For i As Integer = 0 To 16
                If arrPokeItemFormes(i, 0) = _PokeData.Data.Name Then
                    cmbItem.SelectedItem = arrPokeItemFormes(i, 1)
                    cmbItem.IsEnabled = False
                    Exit For
                End If
            Next

            cmbMove.Text = vbNullString
            'cmbItem.Text = vbNullString
            'cmbNature.Text = vbNullString
            cmbMove.SelectedItem = Nothing
            cmbMove2.SelectedItem = Nothing

            'ApplyMoveset(New MovesetData(_PokeData.Data.Name))

            'pUpdateStats()
            GetMovesets(.Name)
        End With
    End Sub

    Private Sub cmbPokeName_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbPokeName.SelectionChanged

    End Sub

    Private Sub GetMovesets(ByVal strName As String)
        If strName = vbNullString Then Exit Sub
        cmbMoveset.SelectedIndex = -1
        '_Movesets = MovesetsManager.LoadMovesets(strName)
        _Movesets.Clear()
        '_Movesets.Add(New MovesetData(PokeData.Data.Name, "Reset All"))

        If Not objDataStream Is Nothing Then
            Try
                Dim objMoveset As StreamResourceInfo = _
                    Application.GetResourceStream(objDataStream, New Uri("movesets/" & strName & ".xml", UriKind.Relative))
                If Not objMoveset Is Nothing Then
                    Dim objXML As XElement = XElement.Load(objMoveset.Stream)
                    For Each _data As XElement In objXML.Descendants(XName.Get("moveset"))
                        Dim objData As New MovesetData(strName)
                        objData.FromXElement(_data, False)
                        If objData.MovesetName <> vbNullString Then _Movesets.AddWithoutUpdate(objData)
                    Next
                End If

                Dim objText As StreamResourceInfo = _
                    Application.GetResourceStream(objDataStream, New Uri("movesets/" & strName & ".txt", UriKind.Relative))
                If Not objText Is Nothing Then
                    Dim strData As String
                    Using objReader As New IO.StreamReader(objText.Stream)
                        strData = (objReader.ReadToEnd)
                        objReader.Close()
                    End Using
                    Dim strMovesets As String() = Split(strData, vbCrLf & vbCrLf)
                    For Each _data As String In strMovesets
                        Dim objData As New MovesetData(strName)
                        objData.FromText(_data, True)
                        If objData.MovesetName <> vbNullString Then _Movesets.AddWithoutUpdate(objData)
                    Next
                End If

                If _Movesets.Count = 0 Then
                    chkMoveFilter.IsEnabled = False
                End If

                _Movesets.UpdateCollection()
            Catch ex As Exception
                'MessageBox.Show(ex.Message)
            End Try
        End If

        DoMovesetsCheck()

        'Activity2.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private Sub ApplyMoveset(objMoveset As MovesetData, Optional ApplyName As Boolean = False)
        With objMoveset
            If ApplyName Then
                If _Xpokedata.ContainsKey(objMoveset.PokeName) Then cmbPokeName.SelectedItem = _Xpokedata(objMoveset.PokeName)
            End If
            'Dim index As Integer
            'Select Case .AbilityIndex
            '    Case 0
            '        index = CType(cmbPokeName.SelectedItem, PokeData).Ability1
            '    Case 1
            '        index = CType(cmbPokeName.SelectedItem, PokeData).Ability2
            '    Case 2
            '        index = CType(cmbPokeName.SelectedItem, PokeData).Ability3
            'End Select
            cmbAbility2.SelectedIndex = .AbilityIndex

            If cmbItem.IsEnabled = True Then cmbItem.SelectedItem = .Item

            _PokeData.Gender = .Gender
            _PokeData.Happiness = .Happiness
            _PokeData.Level = .Level

            _PokeData.EVs = .EVs
            _PokeData.IVs = .IVs
            
            cmbNature.SelectedItem = .Nature 'Call pUpdatestats

            _Moves.Clear()
            For Each strTemp As String In .Moves
                If _Xmovedata.ContainsKey(strTemp) Then _Moves.Add(_Xmovedata(strTemp))
            Next
        End With
    End Sub

    Private Sub UpdateChkMoveFilter()
        If _Moves.Count = 0 Then
            chkMoveFilter.IsEnabled = False
        Else
            chkMoveFilter.IsEnabled = True
        End If
        If chkMoveFilter.IsEnabled Then
            cmbMove2.Visibility = IIf(chkMoveFilter.IsChecked, Visibility.Visible, Visibility.Collapsed)
            cmbMove.Visibility = IIf(chkMoveFilter.IsChecked, Visibility.Collapsed, Visibility.Visible)
        Else
            cmbMove2.Visibility = Windows.Visibility.Collapsed
            cmbMove.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Private Sub cmbMoveset_LostFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles cmbMoveset.LostFocus

    End Sub

    Private Sub cmbMoveset_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbMoveset.SelectionChanged
        On Error Resume Next
        If cmbMoveset.SelectedIndex = -1 Then Exit Sub
        Dim objMoveset As MovesetData = CType(cmbMoveset.SelectedItem, MovesetData)
        If Not objMoveset.XMLData Is Nothing Then objMoveset.LoadXML()
        ApplyMoveset(objMoveset)
    End Sub

    Private Sub chkAbiFilter_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkAbiFilter.Click
        cmbAbility2.Visibility = IIf(chkAbiFilter.IsChecked, Visibility.Visible, Visibility.Collapsed)
        cmbAbility.Visibility = IIf(chkAbiFilter.IsChecked, Visibility.Collapsed, Visibility.Visible)
    End Sub

    Private Sub chkMoveFilter_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkMoveFilter.Click
        cmbMove2.Visibility = IIf(chkMoveFilter.IsChecked, Visibility.Visible, Visibility.Collapsed)
        cmbMove.Visibility = IIf(chkMoveFilter.IsChecked, Visibility.Collapsed, Visibility.Visible)
    End Sub

    Private Sub ExtractData()
        Dim strEVs() As String = Split(txtEV.Text, "-")
        Dim strIVs() As String = Split(txtIV.Text, "-")
        If strEVs.Length <> 6 Or strIVs.Length <> 6 Then Call pUpdateStats() : Exit Sub

        Dim _hpEV As Double = Val(strEVs(0))
        Dim _atkEV As Double = Val(strEVs(1))
        Dim _defEV As Double = Val(strEVs(2))
        Dim _spatkEV As Double = Val(strEVs(3))
        Dim _spdefEV As Double = Val(strEVs(4))
        Dim _speedEV As Double = Val(strEVs(5))

        With _PokeData.EVs
            .HP = IIf(_hpEV > 255, 255, IIf(_hpEV < 0, 0, _hpEV))
            .Atk = IIf(_atkEV > 255, 255, IIf(_atkEV < 0, 0, _atkEV))
            .Def = IIf(_defEV > 255, 255, IIf(_defEV < 0, 0, _defEV))
            .SpAtk = IIf(_spatkEV > 255, 255, IIf(_spatkEV < 0, 0, _spatkEV))
            .SpDef = IIf(_spdefEV > 255, 255, IIf(_spdefEV < 0, 0, _spdefEV))
            .Speed = IIf(_speedEV > 255, 255, IIf(_speedEV < 0, 0, _speedEV))
        End With

        Dim _hpIV As Double = Val(strIVs(0))
        Dim _atkIV As Double = Val(strIVs(1))
        Dim _defIV As Double = Val(strIVs(2))
        Dim _spatkIV As Double = Val(strIVs(3))
        Dim _spdefIV As Double = Val(strIVs(4))
        Dim _speedIV As Double = Val(strIVs(5))

        With _PokeData.IVs
            .HP = IIf(_hpIV > 31, 31, IIf(_hpIV < 0, 0, _hpIV))
            .Atk = IIf(_atkIV > 31, 31, IIf(_atkIV < 0, 0, _atkIV))
            .Def = IIf(_defIV > 31, 31, IIf(_defIV < 0, 0, _defIV))
            .SpAtk = IIf(_spatkIV > 31, 31, IIf(_spatkIV < 0, 0, _spatkIV))
            .SpDef = IIf(_spdefIV > 31, 31, IIf(_spdefIV < 0, 0, _spdefIV))
            .Speed = IIf(_speedIV > 31, 31, IIf(_speedIV < 0, 0, _speedIV))
        End With

        Call pUpdateStats()
    End Sub

    Private Sub pUpdateStats()
        SaveData()

        txtEV.Text = _PokeData.EVs.HP & "-" & _PokeData.EVs.Atk & "-" & _PokeData.EVs.Def & _
    "-" & _PokeData.EVs.SpAtk & "-" & _PokeData.EVs.SpDef & "-" & _PokeData.EVs.Speed
        txtIV.Text = _PokeData.IVs.HP & "-" & _PokeData.IVs.Atk & "-" & _PokeData.IVs.Def & _
    "-" & _PokeData.IVs.SpAtk & "-" & _PokeData.IVs.SpDef & "-" & _PokeData.IVs.Speed
        txtStats.Text = strStatWithBoost(Me.PokeStats.HP) & "-" & _
            strStatWithBoost(Me.PokeStats.Atk, PokeData.Boosts.Atk) & "-" & _
            strStatWithBoost(Me.PokeStats.Def, PokeData.Boosts.Def) & "-" & _
            strStatWithBoost(Me.PokeStats.SpAtk, PokeData.Boosts.SpAtk) & "-" & _
            strStatWithBoost(Me.PokeStats.SpDef, PokeData.Boosts.SpDef) & "-" & _
            strStatWithBoost(Me.PokeStats.Speed, PokeData.Boosts.Speed)
        If cmbItem.IsEnabled = False Then
            If InStr(LCase(_PokeData.Data.Name), "arceus") = 0 And _PokeData.Ability <> 121 And _
                LCase(_PokeData.Data.Name) <> "giratina-o" Then
                cmbItem.IsEnabled = True
            End If
        End If
    End Sub

    Private Function strStatWithBoost(ByVal Stat As Integer, Optional ByVal Boost As Integer = 0) As String
        If Boost = 0 Then
            Return Stat
        Else
            Return Stat & " (" & IIf(Boost > 0, "+" & Boost, Boost) & ")"
        End If
    End Function

    Public Sub SaveData()
        On Error Resume Next
        With _PokeData
            .Ability = FindArray(arrAbilities, cmbAbility.SelectedItem)
            .Nature = GetNature(cmbNature.SelectedItem)
            .Item = cmbItem.SelectedItem
            .Data = IIf(cmbPokeName.SelectedItem = Nothing, New PokeData, CType(cmbPokeName.SelectedItem, PokeData))
            '.Data.Type1 = IIf(cmbType1.SelectedIndex = -1, 0, cmbType1.SelectedIndex)
            '.Data.Type2 = IIf(cmbType2.SelectedIndex = -1, 0, cmbType2.SelectedIndex)
        End With
    End Sub

    Private Sub cmbMove_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbMove.SelectionChanged
        If cmbMove.SelectedItem Is Nothing Then Exit Sub
        Dim pMoveData As Move = cmbMove.SelectedItem
        Dim iBoost As Boolean = True
        Dim bIsNotSimple As Boolean = Not (FindArray(arrAbilities, cmbAbility.SelectedItem) = 86)
        If pMoveData.Name <> vbNullString Then
            With _PokeData
                Select Case LCase(pMoveData.Name)
                    Case "acid armor"
                        .Boosts.Def += IIf(bIsNotSimple, 2, 4)
                    Case "agility"
                        .Boosts.Speed += IIf(bIsNotSimple, 2, 4)
                    Case "amnesia"
                        .Boosts.SpDef += IIf(bIsNotSimple, 2, 4)
                    Case "barrier"
                        .Boosts.Def += IIf(bIsNotSimple, 2, 4)
                    Case "belly drum"
                        .Boosts.Atk = 6
                    Case "bulk up"
                        .Boosts.Atk += IIf(bIsNotSimple, 1, 2)
                        .Boosts.Def += IIf(bIsNotSimple, 1, 2)
                    Case "quiver dance"
                        .Boosts.SpAtk += IIf(bIsNotSimple, 1, 2)
                        .Boosts.SpDef += IIf(bIsNotSimple, 1, 2)
                        .Boosts.Speed += IIf(bIsNotSimple, 1, 2)
                    Case "calm mind"
                        .Boosts.SpAtk += IIf(bIsNotSimple, 1, 2)
                        .Boosts.SpDef += IIf(bIsNotSimple, 1, 2)
                    Case "cheer up"
                        .Boosts.Atk += IIf(bIsNotSimple, 1, 2)
                        .Boosts.SpAtk += IIf(bIsNotSimple, 1, 2)
                    Case "growth"
                        If GetWeather() = _Weather.Sunshine Then
                            .Boosts.Atk += IIf(bIsNotSimple, 2, 4)
                            .Boosts.SpAtk += IIf(bIsNotSimple, 2, 4)
                        Else
                            .Boosts.Atk += IIf(bIsNotSimple, 1, 2)
                            .Boosts.SpAtk += IIf(bIsNotSimple, 1, 2)
                        End If
                    Case "hone claws"
                        .Boosts.Atk += IIf(bIsNotSimple, 1, 2)
                    Case "coil"
                        .Boosts.Atk += IIf(bIsNotSimple, 1, 2)
                        .Boosts.Def += IIf(bIsNotSimple, 1, 2)
                    Case "cosmic power", "defend order"
                        .Boosts.Def += IIf(bIsNotSimple, 1, 2)
                        .Boosts.SpDef += IIf(bIsNotSimple, 1, 2)
                    Case "cotton guard"
                        .Boosts.Def += IIf(bIsNotSimple, 3, 6)
                    Case "curse"
                        If _PokeData.Data.Type1 <> 8 And _PokeData.Data.Type2 <> 8 Then
                            .Boosts.Def += IIf(bIsNotSimple, 1, 2)
                            .Boosts.Atk += IIf(bIsNotSimple, 1, 2)
                            .Boosts.Speed -= IIf(bIsNotSimple, 1, 2)
                        End If
                    Case "defense curl", "harden"
                        .Boosts.Def += IIf(bIsNotSimple, 1, 2)
                    Case "dragon dance"
                        .Boosts.Atk += IIf(bIsNotSimple, 1, 2)
                        .Boosts.Speed += IIf(bIsNotSimple, 1, 2)
                    Case "howl", "meditate", "sharpen"
                        .Boosts.Atk += IIf(bIsNotSimple, 1, 2)
                    Case "nasty plot"
                        .Boosts.SpAtk += IIf(bIsNotSimple, 2, 4)
                    Case "shell smash"
                        .Boosts.Atk += IIf(bIsNotSimple, 2, 4)
                        .Boosts.SpAtk += IIf(bIsNotSimple, 2, 4)
                        .Boosts.Speed += IIf(bIsNotSimple, 2, 4)
                        .Boosts.Def -= IIf(bIsNotSimple, 1, 2)
                        .Boosts.SpDef -= IIf(bIsNotSimple, 1, 2)
                    Case "swords dance"
                        .Boosts.Atk += IIf(bIsNotSimple, 2, 4)
                    Case "tail glow"
                        .Boosts.SpAtk += IIf(bIsNotSimple, 3, 6)
                    Case Else
                        iBoost = False
                End Select

                .CurMove = pMoveData
            End With
        End If

        If iBoost Then
            With _PokeData.Boosts
                .HP = IIf(.HP > 6, 6, IIf(.HP < -6, -6, .HP))
                .Atk = IIf(.Atk > 6, 6, IIf(.Atk < -6, -6, .Atk))
                .Def = IIf(.Def > 6, 6, IIf(.Def < -6, -6, .Def))
                .SpAtk = IIf(.SpAtk > 6, 6, IIf(.SpAtk < -6, -6, .SpAtk))
                .SpDef = IIf(.SpDef > 6, 6, IIf(.SpDef < -6, -6, .SpDef))
                .Speed = IIf(.Speed > 6, 6, IIf(.Speed < -6, -6, .Speed))
            End With
            Call pUpdateStats()
            'cmbMove.SelectedValue = vbNullString
        End If
    End Sub

    Private Sub btnEditMove_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnEditMove.Click
        Try
            Dim objWndMove As New wndEditMove
            objWndMove.PokeData = Me.PokeData
            objWndMove.LoadData()

            AddHandler objWndMove.Closed, _
                Sub(s1, e1)
                    If objWndMove.DialogResult = False Then Exit Sub
                    With _PokeData.CurMove
                        If .Name = vbNullString Then
                            cmbMove.Text = .Power & " Power " & arrTypes(.Type) & " " & IIf(.DmgType = _DmgType.Physical, "Physical", "Special")
                        Else
                            cmbMove.SelectedItem = _PokeData.CurMove
                        End If
                    End With
                End Sub
            objWndMove.Show()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnPokeOps_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnPokeOps.Click
        Try
            Dim objAttr As New wndAttributes
            objAttr.PopupForm(Me.PokeData)
            AddHandler objAttr.Closed, AddressOf pUpdateStats
            objAttr.Show()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    'Private Sub KeyFilter(ByVal sender As Object, ByVal e As System.Windows.Input.TextCompositionEventArgs)
    '    Try
    '        Dim strAllowed As String = "-0123456789"
    '        If InStr(strAllowed, e.Text) = 0 Then e.Handled = True
    '        If e.Text = "-" Then
    '            If Text.RegularExpressions.Regex.Matches(txtEV.Text, "-").Count = 5 And _
    '                Text.RegularExpressions.Regex.Matches(txtIV.Text, "-").Count = 5 Then
    '                e.Handled = True
    '            End If
    '        End If
    '        MessageBox.Show(e.Handled)
    '    Catch ex As Exception
    '        MessageBox.Show(ex.Message)
    '    End Try
    'End Sub

    Private Sub btnEditStats_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnEditStats.Click
        Try
            Dim objEditStats As New wndEditStats
            objEditStats.PopupForm(Me.PokeData)
            AddHandler objEditStats.Closed, AddressOf pUpdateStats
            objEditStats.Show()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnEditEV_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnEditEV.Click
        Try
            Dim objEditEVs As New wndEVs
            objEditEVs.PokeData = PokeData
            AddHandler objEditEVs.Closed, AddressOf pUpdateStats
            objEditEVs.Show()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnEditIVs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnEditIVs.Click
        Try
            Dim objEditIVs As New wndIVs
            objEditIVs.PokeData = PokeData
            AddHandler objEditIVs.Closed, AddressOf pUpdateStats
            objEditIVs.Show()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnFieldOps_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnFieldOps.Click
        Try
            Dim objEditFieldOps As New wndFOptions
            objEditFieldOps.PopupForm(Me.PokeData, IsAttacker, IsMainPoke)
            AddHandler objEditFieldOps.Closed, AddressOf pUpdateStats
            objEditFieldOps.Show()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub txtEV_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtEV.LostFocus
        ExtractData()
    End Sub

    Private Sub txtIV_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtIV.LostFocus
        ExtractData()
    End Sub

    Private Sub btnBoosts_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnBoosts.Click
        Try
            Dim objBoosts As New wndBoosts
            objBoosts.PokeData = PokeData
            objBoosts.LoadData()
            AddHandler objBoosts.Closed, AddressOf pUpdateStats
            objBoosts.Show()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        With _PokeData.Boosts
            .HP = 0
            .Atk = 0
            .Def = 0
            .SpAtk = 0
            .SpDef = 0
            .Speed = 0
        End With
        pUpdateStats()
    End Sub

    Private Sub mnuImport_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles mnuImport.Click
        Try
            Dim objImport As New wndImport
            AddHandler objImport.Closed, Sub(s, a)
                                             If objImport.DialogResult Then
                                                 Me.Import = objImport.txtMain.Text
                                             End If
                                         End Sub
            objImport.Show()
            'Dim objImport As New wndImport
            'AddHandler objImport.Closed, Sub(s, a)
            '                                 If objImport.DialogResult Then
            '                                     If objImport.txtMain.Text = vbNullString Then Exit Sub
            '                                     Dim strMovesets() As String = Split(objImport.txtMain.Text, vbCr & vbCr)
            '                                     MessageBox.Show(strMovesets.Count)
            '                                     For Each strTemp As String In strMovesets
            '                                         Dim objMoveset As New MovesetData
            '                                         objMoveset.FromText(strTemp, True)
            '                                         Dim lstTemp As List(Of MovesetData) = MovesetsManager.LoadMovesets(objMoveset.PokeName)
            '                                         lstTemp.Add(objMoveset)
            '                                         MovesetsManager.SaveMovesets(objMoveset.PokeName, lstTemp)
            '                                     Next
            '                                     MovesetsManager.SaveChanges()
            '                                 End If
            '                             End Sub
            'objImport.Show()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub mnuExport_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles mnuExport.Click
        Try
            Dim objImport As New wndImport
            objImport.txtMain.IsReadOnly = True
            With PokeData
                If .Data.Name <> vbNullString Then
                    Dim EVs As String = GetStatString(.EVs)
                    Dim IVs As String = GetStatString(.IVs, 31)
                    objImport.txtMain.Text = .Data.Name & _
                        IIf(.Gender = _Gender.Female, " (F)", IIf(.Gender = _Gender.Male, " (M)", vbNullString)) & _
                        IIf(.Item = vbNullString, vbNullString, " @ " & .Item) & vbCrLf & _
                        "Trait: " & arrAbilities(.Ability) & vbCrLf & _
                        IIf(EVs <> vbNullString, "EVs: " & EVs & vbCrLf, vbNullString) & _
                        IIf(IVs <> vbNullString, "IVs: " & IVs & vbCrLf, vbNullString) & _
                        IIf(cmbNature.Text = vbNullString, vbNullString, cmbNature.Text & " Nature" & vbCrLf) & _
                        IIf(.CurMove.Name = vbNullString, vbNullString, "- " & .CurMove.Name)
                End If
            End With
            objImport.Title = "Export"
            objImport.Show()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub


    Private Sub Button2_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button2.Click
        With _PokeData.IVs
            .HP = 31
            .Atk = 31
            .Def = 31
            .SpAtk = 31
            .SpDef = 31
            .Speed = 31
        End With
        pUpdateStats()
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button3.Click
        With _PokeData.IVs
            .HP = 0
            .Atk = 0
            .Def = 0
            .SpAtk = 0
            .SpDef = 0
            .Speed = 0
        End With
        pUpdateStats()
    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button4.Click
        If IsAttacker Then
            _PokeData.EVs.Atk = 252
            _PokeData.EVs.Speed = 252
            _PokeData.EVs.HP = 0
            _PokeData.EVs.Def = 0
            _PokeData.EVs.SpDef = 0
            _PokeData.EVs.SpAtk = 0
        Else
            _PokeData.EVs.Def = 252
            _PokeData.EVs.HP = 252
            _PokeData.EVs.Atk = 0
            _PokeData.EVs.Speed = 0
            _PokeData.EVs.SpDef = 0
            _PokeData.EVs.SpAtk = 0
        End If
        pUpdateStats()
    End Sub

    Private Sub Button5_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button5.Click
        If IsAttacker Then
            _PokeData.EVs.SpAtk = 252
            _PokeData.EVs.Speed = 252
            _PokeData.EVs.HP = 0
            _PokeData.EVs.Def = 0
            _PokeData.EVs.SpDef = 0
            _PokeData.EVs.Atk = 0
        Else
            _PokeData.EVs.SpDef = 252
            _PokeData.EVs.HP = 252
            _PokeData.EVs.Atk = 0
            _PokeData.EVs.Speed = 0
            _PokeData.EVs.Def = 0
            _PokeData.EVs.SpAtk = 0
        End If
        pUpdateStats()
    End Sub

    Private Sub UserControl_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded

    End Sub

    Private Sub chkMovesets_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles chkMovesets.Checked
        DoMovesetsCheck()
    End Sub

    Private Sub chkMovesets_Unchecked(sender As Object, e As System.Windows.RoutedEventArgs) Handles chkMovesets.Unchecked
        DoMovesetsCheck()
    End Sub

    Private Sub DoMovesetsCheck()
        If _Movesets Is Nothing OrElse _Movesets.Count = 0 Then
            cmbMoveset.ItemsSource = _DefaultSets
            chkMovesets.IsEnabled = False
        Else
            chkMovesets.IsEnabled = True
            If chkMovesets.IsChecked Then
                cmbMoveset.ItemsSource = _Movesets
            Else
                cmbMoveset.ItemsSource = _DefaultSets
            End If
        End If
    End Sub

    Private Sub Button6_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button6.Click
        With _PokeData.EVs
            .HP = 0
            .Atk = 0
            .Def = 0
            .SpAtk = 0
            .SpDef = 0
            .Speed = 0
        End With
        pUpdateStats
    End Sub

    Private Sub cmbNature_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cmbNature.SelectionChanged
        pUpdateStats()
    End Sub
End Class
