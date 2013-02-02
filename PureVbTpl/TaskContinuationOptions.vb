Namespace Tasks

  <Flags()> _
  Public Enum TaskContinuationOptions
    ''' <summary>
    ''' Specifies that the default behavior should be used.
    ''' </summary>
    None = TaskCreationOptions.None
    ''' <summary>
    ''' A hint to a <see cref="TaskScheduler">TaskScheduler</see> to schedule a task in as fair a manner as possible, meaning that tasks scheduled sooner will be more likely to be run sooner, and tasks scheduled later will be more likely to be run later.
    ''' </summary>
    PreferFairness = TaskCreationOptions.PreferFairness
    ''' <summary>
    ''' Specifies that a task will be a long-running, coarse-grained operation. It provides a hint to the <see cref="TaskScheduler">TaskScheduler</see> that oversubscription may be warranted.
    ''' </summary>
    LongRunning = TaskCreationOptions.LongRunning
    ''' <summary>
    ''' Specifies that a task is attached to a parent in the task hierarchy.
    ''' </summary>
    AttachedToParent = TaskCreationOptions.AttachedToParent

    NotOnRanToCompletion = 1 << 16
    NotOnFaulted = 1 << 17
    OnlyOnCanceled = 1 << 18
    NotOnCanceled = 1 << 19
    OnlyOnFaulted = 1 << 20
    OnlyOnRanToCompletion = 1 << 21
  End Enum

End Namespace