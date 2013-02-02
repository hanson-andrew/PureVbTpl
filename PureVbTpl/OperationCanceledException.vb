Namespace Tasks

  Public Class OperationCanceledException
    Inherits System.OperationCanceledException

    Private _cancellationToken As CancellationToken

    Public Sub New(ByVal cancellationToken As CancellationToken)
      MyBase.New()
    End Sub

    Public Sub New(ByVal message As String, ByVal cancellationToken As CancellationToken)
      MyBase.New(message)
      Me._cancellationToken = cancellationToken
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception, ByVal cancellationToken As CancellationToken)
      MyBase.New(message, innerException)
      Me._cancellationToken = cancellationToken
    End Sub

    Public ReadOnly Property CancellationToken() As CancellationToken
      Get
        Return Me._cancellationToken
      End Get
    End Property

  End Class

End Namespace