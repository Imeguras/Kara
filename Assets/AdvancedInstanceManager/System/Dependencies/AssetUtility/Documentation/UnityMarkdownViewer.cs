using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;

namespace AssetUtility.Documentation
{

    static class UnityMarkdownViewer
    {

        const string cloneUri = "https://github.com/Zumwani/UnityMarkdownViewer.git";
        public const string repoUri = "https://github.com/gwaredd/UnityMarkdownViewer";
        public const string packageName = "com.mischief.markdownviewer";

        static bool? m_isInstalled;
        public static bool? isInstalled
        {
            get
            {
#if UNITY_MARKDOWN_VIEWER
                return m_isInstalled;
#else
                return false;
#endif
            }
            private set => m_isInstalled = value;
        }

        public static bool isRefreshing { get; private set; }
        public static bool isInstalling { get; private set; }

        public static Error error { get; private set; }
        public static bool isRefreshError { get; private set; }
        public static bool isInstallError { get; private set; }

        [InitializeOnLoadMethod]
        static void OnLoad()
        {
            //Events.registeredPackages += Events_registeredPackages;
            //Events.registeringPackages += Events_registeringPackages;
        }

        //static void Events_registeringPackages(PackageRegistrationEventArgs e)
        //{
        //    if (e.removed.Any(package => package.name == packageName))
        //        ScriptingDefineUtility.Set("UNITY_MARKDOWN_VIEWER", false);
        //    isInstalled = null;
        //    Refresh();
        //}

        //static void Events_registeredPackages(PackageRegistrationEventArgs e)
        //{
        //    Refresh();
        //}

        public static void Refresh()
        {

            if (isRefreshing)
                return;
            isRefreshing = true;

            error = null;
            var request = Client.List();
            EditorApplication.update += Update;

            void Update()
            {

                if (!request.IsCompleted)
                    return;

                EditorApplication.update -= Update;
                if (request.Status == StatusCode.Success)
                {

                    isInstalled = request.Result.Any(package => package.name == packageName);
                    isRefreshing = false;

                }
                else
                {

                    isInstalled = null;
                    error = request.Error;
                    isInstallError = false;
                    isRefreshError = true;

                }

                ScriptingDefineUtility.Set("UNITY_MARKDOWN_VIEWER", m_isInstalled ?? false);

                if (DocumentationViewer.GetWindowInstance(out var instance))
                    instance.Repaint();

            }

        }

        public static void Install()
        {

            isInstalling = true;

            EditorUtility.DisplayProgressBar("Unity Package Manager", "Resolving packages", 0);
            var request = Client.Add(cloneUri);
            EditorApplication.update += Update;

            EditorApplication.projectChanged += ProjectChanged;

            void Update()
            {

                if (!request.IsCompleted)
                    return;

                if (request.Status != StatusCode.Success)
                {
                    error = request.Error;
                    isInstallError = true;
                    isRefreshError = false;
                }

                EditorApplication.update -= Update;
                EditorUtility.ClearProgressBar();

                ScriptingDefineUtility.Set("UNITY_MARKDOWN_VIEWER");
                if (DocumentationViewer.GetWindowInstance(out var instance))
                    instance.Repaint();

            }

            void ProjectChanged()
            {
                EditorApplication.projectChanged -= ProjectChanged;
                isInstalling = false;
            }

        }

    }

}