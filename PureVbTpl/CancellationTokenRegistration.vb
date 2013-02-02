Namespace Tasks

  Public Structure CancellationTokenRegistration
    Implements IEquatable(Of CancellationTokenRegistration), IDisposable

    Private _callback As Action

    Public Sub New(ByVal callback As Action)
      Me._callback = callback
    End Sub

    Public ReadOnly Property Callback() As Action
      Get
        Return Me._callback
      End Get
    End Property

    Public Sub Dispose() Implements System.IDisposable.Dispose

    End Sub

    Public Overloads Function Equals(ByVal other As CancellationTokenRegistration) As Boolean Implements System.IEquatable(Of CancellationTokenRegistration).Equals

    End Function

  End Structure

End Namespace