Imports System.Threading

Namespace Tasks

  Public MustInherit Class TaskScheduler

    Private Shared _default As TaskScheduler = New ThreadPoolTaskScheduler()
    Private Shared _current As TaskScheduler = _default

    Private _id As Integer

    Protected Sub New()

    End Sub

    Public Shared ReadOnly Property Current() As TaskScheduler
      Get
        Return TaskScheduler._current
      End Get
    End Property

    Public Shared ReadOnly Property [Default]() As TaskScheduler
      Get
        Return TaskScheduler._default
      End Get
    End Property

    Public ReadOnly Property Id() As Integer
      Get
        Return _id
      End Get
    End Property

    Public Overridable ReadOnly Property MaximumConcurrencyLevel() As Integer
      Get
        Return Integer.MaxValue
      End Get
    End Property

    Public Shared Event UnobservedTaskException As EventHandler(Of UnobservedTaskExceptionEventArgs)

    Public Shared Function FromCurrentSynchronizationContext() As TaskScheduler
      Return New SynchronizationContextTaskScheduler(SynchronizationContext.Current)
    End Function

    Protected Friend Overridable Function TryDequeue(ByVal task As Task) As Boolean
      Return False 'The default implementation is to do nothing.
    End Function

    Protected Function TryExecuteTask(ByVal task As Task) As Boolean
      task.Execute(Me)
    End Function

    Protected MustOverride Function TryExecuteTaskInline(ByVal task As Task, ByVal taskWasPreviouslyQueued As Boolean) As Boolean
    Protected MustOverride Function GetScheduledTasks() As IEnumerable(Of Task)
    Protected Friend MustOverride Sub QueueTask(ByVal task As Task)

  End Class

End Namespace