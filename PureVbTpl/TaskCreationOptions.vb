Namespace Tasks

  <Flags()> _
  Public Enum TaskCreationOptions
    ''' <summary>
    ''' Specifies that the default behavior should be used.
    ''' </summary>
    None = 0
    ''' <summary>
    ''' A hint to a <see cref="TaskScheduler">TaskScheduler</see> to schedule a task in as fair a manner as possible, meaning that tasks scheduled sooner will be more likely to be run sooner, and tasks scheduled later will be more likely to be run later.
    ''' </summary>
    PreferFairness = 1
    ''' <summary>
    ''' Specifies that a task will be a long-running, coarse-grained operation. It provides a hint to the <see cref="TaskScheduler">TaskScheduler</see> that oversubscription may be warranted.
    ''' </summary>
    LongRunning = 1 << 1
    ''' <summary>
    ''' Specifies that a task is attached to a parent in the task hierarchy.
    ''' </summary>
    AttachedToParent = 1 << 2
  End Enum

End Namespace