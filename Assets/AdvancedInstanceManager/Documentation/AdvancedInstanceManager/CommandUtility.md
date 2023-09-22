```csharp
InstanceManager.Utility.CommandUtility
```




An utility class for running commands in the system terminal.


### Methods:

>##### System.Threading.Tasks.Task<int> RunCommand(string windows = null, string linux = null, string osx = null)
>
>
>
>Runs a command in the system terminal, chosen depending on which platform we're currently running on. Error is logged in console.
>

>##### System.Threading.Tasks.Task<int> RunCommandWindows(string command)
>
>
>
>Runs the command in the windows system terminal. Error is logged in console.
>

>##### System.Threading.Tasks.Task<int> RunCommandLinuxOSX(string command)
>
>
>
>Runs the command in the linux system terminal. Error is logged in console.
>