#if UNITY_EDITOR

class DocumentationRef_GeneratedClass : AssetUtility.Documentation.DocumentationViewer
{

    //An id which is used to locate documentation folder, using ScriptableObject with same id string variable
    protected override string id => "279c6b22417cc464f9b4728266795716";
    protected override string homeFile => "home.md";
    protected override string sidebarFile => "_sidebar.md";

    //Call on this method to open window programmatically
    [UnityEditor.MenuItem("Tools/Lazy/Instance Manager - Documentation")]
    static void Open() =>
        Open<DocumentationRef_GeneratedClass>(title: "Documentation");

}

#endif
