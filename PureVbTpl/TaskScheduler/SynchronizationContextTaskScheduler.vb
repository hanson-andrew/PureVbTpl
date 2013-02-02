Imports System.Threading

Namespace Tasks

  Public Class SynchronizationContextTaskScheduler
    Inherits TaskScheduler

    Private _context As SynchronizationContext
    Private _tasks As List(Of Task) = New List(Of Task)

    Public Sub New(ByVal synchronizationContext As SynchronizationContext)
      Me._context = synchronizationContext
    End Sub

    Protected Overrides Function GetScheduledTasks() As System.Collections.Generic.IEnumerable(Of Task)
      Return Nothing
      'Dim lockTaken As Boolean = False
      'Try
      ' Monitor.TryEnter(_tasks, lockTaken)
      ' If (lockTaken) Then
      ' Return _tasks.ToArray()
      ' Else
      ' Throw New NotSupportedException()
      ' End If
      ' Finally
      ' If (lockTaken) Then
      ' Monitor.Exit(_tasks)
      ' End If
      ' End Try
    End Function

    Protected Friend Overrides Sub QueueTask(ByVal task As Task)
      If Me._context.Equals(SynchronizationContext.Current) Then
        MyBase.TryExecuteTask(task)
      Else
        Me._context.Post(AddressOf MyBase.TryExecuteTask, task)
      End If
      'SyncLock (_tasks)
      '_tasks.Add(task)
      'NotifyContextOfPendingWork()
      'End SyncLock
    End Sub

    'Private Sub NotifyContextOfPendingWork()      
    '  Me._context.Send(AddressOf ProcessInternal, Nothing)
    'End Sub

    'Private Sub ProcessInternal()
    ' Process all available items in the queue.
    'While (_tasks.Count > 0)
    'Dim item As Task
    '    SyncLock (_tasks)
    ' Get the next item from the queue
    '      item = _tasks.First
    '      _tasks.RemoveAt(0)
    '    End SyncLock
    ' Execute the task we pulled out of the queue
    '    MyBase.TryExecuteTask(item)
    '  End While
    'End Sub

    Protected Overrides Function TryExecuteTaskInline(ByVal task As Task, ByVal taskWasPreviouslyQueued As Boolean) As Boolean
      If (taskWasPreviouslyQueued) Then
        TryDequeue(task)
      End If
      ' Try to run the task.
      Return MyBase.TryExecuteTask(task)
    End Function

    ''' <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
    ''' <param name="task">The task to be removed.</param>
    ''' <returns>Whether the task could be found and removed.</returns>
    Protected Friend Overrides Function TryDequeue(ByVal task As Task) As Boolean
      'SyncLock (_tasks)
      ' Return _tasks.Remove(task)
      ' End SyncLock
    End Function
  End Class

End Namespace