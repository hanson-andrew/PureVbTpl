Imports System.Threading

Namespace Tasks

  Public Structure CancellationToken

    Private Shared _none As CancellationToken = New CancellationToken()

    Private _canBeCanceled As Boolean
    Private _waitHandle As WaitHandle
    Friend _cancellationTokenSource As CancellationTokenSource

    Friend Sub New(ByVal tokenSource As CancellationTokenSource)
      Me._cancellationTokenSource = tokenSource
    End Sub

    Public Property CanBeCanceled() As Boolean
      Get
        Return Me._canBeCanceled
      End Get
      Set(ByVal value As Boolean)
        Me._canBeCanceled = value
      End Set
    End Property

    Public Property IsCancellationRequested() As Boolean
      Get
        If Not Me._cancellationTokenSource Is Nothing Then
          Return Me._cancellationTokenSource.IsCancellationRequested
        Else
          Return False
        End If
      End Get
      Set(ByVal value As Boolean)
        If Not Me._cancellationTokenSource Is Nothing AndAlso value = True Then
          Me._cancellationTokenSource.Cancel()
        End If
      End Set
    End Property

    Public Shared ReadOnly Property None() As CancellationToken
      Get
        Return _none
      End Get
    End Property

    Public Property WaitHandle() As WaitHandle
      Get
        Return Me._waitHandle
      End Get
      Set(ByVal value As WaitHandle)
        Me._waitHandle = value
      End Set
    End Property

    Public Overloads Function Register(ByVal callback As Action) As CancellationTokenRegistration
      Return New CancellationTokenRegistration(callback)
    End Function

    Public Overloads Function Register(ByVal callback As Action, ByVal useSynchronizationContext As Boolean) As CancellationTokenRegistration
      Throw New NotImplementedException()
    End Function

    Public Overloads Function Register(ByVal callback As Action(Of Object), ByVal state As Object) As CancellationTokenRegistration
      Throw New NotImplementedException()
    End Function

    Public Overloads Function Register(ByVal callback As Action(Of Object), ByVal state As Object, ByVal useSynchronizationContext As Boolean) As CancellationTokenRegistration
      Throw New NotImplementedException()
    End Function

    Public Sub ThrowIfCancellationRequested()
      If Not Me._cancellationTokenSource Is Nothing AndAlso Me._cancellationTokenSource.IsCancellationRequested Then
        Throw New OperationCanceledException(Me)
      End If
    End Sub

  End Structure

End Namespace