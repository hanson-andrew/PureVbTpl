Imports System.Threading

Namespace Tasks

  ''' <summary>
  ''' Provides a task scheduler that ensures a maximum concurrency level While
  ''' running on top of the ThreadPool.
  ''' </summary>
  Public Class ThreadPoolTaskScheduler
    Inherits TaskScheduler

    ''' <summary>The list of tasks to be executed.</summary>
    Private ReadOnly _tasks As LinkedList(Of Task) = New LinkedList(Of Task)() ' protected by lock(_tasks)

    Public Sub New()

    End Sub
    Protected Friend Overrides Sub QueueTask(ByVal t As Task)
      SyncLock (_tasks)
        _tasks.AddLast(t)
        NotifyThreadPoolOfPendingWork()
      End SyncLock
    End Sub

    Private Sub NotifyThreadPoolOfPendingWork()
      ThreadPool.UnsafeQueueUserWorkItem(AddressOf ProcessInternal, Nothing)
    End Sub

    Private Sub ProcessInternal()
      While (True)
        Dim item As Task
        SyncLock (_tasks)
          ' When there are no more items to be processed,
          ' note that we're done processing, and get out.
          ' This is done as an Exit While instead of in the while condition
          ' because we don't want to acquire the lock until we are inside the while
          ' loop (don't want to lock it outside).
          If (_tasks.Count = 0) Then
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
    End Sub

    ''' <summary>Attempts to execute the specified task on the current thread.</summary>
    ''' <param name="t">The task to be executed.</param>
    ''' <param name="taskWasPreviouslyQueued"></param>
    ''' <returns>Whether the task could be executed on the current thread.</returns>
    Protected Overrides Function TryExecuteTaskInline(ByVal t As Task, ByVal taskWasPreviouslyQueued As Boolean) As Boolean
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