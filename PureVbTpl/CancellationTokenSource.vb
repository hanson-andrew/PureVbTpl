Namespace Tasks

  Public Class CancellationTokenSource
    Implements IDisposable

    Private disposedValue As Boolean

    Private _timer As System.Threading.Timer
    Private _isCancellationRequested As Boolean
    Private _token As CancellationToken

    Private _sourceTokens As CancellationToken()

    Public Sub New()
      Me.New(-1)
    End Sub

    Public Sub New(ByVal millisecondsDelay As Integer)
      Me.New(TimeSpan.FromMilliseconds(millisecondsDelay))
    End Sub

    Public Sub New(ByVal delay As TimeSpan)
      If delay.TotalMilliseconds > Int32.MaxValue Then
        Throw New ArgumentException(String.Format("Value must be less than {0}", Int32.MaxValue), "delay")
      End If
      If delay.TotalMilliseconds < -1 Then
        Throw New ArgumentException("Value must be greater than or equal to -1", "delay")
      End If
      Me._token = New CancellationToken(Me)
      Me._timer = New System.Threading.Timer(AddressOf Me.HandleDelayExpired, Nothing, Convert.ToInt64(delay.TotalMilliseconds), System.Threading.Timeout.Infinite)
      Me._isCancellationRequested = False
    End Sub

    Private Sub New(ByVal sourceTokens As CancellationToken())
      Me.New(-1)
      Me._sourceTokens = sourceTokens
    End Sub

    Public Overridable ReadOnly Property IsCancellationRequested() As Boolean
      Get
        Return Threading.Thread.VolatileRead(Me._isCancellationRequested)
      End Get
    End Property

    Public Overridable ReadOnly Property Token() As CancellationToken
      Get
        Return Me._token
      End Get
    End Property

    Public Overloads Sub Cancel()
      Threading.Thread.VolatileWrite(Me._isCancellationRequested, True)
    End Sub

    Public Overloads Sub Cancel(ByVal throwOnFirstException As Boolean)
      Threading.Thread.VolatileWrite(Me._isCancellationRequested, True)
    End Sub

    Public Overloads Sub CancelAfter(ByVal millisecondsDelay As Integer)
      Me.CancelAfter(TimeSpan.FromMilliseconds(millisecondsDelay))
    End Sub

    Public Overloads Sub CancelAfter(ByVal delay As TimeSpan)
      Me._timer.Change(delay, TimeSpan.FromMilliseconds(-1))
    End Sub

    Private Sub HandleDelayExpired(ByVal state As Object)
      Me.Cancel()
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
      If Not Me.disposedValue Then
        If disposing Then
          Me._timer.Dispose()
        End If
      End If
      Me.disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
      Dispose(True)
      GC.SuppressFinalize(Me)
    End Sub

    Public Shared Function CreateLinkedTokenSource(ByVal token1 As CancellationToken, ByVal token2 As CancellationToken) As CancellationTokenSource
      Return New CancellationTokenSource(New CancellationToken() {token1, token2})
    End Function

    Public Shared Function CreateLinkedTokenSource(ByVal ParamArray tokens As CancellationToken()) As CancellationTokenSource
      Return New CancellationTokenSource(tokens)
    End Function

  End Class

End Namespace