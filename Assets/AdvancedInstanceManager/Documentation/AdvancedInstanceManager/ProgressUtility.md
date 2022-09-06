```csharp
InstanceManager.Utility.ProgressUtility
```







Provides functions for running tasks that display progress.


Note that progress is only displayed in Unity 2020 and higher, task will still run in earlier versions of unity, but no progress will be shown.



### Methods:

>##### System.Threading.Tasks.Task RunTask(string displayName, System.Threading.Tasks.Task task, System.Action<System.Threading.Tasks.Task> onComplete = null, string description = null, int minDisplayTime = 250, bool canRun = True, bool hideProgress = False, bool hideError = False)
>
>
>
>Shows progress in unity for a task. Only inderminate progress bar is supported.
>
>displayName: The display name for the progress item.
>
>task: The task that display progress for. The task cannot be running already.
>
>onComplete: The callback to be invoked when task is done (minDisplayTime has no effect).
>
>description: The description for the progress item.
>
>minDisplayTime: The minimum display time of the progress bar, makes sure that the progress is displayed and readable, instead of just flickering.
>
>canRun: Prevents the task from running and does not create a progress item if false.
>
>hideError: Prevents the error from getting logged.
>
>hideProgress: Prevents progress from being shown.
>