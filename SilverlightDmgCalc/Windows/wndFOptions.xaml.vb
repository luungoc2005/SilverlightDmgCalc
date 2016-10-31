Partial Public Class wndFOptions
    Inherits ChildWindow
    Dim pkData As Pokemon

    Public Sub PopupForm(ByVal PokeData As Pokemon, ByVal IsAttacker As Boolean, ByVal MainPoke As Boolean)
        With PokeData.fOptions
            chkCharge.IsChecked = .Charge
            chkFF.IsChecked = .FlashFire
            chkHH.IsChecked = .HelpingHand
            chkLS.IsChecked = .LightScreen
            chkMF.IsChecked = .MeFirst
            sldMetro.Value = .Metronome
            chkMS.IsChecked = .MudSport
            chkRF.IsChecked = .Reflect
            chkWS.IsChecked = .WaterSport
            chkSR.IsChecked = .Hazards.StealthRock
            chkSpikes.IsChecked = (.Hazards.Spikes <> 0)
            Select Case .Hazards.Spikes
                Case 1
                    optSpikes1.IsChecked = True
                Case 2
                    optSpikes2.IsChecked = True
                Case 3
                    optSpikes3.IsChecked = True
                Case Else
                    optSpikes1.IsChecked = True
            End Select
        End With

        chkFF.IsEnabled = IsAttacker
        chkMF.IsEnabled = IsAttacker
        chkHH.IsEnabled = IsAttacker
        chkCharge.IsEnabled = IsAttacker
        sldMetro.IsEnabled = IsAttacker

        chkSR.IsEnabled = Not IsAttacker
        chkSpikes.IsEnabled = Not IsAttacker
        optSpikes1.IsEnabled = chkSpikes.IsChecked And chkSpikes.IsEnabled
        optSpikes2.IsEnabled = chkSpikes.IsChecked And chkSpikes.IsEnabled
        optSpikes3.IsEnabled = chkSpikes.IsChecked And chkSpikes.IsEnabled
        chkRF.IsEnabled = Not IsAttacker
        chkLS.IsEnabled = Not IsAttacker
        chkWS.IsEnabled = Not IsAttacker
        chkMS.IsEnabled = Not IsAttacker

        pkData = PokeData
    End Sub

    Private Sub chkSpikes_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkSpikes.Checked
        optSpikes1.IsEnabled = chkSpikes.IsChecked And chkSpikes.IsEnabled
        optSpikes2.IsEnabled = chkSpikes.IsChecked And chkSpikes.IsEnabled
        optSpikes3.IsEnabled = chkSpikes.IsChecked And chkSpikes.IsEnabled
    End Sub

    Private Sub chkSpikes_Unchecked(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkSpikes.Unchecked
        optSpikes1.IsEnabled = chkSpikes.IsChecked And chkSpikes.IsEnabled
        optSpikes2.IsEnabled = chkSpikes.IsChecked And chkSpikes.IsEnabled
        optSpikes3.IsEnabled = chkSpikes.IsChecked And chkSpikes.IsEnabled
    End Sub

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles OKButton.Click
        With pkData.fOptions
            .Charge = chkCharge.IsChecked
            .FlashFire = chkFF.IsChecked
            .HelpingHand = chkHH.IsChecked
            .LightScreen = chkLS.IsChecked
            .MeFirst = chkMF.IsChecked
            .Metronome = sldMetro.Value
            .MudSport = chkMS.IsChecked
            .Reflect = chkRF.IsChecked
            .WaterSport = chkWS.IsChecked
            .Hazards.StealthRock = chkSR.IsChecked
            .Hazards.Spikes = IIf(Not chkSpikes.IsChecked, 0, _
                                IIf(optSpikes1.IsChecked, 1, _
                                    IIf(optSpikes2.IsChecked, 2, 3)))
        End With
        Me.DialogResult = True
    End Sub

    Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles CancelButton.Click
        Me.DialogResult = False
    End Sub

    Private Sub sldMetro_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles sldMetro.ValueChanged
        sldMetro.Value = Math.Round(sldMetro.Value)
    End Sub
End Class
