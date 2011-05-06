What?
====

This a simple `ISubDependencyResolver` implementation for Castle Windsor that allows injected arrays to be ordered by specifying one or more components' relation to other components.

Example!
====

Imagine we have some kind of task interface:
````
public interface ITask {}
````

which should be executed by some service that gets the tasks injected:
````
public class TaskExecutor
{
    public TaskExecutor(ITaks[] tasks) { ... }
}
````

Now, in order to affect the order of the tasks, with `OrderedArrayResolver` we can do this:

````
[ExecutesAfter(typeof(PrepareSomeStuff))]
[ExecutesBefore(typeof(FinishOffSomeStuff))]
public class DoStuff : ITask {}

[ExecutesAfter(typeof(PrepareSomeStuff))]
[ExecutesBefore(typeof(FinishOffSomeStuff))]
public class DoMoreStuff : ITask {}

public class PrepareSomeStuff : ITask {}

[ExecutesAfter(typeof(PrepareSomeStuff))]
public class FinishOffSomeStuff : ITask {}
````

