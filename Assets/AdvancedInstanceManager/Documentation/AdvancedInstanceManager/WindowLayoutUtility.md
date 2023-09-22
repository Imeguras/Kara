```csharp
InstanceManager.Utility.WindowLayoutUtility
```




Provides methods for enumerating and applying window layouts.


### Properties:

>##### bool isAvailable { get; }
>
>
>
>Gets whatever the utility was able to find the internal unity methods or not.
>

>##### string layoutsPath { get;  set; }
>
>
>
>The path to the layouts folder.
>

>##### InstanceManager.Utility.WindowLayoutUtility.Layout[] availableLayouts { get; }
>
>
>
>Finds all available layouts.
>

### Methods:

>##### InstanceManager.Utility.WindowLayoutUtility.Layout Find(string name)
>
>
>
>Finds the specified layout by name.
>

>##### System.Nullable<InstanceManager.Utility.WindowLayoutUtility.Layout> GetCurrent()
>
>
>
>Gets the current layout.
>