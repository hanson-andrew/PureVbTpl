Namespace Tasks

  Public Class Task(Of TResult)
    Inherits Task

    Private Shared _factory As TaskFactory(Of TResult) = New TaskFactory(Of TResult)

    Private _function As Func(Of TResult)
    Private _objectFunction As Func(Of Object, TResult)
    Private _state As Object

    Private _continuationScheduler As TaskScheduler
    Private _continuation As Task

    Private _result As TResult

    Public Sub New(ByVal [function] As Func(Of TResult))
      Me.New([function], CancellationToken.None, TaskCreationOptions.None)
    End Sub

    Public Sub New(ByVal [function] As Func(Of TResult), ByVal cancellationToken As CancellationToken)
      Me.New([function], cancellationToken, TaskCreationOptions.None)
    End Sub

    Public Sub New(ByVal [function] As Func(Of TResult), ByVal creationOptions As TaskCreationOptions)
      Me.New([function], CancellationToken.None, creationOptions)
    End Sub

    Public Sub New(ByVal [function] As Func(Of TResult), ByVal cancellationToken As CancellationToken, ByVal creationOptions As TaskCreationOptions)
      MyBase.New(cancellationToken, creationOptions)
      Me._function = [function]
    End Sub

    Public Sub New(ByVal [function] As Func(Of Object, TResult), ByVal state As Object)
      Me.New([function], state, CancellationToken.None, TaskCreationOptions.None)
    End Sub

    Public Sub New(ByVal [function] As Func(Of Object, TResult), ByVal state As Object, ByVal cancellationToken As CancellationToken)
      Me.New([function], state, cancellationToken, TaskCreationOptions.None)
    End Sub

    Public Sub New(ByVal [function] As Func(Of Object, TResult), ByVal state As Object, ByVal creationOptions As TaskCreationOptions)
      Me.New([function], state, CancellationToken.None, creationOptions)
    End Sub

    Public Sub New(ByVal [function] As Func(Of Object, TResult), ByVal state As Object, ByVal cancellationToken As CancellationToken, ByVal creationOptions As TaskCreationOptions)
      MyBase.New(state, cancellationToken, creationOptions)
      Me._state = state
      Me._objectFunction = [function]
    End Sub

    Friend Sub New(ByVal [function] As Func(Of Task, TResult), ByVal state As Task, ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions)
      MyBase.New(state, cancellationToken, continuationOptions)
      Me._state = state
      Dim objectFunction As Func(Of Object, TResult) = Function(obj) [function].Invoke(state)
      Me._objectFunction = objectFunction
    End Sub

    'Private Function FunctionWrapper(ByVal state As Object) As TResult
    '  Return Me._objectFunction.Invoke(state)
    'End Function

    Public Overloads Shared ReadOnly Property Factory() As TaskFactory(Of TResult)
      Get
        Return _factory
      End Get
    End Property

    Public Property Result() As TResult
      Get
        Return Me._result
      End Get
      Friend Set(ByVal value As TResult)
        Me._result = value
      End Set
    End Property

    Public Overloads Function ContinueWith(ByVal continuationAction As Action(Of Task(Of TResult)))
      Return Me.ContinueWith(continuationAction, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default)
    End Function

    Public Overloads Function ContinueWith(ByVal continuationAction As Action(Of Task(Of TResult)), ByVal cancellationToken As CancellationToken)
      Return Me.ContinueWith(continuationAction, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default)
    End Function

    Public Overloads Function ContinueWith(ByVal continuationAction As Action(Of Task(Of TResult)), ByVal continuationOptions As TaskContinuationOptions) As Task
      Return Me.ContinueWith(continuationAction, CancellationToken.None, continuationOptions, TaskScheduler.Default)
    End Function

    Public Overloads Function ContinueWith(ByVal continuationAction As Action(Of Task(Of TResult)), ByVal scheduler As TaskScheduler) As Task
      Return Me.ContinueWith(continuationAction, CancellationToken.None, TaskContinuationOptions.None, scheduler)
    End Function

    Public Overloads Function ContinueWith(ByVal continuationAction As Action(Of Task(Of TResult)), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task
      Return MyBase.ContinueWith(AddressOf continuationAction.Invoke, cancellationToken, continuationOptions, scheduler)
    End Function

    'Private Class TaskWrapper
    '  Private _action As Action(Of Task(Of TResult))
    '  Public Sub New(ByVal action As Action(Of Task(Of TResult)))
    '    Me._action = action
    '  End Sub

    '  Private Sub Execute(ByVal task As Task)
    '    Me._action.Invoke(task)
    '  End Sub

    '  Public Function GetAction() As Action(Of Task)
    '    Return AddressOf Me.Execute
    '  End Function
    'End Class

    'Private Sub ContinuationAction(ByVal task As Task)

    'End Sub

    Public Overloads Function ContinueWith(Of TNewResult)(ByVal continuationFunction As Func(Of Task(Of TResult), TNewResult)) As Task(Of TNewResult)
      Return Me.ContinueWith(Of TNewResult)(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default)
    End Function

    Public Overloads Function ContinueWith(Of TNewResult)(ByVal continuationFunction As Func(Of Task(Of TResult), TNewResult), ByVal cancellationToken As CancellationToken) As Task(Of TNewResult)
      Return Me.ContinueWith(Of TNewResult)(continuationFunction, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default)
    End Function

    Public Overloads Function ContinueWith(Of TNewResult)(ByVal continuationFunction As Func(Of Task(Of TResult), TNewResult), ByVal continuationOptions As TaskContinuationOptions) As Task(Of TNewResult)
      Return Me.ContinueWith(Of TNewResult)(continuationFunction, CancellationToken.None, continuationOptions, TaskScheduler.Default)
    End Function

    Public Overloads Function ContinueWith(Of TNewResult)(ByVal continuationFunction As Func(Of Task(Of TResult), TNewResult), ByVal scheduler As TaskScheduler) As Task(Of TNewResult)
      Return Me.ContinueWith(Of TNewResult)(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler)
    End Function

    Public Overloads Function ContinueWith(Of TNewResult)(ByVal continuationFunction As Func(Of Task(Of TResult), TNewResult), ByVal cancellationToken As CancellationToken, ByVal continuationOptions As TaskContinuationOptions, ByVal scheduler As TaskScheduler) As Task(Of TNewResult)
      Return MyBase.ContinueWith(Of TNewResult)(AddressOf continuationFunction.Invoke, cancellationToken, continuationOptions, scheduler)
    End Function

    'Private Class TaskFunctionWrapper(Of TNewResult)
    '  Private _func As Func(Of Task(Of TResult), TNewResult)
    '  Public Sub New(ByVal func As Func(Of Task(Of TResult), TNewResult))
    '    Me._func = func
    '  End Sub
    '  Private Function Execute(ByVal task As Task(Of TResult)) As TNewResult
    '    Return Me._func.Invoke(task)
    '  End Function
    '  Public Function GetFunc() As Func(Of Task, TNewResult)
    '    Return AddressOf Me.Execute
    '  End Function
    'End Class

    Protected Overrides Sub InvokeAction()
      If Not IsNothing(Me._function) Then
        Me._result = Me._function.Invoke()
      ElseIf Not IsNothing(Me._objectFunction) Then
        Me._result = Me._objectFunction.Invoke(Me._state)
      Else
        Throw New NotSupportedException("No function was specified")
      End If
    End Sub

  End Class

End Namespace