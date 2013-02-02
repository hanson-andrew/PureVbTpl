Namespace Tasks

  Public Class TaskFactory

    Private _cancellationToken As CancellationToken
    Private _continuationOptions As TaskContinuationOptions
    Private _creationOptions As TaskCreationOptions
    Private _scheduler As TaskScheduler

    Private _continuationAction As Action(Of Task())
    Private _continuationFunction As Func(Of Task(), Object)
    Private _continuationTasks As Task()

    Public Sub New()
      Me.New(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Current)
    End Sub

    Public Sub New(ByVal cancellationToken As CancellationToken)
      Me.New(cancellationToken, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Current)
    End Sub

    Public Sub New(ByVal taskScheduler As TaskScheduler)
      Me.New(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, taskScheduler)
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

    Public ReadOnly Property CancellationToken() As CancellationToken
      Get
        Return Me._cancellationToken
      End Get
    End Property

    Public ReadOnly Property ContinuationOptions() As TaskContinuationOptions
      Get
        Return Me._continuationOptions
      End Get
    End Property

    Public ReadOnly Property CreationOptions() As TaskCreationOptions
      Get
        Return Me._creationOptions
      End Get
    End Property

    Public ReadOnly Property Scheduler() As TaskScheduler
      Get
        Return Me._scheduler
      End Get
    End Property

    Public Function ContinueWhenAll(ByVal tasks As Task(), ByVal continuationAction As Action(Of Task())) As Task
      Return Me.ContinueWhenAll(tasks, continuationAction, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current)
    End Function

    Public Function ContinueWhenAll(ByVal tasks As Task(), ByVal continuationAction As Action(Of Task()), ByVal cancellationToken As CancellationToken) As Task
      Return Me.ContinueWhenAll(tasks, continuationAction, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current)
    End Function

    Public Function ContinueWhenAll(ByVal tasks As Task(), ByVal continuationAction As Action(Of Task()), ByVal continuationOptions As TaskContinuationOptions) As Task
      Return Me.ContinueWhenAll(tasks, continuationAction, CancellationToken.None, continuationOptions, TaskScheduler.Current)
    End Function

    Public Function ContinueWhenAll(ByVal tasks As Task(), ByVal continuationAction As Action(Of Task()), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task
      Dim t As Task = New Task(AddressOf Me.WaitAll, tasks, cancellationToken)
      Me._continuationTasks = tasks
      Return t.ContinueWith(AddressOf Me.ContinueWhenAllWrapper, cancellationToken, continuationOptions, scheduler)
    End Function

    Private Sub ContinueWhenAllWrapper(ByVal tasks As Object)
      Me._continuationAction(Me._continuationTasks)
    End Sub

    Private Function ContinueWhenAllWrapper(Of TResult)(ByVal tasks As Object) As TResult
      Return Me._continuationFunction.Invoke(tasks)
    End Function

    Private Sub WaitAll(ByVal tasks As Object)
      Task.WaitAll(CType(tasks, Task()))
    End Sub

    Private Sub WaitAny(ByVal tasks As Object)
      Task.WaitAny(CType(tasks, Task()))
    End Sub

    Public Function ContinueWhenAll(Of TResult)(ByVal tasks As Task(), ByVal continuationFunction As Func(Of Task(), TResult)) As Task(Of TResult)
      Return Me.ContinueWhenAll(Of TResult)(tasks, continuationFunction, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current)
    End Function

    Public Function ContinueWhenAll(Of TResult)(ByVal tasks As Task(), ByVal continuationFunction As Func(Of Task(), TResult), ByVal cancellationToken As CancellationToken) As Task(Of TResult)
      Return Me.ContinueWhenAll(Of TResult)(tasks, continuationFunction, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current)
    End Function

    Public Function ContinueWhenAll(Of TResult)(ByVal tasks As Task(), ByVal continuationFunction As Func(Of Task(), TResult), ByVal continuationOptions As TaskContinuationOptions) As Task(Of TResult)
      Return Me.ContinueWhenAll(Of TResult)(tasks, continuationFunction, CancellationToken.None, continuationOptions, TaskScheduler.Current)
    End Function

    Public Function ContinueWhenAll(Of TResult)(ByVal tasks As Task(), ByVal continuationFunction As Func(Of Task(), TResult), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task(Of TResult)
      Dim t As Task = New Task(AddressOf Me.WaitAll, tasks, cancellationToken)
      Me._continuationTasks = tasks
      Return t.ContinueWith(Of TResult)(AddressOf Me.ContinueWhenAllWrapper(Of TResult), cancellationToken, continuationOptions, scheduler)
    End Function

    Public Function ContinueWhenAll(Of TAntecedentResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationAction As Action(Of Task(Of TAntecedentResult)())) As Task
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAll(Of TAntecedentResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationAction As Action(Of Task(Of TAntecedentResult)()), ByVal cancellationToken As CancellationToken) As Task
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAll(Of TAntecedentResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationAction As Action(Of Task(Of TAntecedentResult)), ByVal continuationOptions As TaskContinuationOptions) As Task
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAll(Of TAntecedentResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationAction As Action(Of Task(Of TAntecedentResult)()), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAll(Of TAntecedentResult, TResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationFunction As Func(Of Task(Of TAntecedentResult)(), TResult)) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAll(Of TAntecedentResult, TResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationFunction As Func(Of Task(Of TAntecedentResult)(), TResult), ByVal cancellationToken As CancellationToken) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAll(Of TAntecedentResult, TResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationFunction As Func(Of Task(Of TAntecedentResult)(), TResult), ByVal continuationOptions As TaskContinuationOptions) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAll(Of TAntecedentResult, TResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationFunction As Func(Of Task(Of TAntecedentResult)(), TResult), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(ByVal task As Task(), ByVal continuationAction As Action(Of Task)) As Task
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(ByVal task As Task(), ByVal continuationAction As Action(Of Task), ByVal cancellationToken As CancellationToken) As Task
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(ByVal tasks As Task(), ByVal continuationAction As Action(Of Task), ByVal continuationOptions As TaskContinuationOptions) As Task
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(ByVal tasks As Task(), ByVal continuationAction As Action(Of Task), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TResult)(ByVal tasks As Task(), ByVal continuationFunction As Func(Of Task, TResult)) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TResult)(ByVal tasks As Task(), ByVal continuationFunction As Func(Of Task, TResult), ByVal cancellationToken As CancellationToken) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TResult)(ByVal tasks As Task(), ByVal continuationFunction As Func(Of Task, TResult), ByVal continuationOptions As TaskContinuationOptions) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TResult)(ByVal tasks As Task(), ByVal continuationFunction As Func(Of Task, TResult), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TAntecedentResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationAction As Action(Of Task(Of TAntecedentResult))) As Task
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TAntecedentResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationAction As Action(Of Task(Of TAntecedentResult)), ByVal cancellationToken As CancellationToken) As Task
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TAntecedentResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationAction As Action(Of Task(Of TAntecedentResult)), ByVal continuationOptions As TaskContinuationOptions) As Task
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TAntecedentResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationAction As Action(Of Task(Of TAntecedentResult)), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TAntecedentResult, TResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationFunction As Func(Of Task(Of TAntecedentResult), TResult)) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TAntecedentResult, TResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationFunction As Func(Of Task(Of TAntecedentResult), TResult), ByVal cancellationToken As CancellationToken) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TAntecedentResult, TResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationFunction As Func(Of Task(Of TAntecedentResult), TResult), ByVal continuationOptions As TaskContinuationOptions) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function ContinueWhenAny(Of TAntecedentResult, TResult)(ByVal tasks As Task(Of TAntecedentResult)(), ByVal continuationFunction As Func(Of Task(Of TAntecedentResult), TResult), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(ByVal asyncResult As IAsyncResult, ByVal endMethod As Action(Of IAsyncResult)) As Task
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(ByVal beginMethod As Func(Of AsyncCallback, Object, IAsyncResult), ByVal endMethod As Action(Of IAsyncResult), ByVal state As Object) As Task
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(ByVal asyncResult As IAsyncResult, ByVal endMethod As Action(Of IAsyncResult), ByVal creationOptions As TaskCreationOptions) As Task
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(ByVal beginMethod As Func(Of AsyncCallback, Object, IAsyncResult), ByVal endMethod As Action(Of IAsyncResult), ByVal state As Object, ByVal creationOptions As TaskCreationOptions) As Task
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(ByVal asyncMethod As IAsyncResult, ByVal endMethod As Action(Of IAsyncResult), ByVal creationOptions As TaskCreationOptions, ByVal scheduelr As TaskScheduler) As Task
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TResult)(ByVal asyncResult As IAsyncResult, ByVal endMethod As Func(Of IAsyncResult, TResult)) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TResult)(ByVal beginMethod As Func(Of AsyncCallback, Object, IAsyncResult), ByVal endMethod As Func(Of IAsyncResult, TResult), ByVal state As Object) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TResult)(ByVal asyncResult As IAsyncResult, ByVal endMethod As Func(Of IAsyncResult, TResult), ByVal creationOptions As TaskCreationOptions) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TResult)(ByVal beginMethod As Func(Of AsyncCallback, Object, IAsyncResult), ByVal endMethod As Func(Of IAsyncResult, TResult), ByVal state As Object, ByVal creationOptions As TaskCreationOptions) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1)(ByVal beginMethod As Func(Of TArg1, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Action(Of IAsyncResult), ByVal arg1 As TArg1, ByVal state As Object) As Task
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TResult)(ByVal asyncResult As IAsyncResult, ByVal endMethod As Func(Of IAsyncResult, TResult), ByVal creationOptions As TaskCreationOptions, ByVal scheduler As TaskScheduler)
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1)(ByVal beginMethod As Func(Of TArg1, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Action(Of IAsyncResult), ByVal arg1 As TArg1, ByVal state As Object, ByVal creationOptions As TaskCreationOptions) As Task
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1, TResult)(ByVal beginMethod As Func(Of TArg1, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Func(Of IAsyncResult, TResult), ByVal arg1 As TArg1, ByVal state As Object) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1, TResult)(ByVal beginMethod As Func(Of TArg1, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Func(Of IAsyncResult, TResult), ByVal arg1 As TArg1, ByVal state As Object, ByVal creationOptiosn As TaskCreationOptions) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1, TArg2)(ByVal beginMethod As Func(Of TArg1, TArg2, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Action(Of IAsyncResult), ByVal arg1 As TArg1, ByVal arg2 As TArg2, ByVal state As Object) As Task
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1, TArg2)(ByVal beginMethod As Func(Of TArg1, TArg2, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Action(Of IAsyncResult), ByVal arg1 As TArg1, ByVal arg2 As TArg2, ByVal state As Object, ByVal creationOptions As TaskCreationOptions) As Task
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1, TArg2, TResult)(ByVal beginMethod As Func(Of TArg1, TArg2, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Func(Of IAsyncResult, TResult), ByVal arg1 As TArg1, ByVal arg2 As TArg2, ByVal state As Object) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1, TArg2, TResult)(ByVal beginMethod As Func(Of TArg1, TArg2, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Func(Of IAsyncResult, TResult), ByVal arg1 As TArg1, ByVal arg2 As TArg2, ByVal state As Object, ByVal creationOptions As TaskCreationOptions) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1, TArg2, TArg3)(ByVal beginMethod As Func(Of TArg1, TArg2, TArg3, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Action(Of IAsyncResult), ByVal arg1 As TArg1, ByVal arg2 As TArg2, ByVal arg3 As TArg3, ByVal state As Object) As Task
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1, TArg2, TArg3)(ByVal beginMethod As Func(Of TArg1, TArg2, TArg3, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Action(Of IAsyncResult), ByVal arg1 As TArg1, ByVal arg2 As TArg2, ByVal arg3 As TArg3, ByVal state As Object, ByVal creationOptions As TaskCreationOptions) As Task
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1, TArg2, TArg3, TResult)(ByVal beginMethod As Func(Of TArg1, TArg2, TArg3, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Func(Of IAsyncResult, TResult), ByVal arg1 As TArg1, ByVal arg2 As TArg2, ByVal arg3 As TArg3, ByVal state As Object) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function FromAsync(Of TArg1, TArg2, TArg3, TResult)(ByVal beginMethod As Func(Of TArg1, TArg2, TArg3, AsyncCallback, Object, IAsyncResult), ByVal endMethod As Func(Of IAsyncResult, TResult), ByVal arg1 As TArg1, ByVal arg2 As TArg2, ByVal arg3 As TArg3, ByVal state As Object, ByVal creationOptions As TaskCreationOptions) As Task(Of TResult)
      Throw New NotImplementedException()
    End Function

    Public Function StartNew(ByVal action As Action) As Task
      Return Me.StartNew(action, Me.CancellationToken, Me.CreationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(ByVal action As Action, ByVal cancellationToken As CancellationToken) As Task
      Return Me.StartNew(action, cancellationToken, Me.CreationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(ByVal action As Action, ByVal creationOptions As TaskCreationOptions) As Task
      Return Me.StartNew(action, Me.CancellationToken, creationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(ByVal action As Action, ByVal cancellationToken As CancellationToken, ByVal creationOptions As TaskCreationOptions, ByVal scheduler As TaskScheduler) As Task
      Dim t As New Task(action, cancellationToken, creationOptions)
      t.Start(scheduler)
      Return t
    End Function

    Public Function StartNew(ByVal action As Action(Of Object), ByVal state As Object) As Task
      Return Me.StartNew(action, state, Me.CancellationToken, Me.CreationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(ByVal action As Action(Of Object), ByVal state As Object, ByVal cancellationToken As CancellationToken) As Task
      Return Me.StartNew(action, state, cancellationToken, Me.CreationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(ByVal action As Action(Of Object), ByVal state As Object, ByVal creationOptions As TaskCreationOptions) As Task
      Return Me.StartNew(action, state, Me.CancellationToken, creationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(ByVal action As Action(Of Object), ByVal state As Object, ByVal cancellationToken As CancellationToken, ByVal creationOptions As TaskCreationOptions, ByVal scheduler As TaskScheduler) As Task
      Dim t As New Task(action, state, cancellationToken, creationOptions)
      t.Start(scheduler)
      Return t
    End Function

    Public Function StartNew(Of TResult)(ByVal [function] As Func(Of TResult)) As Task(Of TResult)
      Return Me.StartNew(Of TResult)([function], Me.CancellationToken, Me.CreationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(Of TResult)(ByVal [function] As Func(Of TResult), ByVal cancellationToken As CancellationToken) As Task(Of TResult)
      Return Me.StartNew(Of TResult)([function], cancellationToken, Me.CreationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(Of TResult)(ByVal [function] As Func(Of TResult), ByVal creationOptions As TaskCreationOptions) As Task(Of TResult)
      Return Me.StartNew(Of TResult)([function], Me.CancellationToken, creationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(Of TResult)(ByVal [function] As Func(Of TResult), ByVal cancellationToken As CancellationToken, ByVal creationOptions As TaskCreationOptions, ByVal scheduler As TaskScheduler) As Task(Of TResult)
      Dim t As Task(Of TResult) = New Task(Of TResult)([function], cancellationToken, creationOptions)
      t.Start(scheduler)
      Return t
    End Function

    Public Function StartNew(Of TResult)(ByVal [function] As Func(Of Object, TResult), ByVal state As Object) As Task(Of TResult)
      Return Me.StartNew(Of TResult)([function], state, Me.CancellationToken, Me.CreationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(Of TResult)(ByVal [function] As Func(Of Object, TResult), ByVal state As Object, ByVal cancellationToken As CancellationToken) As Task(Of TResult)
      Return Me.StartNew(Of TResult)([function], state, cancellationToken, Me.CreationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(Of TResult)(ByVal [function] As Func(Of Object, TResult), ByVal state As Object, ByVal creationOptions As TaskCreationOptions) As Task(Of TResult)
      Return Me.StartNew(Of TResult)([function], state, Me.CancellationToken, creationOptions, Me.Scheduler)
    End Function

    Public Function StartNew(Of TResult)(ByVal [function] As Func(Of Object, TResult), ByVal state As Object, ByVal cancellationToken As CancellationToken, ByVal creationOptions As TaskCreationOptions, ByVal scheduler As TaskScheduler) As Task(Of TResult)
      Dim t As Task(Of TResult) = New Task(Of TResult)([function], state, cancellationToken, creationOptions)
      t.Start(scheduler)
      Return t
    End Function


  End Class

End Namespace