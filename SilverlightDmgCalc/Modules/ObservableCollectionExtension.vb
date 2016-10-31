Imports System.Collections.Specialized

Public Class ObservableCollection(Of T) : Inherits System.Collections.ObjectModel.ObservableCollection(Of T)
    Public Property HaltUpdate As Boolean

    Protected Overrides Sub OnPropertyChanged(ByVal e As System.ComponentModel.PropertyChangedEventArgs)
        If Not HaltUpdate Then MyBase.OnPropertyChanged(e)
    End Sub

    Public Sub AddWithoutUpdate(ByVal item As T)
        Items.Add(item)
    End Sub

    Public Sub ChangeItem(ByVal item As T, ByVal index As Integer)
        Items(index) = item
    End Sub

    Public Sub UpdateCollection()
        OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal collection As IEnumerable(Of T))
        MyBase.New(collection)
    End Sub
End Class
