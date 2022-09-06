```csharp
InstanceManager.Utility.InstanceUtility
```




Provides utility functions for working with secondary instances.


### Events:

>##### System.Action onInstancesChanged
>
>
>
>Occurs when an instance is changed.
>

### Fields:

>##### string instanceFileName
>
>
>
>The name of the instance settings file.
>

### Methods:

>##### InstanceManager.Models.UnityInstance LocalInstance()
>
>
>
>Loads local instance file. Returns null if none exists or instance is primary.
>

>##### InstanceManager.Models.UnityInstance Find(string id)
>
>
>
>Finds the secondary instance with the specified id.
>

>##### System.Collections.Generic.IEnumerable<InstanceManager.Models.UnityInstance> Enumerate()
>
>
>
>Enumerates all secondary instances for this project.
>

>##### InstanceManager.Models.UnityInstance Create()
>
>
>
>Create a new secondary instance. Returns null if current instance is secondary.
>

>##### System.Threading.Tasks.Task Repair(InstanceManager.Models.UnityInstance instance, string path)
>
>
>
>Repairs the instance. No effect if current instance is secondary.
>

>##### bool NeedsRepair(InstanceManager.Models.UnityInstance instance)
>
>
>
>Gets if the instance needs to be repaired.
>

>##### bool IsInstanceBeingSetUp(InstanceManager.Models.UnityInstance instance)
>
>
>
>Gets if the UnityInstance is being set up, this would be when its being created, or when being removed.
>