```csharp
InstanceManager.Models.UnityInstance
```




Represents a secondary unity instance.


### Properties:

>##### bool needsRepair { get; }
>
>
>
>Gets if this instance needs repairing.
>

>##### string displayName { get;  set; }
>
>
>
>The display name of this instance.
>

>##### string effectiveDisplayName { get; }
>
>
>
>Gets either displayName has value.
>

>##### string preferredLayout { get;  set; }
>
>
>
>Gets or sets the window layout.
>

>##### bool autoSync { get;  set; }
>
>
>
>Gets or sets whatever this instance should auto sync asset changes.
>

>##### bool openEditorInPrimaryEditor { get;  set; }
>
>
>
>Gets or sets whatever scripts should open in the editor that is associated with the primary instance.
>

>##### bool enterPlayModeAutomatically { get;  set; }
>
>
>
>Gets or sets whatever this instance should enter / exit play mode automatically when primary instance does.
>

>##### string[] scenes { get;  set; }
>
>
>
>Gets the scenes this instance should open when starting.
>

>##### bool isRunning { get; }
>
>
>
>Gets whatever this instance is running.
>

>##### string id { get; }
>
>
>
>Gets the id of this instance.
>

>##### string primaryID { get; }
>
>
>
>Gets the primary instance id that this instance is associated with.
>

>##### string path { get; }
>
>
>
>Gets the path of this instance.
>

>##### bool isSettingUp { get; }
>
>
>
>Gets if the instance is currently being set up.
>

>##### System.Diagnostics.Process InstanceProcess { get;  set; }
>
>
>
>Gets the process of this instance, if it is running.
>

### Methods:

>##### void Save()
>
>
>
>Saves the instance settings to disk.
>

>##### void Remove()
>
>
>
>Removes the instance from disk.
>

>##### void Refresh()
>
>
>
>Refreshes this UnityInstance.
>

>##### void SetScene(string path, System.Nullable<bool> enabled = null, System.Nullable<int> index = null)
>
>
>
>Set property of scene.
>
>enabled: Set whatever this scene is enabled or not.
>
>index: Set the index of this scene.
>

>##### void ToggleOpen()
>
>
>
>Open if not running, othewise close.
>

>##### void Open()
>
>
>
>Open instance.
>

>##### void Close()
>
>
>
>Closes this instance.
>

>##### void Close(System.Action onClosed = null)
>
>
>
>Closes this instance.
>
>onClosed: Callback when instance is fully closed, since closing happens async.
>