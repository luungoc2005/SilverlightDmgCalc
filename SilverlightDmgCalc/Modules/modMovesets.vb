Imports System.Xml.Linq
Imports System.IO.IsolatedStorage

Public Structure MovesetData
    Dim PokeName As String
    Dim Level As Byte
    Dim _MovesetName As String
    Public Property MovesetName As String
        Get
            Return IIf(_MovesetName = vbNullString, PokeName, _MovesetName)
        End Get
        Set(value As String)
            _MovesetName = value
        End Set
    End Property
    Dim AbilityIndex As Byte
    Dim Nature As String
    Dim Item As String
    Dim EVs As Stats
    Dim IVs As Stats
    Dim Moves As Collection
    Dim Happiness As Byte
    Dim Gender As _Gender
    Public Sub New(_PokeName As String, Optional MovesetName As String = vbNullString, Optional _Item As String = vbNullString, _
                   Optional _Nature As String = vbNullString, Optional HPEV As Byte = 0, Optional AtkEV As Byte = 0, Optional DefEV As Byte = 0, _
                   Optional SpAtkEV As Byte = 0, Optional SpDefEV As Byte = 0, Optional SpeEV As Byte = 0)

        Moves = New Collection
        PokeName = _PokeName
        Level = 100
        Happiness = 255
        AbilityIndex = 0
        With IVs
            .HP = 31
            .Atk = 31
            .Def = 31
            .SpAtk = 31
            .SpDef = 31
            .Speed = 31
        End With
        Gender = _Gender.Male

        Item = _Item
        With EVs
            .HP = HPEV
            .Atk = AtkEV
            .Def = DefEV
            .SpAtk = SpAtkEV
            .SpDef = SpDefEV
            .Speed = SpeEV
        End With

        _MovesetName = MovesetName
        Nature = _Nature
    End Sub
    Dim XMLData As XElement
    Dim TextData As String
    Public Sub FromXElement(ByVal data As XElement, Optional LoadFully As Boolean = True)
        On Error Resume Next
        MovesetName = data.Attribute(("name")).Value
        If LoadFully Then
            LoadXML()
        Else
            XMLData = data
        End If
    End Sub
    Public Sub LoadXML()
        On Error Resume Next
        If Not XMLData Is Nothing Then
            Level = Byte.Parse(XMLData.Element(("level")).Value)
            AbilityIndex = Byte.Parse(XMLData.Element(("ability")).Value)
            Nature = XMLData.Element(("nature")).Value
            Item = XMLData.Element(("item")).Value
            With EVs
                .HP = Byte.Parse(XMLData.Element(("ev_hp")).Value)
                .Atk = Byte.Parse(XMLData.Element(("ev_atk")).Value)
                .Def = Byte.Parse(XMLData.Element(("ev_def")).Value)
                .SpAtk = Byte.Parse(XMLData.Element(("ev_spatk")).Value)
                .SpDef = Byte.Parse(XMLData.Element(("ev_spdef")).Value)
                .Speed = Byte.Parse(XMLData.Element(("ev_speed")).Value)
            End With
            With IVs
                .HP = Byte.Parse(XMLData.Element(("IV_hp")).Value)
                .Atk = Byte.Parse(XMLData.Element(("IV_atk")).Value)
                .Def = Byte.Parse(XMLData.Element(("IV_def")).Value)
                .SpAtk = Byte.Parse(XMLData.Element(("IV_spatk")).Value)
                .SpDef = Byte.Parse(XMLData.Element(("IV_spdef")).Value)
                .Speed = Byte.Parse(XMLData.Element(("IV_speed")).Value)
            End With
            For Each objMove As XElement In XMLData.Elements(("move"))
                Moves.Add(objMove.Value)
            Next
            Happiness = Byte.Parse(XMLData.Element(("happiness")).Value)
            Gender = CType(XMLData.Element(("gender")).Value, _Gender)
            XMLData = Nothing
        End If
    End Sub
    Public Sub FromText(data As String, Optional IgnoreName As Boolean = True)
        On Error Resume Next
        Level = 100
        Happiness = 255
        AbilityIndex = 0
        With IVs
            .HP = 31
            .Atk = 31
            .Def = 31
            .SpAtk = 31
            .SpDef = 31
            .Speed = 31
        End With
        Gender = _Gender.Male
        Moves = New Collection

        Dim objData As PokeData = Nothing
        Dim strTemp() As String = data.Split(ControlChars.NewLine.ToCharArray, StringSplitOptions.RemoveEmptyEntries)
        Dim iLineCount As Integer
        Dim colMovesTemp As New Collection

        For Each strLine As String In strTemp
            If strLine <> vbNullString Then
                If iLineCount = 0 Then
                    Dim strName() As String = Split(strLine, "@")
                    Dim strNameData() As String = Split(Trim(strName(0)), " ")

                    Dim strNameTest As String = Trim(strNameData(0))

                    If LCase(strNameData(UBound(strNameData))) = "(m)" Then
                        Gender = _Gender.Male
                    ElseIf LCase(strNameData(UBound(strNameData))) = "(f)" Then
                        Gender = _Gender.Female
                    Else
                        Gender = _Gender.Genderless
                    End If

                    Dim iNameIndex As Byte = UBound(strNameData) - IIf(Gender = _Gender.Genderless, 0, 1)
                    If iNameIndex < 0 Then Exit Sub

                    If LCase(PokeName) <> LCase(strNameTest) Then
                        If iNameIndex = 1 Then
                            MovesetName = strNameTest
                        Else
                            For i As Integer = 0 To iNameIndex - 1
                                MovesetName += strNameData(i) & IIf(i = iNameIndex - 1, "", " ")
                            Next
                        End If
                        Dim strRealName As String = Trim(strNameData(iNameIndex))
                        If Right(strRealName, 1) = ")" And Left(strRealName, 1) = "(" Then strRealName = strRealName.Substring(1, strRealName.Length - 2)
                        strRealName = Replace(strRealName, "-Therian", "-T")
                        If strRealName <> (PokeName) Then
                            'Warning: PokeName is incorrect
                            If Not IgnoreName Then
                                Exit Sub
                            Else
                                If _Xpokedata.ContainsKey(strRealName) Then objData = _Xpokedata(strRealName)
                            End If
                        Else
                            If _Xpokedata.ContainsKey(PokeName) Then objData = _Xpokedata(PokeName)
                        End If
                    Else
                        If _Xpokedata.ContainsKey(PokeName) Then objData = _Xpokedata(PokeName)
                    End If

                    If strName.GetUpperBound(0) = 1 Then
                        Item = vbNullString
                        Dim strTestItem As String = LCase(Replace(strName(1), " ", vbNullString))
                        If strTestItem <> "noitem" And strTestItem <> vbNullString Then
                            For Each strTest As String In colItems
                                If LCase(Trim(strName(1))) = LCase(strTest) Then
                                    Item = strTest
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                ElseIf iLineCount >= 1 Then
                    If InStr(LCase(strLine), "trait") <> 0 Or InStr(LCase(strLine), "ability") <> 0 Then
                        Dim strAbility() As String = Split(strLine, ":")
                        Dim strAbilityTest As String = LCase(Trim(strAbility(1)))
                        Dim intAbility As Byte = FindArray(arrAbilities, strAbilityTest)
                        If intAbility = objData.Ability3 Then
                            AbilityIndex = 2
                        ElseIf intAbility = objData.Ability2 Then
                            AbilityIndex = 1
                        Else
                            AbilityIndex = 0
                        End If

                    ElseIf InStr(strLine, "EVs") <> 0 Then
                        Dim strEVs() As String = Split(strLine, ":")
                        Dim strEVsDat() As String = Split(Replace(LCase(strEVs(1)), " ", vbNullString), "/")
                        For Each strEVTemp As String In strEVsDat
                            If InStr(strEVTemp, "hp") <> 0 Then
                                EVs.HP = Val(strEVTemp)
                            ElseIf InStr(strEVTemp, "spe") <> 0 Or InStr(strEVTemp, "speed") <> 0 _
                                 Or InStr(strEVTemp, "spd") <> 0 Then
                                EVs.Speed = Val(strEVTemp)
                            ElseIf InStr(strEVTemp, "spatk") <> 0 Or InStr(strEVTemp, "satk") <> 0 _
                                 Or InStr(strEVTemp, "special attack") <> 0 Then
                                EVs.SpAtk = Val(strEVTemp)
                            ElseIf InStr(strEVTemp, "spdef") <> 0 Or InStr(strEVTemp, "sdef") <> 0 _
                                 Or InStr(strEVTemp, "special defense") <> 0 Then
                                EVs.SpDef = Val(strEVTemp)
                            ElseIf InStr(strEVTemp, "atk") <> 0 Or InStr(strEVTemp, "attack") <> 0 Then
                                EVs.Atk = Val(strEVTemp)
                            ElseIf InStr(strEVTemp, "def") <> 0 Or InStr(strEVTemp, "defense") <> 0 Then
                                EVs.Def = Val(strEVTemp)
                            End If
                        Next
                    ElseIf InStr(strLine, "ivs") <> 0 Then
                        Dim strIVs() As String = Split(strLine, ":")
                        Dim strIVsDat() As String = Split(Replace(LCase(strIVs(1)), " ", vbNullString), "/")
                        For Each strIVTemp As String In strIVsDat
                            If InStr(strIVTemp, "hp") <> 0 Then
                                IVs.HP = Val(strIVTemp)
                            ElseIf InStr(strIVTemp, "spe") <> 0 Or InStr(strIVTemp, "speed") <> 0 _
                                 Or InStr(strIVTemp, "spd") <> 0 Then
                                IVs.Speed = Val(strIVTemp)
                            ElseIf InStr(strIVTemp, "spatk") <> 0 Or InStr(strIVTemp, "satk") <> 0 _
                                 Or InStr(strIVTemp, "special attack") <> 0 Then
                                IVs.SpAtk = Val(strIVTemp)
                            ElseIf InStr(strIVTemp, "spdef") <> 0 Or InStr(strIVTemp, "sdef") <> 0 _
                                 Or InStr(strIVTemp, "special defense") <> 0 Then
                                IVs.SpDef = Val(strIVTemp)
                            ElseIf InStr(strIVTemp, "atk") <> 0 Or InStr(strIVTemp, "attack") <> 0 Then
                                IVs.Atk = Val(strIVTemp)
                            ElseIf InStr(strIVTemp, "def") <> 0 Or InStr(strIVTemp, "defense") <> 0 Then
                                IVs.Def = Val(strIVTemp)
                            End If
                        Next
                    ElseIf InStr(LCase(strLine), "nature") Then
                        If InStr(strLine, ":") = 0 Then
                            Dim strNature() As String = Split(LCase(Trim(strLine)), " ")
                            For Each strNatureTest As String In arrNatures
                                If strNature(0) = LCase(strNatureTest) Then
                                    Nature = strNatureTest
                                    Exit For
                                End If
                            Next
                        Else
                            Dim strNature() As String = Split(LCase(Trim(strLine)), ":")
                            For Each strNatureTest As String In arrNatures
                                If strNature(1) = LCase(strNatureTest) Then
                                    Nature = strNatureTest
                                    Exit For
                                End If
                            Next
                        End If
                    Else
                        colMovesTemp.Add(Trim(ExLTrim(strLine)))
                    End If
                End If

                iLineCount += 1
            Else
                Exit For
            End If
        Next
        PokeName = objData.Name
        If colMovesTemp.Count > 0 Then
            Moves.Clear()
            For Each strTest As String In colMovesTemp
                Moves.Add(strTest)
            Next
        End If
    End Sub
    Public Function ToText() As String
        If PokeName <> vbNullString Then
            Dim strResult As String
            Dim strEVs As String = GetStatString(EVs, 0)
            Dim strIVs As String = GetStatString(IVs, 31)
            strResult = IIf(_MovesetName = vbNullString, PokeName, MovesetName & " (" & PokeName & ")") & _
                IIf(Gender = _Gender.Male, " (M) ", IIf(Gender = _Gender.Female, " (F) ", vbNullString)) & _
                      IIf(Item <> vbNullString, "@ " & Item, vbNullString) & vbCr & _
                      IIf(strEVs = vbNullString, vbNullString, "EVs: " & strEVs & vbCr) & _
                      IIf(strIVs = vbNullString, vbNullString, "IVs: " & strIVs & vbCr)
            If Not Moves Is Nothing Then
                For Each strMove As String In Moves
                    strResult += "- " & strMove & vbCr
                Next
            End If
            Return System.Text.RegularExpressions.Regex.Replace(strResult, "\s{2,}", " ")
        Else
            Return vbNullString
        End If
    End Function
End Structure

Class MovesetsManager

    Public Shared tmpStorage As IsolatedStorageSettings = IsolatedStorageSettings.ApplicationSettings

    Public Shared Function LoadMovesets(strPokeName As String) As List(Of MovesetData)
        Try
            Return (tmpStorage(strPokeName))
        Catch ex As Exception
            Return New List(Of MovesetData)
        End Try
    End Function

    Public Shared Function SaveMovesets(strPokeName As String, strMovesetList As List(Of MovesetData)) As Boolean
        Try
            If tmpStorage.Contains(strPokeName) Then
                tmpStorage(strPokeName) = strMovesetList
            Else
                tmpStorage.Add(strPokeName, strMovesetList)
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Shared Function DelMovesets(strPokeName As String) As Boolean
        Try
            Return tmpStorage.Remove(strPokeName)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Shared Function SaveChanges() As Boolean
        Try
            tmpStorage.Save()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Shared Function ClearAll() As Boolean
        Try
            tmpStorage.Clear()
            tmpStorage.Save()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class
