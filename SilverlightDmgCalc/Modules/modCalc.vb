Module modCalc

    Public Function GetDmgType(ByVal atkMove As Move, ByVal FOptions As Field_Effects) As _DmgType
        'Copy-pasted
        Dim dAtkMove As Move = atkMove 'Move that the defender takes
        If colSpHitDefMoves.Contains(dAtkMove.Name) Then dAtkMove.DmgType = _DmgType.Physical
        If FOptions.WonderRoom Then
            dAtkMove.DmgType = IIf(dAtkMove.DmgType = _DmgType.Physical, _DmgType.Special, _DmgType.Physical)
        End If
        Return dAtkMove.DmgType
    End Function

    Public Function GetTypeMatch(ByVal Attacker As Pokemon, ByVal _AtkMove As Move, ByVal Defender As Pokemon, ByVal FOptions As Field_Effects, Optional ApplyAbilityEff As Boolean = False) 'Optional ByRef AbilityEff As Boolean = False)

        Dim Type1 As Single, Type2 As Single, TypeMatch As Single
        Type1 = Eff(Defender.Data.Type1, _AtkMove.Type)
        Type2 = Eff(Defender.Data.Type2, _AtkMove.Type)
        If ((_AtkMove.Type = 2 Or _AtkMove.Type = 1) And Attacker.Ability = 113) Then
            If Type1 = 0 Then Type1 = 1
            If Type2 = 0 Then Type2 = 1
            Attacker.AbilityEff = True
        End If
        TypeMatch = IIf(Type1 = 0 And FOptions.NoImmunity, 1, Type1) * IIf(Type2 = 0 And FOptions.NoImmunity, 1, Type2)

        'Immunities
        If Not FOptions.NoImmunity Then
            'Abilities
            If Not (Attacker.Ability = 104 Or Attacker.Ability = 163 Or Attacker.Ability = 164) Then
                'Foe's ability
                Dim FA As Single
                Select Case Defender.Ability
                    Case 47
                        FA = IIf(_AtkMove.Type = 10 Or _AtkMove.Type = 15, 0.5, 1)
                    Case 85
                        FA = IIf(_AtkMove.Type = 10, 0.5, 1)
                    Case 87
                        FA = IIf(_AtkMove.Type = 10, 1.25, 1)
                    Case Else
                        FA = 1
                End Select
                If FA <> 1 Then Defender.AbilityEff = True
                If ApplyAbilityEff Then TypeMatch = TypeMatch * FA

                'Immunities
                If Defender.Ability = 25 Then 'Wonder Guard
                    If TypeMatch < 2 Then TypeMatch = 0 : Defender.AbilityEff = True
                Else
                    For i = 0 To arrImmuneAbilities.GetUpperBound(0)
                        If arrImmuneAbilities(i, 0) = Defender.Ability And _
                            arrImmuneAbilities(i, 1) = _AtkMove.Type Then TypeMatch = 0 : Defender.AbilityEff = True : Exit For
                    Next
                End If
            End If

            If Defender.Item = "Air Balloon" And _AtkMove.Type = 5 Then TypeMatch = 0 : Defender.ItemEff = True
            If (_AtkMove.Type = 5 And Defender.Item = "Iron Ball") Then
                If Type1 = 0 Then Type1 = 1
                If Type2 = 0 Then Type2 = 1
                Defender.ItemEff = True
                TypeMatch = IIf(Type1 = 0 And FOptions.NoImmunity, 1, Type1) * IIf(Type2 = 0 And FOptions.NoImmunity, 1, Type2)
            End If
        End If

        Return TypeMatch
    End Function

    Public Function Calc_Damage(ByVal Attacker As Pokemon, ByVal Defender As Pokemon, _
                                ByVal AtkMove As Move, _
                                ByVal Random As Byte, _
                                ByVal Weather As _Weather, Optional ByVal _AtkStat As Integer = 0, Optional ByVal _DefStat As Integer = 0) As Integer
        'Damage Formula = (((((((Level × 2 ÷ 5) + 2) × BasePower × [Sp]Atk ÷ 50) ÷ [Sp]Def) × Mod1) + 2) × 
        '         CH × Mod2 × R ÷ 100) × STAB × Type1 × Type2 × Mod3
        Attacker.AbilityEff = False
        Attacker.ItemEff = False
        Defender.AbilityEff = False
        Defender.ItemEff = False

        Dim DefAbility As Integer : If (Attacker.Ability = 104 Or Attacker.Ability = 163 Or Attacker.Ability = 164) Then DefAbility = Defender.Ability : Defender.Ability = 0

        Dim BasePower As Single
        Dim AtkStat As Single
        Dim DefStat As Single
        Dim CH As Single
        Dim STAB As Single
        Dim Mod1 As Single
        Dim Mod2 As Single
        Dim Mod3 As Single
        Dim FOptions As Field_Effects = GetFOptions(Attacker, Defender)
        Dim _AtkMove As Move = GetMovePower(Attacker, Defender, AtkMove, FOptions)
        If _AtkMove.Power = 0 Then Return 0

        Dim TypeMatch As Single = GetTypeMatch(Attacker, _AtkMove, Defender, FOptions)

        'Base Power
        Dim HH As Single, BP As Single, IT As Single, CHG As Single, MS As Single, WS As Single, UA As Single, FA As Single
        HH = IIf(FOptions.HelpingHand, 1.5, 1)
        BP = _AtkMove.Power

        ' IT
        'Type boosting items & plates
        If Attacker.Item = arrTypeItems(_AtkMove.Type, 1) Or Attacker.Item = arrTypeItems(_AtkMove.Type, 2) Then
            IT = 1.2
        ElseIf Attacker.Item = arrTypeItems(_AtkMove.Type, 3) Then
            IT = 1.5
        Else
            Select Case Attacker.Item
                Case "Muscle Band"
                    IT = IIf(_AtkMove.DmgType = _DmgType.Physical, 1.1, 1)
                Case "Wise Glasses"
                    IT = IIf(_AtkMove.DmgType = _DmgType.Special, 1.1, 1)
                Case "Adamant Orb"
                    IT = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 9) And _
                           Attacker.Data.Name = "Dialga", 1.2, 1)
                Case "Lustrous Orb"
                    IT = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 11) And _
                            Attacker.Data.Name = "Palkia", 1.2, 1)
                Case "Griseous Orb"
                    IT = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 8) And _
                            Attacker.Data.Name = "Giratina-O", 1.2, 1)
                Case Else
                    IT = 1
            End Select
        End If
        If IT > 1 Then Attacker.ItemEff = True

        CHG = IIf(FOptions.Charge And _AtkMove.Type = 13, 2, 1)
        MS = IIf(FOptions.MudSport And _AtkMove.Type = 13, 0.5, 1)
        WS = IIf(FOptions.WaterSport And _AtkMove.Type = 10, 0.5, 1)
        ' UA
        Select Case Attacker.Ability
            Case 79
                UA = IIf(Attacker.Gender = Defender.Gender, 1.25, _
                       IIf((Attacker.Gender = _Gender.Male And Defender.Gender = _Gender.Female) _
                           Or (Attacker.Gender = _Gender.Female And Defender.Gender = _Gender.Male), _
                           0.75, 1))
            Case 120
                UA = IIf(colRecoilMoves.Contains(_AtkMove.Name), 1.2, 1)
            Case 89
                UA = IIf(colPunchMoves.Contains(_AtkMove.Name), 1.2, 1)
            Case 66
                UA = IIf(_AtkMove.Type = 10 And Attacker.HPPercent <= 33, 1.5, 1)
            Case 65
                UA = IIf(_AtkMove.Type = 12 And Attacker.HPPercent <= 33, 1.5, 1)
            Case 67
                UA = IIf(_AtkMove.Type = 11 And Attacker.HPPercent <= 33, 1.5, 1)
            Case 68
                UA = IIf(_AtkMove.Type = 7 And Attacker.HPPercent <= 33, 1.5, 1)
            Case 101
                UA = IIf(_AtkMove.Power <= 60, 1.5, 1)
            Case 125
                UA = IIf(_AtkMove.EffPercent > 0 And _AtkMove.EffPercent < 1, 1.3, 1)
            Case 159
                UA = IIf((_AtkMove.Type = 5 Or _AtkMove.Type = 6 Or _AtkMove.Type = 9) And _
                         FOptions.Weather = _Weather.Sandstorm, 1.3, 1)
            Case Else
                UA = 1
        End Select
        If UA <> 1 Then Attacker.AbilityEff = True

        ' FA
        Select Case Defender.Ability
            Case 47
                FA = IIf(_AtkMove.Type = 10 Or _AtkMove.Type = 15, 0.5, 1)
            Case 85
                FA = IIf(_AtkMove.Type = 10, 0.5, 1)
            Case 87
                FA = IIf(_AtkMove.Type = 10, 1.25, 1)
            Case Else
                FA = 1
        End Select
        If FA <> 1 Then Defender.AbilityEff = True

        BasePower = Math.Floor(Math.Floor(Math.Floor(Math.Floor(Math.Floor(Math.Floor(Math.Floor( _
                    HH * BP) * IT) * CHG) * MS) * WS) * UA) * FA)

        'Atk Stat
        Dim _TmpAttacker As Pokemon = IIf(LCase(_AtkMove.Name) = "foul play", Defender, Attacker)
        Dim AStat As Integer, AM As Single, IM As Single
        AStat = IIf(_AtkMove.DmgType = _DmgType.Physical, _
                 CalcPokeStats(_TmpAttacker, Defender.Ability = 109, FOptions.CriticalHit).Atk, _
                IIf(_AtkMove.DmgType = _DmgType.Special, _
                    CalcPokeStats(_TmpAttacker, Defender.Ability = 109, FOptions.CriticalHit).SpAtk, 1))
        If _AtkStat <> 0 Then AStat = _AtkStat

        Select Case _TmpAttacker.Ability
            Case 74
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical, 2, 1)
            Case 37
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical, 2, 1)
            Case 122
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical And FOptions.Weather = _Weather.Sunshine, 1.5, 1)
            Case 62
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.Status <> _Status.None, 1.5, 1)
            Case 55
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical, 1.5, 1)
            Case 112
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.AbilityEff, 0.5, 1)
            Case 57
                AM = IIf(_AtkMove.DmgType = _DmgType.Special And _TmpAttacker.AbilityEff, 1.5, 1)
            Case 58
                AM = IIf(_AtkMove.DmgType = _DmgType.Special And _TmpAttacker.AbilityEff, 1.5, 1)
            Case 94
                AM = IIf(_AtkMove.DmgType = _DmgType.Special And FOptions.Weather = _Weather.Sunshine, 1.5, 1)
            Case 137
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.Status = _Status.Poison, 1.5, 1)
            Case 138
                AM = IIf(_AtkMove.DmgType = _DmgType.Special And _TmpAttacker.Status = _Status.Burn, 1.5, 1)
            Case 129
                AM = IIf(_TmpAttacker.HPPercent < 50, 0.5, 1)
            Case Else
                AM = 1
        End Select
        If AM <> 1 Then Attacker.AbilityEff = True

        'IM
        Select Case Attacker.Item
            Case "Choice Band"
                IM = IIf(_AtkMove.DmgType = _DmgType.Physical, 1.5, 1)
            Case "Light Ball"
                IM = IIf(Attacker.Data.Name = "Pikachu", 2, 1)
            Case "Thick Club"
                IM = IIf(_AtkMove.DmgType = _DmgType.Physical And _
                       (Attacker.Data.Name = "Cubone" Or Attacker.Data.Name = "Marowak"), _
                       2, 1)
            Case "Choice Specs"
                IM = IIf(_AtkMove.DmgType = _DmgType.Special, 1.5, 1)
            Case "Soul Dew"
                IM = IIf(_AtkMove.DmgType = _DmgType.Special And _
                       (Attacker.Data.Name = "Latias" Or Attacker.Data.Name = "Latios"), _
                           1.5, 1)
            Case "Deepseatooth"
                IM = IIf(_AtkMove.DmgType = _DmgType.Special And Attacker.Data.Name = "Clamperl", _
                       2, 1)
            Case Else
                IM = 1
        End Select
        If IM <> 1 Then Attacker.ItemEff = True

        AtkStat = Math.Floor(Math.Floor(AStat * AM) * IM)

        'DefStat
        Dim dAtkMove As Move = _AtkMove 'Move that the defender takes
        If colSpHitDefMoves.Contains(dAtkMove.Name) Then dAtkMove.DmgType = _DmgType.Physical
        If FOptions.WonderRoom Then
            dAtkMove.DmgType = IIf(dAtkMove.DmgType = _DmgType.Physical, _DmgType.Special, _DmgType.Physical)
        End If

        Dim DStat As Single, DMod As Single
        Dim _TmpDefender As Pokemon = Defender
        If _DefStat <> 0 Then
            If LCase(_AtkMove.Name) = "chip away" Then
                If _TmpDefender.Boosts.Def > 0 Then _TmpDefender.Boosts.Def = 0
                If _TmpDefender.Boosts.SpDef > 0 Then _TmpDefender.Boosts.SpDef = 0
            End If
            DStat = _DefStat
        Else
            DStat = IIf(dAtkMove.DmgType = _DmgType.Physical, _
                     CalcPokeStats(_TmpDefender, Attacker.Ability = 109, FOptions.CriticalHit).Def, _
                    IIf(dAtkMove.DmgType = _DmgType.Special, _
                        CalcPokeStats(_TmpDefender, Attacker.Ability = 109, FOptions.CriticalHit).SpDef, 1))
        End If
        'DMod
        If Defender.Data.Name = "Ditto" And Defender.Item = "Metal Powder" Then
            DMod = 1.5
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Physical And Defender.Ability = 63 And Defender.Status <> _Status.None Then
            DMod = 1.5
            Defender.AbilityEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And _
            (Defender.Data.Type1 = 6 Or Defender.Data.Type2 = 6) _
            And FOptions.Weather = _Weather.Sandstorm Then
            DMod = 1.5
        ElseIf dAtkMove.DmgType = _DmgType.Special And _
            Defender.Item = "Soul Dew" And _
            (Defender.Data.Name = "Latias" Or Defender.Data.Name = "Latios") Then
            DMod = 1.5
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And Defender.Item = "Deepseascale" _
            And Defender.Data.Name = "Clamperl" Then
            DMod = 2
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And Defender.Ability = 122 _
            And FOptions.Weather = _Weather.Sunshine Then
            DMod = 1.5
            Defender.AbilityEff = True
        Else
            DMod = 1
        End If
        'EvoStone
        Dim ES As Single = IIf(Defender.Item = "Eviolite", 1.5, 1)
        If ES <> 1 Then Defender.ItemEff = True
        DefStat = Math.Floor(Math.Floor(DStat * DMod) * ES)

        'Mod1
        'Mod1 = BRN × RL × TVT × SR × FF
        Dim BRN As Single, RL As Single, TVT As Single, SR As Single, FF As Single
        BRN = IIf((Attacker.Status = _Status.Burn And _AtkMove.DmgType = _DmgType.Physical) _
                  And Attacker.Ability <> 62, 0.5, 1)
        If FOptions.CriticalHit Or Attacker.Ability = 151 Or _AtkMove.Name = "Brick Break" Then
            RL = 1
        ElseIf dAtkMove.DmgType = _DmgType.Physical And FOptions.Reflect Then
            RL = IIf(FOptions.Doubles, 2 / 3, 0.5)
        ElseIf dAtkMove.DmgType = _DmgType.Special And FOptions.LightScreen Then
            RL = IIf(FOptions.Doubles, 2 / 3, 0.5)
        Else
            RL = 1
        End If
        TVT = IIf(FOptions.Doubles And (_AtkMove.Target = 3 Or _AtkMove.Target = 4), 0.75, 1)
        If FOptions.Weather = _Weather.Sunshine Then
            SR = IIf(_AtkMove.Type = 10, 1.5, IIf(_AtkMove.Type = 11, 0.5, 1))
        ElseIf FOptions.Weather = _Weather.Rain Then
            SR = IIf(_AtkMove.Type = 11, 1.5, IIf(_AtkMove.Type = 10, 0.5, 1))
        Else
            SR = 1
        End If
        FF = IIf(FOptions.FlashFire And Attacker.Ability = 18 And _AtkMove.Type = 10, 1.5, 1)
        If FF <> 1 Then Attacker.AbilityEff = True

        Mod1 = BRN * RL * TVT * SR * FF

        'Mod2
        Mod2 = 1
        If Attacker.Item = "Life Orb" Then Mod2 = Mod2 * 1.3
        If Attacker.Item = "Metronome" Then Mod2 = Mod2 * (1 + (FOptions.Metronome * 0.1))
        If Mod2 <> 1 Then Attacker.ItemEff = True
        If FOptions.MeFirst Then Mod2 = Mod2 * 1.5

        'Mod3
        'Mod3 = SRF × EB × TL × TRB
        Dim SRF As Single, EB As Single, TL As Single, TRB As Single, MSC As Single
        SRF = IIf((Defender.Ability = 116 Or Defender.Ability = 111) And TypeMatch > 1, 0.75, 1)
        EB = IIf(Attacker.Item = "Expert Belt" And TypeMatch > 1, 1.2, 1)
        TL = IIf(Attacker.Ability = 110 And TypeMatch < 1, 2, 1)
        If _AtkMove.Type = 1 And Defender.Item = "Chilan Berry" Then
            TRB = 0.5
        ElseIf TypeMatch > 1 And Defender.Item = arrTypeItems(_AtkMove.Type, 0) Then
            TRB = 0.5
        Else
            TRB = 1
        End If

        If EB <> 1 Then Attacker.ItemEff = True
        If TRB < 1 Then Defender.ItemEff = True

        MSC = IIf(Defender.Ability = 136 And Defender.HPPercent = 100, 0.5, 1)
        If MSC < 1 Then Defender.AbilityEff = True

        Mod3 = SRF * EB * TL * TRB * MSC

        CH = IIf(FOptions.CriticalHit, IIf(Attacker.Ability = 97, 3, 2), 1)
        If CH > 2 Then Attacker.AbilityEff = True

        STAB = IIf(Attacker.Data.Type1 = _AtkMove.Type Or Attacker.Data.Type2 = _AtkMove.Type, _
                 IIf(Attacker.Ability = 91, 2, 1.5), 1)
        If STAB > 1.5 Then Attacker.AbilityEff = True

        If AtkStat = 0 Or DefStat = 0 Then Return 0

        Dim iResult As Integer = 0
        iResult = Math.Floor(Attacker.Level * 2 / 5) + 2
        iResult = Math.Floor(iResult * BasePower * AtkStat / 50)
        iResult = Math.Floor(iResult / DefStat)
        iResult = Math.Floor(iResult * Mod1) + 2
        iResult = Math.Floor(iResult * CH * Mod2 * Random / 100)
        iResult = Math.Floor(iResult * STAB * TypeMatch * Mod3)

        If (Attacker.Ability = 104 Or Attacker.Ability = 163 Or Attacker.Ability = 164) Then Defender.Ability = DefAbility

        Return iResult
    End Function

    Public Function CalcAll_Damage(ByVal Attacker As Pokemon, ByVal Defender As Pokemon, _
                            ByVal AtkMove As Move, _
                            ByVal Weather As _Weather, Optional ByVal _AtkStat As Integer = 0, Optional ByVal _DefStat As Integer = 0) As Collection

        Dim colResult As New Collection

        'Damage Formula = (((((((Level × 2 ÷ 5) + 2) × BasePower × [Sp]Atk ÷ 50) ÷ [Sp]Def) × Mod1) + 2) × 
        '         CH × Mod2 × R ÷ 100) × STAB × Type1 × Type2 × Mod3
        Attacker.AbilityEff = False
        Attacker.ItemEff = False
        Defender.AbilityEff = False
        Defender.ItemEff = False

        Dim DefAbility As Integer : If (Attacker.Ability = 104 Or Attacker.Ability = 163 Or Attacker.Ability = 164) Then DefAbility = Defender.Ability : Defender.Ability = 0

        Dim BasePower As Single
        Dim AtkStat As Single
        Dim DefStat As Single
        Dim CH As Single
        Dim STAB As Single
        Dim Mod1 As Single
        Dim Mod2 As Single
        Dim Mod3 As Single
        Dim FOptions As Field_Effects = GetFOptions(Attacker, Defender)
        Dim _AtkMove As Move = GetMovePower(Attacker, Defender, AtkMove, FOptions)
        If _AtkMove.Power = 0 Then GoTo NullResult

        Dim TypeMatch As Single = GetTypeMatch(Attacker, _AtkMove, Defender, FOptions)

        'Base Power
        Dim HH As Single, BP As Single, IT As Single, CHG As Single, MS As Single, WS As Single, UA As Single, FA As Single
        HH = IIf(FOptions.HelpingHand, 1.5, 1)
        BP = _AtkMove.Power

        ' IT
        'Type boosting items & plates
        If Attacker.Item = arrTypeItems(_AtkMove.Type, 1) Or Attacker.Item = arrTypeItems(_AtkMove.Type, 2) Then
            IT = 1.2
        ElseIf Attacker.Item = arrTypeItems(_AtkMove.Type, 3) Then
            IT = 1.5
        Else
            Select Case Attacker.Item
                Case "Muscle Band"
                    IT = IIf(_AtkMove.DmgType = _DmgType.Physical, 1.1, 1)
                Case "Wise Glasses"
                    IT = IIf(_AtkMove.DmgType = _DmgType.Special, 1.1, 1)
                Case "Adamant Orb"
                    IT = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 9) And _
                           Attacker.Data.Name = "Dialga", 1.2, 1)
                Case "Lustrous Orb"
                    IT = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 11) And _
                            Attacker.Data.Name = "Palkia", 1.2, 1)
                Case "Griseous Orb"
                    IT = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 8) And _
                            Attacker.Data.Name = "Giratina-O", 1.2, 1)
                Case Else
                    IT = 1
            End Select
        End If
        If IT > 1 Then Attacker.ItemEff = True

        CHG = IIf(FOptions.Charge And _AtkMove.Type = 13, 2, 1)
        MS = IIf(FOptions.MudSport And _AtkMove.Type = 13, 0.5, 1)
        WS = IIf(FOptions.WaterSport And _AtkMove.Type = 10, 0.5, 1)
        ' UA
        Select Case Attacker.Ability
            Case 79
                UA = IIf(Attacker.Gender = Defender.Gender, 1.25, _
                       IIf((Attacker.Gender = _Gender.Male And Defender.Gender = _Gender.Female) _
                           Or (Attacker.Gender = _Gender.Female And Defender.Gender = _Gender.Male), _
                           0.75, 1))
            Case 120
                UA = IIf(colRecoilMoves.Contains(_AtkMove.Name), 1.2, 1)
            Case 89
                UA = IIf(colPunchMoves.Contains(_AtkMove.Name), 1.2, 1)
            Case 66
                UA = IIf(_AtkMove.Type = 10 And Attacker.HPPercent <= 33, 1.5, 1)
            Case 65
                UA = IIf(_AtkMove.Type = 12 And Attacker.HPPercent <= 33, 1.5, 1)
            Case 67
                UA = IIf(_AtkMove.Type = 11 And Attacker.HPPercent <= 33, 1.5, 1)
            Case 68
                UA = IIf(_AtkMove.Type = 7 And Attacker.HPPercent <= 33, 1.5, 1)
            Case 101
                UA = IIf(_AtkMove.Power <= 60, 1.5, 1)
            Case 125
                UA = IIf(_AtkMove.EffPercent > 0 And _AtkMove.EffPercent < 1, 1.3, 1)
            Case 159
                UA = IIf((_AtkMove.Type = 5 Or _AtkMove.Type = 6 Or _AtkMove.Type = 9) And _
                         FOptions.Weather = _Weather.Sandstorm, 1.3, 1)
            Case Else
                UA = 1
        End Select
        If UA <> 1 Then Attacker.AbilityEff = True

        ' FA
        Select Case Defender.Ability
            Case 47
                FA = IIf(_AtkMove.Type = 10 Or _AtkMove.Type = 15, 0.5, 1)
            Case 85
                FA = IIf(_AtkMove.Type = 10, 0.5, 1)
            Case 87
                FA = IIf(_AtkMove.Type = 10, 1.25, 1)
            Case Else
                FA = 1
        End Select
        If FA <> 1 Then Defender.AbilityEff = True

        BasePower = Math.Floor(Math.Floor(Math.Floor(Math.Floor(Math.Floor(Math.Floor(Math.Floor( _
                    HH * BP) * IT) * CHG) * MS) * WS) * UA) * FA)

        'Atk Stat
        Dim _TmpAttacker As Pokemon = IIf(LCase(_AtkMove.Name) = "foul play", Defender, Attacker)
        Dim AStat As Integer, AM As Single, IM As Single
        AStat = IIf(_AtkMove.DmgType = _DmgType.Physical, _
                 CalcPokeStats(_TmpAttacker, Defender.Ability = 109, FOptions.CriticalHit).Atk, _
                IIf(_AtkMove.DmgType = _DmgType.Special, _
                    CalcPokeStats(_TmpAttacker, Defender.Ability = 109, FOptions.CriticalHit).SpAtk, 1))
        If _AtkStat <> 0 Then AStat = _AtkStat

        Select Case _TmpAttacker.Ability
            Case 74
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical, 2, 1)
            Case 37
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical, 2, 1)
            Case 122
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical And FOptions.Weather = _Weather.Sunshine, 1.5, 1)
            Case 62
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.Status <> _Status.None, 1.5, 1)
            Case 55
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical, 1.5, 1)
            Case 112
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.AbilityEff, 0.5, 1)
            Case 57
                AM = IIf(_AtkMove.DmgType = _DmgType.Special And _TmpAttacker.AbilityEff, 1.5, 1)
            Case 58
                AM = IIf(_AtkMove.DmgType = _DmgType.Special And _TmpAttacker.AbilityEff, 1.5, 1)
            Case 94
                AM = IIf(_AtkMove.DmgType = _DmgType.Special And FOptions.Weather = _Weather.Sunshine, 1.5, 1)
            Case 137
                AM = IIf(_AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.Status = _Status.Poison, 1.5, 1)
            Case 138
                AM = IIf(_AtkMove.DmgType = _DmgType.Special And _TmpAttacker.Status = _Status.Burn, 1.5, 1)
            Case 129
                AM = IIf(_TmpAttacker.HPPercent < 50, 0.5, 1)
            Case Else
                AM = 1
        End Select
        If AM <> 1 Then Attacker.AbilityEff = True

        'IM
        Select Case Attacker.Item
            Case "Choice Band"
                IM = IIf(_AtkMove.DmgType = _DmgType.Physical, 1.5, 1)
            Case "Light Ball"
                IM = IIf(Attacker.Data.Name = "Pikachu", 2, 1)
            Case "Thick Club"
                IM = IIf(_AtkMove.DmgType = _DmgType.Physical And _
                       (Attacker.Data.Name = "Cubone" Or Attacker.Data.Name = "Marowak"), _
                       2, 1)
            Case "Choice Specs"
                IM = IIf(_AtkMove.DmgType = _DmgType.Special, 1.5, 1)
            Case "Soul Dew"
                IM = IIf(_AtkMove.DmgType = _DmgType.Special And _
                       (Attacker.Data.Name = "Latias" Or Attacker.Data.Name = "Latios"), _
                           1.5, 1)
            Case "Deepseatooth"
                IM = IIf(_AtkMove.DmgType = _DmgType.Special And Attacker.Data.Name = "Clamperl", _
                       2, 1)
            Case Else
                IM = 1
        End Select
        If IM <> 1 Then Attacker.ItemEff = True

        AtkStat = Math.Floor(Math.Floor(AStat * AM) * IM)

        'DefStat
        Dim dAtkMove As Move = _AtkMove 'Move that the defender takes
        If colSpHitDefMoves.Contains(dAtkMove.Name) Then dAtkMove.DmgType = _DmgType.Physical
        If FOptions.WonderRoom Then
            dAtkMove.DmgType = IIf(dAtkMove.DmgType = _DmgType.Physical, _DmgType.Special, _DmgType.Physical)
        End If

        Dim DStat As Single, DMod As Single
        Dim _TmpDefender As Pokemon = Defender
        If _DefStat <> 0 Then
            If LCase(_AtkMove.Name) = "chip away" Then
                If _TmpDefender.Boosts.Def > 0 Then _TmpDefender.Boosts.Def = 0
                If _TmpDefender.Boosts.SpDef > 0 Then _TmpDefender.Boosts.SpDef = 0
            End If
            DStat = _DefStat
        Else
            DStat = IIf(dAtkMove.DmgType = _DmgType.Physical, _
                     CalcPokeStats(_TmpDefender, Attacker.Ability = 109, FOptions.CriticalHit).Def, _
                    IIf(dAtkMove.DmgType = _DmgType.Special, _
                        CalcPokeStats(_TmpDefender, Attacker.Ability = 109, FOptions.CriticalHit).SpDef, 1))
        End If
        'DMod
        If Defender.Data.Name = "Ditto" And Defender.Item = "Metal Powder" Then
            DMod = 1.5
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Physical And Defender.Ability = 63 And Defender.Status <> _Status.None Then
            DMod = 1.5
            Defender.AbilityEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And _
            (Defender.Data.Type1 = 6 Or Defender.Data.Type2 = 6) _
            And FOptions.Weather = _Weather.Sandstorm Then
            DMod = 1.5
        ElseIf dAtkMove.DmgType = _DmgType.Special And _
            Defender.Item = "Soul Dew" And _
            (Defender.Data.Name = "Latias" Or Defender.Data.Name = "Latios") Then
            DMod = 1.5
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And Defender.Item = "Deepseascale" _
            And Defender.Data.Name = "Clamperl" Then
            DMod = 2
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And Defender.Ability = 122 _
            And FOptions.Weather = _Weather.Sunshine Then
            DMod = 1.5
            Defender.AbilityEff = True
        Else
            DMod = 1
        End If
        'EvoStone
        Dim ES As Single = IIf(Defender.Item = "Eviolite", 1.5, 1)
        If ES <> 1 Then Defender.ItemEff = True
        DefStat = Math.Floor(Math.Floor(DStat * DMod) * ES)

        'Mod1
        'Mod1 = BRN × RL × TVT × SR × FF
        Dim BRN As Single, RL As Single, TVT As Single, SR As Single, FF As Single
        BRN = IIf((Attacker.Status = _Status.Burn And _AtkMove.DmgType = _DmgType.Physical) _
                  And Attacker.Ability <> 62, 0.5, 1)
        If FOptions.CriticalHit Or Attacker.Ability = 151 Or _AtkMove.Name = "Brick Break" Then
            RL = 1
        ElseIf dAtkMove.DmgType = _DmgType.Physical And FOptions.Reflect Then
            RL = IIf(FOptions.Doubles, 2 / 3, 0.5)
        ElseIf dAtkMove.DmgType = _DmgType.Special And FOptions.LightScreen Then
            RL = IIf(FOptions.Doubles, 2 / 3, 0.5)
        Else
            RL = 1
        End If
        TVT = IIf(FOptions.Doubles And (_AtkMove.Target = 3 Or _AtkMove.Target = 4), 0.75, 1)
        If FOptions.Weather = _Weather.Sunshine Then
            SR = IIf(_AtkMove.Type = 10, 1.5, IIf(_AtkMove.Type = 11, 0.5, 1))
        ElseIf FOptions.Weather = _Weather.Rain Then
            SR = IIf(_AtkMove.Type = 11, 1.5, IIf(_AtkMove.Type = 10, 0.5, 1))
        Else
            SR = 1
        End If
        FF = IIf(FOptions.FlashFire And Attacker.Ability = 18 And _AtkMove.Type = 10, 1.5, 1)
        If FF <> 1 Then Attacker.AbilityEff = True

        Mod1 = BRN * RL * TVT * SR * FF

        'Mod2
        Mod2 = 1
        If Attacker.Item = "Life Orb" Then Mod2 = Mod2 * 1.3
        If Attacker.Item = "Metronome" Then Mod2 = Mod2 * (1 + (FOptions.Metronome * 0.1))
        If Mod2 <> 1 Then Attacker.ItemEff = True
        If FOptions.MeFirst Then Mod2 = Mod2 * 1.5

        'Mod3
        'Mod3 = SRF × EB × TL × TRB
        Dim SRF As Single, EB As Single, TL As Single, TRB As Single, MSC As Single
        SRF = IIf((Defender.Ability = 116 Or Defender.Ability = 111) And TypeMatch > 1, 0.75, 1)
        EB = IIf(Attacker.Item = "Expert Belt" And TypeMatch > 1, 1.2, 1)
        TL = IIf(Attacker.Ability = 110 And TypeMatch < 1, 2, 1)
        If _AtkMove.Type = 1 And Defender.Item = "Chilan Berry" Then
            TRB = 0.5
        ElseIf TypeMatch > 1 And Defender.Item = arrTypeItems(_AtkMove.Type, 0) Then
            TRB = 0.5
        Else
            TRB = 1
        End If

        If EB <> 1 Then Attacker.ItemEff = True
        If TRB < 1 Then Defender.ItemEff = True

        MSC = IIf(Defender.Ability = 136 And Defender.HPPercent = 100, 0.5, 1)
        If MSC < 1 Then Defender.AbilityEff = True

        Mod3 = SRF * EB * TL * TRB * MSC

        CH = IIf(FOptions.CriticalHit, IIf(Attacker.Ability = 97, 3, 2), 1)
        If CH > 2 Then Attacker.AbilityEff = True

        STAB = IIf(Attacker.Data.Type1 = _AtkMove.Type Or Attacker.Data.Type2 = _AtkMove.Type, _
                 IIf(Attacker.Ability = 91, 2, 1.5), 1)
        If STAB > 1.5 Then Attacker.AbilityEff = True

        If AtkStat = 0 Or DefStat = 0 Then GoTo NullResult

        Dim iResult As Integer = 0
        iResult = Math.Floor(Attacker.Level * 2 / 5) + 2
        iResult = Math.Floor(iResult * BasePower * AtkStat / 50)
        iResult = Math.Floor(iResult / DefStat)
        iResult = Math.Floor(iResult * Mod1) + 2

        For Random As Byte = 85 To 100
            Dim iResult2 As Integer

            iResult2 = Math.Floor(iResult * CH * Mod2 * Random / 100)
            iResult2 = Math.Floor(iResult2 * STAB * TypeMatch * Mod3)

            colResult.Add(iResult2)
        Next

        If (Attacker.Ability = 104 Or Attacker.Ability = 163 Or Attacker.Ability = 164) Then Defender.Ability = DefAbility

        Return colResult

NullResult:
        If (Attacker.Ability = 104 Or Attacker.Ability = 163 Or Attacker.Ability = 164) Then Defender.Ability = DefAbility
        For i As Byte = 0 To 15 : colResult.Add(0) : Next : Return colResult
    End Function

    Private Function CalcStat(ByVal BaseStat As Byte, ByVal Level As Byte, ByVal IV As Byte, ByVal EV As Byte, _
                              Optional ByVal Nature As Byte = 0, Optional ByVal HP As Boolean = False, _
                              Optional ByVal StatChange As SByte = 0) As Short
        If BaseStat = 0 Then Return 0
        If BaseStat = 1 Then Return 1

        Dim _Nature As Single

        Select Case Nature
            Case 0 : _Nature = 1
            Case 1 : _Nature = 0.9
            Case 2 : _Nature = 1.1
        End Select

        Try
            Dim Result As Single

            If HP = False Then
                Result = Math.Floor(Math.Floor(((IV + (2 * BaseStat) + EV / 4) * Level) / 100 + 5) * _Nature)

                Select Case StatChange
                    Case 6
                        Result = Result / 2 * 8
                    Case 5
                        Result = Result / 2 * 7
                    Case 4
                        Result = Result / 2 * 6
                    Case 3
                        Result = Result / 2 * 5
                    Case 2
                        Result = Result / 2 * 4
                    Case 1
                        Result = Result / 2 * 3
                    Case 0
                        Result = Result / 2 * 2
                    Case -1
                        Result = Result / 3 * 2
                    Case -2
                        Result = Result / 4 * 2
                    Case -3
                        Result = Result / 5 * 2
                    Case -4
                        Result = Result / 6 * 2
                    Case -5
                        Result = Result / 7 * 2
                    Case -6
                        Result = Result / 8 * 2
                End Select

            Else
                Result = Math.Floor(((IV + (2 * BaseStat) + EV / 4 + 100) * Level) / 100 + 10)
            End If

            Return Math.Floor(Result)
        Catch ex As Exception
            If Not ex.Equals(IntPtr.Zero) Then
                Return 1
            End If
        End Try
        Return 1
    End Function

    Public Function CalcPokeStats(ByVal srcPoke As Pokemon, Optional ByVal Unaware As Boolean = False, Optional Critical As Boolean = False) As StatsData
        Dim dResult As New StatsData

        If srcPoke.Data Is Nothing Then
            Return dResult
        End If

        Dim dBoosts As StatsData = IIf(Unaware, New StatsData, srcPoke.Boosts)
        If (Not Unaware) And Critical Then
            With dBoosts
                If .Atk < 0 Then .Atk = 0
                If .Def > 0 Then .Def = 0
                If .SpAtk < 0 Then .SpAtk = 0
                If .SpDef > 0 Then .SpDef = 0
            End With
        End If
        With dResult
            .HP = CalcStat(srcPoke.Data.BaseStats.HP, srcPoke.Level, srcPoke.IVs.HP, srcPoke.EVs.HP, _
                         srcPoke.Nature.HP, True, dBoosts.HP)
            .Atk = CalcStat(srcPoke.Data.BaseStats.Atk, srcPoke.Level, srcPoke.IVs.Atk, srcPoke.EVs.Atk, _
                        srcPoke.Nature.Atk, False, dBoosts.Atk)
            .Def = CalcStat(srcPoke.Data.BaseStats.Def, srcPoke.Level, srcPoke.IVs.Def, srcPoke.EVs.Def, _
                          srcPoke.Nature.Def, False, dBoosts.Def)
            .SpAtk = CalcStat(srcPoke.Data.BaseStats.SpAtk, srcPoke.Level, srcPoke.IVs.SpAtk, srcPoke.EVs.SpAtk, _
                          srcPoke.Nature.SpAtk, False, dBoosts.SpAtk)
            .SpDef = CalcStat(srcPoke.Data.BaseStats.SpDef, srcPoke.Level, srcPoke.IVs.SpDef, srcPoke.EVs.SpDef, _
                         srcPoke.Nature.SpDef, False, dBoosts.SpDef)
            .Speed = CalcStat(srcPoke.Data.BaseStats.Speed, srcPoke.Level, srcPoke.IVs.Speed, srcPoke.EVs.Speed, _
                         srcPoke.Nature.Speed, False, dBoosts.Speed)
        End With
        Return dResult
    End Function

#Region "Specific moves"
    Private Function CalcWaterSpoutEruption(ByVal pkAttacker As Pokemon) As Short
        Return Math.Round(pkAttacker.HPPercent / 100) * 150
    End Function

    Private Function CalcPokeWeight(ByVal pkData As Pokemon) As Short
        If pkData.Item = "Float Stone" Then
            Return Math.Floor(pkData.Data.Weight / 2)
        Else
            Return pkData.Data.Weight
        End If
    End Function

    Private Function GKLowKick(ByVal pkDefender As Pokemon) As Short
        Dim intWeight As Short = CalcPokeWeight(pkDefender)
        If intWeight <= 10 Then
            Return 20
        ElseIf intWeight <= 25 Then
            Return 40
        ElseIf intWeight <= 50 Then
            Return 60
        ElseIf intWeight <= 100 Then
            Return 80
        ElseIf intWeight <= 200 Then
            Return 100
        Else
            Return 120
        End If
    End Function

    Private Function WeatherSpeedBoost(ByVal pkPoke As Pokemon, ByVal FOptions As Field_Effects) As Short
        If pkPoke.Ability = 33 And FOptions.Weather = _Weather.Rain Then
            Return 2
        ElseIf pkPoke.Ability = 34 And FOptions.Weather = _Weather.Sunshine Then
            Return 2
        ElseIf pkPoke.Ability = 146 And FOptions.Weather = _Weather.Sandstorm Then
            Return 2
        Else
            Return 1
        End If
    End Function

    Private Function CalcPokeSpeed(ByVal pkAttacker As Pokemon, ByVal pkDefender As Pokemon, ByVal fOptions As Field_Effects, Optional ByVal CalcAttacker As Boolean = True) As Short
        Dim pkData As Pokemon = IIf(CalcAttacker, pkAttacker, pkDefender)
        Dim intBase As Short = CalcPokeStats(pkData, (pkAttacker.Ability = 109 Or pkDefender.Ability = 109)).Speed
        Dim aMod1 As Single = IIf(pkData.Item = "Choice Scarf", 1.5, 1)
        Dim aMod2 As Single = IIf(colSpeedLowerItems.Contains(pkData.Item), 0.5, 1)
        Dim aMod3 As Single = IIf(pkData.Status = _Status.Paralysis, 0.25, 1)
        Dim aMod4 As Single = WeatherSpeedBoost(pkData, fOptions)
        Return Math.Floor(Math.Floor(Math.Floor(intBase * aMod3) * aMod1 * aMod2) * aMod4)
    End Function

    Private Function GyroBall(ByVal pkAttacker As Pokemon, ByVal pkDefender As Pokemon, ByVal FOptions As Field_Effects) As Short
        '1 + floor(25 * Target's Speed / User's Speed)
        Dim iPower As Short
        iPower = 1 + Math.Floor(25 * CalcPokeSpeed(pkAttacker, pkDefender, FOptions, False) _
                                / CalcPokeSpeed(pkAttacker, pkDefender, FOptions, True))
        Return IIf(iPower > 150, 150, iPower)
    End Function

    Private Function ElectroBall(ByVal pkAttacker As Pokemon, ByVal pkDefender As Pokemon, ByVal FOptions As Field_Effects) As Short
        Dim dSpeed As Short = CalcPokeSpeed(pkAttacker, pkDefender, FOptions, False)
        Dim aSpeed As Short = CalcPokeSpeed(pkAttacker, pkDefender, FOptions, True)
        If (dSpeed / aSpeed) <= (1 / 4) Then
            Return 150
        ElseIf (dSpeed / aSpeed) <= (1 / 3) Then
            Return 120
        ElseIf (dSpeed / aSpeed) <= (1 / 2) Then
            Return 80
        Else
            Return 60
        End If
    End Function

    Private Function CalcHeatCrashHeavySlam(ByVal pkAttacker As Pokemon, ByVal pkDefender As Pokemon) As Short
        Dim aWeight As Short = CalcPokeWeight(pkAttacker)
        Dim dWeight As Short = CalcPokeWeight(pkDefender)
        If (dWeight / aWeight) <= (1 / 5) Then
            Return 120
        ElseIf (dWeight / aWeight) <= (1 / 4) Then
            Return 100
        ElseIf (dWeight / aWeight) <= (1 / 3) Then
            Return 80
        ElseIf (dWeight / aWeight) <= (1 / 2) Then
            Return 60
        Else
            Return 40
        End If
    End Function

    Public Function CalcHP(ByVal HP As Byte, ByVal Atk As Byte, ByVal Def As Byte, ByVal Spd As Byte, ByVal SpAtk As Byte, ByVal SpDef As Byte) As Move
        Dim mResult As New Move
        Try
            Dim sTypes As String() = {"Fighting", "Flying", "Poison", "Ground", "Rock", "Bug", "Ghost", "Steel", "Fire", "Water", "Grass", "Electric", "Psychic", "Ice", "Dragon", "Dark"}
            Dim T1 As Byte, T2 As Byte, T3 As Byte, T4 As Byte, T5 As Byte, T6 As Byte
            T1 = 0 : T2 = 0 : T3 = 0 : T4 = 0 : T5 = 0 : T6 = 0
            If HP Mod 2 = 1 Then T1 = 1
            If Atk Mod 2 = 1 Then T2 = 2
            If Def Mod 2 = 1 Then T3 = 4
            If Spd Mod 2 = 1 Then T4 = 8
            If SpAtk Mod 2 = 1 Then T5 = 16
            If SpDef Mod 2 = 1 Then T6 = 32
            mResult.Name = "Hidden Power " & sTypes(Math.Floor((T1 + T2 + T3 + T4 + T5 + T6) * 15 / 63))
            mResult.Type = FindArray(arrTypes, sTypes(Math.Floor((T1 + T2 + T3 + T4 + T5 + T6) * 15 / 63)))

            T1 = 0 : T2 = 0 : T3 = 0 : T4 = 0 : T5 = 0 : T6 = 0
            If HP Mod 4 = 2 Or HP Mod 4 = 3 Then T1 = 1
            If Atk Mod 4 = 2 Or Atk Mod 4 = 3 Then T2 = 2
            If Def Mod 4 = 2 Or Def Mod 4 = 3 Then T3 = 4
            If Spd Mod 4 = 2 Or Spd Mod 4 = 3 Then T4 = 8
            If SpAtk Mod 4 = 2 Or SpAtk Mod 4 = 3 Then T5 = 16
            If SpDef Mod 4 = 2 Or SpDef Mod 4 = 3 Then T6 = 32
            mResult.Power = Math.Floor((T1 + T2 + T3 + T4 + T5 + T6) * 40 / 63 + 30)

        Catch ex As Exception
            MessageBox.Show("Error trying to retrieve Hidden Power's data", "Error", MessageBoxButton.OK)
        End Try
        Return mResult
    End Function

    Private Function CalcReturn(ByVal pkAttacker As Pokemon) As Short
        Return Math.Max(Math.Floor(pkAttacker.Happiness / 2.5), 1)
    End Function

    Private Function CalcFrustration(ByVal pkAttacker As Pokemon) As Short
        Return Math.Max(Math.Floor((255 - pkAttacker.Happiness) / 2.5), 1)
    End Function

    Private Function CalcStoredPower(ByVal pkAttacker As Pokemon) As Short
        Return 20 + (CalcBoostsTotal(pkAttacker) * 20)
    End Function

    Private Function CalcPunishment(ByVal pkDefender As Pokemon) As Short
        Dim intPower As Integer = 60 + (CalcBoostsTotal(pkDefender) * 20)
        Return IIf(intPower > 200, 200, intPower)
    End Function

    Private Function CalcBoostsTotal(ByVal PokeData As Pokemon) As Short
        With PokeData.Boosts
            Return (IIf(.Atk <= 0, 0, .Atk) + IIf(.Def <= 0, 0, .Def) + IIf(.SpAtk <= 0, 0, .SpAtk) + IIf(.SpDef <= 0, 0, .SpDef) + IIf(.Speed <= 0, 0, .Speed))
        End With
    End Function

    Private Function CalcFlailReversal(ByVal pkAttacker As Pokemon) As Short
        If pkAttacker.HPPercent >= 71 Then
            Return 20
        ElseIf pkAttacker.HPPercent >= 36 Then
            Return 40
        ElseIf pkAttacker.HPPercent >= 21 Then
            Return 80
        ElseIf pkAttacker.HPPercent >= 11 Then
            Return 100
        ElseIf pkAttacker.HPPercent >= 5 Then
            Return 150
        Else
            Return 200
        End If
    End Function

    Public Function GetMovePower(ByVal pkAttacker As Pokemon, ByVal pkDefender As Pokemon, ByVal MoveData As Move, ByVal FOptions As Field_Effects) As Move
        Dim _MoveData As Move = MoveData
        With _MoveData
            Select Case LCase(.Name)
                Case "hidden power"
                    _MoveData = CalcHP(pkAttacker.IVs.HP, pkAttacker.IVs.Atk, pkAttacker.IVs.Def, _
                                     pkAttacker.IVs.Speed, pkAttacker.IVs.SpAtk, pkAttacker.IVs.SpDef)
                Case "grass knot"
                    .Power = GKLowKick(pkDefender)
                    .DmgType = _DmgType.Special
                Case "low kick"
                    .Power = GKLowKick(pkDefender)
                    .DmgType = _DmgType.Physical
                Case "water spout", "eruption"
                    .Power = CalcWaterSpoutEruption(pkAttacker)
                    .DmgType = _DmgType.Special
                Case "gyro ball"
                    .Power = GyroBall(pkAttacker, pkDefender, FOptions)
                    .DmgType = _DmgType.Physical
                Case "return"
                    .Power = CalcReturn(pkAttacker)
                    .DmgType = _DmgType.Physical
                Case "frustration"
                    .Power = CalcFrustration(pkAttacker)
                    .DmgType = _DmgType.Physical
                Case "judgment"
                    If InStr(pkAttacker.Item, "Plate") > 0 Then
                        .DmgType = _DmgType.Special
                        If InStr(LCase(pkAttacker.Data.Name), "arceus") > 0 Then
                            .Type = pkAttacker.Data.Type1
                        End If
                    End If
                Case "acrobatics"
                    If Not colItems.Contains(pkAttacker.Item) Or pkAttacker.Item = "Flying Gem" Then
                        .Power = 110
                    Else
                        .Power = 55
                    End If
                Case "stored power"
                    .Power = CalcStoredPower(pkAttacker)
                Case "punishment"
                    .Power = CalcPunishment(pkDefender)
                Case "facade"
                    .Power = IIf(pkAttacker.Status = _Status.None, 70, 140)
                Case "brine"
                    If pkDefender.HPPercent < 50 Then
                        .Power = 130
                    Else
                        .Power = 65
                    End If
                Case "venoshock"
                    .Power = IIf(pkDefender.Status = _Status.Poison, 130, 65)
                Case "hex"
                    .Power = IIf(pkDefender.Status = _Status.None, 50, 100)
                Case "reversal", "flail"
                    .Power = CalcFlailReversal(pkAttacker)
                Case "wring out", "crush grip"
                    .Power = 120 * (pkDefender.HPPercent / 100)
                Case "electro ball"
                    .Power = ElectroBall(pkAttacker, pkDefender, FOptions)
                Case "heat crash", "heavy slam"
                    .Power = CalcHeatCrashHeavySlam(pkAttacker, pkDefender)
                Case "wake-up slap"
                    .Power = IIf(pkDefender.Status = _Status.Sleep, 120, 60)
                Case "smellingsalt"
                    .Power = IIf(pkDefender.Status = _Status.Paralysis, 120, 60)
                Case "bug bite", "pluck"
                    .Power = IIf(InStr(pkDefender.Item, "Berry") <> 0, 120, 60)
                Case "solarbeam"
                    .Power = IIf((FOptions.Weather <> _Weather.Sunshine) And (FOptions.Weather <> _Weather.None), 60, 120)
                Case "weather ball"
                    If FOptions.Weather = _Weather.None Then
                        .Power = 50
                        .Type = 1
                    Else
                        .Power = 100
                        Select Case FOptions.Weather
                            Case _Weather.Rain
                                .Type = 11
                            Case _Weather.Sunshine
                                .Type = 10
                            Case _Weather.Hail
                                .Type = 15
                            Case _Weather.Sandstorm
                                .Type = 6
                            Case Else

                        End Select
                    End If
            End Select
        End With
        Return _MoveData
    End Function
#End Region

#Region "Other"
    Public Function GetFOptions(ByVal pkAttacker As Pokemon, ByVal pkDefender As Pokemon) As Field_Effects
        Dim fData As Field_Effects
        With fData
            .CriticalHit = (pkAttacker.fOptions.CriticalHit Or pkDefender.fOptions.CriticalHit)
            .Doubles = (pkAttacker.fOptions.Doubles Or pkDefender.fOptions.Doubles)
            .NoImmunity = (pkAttacker.fOptions.NoImmunity Or pkDefender.fOptions.NoImmunity)

            .FlashFire = pkAttacker.fOptions.FlashFire
            .MeFirst = pkAttacker.fOptions.MeFirst
            .HelpingHand = pkAttacker.fOptions.HelpingHand
            .Charge = pkAttacker.fOptions.Charge
            .Metronome = pkAttacker.fOptions.Metronome

            .Reflect = pkDefender.fOptions.Reflect
            .LightScreen = pkDefender.fOptions.LightScreen
            .MudSport = pkDefender.fOptions.MudSport
            .WaterSport = pkDefender.fOptions.WaterSport
            .Hazards.Spikes = pkDefender.fOptions.Hazards.Spikes
            .Hazards.StealthRock = pkDefender.fOptions.Hazards.StealthRock

            .Weather = IIf(pkAttacker.fOptions.Weather = _Weather.None, pkDefender.fOptions.Weather, pkAttacker.fOptions.Weather)
        End With
        Return fData
    End Function
#End Region

End Module
