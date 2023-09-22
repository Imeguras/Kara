using System.Linq;
using InstanceManager.Models;
using InstanceManager.Utility;
using UnityEditor;
using UnityEngine;

namespace InstanceManager.Editor
{

    /// <summary>The instance manager window.</summary>
    public partial class InstanceManagerWindow : EditorWindow
    {

        /// <summary>Opens the <see cref="InstanceManagerWindow"/>.</summary>
        [MenuItem("Tools/Lazy/Instance Manager")]
        public static void Open()
        {
            var w = GetWindow<InstanceManagerWindow>();
            w.titleContent = new GUIContent("Instance Manager");
        }

        static class Style
        {

            public static GUIStyle margin { get; private set; }
            public static GUIStyle createButton { get; private set; }
            public static GUIStyle row { get; private set; }
            public static GUIStyle noItemsText { get; private set; }
            public static GUIStyle removeButton { get; private set; }

            public static GUIStyle secondaryInstanceMargin { get; private set; }
            public static GUIStyle elementMargin12 { get; private set; }
            public static GUIStyle elementMargin0 { get; private set; }

            public static GUIStyle searchBox { get; private set; }
            public static GUIStyle scenesList { get; private set; }
            public static GUIStyle scenesSeparator { get; private set; }

            public static GUIStyle moveSceneButton { get; private set; }

            public static GUIStyle menu { get; private set; }

            public static GUIStyle header { get; private set; }
            public static GUIStyle folder { get; private set; }

            public static void Initialize()
            {

                if (margin is null) margin = new GUIStyle() { margin = new RectOffset(12, 12, 12, 12) };
                if (createButton is null) createButton = new GUIStyle(GUI.skin.button) { margin = new RectOffset(0, 0, 8, 0), padding = new RectOffset(12, 12, 6, 6) };
                if (row is null) row = new GUIStyle(EditorStyles.toolbar) { padding = new RectOffset(12, 12, 12, 6), fixedHeight = 42 };
                if (noItemsText is null) noItemsText = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState() { textColor = new Color(1, 1, 1, 0.75f) } };
                if (removeButton is null) removeButton = new GUIStyle(GUI.skin.button) { margin = new RectOffset(12, 0, 0, 0) };

                if (secondaryInstanceMargin is null) secondaryInstanceMargin = new GUIStyle() { margin = new RectOffset(6, 6, 6, 6) };
                if (elementMargin0 is null) elementMargin0 = new GUIStyle() { margin = new RectOffset(0, 0, 0, 0) };
                if (elementMargin12 is null) elementMargin12 = new GUIStyle() { margin = new RectOffset(0, 0, 12, 0) };

                if (searchBox is null) searchBox = new GUIStyle(EditorStyles.textField) { margin = new RectOffset(0, 6, 0, 0) };
                if (scenesList is null) scenesList = new GUIStyle(GUI.skin.box) { margin = new RectOffset(6, 6, 8, 6) };
                if (scenesSeparator is null) scenesSeparator = new GUIStyle(GUI.skin.box);
                if (scenesSeparator is null) scenesSeparator.normal.background = EditorGUIUtility.whiteTexture;

                if (moveSceneButton is null) moveSceneButton = new GUIStyle(GUI.skin.button) { fontSize = 18, fixedWidth = 21, fixedHeight = 21, padding = new RectOffset(2, 0, 0, 0) };
                if (menu is null) menu = new GUIStyle(GUI.skin.button) { fixedWidth = 16, fixedHeight = 19, padding = new RectOffset(left: 2, top: 1, right: 0, bottom: 0) };

                if (header is null) header = new GUIStyle(EditorStyles.label) { fontSize = 20, fixedHeight = 24, padding = new RectOffset(2, 0, -4, 0) };
                if (folder is null) folder = new GUIStyle(GUI.skin.button) { padding = new RectOffset(2, 2, 2, 2), fixedWidth = 18, fixedHeight = 18 };

            }

        }

        static class Content
        {

            public static GUIContent noInstances { get; private set; }
            public static GUIContent status { get; private set; }
            public static GUIContent running { get; private set; }
            public static GUIContent notRunning { get; private set; }

            public static GUIContent emptyString { get; private set; }
            public static GUIContent close { get; private set; }
            public static GUIContent open { get; private set; }
            public static GUIContent openScenes { get; private set; }

            public static GUIContent showInExplorer { get; private set; }
            public static GUIContent options { get; private set; }
            public static GUIContent delete { get; private set; }

            public static GUIContent symLinkerUpdate { get; private set; }
            public static GUIContent symLinkerNotInstalled { get; private set; }
            public static GUIContent update { get; private set; }
            public static GUIContent install { get; private set; }
            public static GUIContent github { get; private set; }
            public static GUIContent createNewInstance { get; private set; }

            public static GUIContent back { get; private set; }
            public static GUIContent reload { get; private set; }
            public static GUIContent autoSync { get; private set; }
            public static GUIContent apply { get; private set; }
            public static GUIContent Apply { get; private set; }
            public static GUIContent autoPlayMode { get; private set; }
            public static GUIContent openPrimaryEditor { get; private set; }
            public static GUIContent preferredLayout { get; private set; }

            public static GUIContent scenesToOpen { get; private set; }

            public static GUIContent up { get; private set; }
            public static GUIContent down { get; private set; }

            public static GUIContent movingInstances { get; private set; }
            public static GUIContent menu { get; private set; }
            public static GUIContent settings { get; private set; }

            public static GUIContent settingsText { get; private set; }
            public static GUIContent instancesPath { get; private set; }
            public static GUIContent repair { get; private set; }

            public static void Initialize()
            {

                if (status is null) status = new GUIContent("Status:");
                if (noInstances is null) noInstances = new GUIContent("No instances found.");
                if (running is null) running = new GUIContent("Running");
                if (notRunning is null) notRunning = new GUIContent("Not running");

                if (emptyString is null) emptyString = new GUIContent(string.Empty);
                if (open is null) open = new GUIContent("Open");
                if (openScenes is null) openScenes = new GUIContent("Open scenes");
                if (close is null) close = new GUIContent("Close");

                if (showInExplorer is null) showInExplorer = new GUIContent("Show in explorer...");
                if (options is null) options = new GUIContent("Options...");
                if (delete is null) delete = new GUIContent("Delete");

                if (symLinkerUpdate is null) symLinkerUpdate = new GUIContent("SymLinker.exe has an update available.");
                if (symLinkerNotInstalled is null) symLinkerNotInstalled = new GUIContent("SymLinker.exe is not installed.");
                if (update is null) update = new GUIContent("Update");
                if (install is null) install = new GUIContent("Install");
                if (github is null) github = new GUIContent("Github");
                if (createNewInstance is null) createNewInstance = new GUIContent("Create new instance");

                if (back is null) back = new GUIContent("←");
                if (reload is null) reload = new GUIContent("↻", "Sync with primary instance");
                if (autoSync is null) autoSync = new GUIContent("Auto sync:");
                if (apply is null) apply = new GUIContent("apply");
                if (Apply is null) Apply = new GUIContent("Apply");
                if (autoPlayMode is null) autoPlayMode = new GUIContent("Enter / exit playmode: ");
                if (openPrimaryEditor is null) openPrimaryEditor = new GUIContent("Open scripts in: ");
                if (preferredLayout is null) preferredLayout = new GUIContent("Preferred layout: ");

                if (scenesToOpen is null) scenesToOpen = new GUIContent("Scenes to open:");

                if (up is null) up = new GUIContent("▴");
                if (down is null) down = new GUIContent("▾");

                if (movingInstances is null) movingInstances = new GUIContent("Moving instances...");
                if (menu is null) menu = new GUIContent("⋮");
                if (settings is null) settings = new GUIContent(EditorGUIUtility.IconContent("d_Preset.Context").image, "Settings");

                if (settingsText is null) settingsText = new GUIContent("Settings");
                if (instancesPath is null) instancesPath = new GUIContent("Instances path:");

                if (repair is null) repair = new GUIContent("Repair");

            }

        }

        static InstanceManagerWindow window;

        static new void Repaint()
        {

            if (!window)
                return;

            if (view == mainView)
                window.ReloadInstances();

            ((EditorWindow)window).Repaint();

        }

        void OnFocus()
        {
            view.OnFocus();
            Repaint();
        }

        void OnEnable()
        {

            window = this;

            ReloadInstances();

            if (InstanceManager.isSecondaryInstance)
                SetInstance(InstanceManager.id);
            else
                SetInstance(PlayerPrefs.GetString("InstanceManagerWindow.SelectedInstance", null));

            view.OnEnable();
            Repaint();

            InstanceUtility.onInstancesChanged -= Repaint;
            InstanceUtility.onInstancesChanged += Repaint;

        }

        void OnDisable()
        {

            view.OnDisable();

            if (InstanceManager.isPrimaryInstance)
                PlayerPrefs.SetString("InstanceManagerWindow.SelectedInstance", instance?.id ?? null);

            InstanceUtility.onInstancesChanged -= Repaint;
            window = null;

        }

        void OnGUI()
        {

            if (GUIExt.UnfocusOnClick())
                Repaint();

            Style.Initialize();
            Content.Initialize();

            if (view.minSize.HasValue)
                minSize = view.minSize.Value;

            BeginCheckResize();
            view.OnGUI();
            EndCheckResize();

        }

        internal UnityInstance[] instances;
        void ReloadInstances() =>
            instances = InstanceUtility.Enumerate().ToArray();

        #region View

        static UnityInstance instance;

        static View m_view;
        static View view { get { if (m_view is null) m_view = mainView; return m_view; } }

        static View mainView { get; } = new MainView();
        static View instanceView { get; } = new InstanceView();
        //static View settingsView { get; } = new SettingsView();

        abstract class View
        {

            public virtual void OnGUI()
            {
            }
            public virtual void OnFocus()
            {
            }
            public virtual void OnEnable()
            {
            }
            public virtual void OnDisable()
            {
            }
            public virtual Vector2? minSize { get; }
            public Rect position => window.position;

            public void Repaint() =>
                InstanceManagerWindow.Repaint();

        }

        static void SetView(View view)
        {

            if (view is null)
                view = mainView;

            if (view == m_view)
                return;

            m_view?.OnDisable();
            m_view = view;
            m_view.OnEnable();

        }

        static void ClearInstance() =>
            SetInstance(null);

        static void SetInstance(string id)
        {

            instance = !string.IsNullOrEmpty(id)
            ? InstanceUtility.Find(id)
            : null;

            SetView(instance is null ? mainView : instanceView);

        }

        #endregion View
        #region Check resize

        internal bool hasResized { get; private set; }

        Rect prevPos;
        protected void BeginCheckResize()
        {
            hasResized =
                prevPos.width != position.width ||
                prevPos.height != position.height;
        }

        protected void EndCheckResize() =>
            prevPos = position;

        #endregion Check resize

    }

}
