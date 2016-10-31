Imports System.Xml.Linq
Imports System.Windows.Threading
Imports System.Windows.Resources

Partial Public Class MainPage
    Inherits UserControl

    Dim IsMainAttacker As Boolean = True
    Dim IsGridMoreOver As Boolean = False
    Dim objGridTimer As New DispatcherTimer

    Public Sub New()
        InitializeComponent()
        Try
            Try
                objDataStream = Application.GetResourceStream(New Uri("/SilverlightDmgCalc;component/data.zip", UriKind.Relative))
                Dim objPokeStream As StreamResourceInfo = Application.GetResourceStream(objDataStream, _
                                                                           New Uri("pokedata.xml", UriKind.Relative))
                Dim objXML As XElement = XElement.Load(objPokeStream.Stream)

                For Each _data As XElement In objXML.Descendants(XName.Get("pokemon"))
                    Dim objData As New PokeData
                    objData.FromXElement(_data)
                    'objData.LoadSprite()
                    _Xpokedata.Add(objData.Name, objData)
                Next

                Dim objMoveStream As StreamResourceInfo = Application.GetResourceStream(objDataStream, _
                                                                           New Uri("movedata.xml", UriKind.Relative))
                objXML = XElement.Load(objMoveStream.Stream)
                For Each _data As XElement In objXML.Descendants(XName.Get("move"))
                    Dim objData As New Move
                    objData.FromXElement(_data)
                    _Xmovedata.Add(objData.Name, objData)
                Next
            Catch ex As Exception
                MessageBox.Show("Error while downloading Pokémon data" & vbCrLf & "Details:" & vbCrLf & ex.Message)
            Finally
                AddGUI(False)

                _GUIMain.UpdateData()

                For Each ctrTemp As GUIContainer In _stackMain.Children
                    ctrTemp._GUIContained.UpdateData()
                Next
            End Try

        Catch ex As Exception
            MessageBox.Show("Error while loading:" & vbCrLf & ex.Message)
        End Try

        AddHandler objGridTimer.Tick, Sub()
                                          If Not IsGridMoreOver Then
                                              gridMore.Visibility = Windows.Visibility.Collapsed
                                          End If
                                      End Sub
        objGridTimer.Interval = TimeSpan.FromMilliseconds(100)
    End Sub

    'Dim objClient As WebClient = New WebClient
    'AddHandler objClient.OpenReadCompleted, _
    '            Sub(s As Object, a As System.Net.OpenReadCompletedEventArgs)
    '                Try
    '                    objDataStream = New StreamResourceInfo(a.Result, vbNull)
    '                    Dim objPokeStream As StreamResourceInfo = Application.GetResourceStream(objDataStream, _
    '                                                                               New Uri("pokedata.xml", UriKind.Relative))
    '                    Dim objXML As XElement = XElement.Load(objPokeStream.Stream)

    '                    For Each _data As XElement In objXML.Descendants(XName.Get("pokemon"))
    '                        Dim objData As New PokeData
    '                        objData.FromXElement(_data)
    '                        objData.LoadSprite()
    '                        _Xpokedata.AddWithoutUpdate(objData)
    '                    Next

    '                    Dim objMoveStream As StreamResourceInfo = Application.GetResourceStream(objDataStream, _
    '                                                                               New Uri("movedata.xml", UriKind.Relative))
    '                    objXML = XElement.Load(objMoveStream.Stream)
    '                    For Each _data As XElement In objXML.Descendants(XName.Get("move"))
    '                        Dim objData As New Move
    '                        objData.FromXElement(_data)
    '                        _Xmovedata.AddWithoutUpdate(objData)
    '                    Next

    '                    _Xpokedata.UpdateCollection()
    '                    _Xmovedata.UpdateCollection()
    '                Catch ex As Exception
    '                    MessageBox.Show("Error while downloading Pokémon data" & vbCrLf & "Details:" & vbCrLf & ex.Message)
    '                Finally
    '                    IsDataDownloaded = True
    '                    _GUIMain.UpdateData()
    '                    For Each ctrTemp As GUIContainer In _stackMain.Children
    '                        ctrTemp._GUIContained.UpdateData()
    '                    Next
    '                End Try
    '            End Sub
    'objClient.OpenReadAsync(New Uri("data.zip", UriKind.Relative), vbNull)

    'Dim objClient As WebClient = New WebClient
    'Dim objClient2 As WebClient = New WebClient

    'AddHandler objClient2.OpenReadCompleted, _
    '            Sub(s As Object, a As System.Net.OpenReadCompletedEventArgs)
    '                Try
    '                    Dim objXML As XElement = XElement.Load(a.Result)
    '                    For Each _data As XElement In objXML.Descendants(XName.Get("move"))
    '                        Dim objData As New Move
    '                        objData.FromXElement(_data)
    '                        _Xmovedata.AddWithoutUpdate(objData)
    '                        If objData.Power > 0 Then
    '                            _XAtkMoves.AddWithoutUpdate(objData)
    '                        End If
    '                    Next

    '                    _Xpokedata.UpdateCollection()
    '                    _Xmovedata.UpdateCollection()
    '                    _XAtkMoves.UpdateCollection()
    '                    a.Result.Close()
    '                Catch ex As Exception
    '                    MessageBox.Show("Error while downloading Pokémon data" & vbCrLf & "Details:" & vbCrLf & ex.Message)
    '                Finally
    '                    IsDataDownloaded = True
    '                    _GUIMain.UpdateData()
    '                    For Each ctrTemp As GUIContainer In lstPokeGUI
    '                        ctrTemp._GUIContained.UpdateData()
    '                    Next
    '                End Try
    '            End Sub

    'Dim objURI2 As Uri = New Uri("movedata.xml", UriKind.Relative)

    'AddHandler objClient.OpenReadCompleted, _
    '    Sub(s As Object, a As System.Net.OpenReadCompletedEventArgs)
    '        Try
    '            Dim objXML As XElement = XElement.Load(a.Result)
    '            For Each _data As XElement In objXML.Descendants(XName.Get("pokemon"))
    '                Dim objData As New PokeData
    '                objData.FromXElement(_data)
    '                _Xpokedata.AddWithoutUpdate(objData)
    '            Next

    '            a.Result.Close()
    '            objClient2.OpenReadAsync(objURI2, vbNull)
    '        Catch ex As Exception
    '            MessageBox.Show("Error while downloading Moves data" & vbCrLf & "Details:" & vbCrLf & ex.Message)
    '        End Try
    '    End Sub

    'Dim objURI As Uri = New Uri("pokedata.xml", UriKind.Relative)
    'objClient.OpenReadAsync(objURI, vbNull)

    Private Sub MainPage_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Dim tmrProgress As New DispatcherTimer
        tmrProgress.Interval = TimeSpan.FromSeconds(1)
        AddHandler tmrProgress.Tick, AddressOf PrgTimerTick
        tmrProgress.Start()

        _pGetMainPageData = AddressOf GatherSharedOptions

    End Sub

    Private Sub AddGUI(ByVal _Animate As Boolean)
        Dim objContainer As New GUIContainer(_GUIMain)

        With objContainer
            .Width = 283
            .Height = 635
            .IsAttacker = Not IsMainAttacker
            .Margin = New Thickness(5, 0, 0, 0)
            '._GUIContained.IsAttacker = True
            ._GUIContained.UpdateData()
            If _Animate Then .Opacity = 0
        End With

        _stackMain.Children.Add(objContainer)

        For Each ctrTemp As GUIContainer In _stackMain.Children
            ctrTemp.UsePercent2 = False
        Next

        If _Animate Then
            Dim _board As New Storyboard
            Dim _opac As New DoubleAnimation

            _opac.From = 0
            _opac.To = 1
            _opac.Duration = TimeSpan.FromMilliseconds(500)
            Storyboard.SetTarget(_opac, objContainer)
            Storyboard.SetTargetProperty(_opac, New PropertyPath(UIElement.OpacityProperty))
            _board.Children.Add(_opac)


            Dim _Zvalue As New DoubleAnimation
            Dim _objProjection As New PlaneProjection
            objContainer.Projection = _objProjection
            _Zvalue.From = 270
            _Zvalue.To = 360
            _Zvalue.Duration = TimeSpan.FromMilliseconds(500)
            Storyboard.SetTarget(_Zvalue, _objProjection)
            Storyboard.SetTargetProperty(_Zvalue, New PropertyPath(PlaneProjection.RotationYProperty))
            _board.Children.Add(_Zvalue)

            _board.Begin()
        End If
    End Sub

    Private Sub PrgTimerTick()
        If _stackMain.Children.Count = 0 Then Exit Sub
        For Each ctrTemp As GUIContainer In _stackMain.Children
            ctrTemp.UsePercent2 = Not ctrTemp.UsePercent2
            ctrTemp.PrgTimerTick()
        Next
    End Sub

    Private Sub RemGUI()
        If _stackMain.Children.Count <= 1 Then Exit Sub

        Dim objContainer As GUIContainer = _stackMain.Children(_stackMain.Children.Count - 1)

        Dim _board As New Storyboard
        Dim _opac As New DoubleAnimation
        _opac.From = 1
        _opac.To = 0
        _opac.Duration = TimeSpan.FromMilliseconds(200)
        Storyboard.SetTarget(_opac, objContainer)
        Storyboard.SetTargetProperty(_opac, New PropertyPath(UIElement.OpacityProperty))
        _board.Children.Add(_opac)


        Dim _Zvalue As New DoubleAnimation
        Dim _objProjection As New PlaneProjection
        objContainer.Projection = _objProjection
        _Zvalue.From = 360
        _Zvalue.To = 270
        _Zvalue.Duration = TimeSpan.FromMilliseconds(200)
        Storyboard.SetTarget(_Zvalue, _objProjection)
        Storyboard.SetTargetProperty(_Zvalue, New PropertyPath(PlaneProjection.RotationYProperty))
        _board.Children.Add(_Zvalue)

        AddHandler _board.Completed, Sub(s, e)
                                         _stackMain.Children.Remove(objContainer)
                                     End Sub

        _board.Begin()
    End Sub

    Private Sub NavBar1_btnAddClicked() Handles NavBar1.btnAddClicked
        AddGUI(True)
    End Sub

    Private Sub NavBar1_btnCalcClicked() Handles NavBar1.btnCalcClicked
        For Each objGUI As GUIContainer In _stackMain.Children
            Call objGUI.Button1_Click()
        Next
    End Sub

    Private Sub NavBar1_btnRemClicked() Handles NavBar1.btnRemClicked
        RemGUI()
    End Sub

    Private Sub NavBar1_btnSwitchClicked() Handles NavBar1.btnSwitchClicked
        IsMainAttacker = Not IsMainAttacker

        _GUIMain.IsAttacker = IsMainAttacker
        For Each objGUI As GUIContainer In _stackMain.Children
            objGUI.IsAttacker = Not IsMainAttacker
        Next
    End Sub

    Private Sub GatherSharedOptions()
        With _GUIMain.PokeData.fOptions
            .Weather = cmbWeather.SelectedIndex
            .CriticalHit = chkCrit.IsChecked
            .WonderRoom = chkWonder.IsChecked
            .Doubles = chkTVT.IsChecked
            .NoImmunity = chkImmunity.IsChecked
        End With
    End Sub

    Private Sub HyperlinkButton2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles HyperlinkButton2.Click
        Dim wndCov As New wndCoverage
        wndCov.Show()
        IsGridMoreOver = False
    End Sub

    Private Sub txtMore_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles txtMore.MouseEnter
        gridMore.Visibility = Windows.Visibility.Visible
        IsGridMoreOver = True
        objGridTimer.Stop()
        objGridTimer.Start()
    End Sub

    Private Sub txtMore_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles txtMore.MouseLeave
        IsGridMoreOver = False
        objGridTimer.Stop()
        objGridTimer.Start()
    End Sub

    Private Sub gridMore_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles gridMore.MouseEnter
        IsGridMoreOver = True
        objGridTimer.Stop()
        objGridTimer.Start()
    End Sub

    Private Sub gridMore_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles gridMore.MouseLeave
        IsGridMoreOver = False

        objGridTimer.Stop()
        objGridTimer.Start()
    End Sub
End Class