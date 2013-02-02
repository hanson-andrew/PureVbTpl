Imports System.Threading

Namespace Tasks

  ''' <summary>
  ''' Provides a task scheduler that ensures a maximum concurrency level While
  ''' running on top of the ThreadPool.
  ''' </summary>
  Public Class LimitedConcurrencyLevelTaskScheduler
    Inherits TaskScheduler

    ''' <summary>Whether the current thread is processing work items.</summary>
    <ThreadStatic()> _
    Private Shared _currentThreadIsProcessingItems As Boolean
    ''' <summary>The list of tasks to be executed.</summary>
    Private ReadOnly _tasks As LinkedList(Of Task) = New LinkedList(Of Task)() ' protected by lock(_tasks)
    ''' <summary>The maximum concurrency level allowed by this scheduler.</summary>
    Private ReadOnly _maxDegreeOfParallelism As Integer
    ''' <summary>Whether the scheduler is currently processing work items.</summary>
    Private _delegatesQueuedOrRunning As Integer = 0 ' protected by lock(_tasks)

    ''' <summary>
    ''' Initializes an instance of the LimitedConcurrencyLevelTaskScheduler class with the
    ''' specified degree of parallelism.
    ''' </summary>
    ''' <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param>
    Public Sub New(ByVal maxDegreeOfParallelism As Integer)

      If (maxDegreeOfParallelism < 1) Then
        Throw New ArgumentOutOfRangeException("maxDegreeOfParallelism")
      End If
      _maxDegreeOfParallelism = maxDegreeOfParallelism

    End Sub
    ''' <summary>Queues a task to the scheduler.</summary>
    ''' <param name="t">The task to be queued.</param>
    Protected Friend Overrides Sub QueueTask(ByVal t As Task)

      ' Add the task to the list of tasks to be processed.  If there aren't enough
      ' delegates currently queued or running to process tasks, schedule another.
      SyncLock (_tasks)

        _tasks.AddLast(t)
        If (_delegatesQueuedOrRunning < _maxDegreeOfParallelism) Then

          _delegatesQueuedOrRunning = _delegatesQueuedOrRunning + 1
          NotifyThreadPoolOfPendingWork()
        End If
      End SyncLock
    End Sub

    ''' <summary>
    ''' Informs the ThreadPool that there's work to be executed for this scheduler.
    ''' </summary>
    Private Sub NotifyThreadPoolOfPendingWork()
      ThreadPool.UnsafeQueueUserWorkItem(AddressOf ProcessInternal, Nothing)
    End Sub

    Private Sub ProcessInternal()
      ' Note that the current thread is now processing work items.
      ' This is necessary to enable inlining of tasks into this thread.
      _currentThreadIsProcessingItems = True
      Try

        ' Process all available items in the queue.
        While (True)
          Dim item As Task
          SyncLock (_tasks)
            ' When there are no more items to be processed,
            ' note that we're done processing, and get out.
            If (_tasks.Count = 0) Then
              _delegatesQueuedOrRunning = _delegatesQueuedOrRunning - 1
              Exit While
            End If

            ' Get the next item from the queue
            item = _tasks.First.Value
            _tasks.RemoveFirst()
          End SyncLock

          ' Execute the task we pulled out of the queue
          MyBase.TryExecuteTask(item)

        End While
        ' We're done processing items on the current thread
      Finally
        _currentThreadIsProcessingItems = False
      End Try
    End Sub

    ''' <summary>Attempts to execute the specified task on the current thread.</summary>
    ''' <param name="t">The task to be executed.</param>
    ''' <param name="taskWasPreviouslyQueued"></param>
    ''' <returns>Whether the task could be executed on the current thread.</returns>
    Protected Overrides Function TryExecuteTaskInline(ByVal t As Task, ByVal taskWasPreviouslyQueued As Boolean) As Boolean

      ' If this thread isn't already processing a task, we don't support inlining
      If (Not _currentThreadIsProcessingItems) Then
        Return False
      End If

      ' If the task was previously queued, remove it from the queue
      If (taskWasPreviouslyQueued) Then
        TryDequeue(t)
      End If
      ' Try to run the task.
      Return MyBase.TryExecuteTask(t)
    End Function

    ''' <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
    ''' <param name="t">The task to be removed.</param>
    ''' <returns>Whether the task could be found and removed.</returns>
    Protected Friend Overrides Function TryDequeue(ByVal t As Task) As Boolean

      SyncLock (_tasks)
        Return _tasks.Remove(t)
      End SyncLock

    End Function

    ''' <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
    Public Overrides ReadOnly Property MaximumConcurrencyLevel() As Integer
      Get
        Return _maxDegreeOfParallelism
      End Get
    End Property

    ''' <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary>
    ''' <returns>An enumerable of the tasks currently scheduled.</returns>
    Protected Overrides Function GetScheduledTasks() As IEnumerable(Of Task)

      Dim lockTaken As Boolean = False
      Try

        Monitor.TryEnter(_tasks, lockTaken)
        If (lockTaken) Then
          Return _tasks.ToArray()
        Else
          Throw New NotSupportedException()
        End If
      Finally

        If (lockTaken) Then
          Monitor.Exit(_tasks)
        End If
      End Try
    End Function
  End Class

End Namespace