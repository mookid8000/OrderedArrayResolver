What?
====

This a simple `ISubDependencyResolver` implementation for Castle Windsor that allows injected arrays to be ordered by specifying one or more components' relation to other components.

What's the problem?
====

Imagine we have some kind of task interface - e.g. `public interface ITask {}` - which should be executed by some service that gets task implementations injected:

````
public class TaskExecutor
{
    public TaskExecutor(ITaks[] tasks) { ... }
}
````

Usually, Windsor would happily inject these by using an `ISubDependencyResolver`, either `ArrayResolver`, `ListResolver`, or `CollectionResolver`. Only problem is that the order would be determined by the order in which components were registered.

One solution, which is as pragmatic as it is clunky, is to add an `int Order { get; }` signature to the `ITask` interface, thus allowing each implementation to return an `int` that would specify their absolute position. 

And then, if the `TaskExecutor` would remember to `.OrderBy(t => t.Order)`, tasks could be executed in order.

The solution?
====

As an experiment, I wanted to see if it would be a cooler to solution to provide only the necessary little ordering hints, where each component could specify its relation to another component.

Most important thing is that this is not necessary - only components with an opinion would need to position itself in relation to another component.

So, in order to affect the order of the tasks, with the `OrderedArrayResolver` we can do this:

````
[ExecutesAfter(typeof(PrepareSomeStuff))]
[ExecutesBefore(typeof(FinishOffSomeStuff))]
public class DoStuff : ITask {}

[ExecutesAfter(typeof(PrepareSomeStuff))]
[ExecutesBefore(typeof(FinishOffSomeStuff))]
[ExecutesAfter(typeof(DoStuff))]
public class DoMoreStuff : ITask {}

public class PrepareSomeStuff : ITask {}

[ExecutesAfter(typeof(PrepareSomeStuff))]
public class FinishOffSomeStuff : ITask {}
````

And at this point, I realize that this example probably calls for two new attributes: `ExecutesBeforeAllAttribute` and `ExecutesAfterAllAttribute`.

Please note
====
that this functionality does not work when you call `ResolveAll` - it only works on injected arrays.

As of Windsor 3.0, this will change, because Windsor 3.0 will have an `IHandlerFilter` hook in `ResolveAll`, similar to the existing `IHandlerSelector` which works for `Resolve`.