Namespace Tasks

  Public Class UnobservedTaskExceptionEventArgs
    Inherits EventArgs

    Private _exception As AggregateException
    Private _observed As Boolean

    Public Sub New(ByVal exception As AggregateException)
      Me._exception = exception
    End Sub

    Public ReadOnly Property Exception() As AggregateException
      Get
        Return Me._exception
      End Get
    End Property

    Public ReadOnly Property Observed() As Boolean
      Get
        Return Me._observed
      End Get
    End Property

    Public Sub SetObserved()
      Me._observed = True
    End Sub

  End Class

End Namespace