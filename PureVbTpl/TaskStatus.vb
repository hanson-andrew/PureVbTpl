Namespace Tasks

  Public Enum TaskStatus
    Created
    WaitingForActivation
    WaitingToRun
    Running
    WaitingForChildrenToComplete
    RanToCompletion
    Canceled
    Faulted
  End Enum

End Namespace