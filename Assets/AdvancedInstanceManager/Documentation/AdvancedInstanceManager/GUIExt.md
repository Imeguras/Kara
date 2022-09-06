```csharp
InstanceManager.Editor.GUIExt
```




Contains a few extra gui functions.


### Methods:

>##### void BeginColorScope(UnityEngine.Color color)
>
>
>
>
>
>
>Begins a color scope, this sets EndColorScope.
>
>
>See also EndColorScope()
>
>

>##### void EndColorScope()
>
>
>
>Ends the color scope, that was started with BeginColorScope(UnityEngine.Color).
>

>##### void BeginEnabledScope(bool enabled, bool overrideWhenAlreadyFalse = False)
>
>
>
>
>
>
>Begins an enabled scope, this sets EndEnabledScope.
>
>
>See also EndColorScope()
>
>

>##### void EndEnabledScope()
>
>
>
>Ends the enabled scope, that was started with BeginEnabledScope(bool).
>

>##### void AddItem(this UnityEditor.GenericMenu menu, UnityEngine.GUIContent content, System.Action action, bool enabled = True, bool isChecked = False, UnityEngine.GUIContent offContent = null)
>
>
>
>Adds an item to this GenericMenu.
>
>menu: The GenericMenu.
>
>content: The content of this item.
>
>action: The action to perform when click, if enabled.
>
>enabled: Sets whatever this item is enabled.
>
>isChecked: Sets if checked.
>
>offContent: The content to display when item disabled, defaults to content if false.
>

>##### void AddItem(this UnityEditor.GenericMenu menu, UnityEngine.GUIContent content, System.Action<bool> action, bool isChecked, bool enabled = True, UnityEngine.GUIContent offContent = null)
>
>
>
>Adds an item to this GenericMenu.
>
>menu: The GenericMenu.
>
>content: The content of this item.
>
>action: The action to perform when click, if enabled.
>
>isChecked: Sets if checked.
>
>enabled: Sets whatever this item is enabled.
>
>offContent: The content to display when item disabled, defaults to content if false.
>

>##### bool UnfocusOnClick()
>
>
>
>
>
>
>Unfocuses elements when blank area of EditorWindow clicked.
>
>
>Returns true if element was unfocused, you may want to Repaint() then.
>
>