using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace InstanceManager.Utility
{

    /// <summary>Provides methods for enumerating and applying window layouts.</summary>
    public static class WindowLayoutUtility
    {

        static readonly MethodInfo loadWindowLayout;
        static readonly MethodInfo getCurrentLayoutPath;
        static readonly PropertyInfo layoutsModePreferencesPath;

        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        static WindowLayoutUtility()
        {

            var windowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");

            loadWindowLayout = windowLayout?.GetMethod("LoadWindowLayout", flags, null, new[] { typeof(string), typeof(bool) }, null);
            getCurrentLayoutPath = windowLayout?.GetMethod("GetCurrentLayoutPath", flags);
            layoutsModePreferencesPath = windowLayout.GetProperty("layoutsModePreferencesPath", flags);

            layoutsPath = (string)layoutsModePreferencesPath?.GetValue(null);

        }

        /// <summary>Gets whatever the utility was able to find the internal unity methods or not.</summary>
        public static bool isAvailable => !(layoutsPath is null);

        /// <summary>The path to the layouts folder.</summary>
        public static string layoutsPath { get; private set; }

        /// <summary>Finds all available layouts.</summary>
        public static Layout[] availableLayouts =>
            isAvailable
            ? Directory.GetFiles(layoutsPath, "*.wlt").
                Select(path => new Layout(path)).
                ToArray()
            : Array.Empty<Layout>();

        /// <summary>Finds the specified layout by name.</summary>
        public static Layout Find(string name) =>
            availableLayouts.FirstOrDefault(l => l.name == name);

        /// <summary>Gets the current layout.</summary>
        public static Layout? GetCurrent()
        {
            if (GetLastLayoutName() is string str)
                return new Layout(layoutsPath + "/" + str);
            return null;
        }

        static string GetLastLayoutName()
        {

            var path = (string)getCurrentLayoutPath?.Invoke(null, null);

            if (!File.Exists(path))
                return null;

            var propertyName = "m_LastLoadedLayoutName: ";

            using (var reader = File.OpenText(path))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.TrimStart().StartsWith(propertyName))
                        return line.Substring(line.IndexOf(": ") + 2);
                }

            }

            return null;

        }

        /// <summary>Represents a window layout.</summary>
        public struct Layout
        {

            /// <summary>Path on disk to this layout.</summary>
            public string path { get; }

            /// <summary>The name of this layout.</summary>
            public string name { get; }

            public Layout(string path)
            {
                this.path = path;
                name = Path.GetFileNameWithoutExtension(path);
            }

            /// <summary>Applies this layout, if available.</summary>
            public void Apply()
            {
                if (isAvailable && File.Exists(path) && path.EndsWith(".wlt") && GetCurrent()?.name != name)
                    loadWindowLayout?.Invoke(null, new object[] { path, true });
            }


        }

    }

}
