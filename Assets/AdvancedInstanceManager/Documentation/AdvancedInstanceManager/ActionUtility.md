```csharp
InstanceManager.Utility.ActionUtility
```




Provides utility functions for working with Action.


### Methods:

>##### void Try(this System.Action action, bool hideError = False)
>
>
>
>Runs the Action in a try catch block.
>
>action: The action to run.
>
>hideError: If true, then the error won't be rethrown.
>

>##### void Try(this System.Action action, System.Exception& exception)
>
>
>
>Runs the Action in a try catch block.
>
>action: The action to run.
>
>exception: The exception that occured.
>