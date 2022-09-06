using UnityEditor;
using UnityEditor.Callbacks;

namespace InstanceManager.Utility
{

    /// <summary>Manages open scripts in primary editor, if selected.</summary>
    static class ScriptAssetOpenUtility
    {

        [InitializeOnLoadMethod]
        static void OnLoad()
        {

            if (InstanceManager.isSecondaryInstance)
                return;

            CrossProcessEventUtility.On("open", (param) =>
            {

                int.TryParse(param.Remove(param.IndexOf("|")), out var line);
                var path = param.Substring(param.IndexOf("|") + 1);

                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (script)
                    AssetDatabase.OpenAsset(script, line);

            });

        }

        [OnOpenAsset(0)]
        public static bool OnOpen(int instanceID, int line)
        {

            if (InstanceManager.isSecondaryInstance && InstanceManager.instance.openEditorInPrimaryEditor &&
                EditorUtility.InstanceIDToObject(instanceID) is MonoScript script)
            {
                CrossProcessEventUtility.SendToHost("open", line + "|" + AssetDatabase.GetAssetPath(script));
                return true;
            }

            return false;

        }

    }

}
