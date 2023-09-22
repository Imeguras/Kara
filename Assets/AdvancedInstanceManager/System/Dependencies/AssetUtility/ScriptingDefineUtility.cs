using System.Linq;
using UnityEditor;

namespace AssetUtility
{

    static class ScriptingDefineUtility
    {

        public static void Unset(string name) =>
            Set(name, false);

        public static void Set(string name, bool enabled = true)
        {

            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            if (enabled && !defines.Contains(name))
                defines.Add(name);
            else if (!enabled)
                defines.Remove(name);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", defines));

        }

    }

}
