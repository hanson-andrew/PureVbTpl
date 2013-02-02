Namespace Tasks

  Public Class TaskFactory(Of TResult)

    Private _cancellationToken As CancellationToken
    Private _creationOptions As TaskCreationOptions
    Private _continuationOptions As TaskContinuationOptions
    Private _scheduler As TaskScheduler

    Public Sub New()
      Me.New(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Current)
    End Sub

    Public Sub New(ByVal cancellationToken As CancellationToken)
      Me.New(cancellationToken, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Current)
    End Sub

    Public Sub New(ByVal scheduler As TaskScheduler)
      Me.New(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, scheduler)
    End Sub

    Public Sub New(ByVal creationOptions As TaskCreationOptions, ByVal continuationOptions As TaskContinuationOptions)
      Me.New(CancellationToken.None, creationOptions, continuationOptions, TaskScheduler.Current)
    End Sub

    Public Sub New(ByVal cancellationToken As CancellationToken, ByVal creationOptions As TaskCreationOptions, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler)
      Me._cancellationToken = cancellationToken
      Me._creationOptions = creationOptions
      Me._continuationOptions = continuationOptions
      Me._scheduler = scheduler
    End Sub

    Public Function ContinueWhenAny(ByVal tasks As Task(), ByVal continuationFunction As Func(Of Task, TResult), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

  End Class

End Namespace