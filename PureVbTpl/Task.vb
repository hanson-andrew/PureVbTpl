Imports System.Threading

Namespace Tasks

  ''' <summary>
  ''' An implementation of the TPL for personal use.
  ''' </summary>
  ''' <remarks>
  ''' This is causing a slight memory leak.
  ''' My implementation seems similar to that of how Microsoft is actually
  ''' implementing their TPL.
  ''' http://blogs.msdn.com/b/pfxteam/archive/2012/03/25/10287435.aspx
  ''' The short story is that, even though we don't dispose of this object
  ''' the finalizer for the object will clean it up, it will just take longer
  ''' This could be an issue in high performance type systems, but 
  ''' generally that's not a cause for concern here.
  ''' </remarks>
  Public Class Task
    Implements IAsyncResult, IDisposable

#Region "Fields"
    Private disposedValue As Boolean

    Private Shared _factory As TaskFactory = New TaskFactory()
    Private Shared _currentId As Nullable(Of Integer)

    Private _waitHandle As EventWaitHandle = New EventWaitHandle(False, EventResetMode.ManualReset)

    Private _action As Action = Nothing
    Private _actionObject As Action(Of Object) = Nothing

    Private _continuationScheduler As TaskScheduler
    Private _continuation As Task

    Private _state As Object = Nothing
    Private _cancellationToken As CancellationToken
    Private _continuationOptions As TaskContinuationOptions
    Private _exception As AggregateException

    Private _isCanceled As Boolean
    Private _isCompleted As Boolean
    Private _isFaulted As Boolean
    Private _status As TaskStatus
    Private _isExecutingSynchronously As Boolean
    Private _completedSynchronously As Boolean
#End Region

#Region "Constructors"
    Protected Sub New(ByVal cancellationToken As CancellationToken, ByVal creationOptions As TaskCreationOptions)
      Me._cancellationToken = cancellationToken
      Me._continuationOptions = creationOptions
    End Sub

    Protected Sub New(ByVal state As Object, ByVal cancellationToken As CancellationToken, ByVal creationOptions As TaskCreationOptions)
      Me._state = state
      Me._cancellationToken = cancellationToken
      Me._continuationOptions = creationOptions
    End Sub

    Public Sub New(ByVal action As Action)
      Me.New(action, CancellationToken.None, TaskCreationOptions.None)
    End Sub

    Public Sub New(ByVal action As Action, ByVal cancellationToken As CancellationToken)
      Me.New(action, cancellationToken, TaskCreationOptions.None)
    End Sub

    Public Sub New(ByVal action As Action, ByVal creationOptions As TaskCreationOptions)
      Me.New(action, CancellationToken.None, creationOptions)
    End Sub

    Public Sub New(ByVal action As Action, ByVal cancellationToken As CancellationToken, ByVal creationOptions As TaskCreationOptions)
      Me._action = action
      Me._cancellationToken = cancellationToken
      Me._continuationOptions = creationOptions
    End Sub

    Public Sub New(ByVal action As Action(Of Object), ByVal state As Object)
      Me.New(action, state, CancellationToken.None, TaskCreationOptions.None)
    End Sub

    Public Sub New(ByVal action As Action(Of Object), ByVal state As Object, ByVal cancellationToken As CancellationToken)
      Me.New(action, state, cancellationToken, TaskCreationOptions.None)
    End Sub

    Public Sub New(ByVal action As Action(Of Object), ByVal state As Object, ByVal creationOptions As TaskCreationOptions)
      Me.New(action, state, CancellationToken.None, creationOptions)
    End Sub

    Public Sub New(ByVal action As Action(Of Object), ByVal state As Object, ByVal cancellationToken As CancellationToken, ByVal creationOptions As TaskCreationOptions)
      Me._actionObject = action
      Me._state = state
      Me._cancellationToken = cancellationToken
      Me._continuationOptions = creationOptions
    End Sub

    Friend Sub New(ByVal action As Action(Of Task), ByVal state As Task, ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions)
      Me.New(New ActionWrapper(action).GetAction(), state, cancellationToken, continuationOptions)
    End Sub
#End Region

    Private Class ActionWrapper
      Private _action As Action(Of Task)
      Public Sub New(ByVal action As Action(Of Task))
        Me._action = action
      End Sub
      Private Sub Execute(ByVal task As Object)
        Me._action.Invoke(task)
      End Sub
      Public Function GetAction() As Action(Of Object)
        Return AddressOf Me.Execute
      End Function
    End Class

#Region "Properties"
    Public ReadOnly Property AsyncState() As Object Implements IAsyncResult.AsyncState
      Get
        Return Me._state
      End Get
    End Property

    Public ReadOnly Property CreationOptions() As TaskCreationOptions
      Get
        Return Me._continuationOptions
      End Get
    End Property

    Public Shared ReadOnly Property CurrentId() As Nullable(Of Integer)
      Get
        Return Task._currentId
      End Get
    End Property

    Public ReadOnly Property Exception() As AggregateException
      Get
        Return Me._exception
      End Get
    End Property

    Public Shared ReadOnly Property Factory() As TaskFactory
      Get
        Return _factory
      End Get
    End Property

    Public ReadOnly Property Id() As Integer
      Get
        Throw New NotImplementedException()
      End Get
    End Property

    Public ReadOnly Property IsCanceled() As Boolean
      Get
        Return Me._isCanceled
      End Get
    End Property

    Public ReadOnly Property IsCompleted() As Boolean Implements System.IAsyncResult.IsCompleted
      Get
        Return Me._isCompleted
      End Get
    End Property

    Public ReadOnly Property IsFaulted() As Boolean
      Get
        Return Me._isFaulted
      End Get
    End Property

    Public ReadOnly Property Status() As TaskStatus
      Get
        Return Me._status
      End Get
    End Property

    Public ReadOnly Property AsyncWaitHandle() As System.Threading.WaitHandle Implements System.IAsyncResult.AsyncWaitHandle
      Get
        Throw New NotImplementedException()
      End Get
    End Property

    Public ReadOnly Property CompletedSynchronously() As Boolean Implements System.IAsyncResult.CompletedSynchronously
      Get
        Return Me._completedSynchronously
      End Get
    End Property

    Protected ReadOnly Property CancellationToken() As CancellationToken
      Get
        Return Me._cancellationToken
      End Get
    End Property

    Private ReadOnly Property WaitHandle() As EventWaitHandle
      Get
        Return Me._waitHandle
      End Get
    End Property
#End Region

#Region "Methods"
    Public Function ContinueWith(ByVal continuationAction As Action(Of Task)) As Task
      Return Me.ContinueWith(continuationAction, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current)
    End Function

    Public Function ContinueWith(ByVal continuationAction As Action(Of Task), ByVal cancellationToken As CancellationToken) As Task
      Return Me.ContinueWith(continuationAction, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current)
    End Function

    Public Function ContinueWith(ByVal continuationAction As Action(Of Task), ByVal continuationOptions As TaskContinuationOptions) As Task
      Return Me.ContinueWith(continuationAction, CancellationToken.None, continuationOptions, TaskScheduler.Current)
    End Function

    Public Function ContinueWith(ByVal continuationAction As Action(Of Task), ByVal scheduler As TaskScheduler) As Task
      Return Me.ContinueWith(continuationAction, CancellationToken.None, TaskContinuationOptions.None, scheduler)
    End Function

    Public Function ContinueWith(ByVal continuationAction As Action(Of Task), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task
      Me._continuationScheduler = scheduler
      Dim t As Task = New Task(continuationAction, Me, cancellationToken, continuationOptions)
      Me._continuation = t
      Return t
    End Function

    Public Function ContinueWith(Of TResult)(ByVal continuationFunction As Func(Of Task, TResult)) As Task(Of TResult)
      Return Me.ContinueWith(continuationFunction, Me._cancellationToken, Me._continuationOptions, TaskScheduler.Current)
    End Function

    Public Function ContinueWith(Of TResult)(ByVal continuationFunction As Func(Of Task, TResult), ByVal cancellationToken As CancellationToken) As Task(Of TResult)
      Return Me.ContinueWith(continuationFunction, cancellationToken, Me._continuationOptions, TaskScheduler.Current)
    End Function

    Public Function ContinueWith(Of TResult)(ByVal continuationFunction As Func(Of Task, TResult), ByVal continuationOptions As TaskContinuationOptions) As Task(Of TResult)
      Return Me.ContinueWith(continuationFunction, Me.CancellationToken, continuationOptions, TaskScheduler.Current)
    End Function

    Public Function ContinueWith(Of TResult)(ByVal continuationFunction As Func(Of Task, TResult), ByVal scheduler As TaskScheduler) As Task(Of TResult)
      Return Me.ContinueWith(continuationFunction, Me.CancellationToken, Me._continuationOptions, scheduler)
    End Function

    Public Function ContinueWith(Of TResult)(ByVal continuationFunction As Func(Of Task, TResult), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task(Of TResult)
      Me._continuationScheduler = scheduler
      Dim t As Task(Of TResult) = New Task(Of TResult)(continuationFunction, Me, cancellationToken, continuationOptions)
      Me._continuation = t
      Return t
    End Function

    Public Sub RunSynchronously()
      Me.RunSynchronously(TaskScheduler.Current)
    End Sub

    Public Sub RunSynchronously(ByVal scheduler As TaskScheduler)
      Me._isExecutingSynchronously = True
      scheduler.QueueTask(Me)
      Me._waitHandle.WaitOne()
    End Sub

    Public Sub Start()
      Me.Start(TaskScheduler.Current)
    End Sub

    Public Sub Start(ByVal scheduler As TaskScheduler)
      Me._isExecutingSynchronously = False
      scheduler.QueueTask(Me)
    End Sub

    Friend Overridable Sub Execute(ByVal scheduler As TaskScheduler)
      Me._waitHandle.Reset()
      If Not Me._cancellationToken.IsCancellationRequested Then
        If Not Me.IsAllowedToContinue() Then
          Me._isCanceled = True
          Me._waitHandle.Set()
          Throw New OperationCanceledException(Me._cancellationToken)
        End If
        Try
          InvokeAction()
        Catch ex As OperationCanceledException
          If Not ex.CancellationToken.Equals(Me._cancellationToken) Then
            Throw New OperationCanceledException(Me._cancellationToken) 'This was cancelled but on a different token somehow. Cancelling this token as well.
          Else
            Me._isCanceled = True
            Me._waitHandle.Set()
            Return
          End If
        Catch ex As Exception
          Me._isFaulted = True
          Me._exception = New AggregateException(ex)
        Finally
          Me._isCompleted = True
          If Me._isExecutingSynchronously Then
            Me._completedSynchronously = True
          End If
          Me._waitHandle.Set()
        End Try
        If Not IsNothing(Me._continuation) AndAlso Me.IsAllowedToContinue() Then
          If Me._isExecutingSynchronously Then
            Me._continuation.RunSynchronously(Me._continuationScheduler)
          Else
            Me._continuation.Start(Me._continuationScheduler)
          End If
        End If
      Else
        Me._isCanceled = True
        Me._waitHandle.Set()
        Me._cancellationToken.ThrowIfCancellationRequested()
      End If
    End Sub

    Protected Overridable Sub InvokeAction()
      If Not IsNothing(Me._action) Then
        Me._action.Invoke()
      ElseIf Not IsNothing(Me._actionObject) Then
        Me._actionObject.Invoke(Me._state)
      Else
        Throw New NotSupportedException("No action was specified")
      End If
    End Sub

    Public Sub Wait()
      Me.Wait(-1, CancellationToken.None)
    End Sub

    Public Sub Wait(ByVal cancellationToken As CancellationToken)
      Me.Wait(-1, cancellationToken)
    End Sub

    Public Function Wait(ByVal millisecondsTimeout As Integer) As Boolean
      Return Me.Wait(millisecondsTimeout, CancellationToken.None)
    End Function

    Public Function Wait(ByVal timeout As TimeSpan) As Boolean
      Return Me.Wait(timeout.Ticks * TimeSpan.TicksPerMillisecond, CancellationToken.None)
    End Function

    Public Function Wait(ByVal millisecondsTimeout As Integer, ByVal cancellationToken As CancellationToken) As Boolean
      Dim timeWaited As Integer = 0
      Dim waitOver As Boolean
      While Not waitOver AndAlso Not millisecondsTimeout = -1 AndAlso timeWaited <= millisecondsTimeout
        waitOver = Me._waitHandle.WaitOne(100)
        cancellationToken.ThrowIfCancellationRequested()
        millisecondsTimeout += 100
      End While
      Return waitOver
    End Function

    Public Shared Sub WaitAll(ByVal ParamArray tasks As Task())
      WaitAll(tasks, -1)
    End Sub

    Public Shared Function WaitAll(ByVal tasks As Task(), ByVal millisecondsTimeout As Integer) As Boolean
      Return WaitAll(tasks, millisecondsTimeout, CancellationToken.None)
    End Function

    Public Shared Sub WaitAll(ByVal tasks As Task(), ByVal cancellationToken As CancellationToken)
      WaitAll(tasks, -1, cancellationToken)
    End Sub

    Public Shared Function WaitAll(ByVal tasks As Task(), ByVal timeout As TimeSpan) As Boolean
      Return WaitAll(tasks, timeout.Ticks * TimeSpan.TicksPerMillisecond, CancellationToken.None)
    End Function

    Public Shared Function WaitAll(ByVal tasks As Task(), ByVal millisecondsTimeout As Integer, ByVal cancellationToken As CancellationToken) As Boolean
      Dim waitHandles As List(Of EventWaitHandle) = New List(Of EventWaitHandle)
      For Each task As Task In tasks
        waitHandles.Add(task.WaitHandle)
      Next
      Dim timeWaited As Integer = 0
      Dim waitOver As Boolean = False
      While Not waitOver AndAlso (millisecondsTimeout = -1 OrElse timeWaited <= millisecondsTimeout)
        waitOver = System.Threading.WaitHandle.WaitAll(waitHandles.ToArray(), 100)
        cancellationToken.ThrowIfCancellationRequested()
        millisecondsTimeout += 100
      End While
      Return waitOver
    End Function

    Public Shared Function WaitAny(ByVal ParamArray tasks As Task()) As Integer
      Return WaitAny(tasks, -1, CancellationToken.None)
    End Function

    Public Shared Function WaitAny(ByVal tasks As Task(), ByVal millisecondsTimeout As Integer) As Integer
      Return WaitAny(tasks, millisecondsTimeout, CancellationToken.None)
    End Function

    Public Shared Function WaitAny(ByVal tasks As Task(), ByVal cancellationToken As CancellationToken) As Integer
      Return WaitAny(tasks, -1, cancellationToken)
    End Function

    Public Shared Function WaitAny(ByVal tasks As Task(), ByVal timeout As TimeSpan) As Integer
      Return WaitAny(tasks, timeout.Ticks * TimeSpan.TicksPerMillisecond, CancellationToken.None)
    End Function

    Public Shared Function WaitAny(ByVal tasks As Task(), ByVal millisecondsTimeout As Integer, ByVal cancellationToken As CancellationToken) As Integer
      Dim waitHandles As List(Of EventWaitHandle) = New List(Of EventWaitHandle)
      For Each task As Task In tasks
        waitHandles.Add(task.WaitHandle)
      Next
      Dim timeWaited As Integer = 0
      Dim waitOver As Boolean = False
      While Not waitOver AndAlso Not millisecondsTimeout = -1 AndAlso timeWaited <= millisecondsTimeout
        waitOver = System.Threading.WaitHandle.WaitAny(waitHandles.ToArray(), 100)
        cancellationToken.ThrowIfCancellationRequested()
        millisecondsTimeout += 100
      End While
      Return waitOver
    End Function

    Friend Function IsAllowedToContinue() As Boolean
      If (Me._continuationOptions And TaskContinuationOptions.NotOnRanToCompletion).Equals(TaskContinuationOptions.NotOnRanToCompletion) AndAlso Not Me.IsCanceled AndAlso Not Me.IsFaulted Then
        Return False
      ElseIf (Me._continuationOptions And TaskContinuationOptions.NotOnFaulted).Equals(TaskContinuationOptions.NotOnFaulted) AndAlso Me.IsFaulted Then
        Return False
      ElseIf (Me._continuationOptions And TaskContinuationOptions.NotOnCanceled).Equals(TaskContinuationOptions.NotOnCanceled) AndAlso Me.IsCanceled Then
        Return False
      ElseIf (Me._continuationOptions And TaskContinuationOptions.OnlyOnCanceled).Equals(TaskContinuationOptions.OnlyOnCanceled) AndAlso Not Me.IsCanceled Then
        Return False
      ElseIf (Me._continuationOptions And TaskContinuationOptions.OnlyOnFaulted).Equals(TaskContinuationOptions.OnlyOnFaulted) AndAlso Not Me.IsFaulted Then
        Return False
      ElseIf (Me._continuationOptions And TaskContinuationOptions.OnlyOnRanToCompletion).Equals(TaskContinuationOptions.OnlyOnRanToCompletion) AndAlso (Me.IsCanceled Or Me.IsFaulted) Then
        Return False
      End If
      Return True
    End Function

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
      If Not Me.disposedValue Then
        If disposing Then
          ' TODO: free other state (managed objects).

        End If

        ' TODO: free your own state (unmanaged objects).
        ' TODO: set large fields to null.
      End If
      Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
      ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
      Dispose(True)
      GC.SuppressFinalize(Me)
    End Sub
#End Region

  End Class

End Namespace