Imports System.Runtime.Serialization
Imports System.Collections.ObjectModel
Imports System.Security.Permissions

Namespace Tasks

  Public Class AggregateException
    Inherits Exception

    Private _innerExceptions As ReadOnlyCollection(Of Exception)

    Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
      MyBase.New(info, context)
    End Sub

    Public Sub New()
      MyBase.New()
    End Sub

    Public Sub New(ByVal message As String)
      MyBase.New(message)
    End Sub

    Public Sub New(ByVal innerExceptions As IEnumerable(Of Exception))
      Me.New(CType(Nothing, String), innerExceptions)
    End Sub

    Public Sub New(ByVal ParamArray innerExceptions As Exception())
      Me.New(innerExceptions.ToList())
    End Sub

    Public Sub New(ByVal message As String, ByVal innerExceptions As IEnumerable(Of Exception))
      MyBase.New(message, innerExceptions.FirstOrDefault())
      Dim exceptionList As List(Of Exception) = New List(Of Exception)(innerExceptions)
      Me._innerExceptions = New ReadOnlyCollection(Of Exception)(exceptionList)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception)
      Me.New(message, New Exception() {innerException})
    End Sub

    Public Sub New(ByVal message As String, ByVal ParamArray innerExceptions As Exception())
      Me.New(message, innerExceptions.ToList())
    End Sub

    Public ReadOnly Property InnerExceptions() As ReadOnlyCollection(Of Exception)
      Get
        Return Me._innerExceptions
      End Get
    End Property

    Public Function Flatten() As AggregateException
      Dim flattenedExceptions As List(Of Exception) = New List(Of Exception)
      For Each exception As Exception In Me._innerExceptions
        If TypeOf exception Is AggregateException Then
          Dim innerExceptions As IEnumerable(Of Exception) = CType(exception, AggregateException).Flatten().InnerExceptions
          For Each innerException As Exception In innerExceptions
            flattenedExceptions.Add(innerExceptions)
          Next
        Else
          flattenedExceptions.Add(exception)
        End If
      Next
      Return New AggregateException(flattenedExceptions)
    End Function

    Public Overrides Function GetBaseException() As System.Exception
      Return Me._innerExceptions.First
    End Function

    <SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter:=True)> _
    Public Overrides Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
      MyBase.GetObjectData(info, context)
      'Currently not adding any data here. The "_innerExceptions" member isn't serializable.
    End Sub

    Public Sub Handle(ByVal predicate As Func(Of Exception, Boolean))
      Dim unhandledExceptions As List(Of Exception) = New List(Of Exception)
      For Each exception As Exception In Me._innerExceptions
        If Not predicate(exception) Then
          unhandledExceptions.Add(exception)
        End If
      Next
      If unhandledExceptions.Count > 0 Then
        Throw New AggregateException(unhandledExceptions)
      End If
    End Sub

    Public Overrides Function ToString() As String
      Dim stringValue As String = String.Empty
      For Each exception As Exception In Me._innerExceptions
        stringValue &= exception.ToString & vbCrLf
      Next
      Return stringValue
    End Function

  End Class

End Namespace