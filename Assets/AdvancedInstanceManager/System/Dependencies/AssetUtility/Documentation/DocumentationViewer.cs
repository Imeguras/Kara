using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace AssetUtility.Documentation
{

    class MyAllPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var window in Resources.FindObjectsOfTypeAll<DocumentationViewer>())
                window.Reload();
        }
    }

    /// <summary>Base class for documentation viewers.</summary>
    abstract partial class DocumentationViewer : EditorWindow
    {

        GUISkin lightSkin;
        GUISkin darkSkin;

        public void Reload()
        {
            OnDisable();
            OnEnable();
        }

        /// <summary>Opens the viewer, with the specified title.</summary>
        protected static void Open<T>(string title = "Documentation") where T : DocumentationViewer
        {
            var w = GetWindow<T>();
            w.titleContent = new GUIContent(title);
            if (w.position.x > Screen.width || w.position.y > Screen.height || w.position.x < 0 || w.position.y < 0)
                w.position = new Rect((Screen.width / 2) - (w.position.width / 2), (Screen.height / 2) - (w.position.height / 2), w.position.width, w.position.height);
        }

        public static bool GetWindowInstance(out DocumentationViewer instance) =>
            instance = Resources.FindObjectsOfTypeAll<DocumentationViewer>().FirstOrDefault();

        /// <summary>The id of this documentation viewer, is used to locate the documentation folder (which could be moved by user of asset).</summary>
        protected abstract string id { get; }

        /// <summary>The path to the home file.</summary>
        protected abstract string homeFile { get; }

        /// <summary>The path to the sidebar file. Set to null to hide sidebar.</summary>
        protected abstract string sidebarFile { get; }

        string sidebar;
        string file;

        string path;

        string SavedPath
        {
            get => PlayerPrefs.GetString("AssetUtility:" + id + ".SelectedFile");
            set => PlayerPrefs.SetString("AssetUtility:" + id + ".SelectedFile", value);
        }

        //float VerticalScroll
        //{
        //    get => PlayerPrefs.GetFloat("AssetUtility:" + id + ".VerticalScroll");
        //    set => PlayerPrefs.SetFloat("AssetUtility:" + id + ".VerticalScroll", value);
        //}

        void OnEnable()
        {

            sidebar = "";
            file = SavedPath;
            path = "";
            //scroll[file] = new Vector2(0, VerticalScroll);

            var assetRef = AssetDatabase.FindAssets("t:" + nameof(DocumentationRef)).Select(id => AssetDatabase.LoadAssetAtPath<DocumentationRef>(AssetDatabase.GUIDToAssetPath(id))).FirstOrDefault(r => r.id == id);
            if (!assetRef)
            {
                Debug.LogError("Could not find path to documentation.");
                return;
            }

            path = Directory.GetParent(AssetDatabase.GetAssetPath(assetRef)).FullName.Substring(Application.dataPath.Length - "Assets".Length).Replace(@"\", "/");

            lightSkin = AssetDatabase.LoadAssetAtPath<GUISkin>($"Packages/{UnityMarkdownViewer.packageName}/Editor/Skin/MarkdownViewerSkin.guiskin");
            darkSkin = AssetDatabase.LoadAssetAtPath<GUISkin>($"Packages/{UnityMarkdownViewer.packageName}/Editor/Skin/MarkdownSkinQs.guiskin");
#if !UNITY_2019
            Events.registeredPackages += Events_registeredPackages;
#endif
            UnityMarkdownViewer.Refresh();
            RegisterhyperLinkEvent();

        }

        void OnDisable()
        {
#if !UNITY_2019
            Events.registeredPackages -= Events_registeredPackages;
#endif
            RegisterhyperLinkEvent(register: false);
#if UNITY_MARKDOWN_VIEWER
            sidebarViewer = null;
            mainViewer = null;
#endif
            SavedPath = file;
            //VerticalScroll = scroll[file].y;
        }


#if !UNITY_2019
        void Events_registeredPackages(PackageRegistrationEventArgs e) =>
            Repaint();
#endif

        void Update()
        {
            OnViewUpdate();
        }

        void OnGUI()
        {

            if (string.IsNullOrEmpty(path))
                return;

            EnsureCorrectPath(ref file, homeFile);
            EnsureCorrectPath(ref sidebar, sidebarFile);

            if (!UnityMarkdownViewer.isInstalled.HasValue)
                UnityMarkdownViewer.Refresh();

            if (UnityMarkdownViewer.isInstalled ?? false)
            {
                SetNormalMinMaxSize();
                OnView();
            }
            else if (UnityMarkdownViewer.isRefreshing || UnityMarkdownViewer.isInstalling)
            {
                SetNormalMinMaxSize();
                OnRefreshing();
            }
            else if (UnityMarkdownViewer.error != null)
            {
                SetNormalMinMaxSize();
                OnError();
            }
            else
            {
                maxSize = minSize;
                OnInstall();
            }

            void SetNormalMinMaxSize()
            {
                minSize = new Vector2(651, 436);
                maxSize = new Vector2(4000, 4000);
            }

        }

        void EnsureCorrectPath(ref string path, string nullPath)
        {

            var docPath = this.path;

            if (docPath.Contains("/advanced-instance-manager")) docPath = docPath.Replace("/advanced-instance-manager", "/com.lazy.advanced-instance-manager");
            if (path.Contains("/advanced-instance-manager")) path = path.Replace("/advanced-instance-manager", "/com.lazy.advanced-instance-manager");
            if (path is null) path = nullPath;
            if (!path.StartsWith(docPath)) path = docPath + "/" + path.TrimStart('/');
            if (!path.EndsWith(".md")) path += ".md";
            if (path.EndsWith("/.md")) path = path.Replace("/.md", "/" + nullPath);

        }

        #region Installing markdown viewer

        class OnUninstall : AssetPostprocessor
        {

            static void OnPostprocessAllAssets(string[] _, string[] deletedAssets, string[] __, string[] ___)
            {

                var removedAssemblies = deletedAssets.Where(a => AssetDatabase.IsValidFolder(a)).SelectMany(a => AssetDatabase.FindAssets("t:asmdef", new[] { a })).Concat(deletedAssets.Where(a => a.EndsWith(".asmdef"))).ToArray();
                var isSelfOrMarkdownViewerUninstalled = removedAssemblies.Any(a => a.EndsWith("Mischief.MDV.Editor.asmdef") || a.EndsWith("AdvancedInstanceManager.asmdef"));

                if (isSelfOrMarkdownViewerUninstalled)
                    ScriptingDefineUtility.Unset("UNITY_MARKDOWN_VIEWER");

            }

        }

        void OnInstall()
        {

            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("The package <b>Unity Markdown Viewer</b> is required to view documentation in editor.", new GUIStyle(EditorStyles.label) { richText = true });

            //'Unity Markdown Viewer' link
            var rect = GUILayoutUtility.GetLastRect();

            var l1 = EditorStyles.label.CalcSize(new GUIContent("The package "));
            var l2 = EditorStyles.boldLabel.CalcSize(new GUIContent("Unity Markdown Viewer"));

            var r = new Rect(rect.xMin + l1.x, rect.yMin, l2.x, rect.height);

            if (GUI.Button(r, new GUIContent("", UnityMarkdownViewer.repoUri), GUIStyle.none))
                Application.OpenURL(UnityMarkdownViewer.repoUri);
            EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);

            if (GUILayout.Button("Install now", new GUIStyle(GUI.skin.button) { margin = new RectOffset(0, 0, 0, bottom: 2) }))
                UnityMarkdownViewer.Install();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("You can also view files in a separate markdown viewer app, by opening files manually, if one is installed.");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndArea();

        }

        const string progressSpinners = "◜◠◝◞◡◟";
        int progressSpinnerIndex;
        double? time;

        void ProgressSpinner()
        {

            if (!time.HasValue || EditorApplication.timeSinceStartup - time > 0.05f)
            {
                progressSpinnerIndex += 1;
                time = EditorApplication.timeSinceStartup;
            }

            if (progressSpinnerIndex >= progressSpinners.Length)
                progressSpinnerIndex = 0;
            GUILayout.Label(progressSpinners[progressSpinnerIndex].ToString(), new GUIStyle(EditorStyles.label) { fontSize = 26 });
            Repaint();

        }

        void OnRefreshing()
        {

            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            ProgressSpinner();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndArea();

        }

        void OnError(string error = "")
        {

            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label($"An error occured (code: {UnityMarkdownViewer.error.errorCode})." + Environment.NewLine + UnityMarkdownViewer.error.message, new GUIStyle(EditorStyles.label) { richText = true });

            if (UnityMarkdownViewer.isRefreshError && GUILayout.Button("Retry"))
                UnityMarkdownViewer.Refresh();
            else if (UnityMarkdownViewer.isInstallError && GUILayout.Button("Retry"))
                UnityMarkdownViewer.Install();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndArea();

        }

        #endregion
        #region Render markdown

#if UNITY_MARKDOWN_VIEWER

        MG.MDV.MarkdownViewer sidebarViewer;
        MG.MDV.MarkdownViewer mainViewer;

#endif

        void RegisterhyperLinkEvent(bool register = true)
        {

#if UNITY_MARKDOWN_VIEWER

            MG.MDV.HyperlinkHelper.HyperlinkOpened -= HyperlinkHelper_HyperlinkOpened;
            if (register)
                MG.MDV.HyperlinkHelper.HyperlinkOpened += HyperlinkHelper_HyperlinkOpened;

#endif

        }

#if UNITY_MARKDOWN_VIEWER
        void HyperlinkHelper_HyperlinkOpened(MG.MDV.HyperlinkOpenEventArgs e)
        {

            if (!e.IsMarkdownFile)
                return;

            e.Cancel = true;

            var file = e.Hyperlink;
            EnsureCorrectPath(ref file, homeFile);
            if (this.file != file)
            {
                this.file = file;
                mainViewer = null;
            }

        }
#endif

        void OnViewUpdate()
        {

#if UNITY_MARKDOWN_VIEWER
            if ((sidebarViewer?.Update() ?? false) || (mainViewer?.Update() ?? false))
            {
                //scroll[file] = new Vector2(0, VerticalScroll);
                Repaint();
            }
#endif

        }

        void OnView()
        {

#if UNITY_MARKDOWN_VIEWER
            EditorGUILayout.BeginHorizontal();
            ViewFile(sidebar, ref sidebarViewer, isSidebar: true);
            ViewFile(file, ref mainViewer, isSidebar: false);
            EditorGUILayout.EndHorizontal();
#endif

        }

#if UNITY_MARKDOWN_VIEWER

        const char ZeroWidthSpace = '​';

        readonly System.Collections.Generic.Dictionary<string, Vector2> scroll = new System.Collections.Generic.Dictionary<string, Vector2>();
        void ViewFile(string file, ref MG.MDV.MarkdownViewer viewer, bool isSidebar)
        {

            if (!scroll.ContainsKey(file))
                scroll.Add(file, Vector2.zero);

            if (viewer == null)
            {

                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(file);
                if (!asset)
                    return;

                viewer = new MG.MDV.MarkdownViewer(
                    skin: MG.MDV.Preferences.DarkSkin ? darkSkin : lightSkin,
                    file,
                    content: ProcessDocument(asset, isSidebar))
                { drawToolbar = false };

            }

            const float sidebarWidth = 250;
            const float margin = 12;

            var width = isSidebar ? sidebarWidth : position.width - sidebarWidth;

            GUILayout.BeginArea(new Rect((isSidebar ? 0 : sidebarWidth) + margin, 0, width - margin, position.height));
            scroll[file] = GUILayout.BeginScrollView(scroll[file], alwaysShowHorizontal: false, alwaysShowVertical: true);

            viewer.Draw(width - GUI.skin.verticalScrollbar.fixedWidth - (margin * 2) - 44);

            GUILayout.EndScrollView();
            GUILayout.EndArea();

        }


        string ProcessDocument(TextAsset asset, bool isSidebar)
        {

            var text = asset.text;

            //Fix images
            if (path.Contains("Packages/"))
                using (var reader = new StringReader(text))
                {
                    while (reader.ReadLine() is string line)
                    {
                        if (line.StartsWith("![](") && !line.StartsWith("![](http"))
                        {
                            var newLine = "![](" + InstanceManager.Utility.Paths.project + "/" + path + "/" + line.Substring("![](".Length);
                            text = text.Replace(line, newLine);
                        }
                    }
                }

            if (isSidebar) //Add home and some spacing
                return ZeroWidthSpace + @"\" + Environment.NewLine + "[Home](Home)" + Environment.NewLine + text;
            else //Add name of current file as header
                return (asset.name.EndsWith("Home") ? ZeroWidthSpace.ToString() : "# " + ObjectNames.NicifyVariableName(asset.name)) + Environment.NewLine + Environment.NewLine + text;

        }

#endif


        #endregion

    }

}
