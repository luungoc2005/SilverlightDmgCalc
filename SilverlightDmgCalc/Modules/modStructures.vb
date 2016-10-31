Imports System.Xml.Linq
'Imports System.Windows.Media.Imaging

#Region "Structures"
Public Class Pokemon
    Public Data As New PokeData
    Public EVs As Stats
    Public IVs As Stats
    Public Boosts As StatsData
    Public Nature As Stats
    Public Item As String
    Public CurMove As Move
    Public Level As Byte
    Public HPPercent As Byte
    Public Gender As _Gender
    Public Status As _Status
    Public Happiness As Byte
    Public Ability As Byte
    Public AbilityEff As Boolean
    Public ItemEff As Boolean = False
    Public fOptions As Field_Effects
    Public Sub New()
        IVs.HP = 31
        IVs.Atk = 31
        IVs.Def = 31
        IVs.SpAtk = 31
        IVs.SpDef = 31
        IVs.Speed = 31
        Happiness = 255
        HPPercent = 100
        Level = 100
        Status = _Status.None
        Gender = _Gender.Male
    End Sub
End Class

Public Structure EntryHazards
    Dim StealthRock As Boolean
    Dim Spikes As Byte
End Structure

Public Structure StatsData
    Dim HP As Short
    Dim Atk As Short
    Dim Def As Short
    Dim SpAtk As Short
    Dim SpDef As Short
    Dim Speed As Short
    Dim Accuracy As Short
    Dim Evasion As Short
End Structure

Public Enum _DmgType
    Physical = 0
    Special = 1
    Status = 2
End Enum

Public Enum _Gender
    Genderless = 0
    Male = 1
    Female = 2
End Enum

Public Enum _Status
    None = 0
    Burn = 1
    Freeze = 2
    Paralysis = 3
    Poison = 4
    Sleep = 5
End Enum

Public Enum _Weather
    None = 0
    Hail = 1
    Rain = 2
    Sandstorm = 3
    Sunshine = 4
End Enum

Public Structure Stats
    Dim HP As Byte
    Dim Atk As Byte
    Dim Def As Byte
    Dim SpAtk As Byte
    Dim SpDef As Byte
    Dim Speed As Byte
End Structure

Public Class PokeData
    Public Number As Short
    Public Property Name As String
    Public Type1 As Byte
    Public Type2 As Byte
    Public Ability1 As Short
    Public Ability2 As Short
    Public Ability3 As Short
    Public BaseStats As New Stats
    Public Weight As Double
    Dim SpritePath As String
    'Public Property Sprite As BitmapImage
    Public Property SpriteVisibility As Visibility

    Public Sub FromXElement(ByVal _data As XElement)
        On Error Resume Next
        Number = Short.Parse(_data.Element(("number")).Value)
        Name = _data.Attribute(("name")).Value
        Type1 = Byte.Parse(_data.Element(("type1")).Value)
        Type2 = Byte.Parse(_data.Element(("type2")).Value)
        Ability1 = Short.Parse(_data.Element(("Ability1")).Value)
        Ability2 = Short.Parse(_data.Element(("Ability2")).Value)
        Ability3 = Short.Parse(_data.Element(("Ability3")).Value)
        With BaseStats
            .HP = Byte.Parse(_data.Element(("baseHP")).Value)
            .Atk = Byte.Parse(_data.Element(("baseAtk")).Value)
            .Def = Byte.Parse(_data.Element(("baseDef")).Value)
            .SpAtk = Byte.Parse(_data.Element(("baseSpAtk")).Value)
            .SpDef = Byte.Parse(_data.Element(("baseSpDef")).Value)
            .Speed = Byte.Parse(_data.Element(("baseSpe")).Value)
        End With
        Weight = GetDouble(_data.Element(("weight")).Value)
        SpritePath = _data.Element("img").Value
        SpriteVisibility = Visibility.Collapsed
    End Sub

    'Public Sub LoadSprite()
    '    'If SpritePath <> vbNullString And Not objDataStream Is Nothing Then
    '    '    Sprite = New BitmapImage
    '    '    Sprite.SetSource(Application.GetResourceStream(objDataStream, _
    '    '                                                   New Uri("icons/" & SpritePath & ".png", UriKind.Relative)).Stream)
    '    '    SpriteVisibility = Visibility.Visible
    '    'End If
    'End Sub
End Class

Public Structure Field_Effects
    Dim Weather As _Weather
    Dim CriticalHit As Boolean
    Dim Doubles As Boolean
    Dim NoImmunity As Boolean
    Dim WonderRoom As Boolean
    'Attacking
    Dim FlashFire As Boolean
    Dim MeFirst As Boolean
    Dim HelpingHand As Boolean
    Dim Charge As Boolean
    Dim Metronome As Byte
    'Defending
    Dim Reflect As Boolean
    Dim LightScreen As Boolean
    Dim MudSport As Boolean
    Dim WaterSport As Boolean
    Dim Hazards As EntryHazards
End Structure

Public Structure Move
    Public Property Name As String
    Dim Type As Byte
    Dim Power As Short
    Dim DmgType As _DmgType
    Dim Accuracy As Byte
    Dim PP As Byte
    Dim Priority As SByte
    Dim EffPercent As Single
    Dim Target As Byte

    Public Sub FromXElement(ByVal data As XElement)
        On Error Resume Next
        Name = data.Attribute(("name")).Value
        Type = Byte.Parse(data.Element(("type")).Value)
        Power = Short.Parse(data.Element(("power")).Value)
        DmgType = CType(data.Element(("dmgtype")).Value, _DmgType)
        Accuracy = Byte.Parse(data.Element(("accuracy")).Value)
        PP = Byte.Parse(data.Element(("pp")).Value)
        Priority = SByte.Parse(data.Element(("priority")).Value)
        EffPercent = GetDouble(data.Element(("eff")).Value)
        Target = Byte.Parse(data.Element(("target")).Value)
    End Sub
End Structure
#End Region
