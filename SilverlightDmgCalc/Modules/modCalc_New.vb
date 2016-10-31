Module modCalc_New
    Private Function ApplyMod(base_value As Integer, modifier As Integer) As Integer
        Return Math.Round((base_value * modifier) / &H1000 - 0.01)
    End Function

    Private Function ChainMod(first_mod As Integer, second_mod As Integer) As Integer
        If second_mod = &H1000 Or second_mod = 0 Then
            Return first_mod
        Else
            Return ((first_mod * second_mod) + &H800 >> 12)
        End If
    End Function

    Public Function Calc_Damage_New(ByVal Attacker As Pokemon, ByVal Defender As Pokemon, _
                            ByVal AtkMove As Move, _
                            ByVal Random As Byte, _
                            ByVal Weather As _Weather, Optional ByVal _AtkStat As Integer = 0, Optional ByVal _DefStat As Integer = 0) As Integer
        Dim FOptions As Field_Effects = GetFOptions(Attacker, Defender)

        Dim _AtkMove As Move = GetMovePower_New(Attacker, Defender, AtkMove, FOptions)

        Dim dAtkMove As Move = _AtkMove 'Move that the defender takes
        If colSpHitDefMoves.Contains(dAtkMove.Name) Then dAtkMove.DmgType = _DmgType.Physical
        If FOptions.WonderRoom Then
            dAtkMove.DmgType = IIf(dAtkMove.DmgType = _DmgType.Physical, _DmgType.Special, _DmgType.Physical)
        End If
        'TODO: If _AtkMove.Power = 0 Then Return 1 (?)
        Dim _TmpDefender As Pokemon = Defender
        If LCase(_AtkMove.Name) = "chip away" Then
            If _TmpDefender.Boosts.Def > 0 Then _TmpDefender.Boosts.Def = 0
            If _TmpDefender.Boosts.SpDef > 0 Then _TmpDefender.Boosts.SpDef = 0
        End If
        Dim DStat As Single
        If _DefStat <> 0 Then
            DStat = _DefStat
        Else
            DStat = IIf(dAtkMove.DmgType = _DmgType.Physical, _
                     CalcPokeStats(_TmpDefender, Attacker.Ability = 109, FOptions.CriticalHit).Def, _
                    IIf(dAtkMove.DmgType = _DmgType.Special, _
                        CalcPokeStats(_TmpDefender, Attacker.Ability = 109, FOptions.CriticalHit).SpDef, 1))
        End If

        Dim _TmpAttacker As Pokemon = IIf(LCase(_AtkMove.Name) = "foul play", Defender, Attacker)

        Dim DefAbility As Integer : If (Attacker.Ability = 104 Or Attacker.Ability = 163 Or Attacker.Ability = 164) Then DefAbility = Defender.Ability : Defender.Ability = 0

        Dim AStat As Integer
        If _AtkStat <> 0 Then
            AStat = _AtkStat
        Else
            AStat = IIf(_AtkMove.DmgType = _DmgType.Physical, _
         CalcPokeStats(_TmpAttacker, Defender.Ability = 109, FOptions.CriticalHit).Atk, _
        IIf(_AtkMove.DmgType = _DmgType.Special, _
            CalcPokeStats(_TmpAttacker, Defender.Ability = 109, FOptions.CriticalHit).SpAtk, 1))
        End If

        'Multi target mod
        Dim mod_multi_target As Single = IIf(FOptions.Doubles, &HC00, &H1000)

        'Weather mod
        Dim weather_mod As Single
        If FOptions.Weather = _Weather.Sunshine Then
            weather_mod = IIf(_AtkMove.Type = 10, &H1800, IIf(_AtkMove.Type = 11, &H800, &H1000))
        ElseIf FOptions.Weather = _Weather.Rain Then
            weather_mod = IIf(_AtkMove.Type = 11, &H1800, IIf(_AtkMove.Type = 10, &H800, &H1000))
        Else
            weather_mod = &H1000
        End If

        'STAB
        Dim STAB_mod As Single
        STAB_mod = IIf(Attacker.Data.Type1 = _AtkMove.Type Or Attacker.Data.Type2 = _AtkMove.Type, _
                 IIf(Attacker.Ability = 91, &H2000, &H1800), &H1000)
        If STAB_mod > &H1800 Then Attacker.AbilityEff = True

        'Type effectiveness
        Dim TypeMatch As Single = GetTypeMatch(Attacker, _AtkMove, Defender, FOptions)
        If TypeMatch = 0 Then Return 0

        'Burn
        Dim burn_mod As Single
        burn_mod = IIf((Attacker.Status = _Status.Burn And _AtkMove.DmgType = _DmgType.Physical) _
          And Attacker.Ability <> 62, 0.5, 1)

        'Final modifier
        Dim final_mod As Single = &H1000

        Dim RL As Single
        If FOptions.CriticalHit Or Attacker.Ability = 151 Or _AtkMove.Name = "Brick Break" Then
            RL = &H1000
        ElseIf dAtkMove.DmgType = _DmgType.Physical And FOptions.Reflect Then
            RL = IIf(FOptions.Doubles, &HA8F, &H800)
        ElseIf dAtkMove.DmgType = _DmgType.Special And FOptions.LightScreen Then
            RL = IIf(FOptions.Doubles, &HA8F, &H800)
        Else
            RL = &H1000
        End If

        Dim MSC As Single
        MSC = IIf(Defender.Ability = 136 And Defender.HPPercent = 100, &H800, &H1000)
        If MSC < 1 Then Defender.AbilityEff = True

        Dim TL As Single
        TL = IIf(Attacker.Ability = 110 And TypeMatch < 1, &H2000, &H1000)

        '(Friend Guard-placeholder)
        Dim SN As Single 'Sniper
        SN = IIf(Attacker.Ability = 97 And FOptions.CriticalHit, &H1800, &H1000)

        Dim SRF As Single
        SRF = IIf((Defender.Ability = 116 Or Defender.Ability = 111) And TypeMatch > 1, &HC00, &H1000)

        Dim MET As Single
        MET = IIf(Attacker.Item <> "Metronome", &H1000, IIf(FOptions.Metronome <= 4, &H1000 + FOptions.Metronome * &H333, &H2000))

        Dim EB As Single
        EB = IIf(Attacker.Item = "Expert Belt" And TypeMatch > 1, &H1333, &H1000)

        Dim LO As Single
        LO = IIf(Attacker.Item = "Life Orb", &H14CC, &H1000)

        Dim TRB As Single
        If _AtkMove.Type = 1 And Defender.Item = "Chilan Berry" Then
            TRB = &H800
        ElseIf TypeMatch > 1 And Defender.Item = arrTypeItems(_AtkMove.Type, 0) Then
            TRB = &H800
        Else
            TRB = &H1000
        End If

        final_mod = ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(RL, MSC), TL), SN), SRF), MET), EB), LO), TRB)

        'Base power modifier
        Dim BP_mod As Single = &H1000

        Dim att_abi_BPmod As Single = &H1000

        Select Case _TmpAttacker.Ability
            Case 101 'Technician
                att_abi_BPmod = IIf(_AtkMove.Power <= 60, &H1800, &H1000)
            Case 138 'Flare Boost
                att_abi_BPmod = IIf(_AtkMove.DmgType = _DmgType.Special And _TmpAttacker.Status = _Status.Burn, &H1800, &H1000)
                'Case 165 (Placeholder)
            Case 120 'Reckless
                att_abi_BPmod = IIf(colRecoilMoves.Contains(_AtkMove.Name), &H1333, &H1000)
            Case 89 'Iron Fist
                att_abi_BPmod = IIf(colPunchMoves.Contains(_AtkMove.Name), &H1333, &H1000)
            Case 137 'Toxic Boost
                att_abi_BPmod = IIf(_AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.Status = _Status.Poison, &H1800, &H1000)
            Case 79 'Rivalry
                att_abi_BPmod = IIf(Attacker.Gender = Defender.Gender, &H1400, _
                       IIf((Attacker.Gender = _Gender.Male And Defender.Gender = _Gender.Female) _
                           Or (Attacker.Gender = _Gender.Female And Defender.Gender = _Gender.Male), _
                           &HC00, &H1000))
            Case 159 'Sand Force
                att_abi_BPmod = IIf((_AtkMove.Type = 5 Or _AtkMove.Type = 6 Or _AtkMove.Type = 9) And _
                         FOptions.Weather = _Weather.Sandstorm, &H14CD, &H1000)
        End Select
        If att_abi_BPmod <> &H1000 Then Attacker.AbilityEff = True

        Dim target_abi_BPmod As Single = &H1000
        Select Case Defender.Ability
            Case 85
                target_abi_BPmod = IIf(_AtkMove.Type = 10, &H800, &H1000)
            Case 87
                target_abi_BPmod = IIf(_AtkMove.Type = 10, &H1400, &H1000)
        End Select
        If target_abi_BPmod <> &H1000 Then Defender.AbilityEff = True

        'Sheer force
        Dim att_SF_mod As Single = &H1000
        If _TmpAttacker.Ability = 125 Then _
            att_SF_mod = IIf(_AtkMove.EffPercent > 0 And _AtkMove.EffPercent < 1, &H14CD, &H1000)

        'Items
        Dim IT_BPmod As Single
        If Attacker.Item = arrTypeItems(_AtkMove.Type, 1) Or Attacker.Item = arrTypeItems(_AtkMove.Type, 2) Then
            IT_BPmod = &H1333
        ElseIf Attacker.Item = arrTypeItems(_AtkMove.Type, 3) Then
            IT_BPmod = &H1800
        Else
            Select Case Attacker.Item
                Case "Muscle Band"
                    IT_BPmod = IIf(_AtkMove.DmgType = _DmgType.Physical, &H1199, &H1000)
                Case "Wise Glasses"
                    IT_BPmod = IIf(_AtkMove.DmgType = _DmgType.Special, &H1199, &H1000)
                Case "Adamant Orb"
                    IT_BPmod = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 9) And _
                           Attacker.Data.Name = "Dialga", &H1333, &H1000)
                Case "Lustrous Orb"
                    IT_BPmod = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 11) And _
                            Attacker.Data.Name = "Palkia", &H1333, &H1000)
                Case "Griseous Orb"
                    IT_BPmod = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 8) And _
                            Attacker.Data.Name = "Giratina-O", &H1333, &H1000)
                Case Else
                    IT_BPmod = &H1000
            End Select
        End If
        If IT_BPmod <> &H1000 Then Attacker.ItemEff = True

        Dim Move_BPmod As Single
        Select Case LCase(_AtkMove.Name)
            Case "facade"
                Move_BPmod = IIf(Attacker.Status = _Status.None, &H1000, &H2000)
            Case "brine"
                If Defender.HPPercent < 50 Then
                    Move_BPmod = &H2000
                Else
                    Move_BPmod = &H1000
                End If
            Case "venoshock"
                Move_BPmod = IIf(Defender.Status = _Status.Poison, &H2000, &H1000)
            Case Else
                Move_BPmod = &H1000
                'Retaliate (Placeholder)
                'Fusion Bolt/Fusion Flare (Placeholder)
        End Select

        Dim MeFirst As Single = IIf(FOptions.MeFirst, &H1800, &H1000)
        Dim SolarBeam As Single = IIf((LCase(_AtkMove.Name) = "solarbeam") And (FOptions.Weather <> _Weather.Sunshine) And (FOptions.Weather <> _Weather.None), &H800, &H1000)
        Dim Charge As Single = IIf(FOptions.Charge, &H2000, &H1000)
        Dim HelpingHand As Single = IIf(FOptions.HelpingHand, &H1800, &H1000)
        Dim WaterSport As Single = IIf(FOptions.WaterSport And _AtkMove.Type = 10, &H548, &H1000)
        Dim MudSport As Single = IIf(FOptions.MudSport And _AtkMove.Type = 13, &H548, &H1000)

        BP_mod = ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(att_abi_BPmod, target_abi_BPmod), att_SF_mod), IT_BPmod), Move_BPmod), MeFirst), SolarBeam), Charge), HelpingHand), WaterSport), MudSport)

        'Attack modifier
        Dim Atk_Mod As Single = &H1000
        Dim ThickFat_Atkmod As Single = IIf(Defender.Ability = 47 And (_AtkMove.Type = 10 Or _AtkMove.Type = 15), &H800, &H1000)
        Dim Abi_Atkmod As Single = &H1000
        Select Case Attacker.Ability
            Case 66
                Abi_Atkmod = IIf(_AtkMove.Type = 10 And Attacker.HPPercent <= 33, &H1800, &H1000)
            Case 65
                Abi_Atkmod = IIf(_AtkMove.Type = 12 And Attacker.HPPercent <= 33, &H1800, &H1000)
            Case 67
                Abi_Atkmod = IIf(_AtkMove.Type = 11 And Attacker.HPPercent <= 33, &H1800, &H1000)
            Case 68
                Abi_Atkmod = IIf(_AtkMove.Type = 7 And Attacker.HPPercent <= 33, &H1800, &H1000)
            Case 62
                Abi_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.Status <> _Status.None, &H1800, &H1000)
            Case 129
                Abi_Atkmod = IIf(Attacker.HPPercent <= 50, &H800, &H1000)
            Case 74
                Abi_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Physical, &H2000, &H1000)
            Case 37
                Abi_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Physical, &H2000, &H1000)
        End Select
        Dim Hustle_Atkmod As Single = IIf(Attacker.Ability = 55 And _AtkMove.DmgType = _DmgType.Physical, &H1800, &H1000)
        Dim FF_Atkmod As Single = IIf(FOptions.FlashFire And Attacker.Ability = 18 And _AtkMove.Type = 10, &H1800, &H1000)
        Dim SS_Atkmod As Single = IIf(Attacker.Ability = 112 And _AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.AbilityEff, &H800, &H1000)
        Dim FG_Atkmod As Single = IIf(Attacker.Ability = 122 And _AtkMove.DmgType = _DmgType.Physical And FOptions.Weather = _Weather.Sunshine, &H1800, &H1000)
        Dim Item_Atkmod As Single = &H1000
        Select Case Attacker.Item
            Case "Choice Band"
                Item_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Physical, &H1800, &H1000)
            Case "Light Ball"
                Item_Atkmod = IIf(Attacker.Data.Name = "Pikachu", &H2000, &H1000)
            Case "Thick Club"
                Item_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Physical And _
                       (Attacker.Data.Name = "Cubone" Or Attacker.Data.Name = "Marowak"), _
                       &H2000, &H1000)
            Case "Choice Specs"
                Item_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Special, &H1800, &H1000)
            Case "Soul Dew"
                Item_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Special And _
                       (Attacker.Data.Name = "Latias" Or Attacker.Data.Name = "Latios"), _
                           &H1800, &H1000)
            Case "Deepseatooth"
                Item_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Special And Attacker.Data.Name = "Clamperl", _
                       &H2000, &H1000)
            Case Else
                Item_Atkmod = &H1000
        End Select
        Atk_Mod = ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ThickFat_Atkmod, Abi_Atkmod), FF_Atkmod), SS_Atkmod), FG_Atkmod), Item_Atkmod)

        'Defense modifier
        Dim Def_Mod As Single = &H1000
        If (dAtkMove.DmgType = _DmgType.Special And _
            (Defender.Data.Type1 = 6 Or Defender.Data.Type2 = 6) _
            And FOptions.Weather = _Weather.Sandstorm) Then
            DStat = ApplyMod(DStat, &H1800)
        End If

        Dim Def_MSMod As Single = &H1000
        If dAtkMove.DmgType = _DmgType.Physical And Defender.Ability = 63 And Defender.Status <> _Status.None Then
            Def_MSMod = &H1800
            Defender.AbilityEff = True
        End If

        Dim Def_ItemMod As Single = &H1000
        If Defender.Data.Name = "Ditto" And Defender.Item = "Metal Powder" Then
            Def_ItemMod = &H2000
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And _
            Defender.Item = "Soul Dew" And _
            (Defender.Data.Name = "Latias" Or Defender.Data.Name = "Latios") Then
            Def_ItemMod = &H1800
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And Defender.Item = "Deepseascale" _
            And Defender.Data.Name = "Clamperl" Then
            Def_ItemMod = &H1800
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And Defender.Ability = 122 _
            And FOptions.Weather = _Weather.Sunshine Then
            Def_ItemMod = &H1800
            Defender.AbilityEff = True
        ElseIf Defender.Item = "Eviolite" Then
            Def_ItemMod = &H1800
            Defender.ItemEff = True
        Else
            Def_ItemMod = &H1000
        End If

        Def_Mod = ChainMod(Def_MSMod, Def_ItemMod)

        Dim iResult As Integer = 0
        iResult = Math.Floor((Attacker.Level * 2) / 5 + 2)
        iResult = iResult * (ApplyMod(_AtkMove.Power, BP_mod))
        iResult = iResult * ApplyMod(ApplyMod(AStat, Hustle_Atkmod), Atk_Mod)
        iResult = Math.Floor(iResult / ApplyMod(DStat, Def_Mod))
        iResult = Math.Floor(iResult / 50 + 2)
        'Modifiers
        iResult = ApplyMod(iResult, weather_mod)
        iResult = IIf(FOptions.CriticalHit, iResult * 2, iResult)
        'Random factor
        iResult = Math.Floor((iResult * (100 - Random)) / 100)
        'Modifiers
        iResult = ApplyMod(iResult, mod_multi_target)
        iResult = ApplyMod(iResult, STAB_mod) - 0.01
        iResult = Math.Floor(iResult * TypeMatch)
        iResult = Math.Floor(iResult * burn_mod)
        iResult = Math.Max(iResult, 1)
        iResult = ApplyMod(iResult, final_mod)

        If (Attacker.Ability = 104 Or Attacker.Ability = 163 Or Attacker.Ability = 164) Then Defender.Ability = DefAbility

        Return iResult
    End Function

    Public Function CalcAll_Damage_New(ByVal Attacker As Pokemon, ByVal Defender As Pokemon, _
                            ByVal AtkMove As Move, _
                            ByVal Weather As _Weather, Optional ByVal _AtkStat As Integer = 0, Optional ByVal _DefStat As Integer = 0) As Collection

        Dim colResult As New Collection
        Dim FOptions As Field_Effects = GetFOptions(Attacker, Defender)

        Dim _AtkMove As Move = GetMovePower_New(Attacker, Defender, AtkMove, FOptions)

        Dim dAtkMove As Move = _AtkMove 'Move that the defender takes
        If colSpHitDefMoves.Contains(dAtkMove.Name) Then dAtkMove.DmgType = _DmgType.Physical
        If FOptions.WonderRoom Then
            dAtkMove.DmgType = IIf(dAtkMove.DmgType = _DmgType.Physical, _DmgType.Special, _DmgType.Physical)
        End If
        'TODO: If _AtkMove.Power = 0 Then Return 1 (?)
        Dim _TmpDefender As Pokemon = Defender
        If LCase(_AtkMove.Name) = "chip away" Then
            If _TmpDefender.Boosts.Def > 0 Then _TmpDefender.Boosts.Def = 0
            If _TmpDefender.Boosts.SpDef > 0 Then _TmpDefender.Boosts.SpDef = 0
        End If

        Dim DefAbility As Integer : If (Attacker.Ability = 104 Or Attacker.Ability = 163 Or Attacker.Ability = 164) Then DefAbility = Defender.Ability : Defender.Ability = 0

        Dim DStat As Single
        If _DefStat <> 0 Then
            DStat = _DefStat
        Else
            DStat = IIf(dAtkMove.DmgType = _DmgType.Physical, _
                     CalcPokeStats(_TmpDefender, Attacker.Ability = 109, FOptions.CriticalHit).Def, _
                    IIf(dAtkMove.DmgType = _DmgType.Special, _
                        CalcPokeStats(_TmpDefender, Attacker.Ability = 109, FOptions.CriticalHit).SpDef, 1))
        End If

        Dim _TmpAttacker As Pokemon = IIf(LCase(_AtkMove.Name) = "foul play", Defender, Attacker)

        Dim AStat As Integer
        If _AtkStat <> 0 Then
            AStat = _AtkStat
        Else
            AStat = IIf(_AtkMove.DmgType = _DmgType.Physical, _
         CalcPokeStats(_TmpAttacker, Defender.Ability = 109, FOptions.CriticalHit).Atk, _
        IIf(_AtkMove.DmgType = _DmgType.Special, _
            CalcPokeStats(_TmpAttacker, Defender.Ability = 109, FOptions.CriticalHit).SpAtk, 1))
        End If

        'Multi target mod
        Dim mod_multi_target As Single = IIf(FOptions.Doubles And (_AtkMove.Target = 3 Or _AtkMove.Target = 4), &HC00, &H1000)

        'Weather mod
        Dim weather_mod As Single
        If FOptions.Weather = _Weather.Sunshine Then
            weather_mod = IIf(_AtkMove.Type = 10, &H1800, IIf(_AtkMove.Type = 11, &H800, &H1000))
        ElseIf FOptions.Weather = _Weather.Rain Then
            weather_mod = IIf(_AtkMove.Type = 11, &H1800, IIf(_AtkMove.Type = 10, &H800, &H1000))
        Else
            weather_mod = &H1000
        End If

        'STAB
        Dim STAB_mod As Single
        STAB_mod = IIf(Attacker.Data.Type1 = _AtkMove.Type Or Attacker.Data.Type2 = _AtkMove.Type, _
                 IIf(Attacker.Ability = 91, &H2000, &H1800), &H1000)
        If STAB_mod > &H1800 Then Attacker.AbilityEff = True

        'Type effectiveness
        Dim TypeMatch As Single = GetTypeMatch(Attacker, _AtkMove, Defender, FOptions)
        If TypeMatch = 0 Then GoTo NullResult

        'Burn
        Dim burn_mod As Single
        burn_mod = IIf((Attacker.Status = _Status.Burn And _AtkMove.DmgType = _DmgType.Physical) _
          And Attacker.Ability <> 62, 0.5, 1)

        'Final modifier
        Dim final_mod As Single = &H1000

        Dim RL As Single
        If FOptions.CriticalHit Or Attacker.Ability = 151 Or _AtkMove.Name = "Brick Break" Then
            RL = &H1000
        ElseIf dAtkMove.DmgType = _DmgType.Physical And FOptions.Reflect Then
            RL = IIf(FOptions.Doubles, &HA8F, &H800)
        ElseIf dAtkMove.DmgType = _DmgType.Special And FOptions.LightScreen Then
            RL = IIf(FOptions.Doubles, &HA8F, &H800)
        Else
            RL = &H1000
        End If

        Dim MSC As Single
        MSC = IIf(Defender.Ability = 136 And Defender.HPPercent = 100, &H800, &H1000)
        If MSC < 1 Then Defender.AbilityEff = True

        Dim TL As Single
        TL = IIf(Attacker.Ability = 110 And TypeMatch < 1, &H2000, &H1000)

        '(Friend Guard-placeholder)
        Dim SN As Single 'Sniper
        SN = IIf(Attacker.Ability = 97 And FOptions.CriticalHit, &H1800, &H1000)

        Dim SRF As Single
        SRF = IIf((Defender.Ability = 116 Or Defender.Ability = 111) And TypeMatch > 1, &HC00, &H1000)

        Dim MET As Single
        MET = IIf(Attacker.Item <> "Metronome", &H1000, IIf(FOptions.Metronome <= 4, &H1000 + FOptions.Metronome * &H333, &H2000))

        Dim EB As Single
        EB = IIf(Attacker.Item = "Expert Belt" And TypeMatch > 1, &H1333, &H1000)

        Dim LO As Single
        LO = IIf(Attacker.Item = "Life Orb", &H14CC, &H1000)

        Dim TRB As Single
        If _AtkMove.Type = 1 And Defender.Item = "Chilan Berry" Then
            TRB = &H800
        ElseIf TypeMatch > 1 And Defender.Item = arrTypeItems(_AtkMove.Type, 0) Then
            TRB = &H800
        Else
            TRB = &H1000
        End If

        final_mod = ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(RL, MSC), TL), SN), SRF), MET), EB), LO), TRB)

        'Base power modifier
        Dim BP_mod As Single = &H1000

        Dim att_abi_BPmod As Single = &H1000

        Select Case _TmpAttacker.Ability
            Case 101 'Technician
                att_abi_BPmod = IIf(_AtkMove.Power <= 60, &H1800, &H1000)
            Case 138 'Flare Boost
                att_abi_BPmod = IIf(_AtkMove.DmgType = _DmgType.Special And _TmpAttacker.Status = _Status.Burn, &H1800, &H1000)
                'Case 165 (Placeholder)
            Case 120 'Reckless
                att_abi_BPmod = IIf(colRecoilMoves.Contains(_AtkMove.Name), &H1333, &H1000)
            Case 89 'Iron Fist
                att_abi_BPmod = IIf(colPunchMoves.Contains(_AtkMove.Name), &H1333, &H1000)
            Case 137 'Toxic Boost
                att_abi_BPmod = IIf(_AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.Status = _Status.Poison, &H1800, &H1000)
            Case 79 'Rivalry
                att_abi_BPmod = IIf(Attacker.Gender = Defender.Gender, &H1400, _
                       IIf((Attacker.Gender = _Gender.Male And Defender.Gender = _Gender.Female) _
                           Or (Attacker.Gender = _Gender.Female And Defender.Gender = _Gender.Male), _
                           &HC00, &H1000))
            Case 159 'Sand Force
                att_abi_BPmod = IIf((_AtkMove.Type = 5 Or _AtkMove.Type = 6 Or _AtkMove.Type = 9) And _
                         FOptions.Weather = _Weather.Sandstorm, &H14CD, &H1000)
        End Select
        If att_abi_BPmod <> &H1000 Then Attacker.AbilityEff = True

        Dim target_abi_BPmod As Single = &H1000
        Select Case Defender.Ability
            Case 85
                target_abi_BPmod = IIf(_AtkMove.Type = 10, &H800, &H1000)
            Case 87
                target_abi_BPmod = IIf(_AtkMove.Type = 10, &H1400, &H1000)
        End Select
        If target_abi_BPmod <> &H1000 Then Defender.AbilityEff = True

        'Sheer force
        Dim att_SF_mod As Single = &H1000
        If _TmpAttacker.Ability = 125 Then _
            att_SF_mod = IIf(_AtkMove.EffPercent > 0 And _AtkMove.EffPercent < 1, &H14CD, &H1000)

        'Items
        Dim IT_BPmod As Single
        If Attacker.Item = arrTypeItems(_AtkMove.Type, 1) Or Attacker.Item = arrTypeItems(_AtkMove.Type, 2) Then
            IT_BPmod = &H1333
        ElseIf Attacker.Item = arrTypeItems(_AtkMove.Type, 3) Then
            IT_BPmod = &H1800
        Else
            Select Case Attacker.Item
                Case "Muscle Band"
                    IT_BPmod = IIf(_AtkMove.DmgType = _DmgType.Physical, &H1199, &H1000)
                Case "Wise Glasses"
                    IT_BPmod = IIf(_AtkMove.DmgType = _DmgType.Special, &H1199, &H1000)
                Case "Adamant Orb"
                    IT_BPmod = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 9) And _
                           Attacker.Data.Name = "Dialga", &H1333, &H1000)
                Case "Lustrous Orb"
                    IT_BPmod = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 11) And _
                            Attacker.Data.Name = "Palkia", &H1333, &H1000)
                Case "Griseous Orb"
                    IT_BPmod = IIf((_AtkMove.Type = 16 Or _AtkMove.Type = 8) And _
                            Attacker.Data.Name = "Giratina-O", &H1333, &H1000)
                Case Else
                    IT_BPmod = &H1000
            End Select
        End If
        If IT_BPmod <> &H1000 Then Attacker.ItemEff = True

        Dim Move_BPmod As Single
        Select Case LCase(_AtkMove.Name)
            Case "facade"
                Move_BPmod = IIf(Attacker.Status = _Status.None, &H1000, &H2000)
            Case "brine"
                If Defender.HPPercent < 50 Then
                    Move_BPmod = &H2000
                Else
                    Move_BPmod = &H1000
                End If
            Case "venoshock"
                Move_BPmod = IIf(Defender.Status = _Status.Poison, &H2000, &H1000)
            Case Else
                Move_BPmod = &H1000
                'Retaliate (Placeholder)
                'Fusion Bolt/Fusion Flare (Placeholder)
        End Select

        Dim MeFirst As Single = IIf(FOptions.MeFirst, &H1800, &H1000)
        Dim SolarBeam As Single = IIf((LCase(_AtkMove.Name) = "solarbeam") And (FOptions.Weather <> _Weather.Sunshine) And (FOptions.Weather <> _Weather.None), &H800, &H1000)
        Dim Charge As Single = IIf(FOptions.Charge, &H2000, &H1000)
        Dim HelpingHand As Single = IIf(FOptions.HelpingHand, &H1800, &H1000)
        Dim WaterSport As Single = IIf(FOptions.WaterSport And _AtkMove.Type = 10, &H548, &H1000)
        Dim MudSport As Single = IIf(FOptions.MudSport And _AtkMove.Type = 13, &H548, &H1000)

        BP_mod = ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(att_abi_BPmod, target_abi_BPmod), att_SF_mod), IT_BPmod), Move_BPmod), MeFirst), SolarBeam), Charge), HelpingHand), WaterSport), MudSport)

        'Attack modifier
        Dim Atk_Mod As Single = &H1000
        Dim ThickFat_Atkmod As Single = IIf(Defender.Ability = 47 And (_AtkMove.Type = 10 Or _AtkMove.Type = 15), &H800, &H1000)
        Dim Abi_Atkmod As Single = &H1000
        Select Case Attacker.Ability
            Case 66
                Abi_Atkmod = IIf(_AtkMove.Type = 10 And Attacker.HPPercent <= 33, &H1800, &H1000)
            Case 65
                Abi_Atkmod = IIf(_AtkMove.Type = 12 And Attacker.HPPercent <= 33, &H1800, &H1000)
            Case 67
                Abi_Atkmod = IIf(_AtkMove.Type = 11 And Attacker.HPPercent <= 33, &H1800, &H1000)
            Case 68
                Abi_Atkmod = IIf(_AtkMove.Type = 7 And Attacker.HPPercent <= 33, &H1800, &H1000)
            Case 62
                Abi_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.Status <> _Status.None, &H1800, &H1000)
            Case 129
                Abi_Atkmod = IIf(Attacker.HPPercent <= 50, &H800, &H1000)
            Case 74
                Abi_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Physical, &H2000, &H1000)
            Case 37
                Abi_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Physical, &H2000, &H1000)
        End Select
        Dim Hustle_Atkmod As Single = IIf(Attacker.Ability = 55 And _AtkMove.DmgType = _DmgType.Physical, &H1800, &H1000)
        Dim FF_Atkmod As Single = IIf(FOptions.FlashFire And Attacker.Ability = 18 And _AtkMove.Type = 10, &H1800, &H1000)
        Dim SS_Atkmod As Single = IIf(Attacker.Ability = 112 And _AtkMove.DmgType = _DmgType.Physical And _TmpAttacker.AbilityEff, &H800, &H1000)
        Dim FG_Atkmod As Single = IIf(Attacker.Ability = 122 And _AtkMove.DmgType = _DmgType.Physical And FOptions.Weather = _Weather.Sunshine, &H1800, &H1000)
        Dim Item_Atkmod As Single = &H1000
        Select Case Attacker.Item
            Case "Choice Band"
                Item_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Physical, &H1800, &H1000)
            Case "Light Ball"
                Item_Atkmod = IIf(Attacker.Data.Name = "Pikachu", &H2000, &H1000)
            Case "Thick Club"
                Item_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Physical And _
                       (Attacker.Data.Name = "Cubone" Or Attacker.Data.Name = "Marowak"), _
                       &H2000, &H1000)
            Case "Choice Specs"
                Item_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Special, &H1800, &H1000)
            Case "Soul Dew"
                Item_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Special And _
                       (Attacker.Data.Name = "Latias" Or Attacker.Data.Name = "Latios"), _
                           &H1800, &H1000)
            Case "Deepseatooth"
                Item_Atkmod = IIf(_AtkMove.DmgType = _DmgType.Special And Attacker.Data.Name = "Clamperl", _
                       &H2000, &H1000)
            Case Else
                Item_Atkmod = &H1000
        End Select
        Atk_Mod = ChainMod(ChainMod(ChainMod(ChainMod(ChainMod(ThickFat_Atkmod, Abi_Atkmod), FF_Atkmod), SS_Atkmod), FG_Atkmod), Item_Atkmod)

        'Defense modifier
        Dim Def_Mod As Single = &H1000
        If (dAtkMove.DmgType = _DmgType.Special And _
            (Defender.Data.Type1 = 6 Or Defender.Data.Type2 = 6) _
            And FOptions.Weather = _Weather.Sandstorm) Then
            DStat = ApplyMod(DStat, &H1800)
        End If


        Dim Def_MSMod As Single = &H1000
        If dAtkMove.DmgType = _DmgType.Physical And Defender.Ability = 63 And Defender.Status <> _Status.None Then
            Def_MSMod = &H1800
            Defender.AbilityEff = True
        End If

        Dim Def_ItemMod As Single = &H1000
        If Defender.Data.Name = "Ditto" And Defender.Item = "Metal Powder" Then
            Def_ItemMod = &H2000
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And _
            Defender.Item = "Soul Dew" And _
            (Defender.Data.Name = "Latias" Or Defender.Data.Name = "Latios") Then
            Def_ItemMod = &H1800
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And Defender.Item = "Deepseascale" _
            And Defender.Data.Name = "Clamperl" Then
            Def_ItemMod = &H1800
            Defender.ItemEff = True
        ElseIf dAtkMove.DmgType = _DmgType.Special And Defender.Ability = 122 _
            And FOptions.Weather = _Weather.Sunshine Then
            Def_ItemMod = &H1800
            Defender.AbilityEff = True
        ElseIf Defender.Item = "Eviolite" Then
            Def_ItemMod = &H1800
            Defender.ItemEff = True
        Else
            Def_ItemMod = &H1000
        End If

        Def_Mod = ChainMod(Def_MSMod, Def_ItemMod)

        Dim iResult As Integer = 0
        iResult = Math.Floor((Attacker.Level * 2) / 5 + 2)
        iResult = iResult * (ApplyMod(_AtkMove.Power, BP_mod))
        iResult = iResult * ApplyMod(ApplyMod(AStat, Hustle_Atkmod), Atk_Mod)
        iResult = Math.Floor(iResult / ApplyMod(DStat, Def_Mod))
        iResult = Math.Floor(iResult / 50 + 2)
        'Modifiers
        iResult = ApplyMod(iResult, weather_mod)
        iResult = IIf(FOptions.CriticalHit, iResult * 2, iResult)
        'Random factor
        For Random As SByte = 15 To 0 Step -1
            Dim iResult2 As Integer

            iResult2 = Math.Floor((iResult * (100 - Random)) / 100)
            'Modifiers
            iResult2 = ApplyMod(iResult2, mod_multi_target)
            iResult2 = ApplyMod(iResult2, STAB_mod)
            iResult2 = Math.Floor(iResult2 * TypeMatch)
            iResult2 = Math.Floor(iResult2 * burn_mod)
            iResult2 = Math.Max(iResult2, 1)
            iResult2 = ApplyMod(iResult2, final_mod)

            colResult.Add(iResult2)
        Next

        If (Attacker.Ability = 104 Or Attacker.Ability = 163 Or Attacker.Ability = 164) Then Defender.Ability = DefAbility

        Return colResult

NullResult:
        If (Attacker.Ability = 104 Or Attacker.Ability = 163 Or Attacker.Ability = 164) Then Defender.Ability = DefAbility
        For i As Byte = 0 To 15 : colResult.Add(0) : Next : Return colResult
    End Function

    Public Function GetMovePower_New(ByVal pkAttacker As Pokemon, ByVal pkDefender As Pokemon, ByVal MoveData As Move, ByVal FOptions As Field_Effects) As Move
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

End Module
