using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace AssetUtility.Documentation
{

    class SetupDocumentationViewerView : View
    {

        public override string header { get; } = "Documentation viewer";

        public string menuItem = "Tools/" + Application.productName + "/Documentation";
        public string title = Application.productName + " - " + "Documentation";

        public string documentationFolder = "";
        public string homeFile = "home.md";
        public string sidebarFile = "_sidebar.md";
        public bool publicOpen = true;
        public string id;

        public override void OnEnable()
        {

            Load();

            if (string.IsNullOrEmpty(documentationFolder))
            {
                var files = Directory.GetFiles(Application.dataPath, "*.md", SearchOption.AllDirectories);
                if (files.Any())
                    documentationFolder = Directory.GetParent(files.First()).FullName.Substring(Application.dataPath.Length - "Assets".Length).Replace(@"\", "/");
            }

        }

        public override void OnDisable() =>
            Save();

        public override void OnGUI()
        {

            EditorGUILayout.LabelField("Setup documentation viewer", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            menuItem = EditorGUILayout.TextField("Menu Item", menuItem);
            title = EditorGUILayout.TextField("Title", title);
            documentationFolder = EditorGUILayout.TextField("Documentation folder", documentationFolder);
            homeFile = EditorGUILayout.TextField(new GUIContent("Home file", "The path to the home or start file. Relative to documentationFolder."), homeFile);
            sidebarFile = EditorGUILayout.TextField(new GUIContent("Sidebar file", "The path to the sidebar file, if one is used. Relative to documentationFolder."), sidebarFile);
            publicOpen = EditorGUILayout.Toggle(new GUIContent("Public Open()", "Specifies whatever the static Open() method should be public or not."), publicOpen);

            if (EditorGUI.EndChangeCheck())
                Save();

            EditorGUILayout.Space(22);
            if (GUILayout.Button("Generate"))
                Generate();

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("\nThe generated file will be put in 'Assets/', and can be moved freely. " +
                "\n\n" +
                "The file can also be modified, just be aware that only one generated file is allowed, and generating another one will prompt to delete the old one.\n", MessageType.Info);

        }

        void Generate()
        {

            if (!EnsureNoGeneratedFileExists())
                return;

            //Create script
            var id1 = GUID.Generate().ToString();
            var script = GenerateClass(id1, homeFile, sidebarFile, menuItem, title, publicOpen);
            File.WriteAllText(Path.Combine(Application.dataPath, generatedClassName + ".cs"), script);

            //Create DocumentationRef
            var assetRef = AssetDatabase.FindAssets("t:" + nameof(DocumentationRef)).Select(id => AssetDatabase.LoadAssetAtPath<DocumentationRef>(AssetDatabase.GUIDToAssetPath(id))).FirstOrDefault(r => r.id == this.id);
            if (assetRef)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(assetRef));

            assetRef = ScriptableObject.CreateInstance<DocumentationRef>();
            assetRef.id = id1;
            Directory.CreateDirectory(documentationFolder);
            AssetDatabase.CreateAsset(assetRef, documentationFolder + "/ref.asset");
            this.id = id1;

            //Refresh asset database and select script
            AssetDatabase.Refresh();
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/" + generatedClassName + ".cs");
            ProjectWindowUtil.ShowCreatedAsset(asset);
            Save();

        }

        bool EnsureNoGeneratedFileExists()
        {

            var scripts = AssetDatabase.FindAssets("t:" + nameof(MonoScript)).Select(AssetDatabase.GUIDToAssetPath).Where(path => path.EndsWith(generatedClassName + ".cs")).ToArray();
            if (!scripts.Any())
                return true;

            if (!EditorUtility.DisplayDialog(
                title: "Found existing generated documentation viewer class...",
                message: "The following generated files were found, only one should exist. In order to continue with generation, they need to be removed.\n\n" +
                    string.Join(Environment.NewLine, scripts),
                ok: "Remove",
                cancel: "Cancel"))
                return false;

            //AssetDatabase.DeleteAssets(scripts, new List<string>());

            return true;

        }

        const string generatedClassName = "DocumentationRef_GeneratedClass";

        static string GenerateClass(string id, string homeFile, string sidebarFile, string menuItem, string title, bool publicOpen) =>
                     $@"#if UNITY_EDITOR"
            + "\n" + $@""
            + "\n" + $@"public class {generatedClassName} : AssetUtility.Documentation.DocumentationViewer"
            + "\n" + @"{"
            + "\n" + $@""
            + "\n" + $@"    //An id which is used to locate documentation folder, using ScriptableObject with same id string variable"
            + "\n" + $@"    protected override string id => ""{id}"";"
            + "\n" + $@"    protected override string homeFile => ""{homeFile}"";"
            + "\n" + $@"    protected override string sidebarFile => ""{sidebarFile}"";"
            + "\n" + $@""
            + (publicOpen ? ("\n" + $@"    //Call on this method to open window programmatically") : "")
            + "\n" + $@"    [UnityEditor.MenuItem(""{menuItem}"")]"
            + "\n" + $@"    {(publicOpen ? "public " : "")}static void Open() =>"
            + "\n" + $@"        Open<{generatedClassName}>(title: ""{title}"");"
            + "\n" + $@""
            + "\n" + @"}"
            + "\n" + $@""
            + "\n" + $@"#endif"
            + "\n" + $@"";

    }

}
