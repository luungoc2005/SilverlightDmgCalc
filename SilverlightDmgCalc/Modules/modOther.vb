Module modOther
    Public Enum CompareFunc
        EqualsTo
        LessOrEquals
        MoreOrEquals
    End Enum

    Public Function GetStatString(ByVal _sData As Stats, Optional ByVal _Default As Byte = 0) As String
        Dim colStats As Collection = New Collection
        With _sData
            If .HP <> _Default Then colStats.Add(.HP & " HP")
            If .Atk <> _Default Then colStats.Add(.Atk & " Atk")
            If .Def <> _Default Then colStats.Add(.Def & " Def")
            If .SpAtk <> _Default Then colStats.Add(.SpAtk & " SAtk")
            If .SpDef <> _Default Then colStats.Add(.SpDef & " SDef")
            If .Speed <> _Default Then colStats.Add(.Speed & " Spd")
        End With
        If colStats.Count = 0 Then Return vbNullString
        Dim strResult As String = vbNullString
        For i = 1 To colStats.Count
            If i <> colStats.Count Then
                strResult += colStats(i) & "/"
            Else
                strResult += colStats(i)
            End If
        Next
        Return Trim(strResult)
    End Function

    Public Function FindVariable(TargetValue As Integer, intMin As Integer, intMax As Integer, CalcFunc As Func(Of Integer, Integer), Optional Reverse As Boolean = False, Optional Mode As CompareFunc = CompareFunc.EqualsTo) As Integer
        Dim tMin As Integer, tMax As Integer
        tMin = intMin
        tMax = intMax
        'If Not (tMin + tMax) Mod 2 Then
        '    tMax += 1
        'End If
        Dim arrValues As New Dictionary(Of Integer, Integer)

        Do While tMax > tMin
            Dim tMid As Integer = (tMin + tMax) / 2
            Dim tResult As Integer = CalcFunc(tMid)

            If Not Reverse Then
                If tResult = TargetValue Then
                    Return tMid
                ElseIf tResult < TargetValue Then
                    tMin = tMid + 1
                Else
                    tMax = tMid - 1
                End If
            Else
                If tResult = TargetValue Then
                    Return tMid
                ElseIf tResult > TargetValue Then
                    tMin = tMid + 1
                Else
                    tMax = tMid - 1
                End If
            End If

            If Not Mode = CompareFunc.EqualsTo Then
                arrValues.Add(tMid, tResult)
            End If
        Loop

        'Not found, attempt to find closest possible value
        Dim arrTemp As New Dictionary(Of Integer, Integer)

        If Mode = CompareFunc.LessOrEquals Then
            For i As Integer = 0 To arrValues.Count - 1
                If arrValues.Values(i) <= TargetValue Then arrTemp.Add(arrValues.Keys(i), arrValues.Values(i))
            Next
            Dim iMax As Integer
            If arrTemp.Count = 0 Then
                Return -1
            ElseIf arrTemp.Count = 1 Then

            Else
                For i As Integer = 1 To arrTemp.Count - 1
                    If arrTemp.Values(i) > arrTemp.Values(iMax) Then
                        iMax = i
                    ElseIf (arrTemp.Values(i) = arrTemp.Values(iMax)) AndAlso (arrTemp.Keys(i) < arrTemp.Keys(iMax)) Then
                        iMax = i
                    End If
                Next
            End If
            Return arrTemp.Keys(iMax)
            'For i As Integer = iMax To tMax
            '    If CalcFunc(i) > TargetValue Then Return i - 1
            'Next
        ElseIf Mode = CompareFunc.MoreOrEquals Then
            For i As Integer = 0 To arrValues.Count - 1
                If arrValues.Values(i) >= TargetValue Then arrTemp.Add(arrValues.Keys(i), arrValues.Values(i))
            Next
            Dim iMin As Integer
            If arrTemp.Count = 0 Then
                Return -1
            ElseIf arrTemp.Count = 1 Then

            Else
                For i As Integer = 1 To arrTemp.Count - 1
                    If arrTemp.Values(i) < arrTemp.Values(iMin) Then
                        iMin = i
                    ElseIf (arrTemp.Values(i) = arrTemp.Values(iMin)) AndAlso (arrTemp.Keys(i) < arrTemp.Keys(iMin)) Then
                        iMin = i
                    End If
                Next
            End If
            Return arrTemp.Keys(iMin)
            'For i As Integer = iMin To tMin Step -1
            '    If CalcFunc(i) < TargetValue Then Return i + 1
            'Next
        End If

        Return -1
    End Function
End Module
