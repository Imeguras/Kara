```csharp
InstanceManager.Utility.CrossProcessEventUtility
```




Provides utility functions for sending 'events' to secondary instances.


### Methods:

>##### void Send(string name, string param = null)
>
>
>
>Sends an event to all open secondary instances.
>
>name: The name of the event.
>
>param: The parameter to send. Must be single line.
>

>##### void Send(InstanceManager.Models.UnityInstance instance, string name, string param = null)
>
>
>
>Sends an event to the specified secondary instance.
>
>instance: The instance to send the event to.
>
>name: The name of the event.
>
>param: The parameter to send. Must be single line.
>

>##### void SendToHost(string name, string param = null)
>
>
>
>Sends an event to the primary instance.
>
>name: The name of the event.
>
>param: The parameter to send. Must be single line.
>

>##### void On(string name, System.Action<string> action)
>
>
>
>Adds a listener to the specified event.
>

>##### void On(string name, System.Action action)
>
>
>
>Adds a listener to the specified event.
>