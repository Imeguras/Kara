using System.Linq;
using UnityEditor.SceneManagement;

namespace InstanceManager
{

    static class SceneUtility
    {

        /// <summary>Open the specified scenes.</summary>
        public static void OpenScenes(params string[] paths)
        {

            paths = paths.Where(path => !string.IsNullOrWhiteSpace(path)).ToArray();

            if (paths is null || paths.Length == 0)
                return;

            var setup = paths.Select(path => new SceneSetup() { path = path, isLoaded = true }).ToArray();
            setup[0].isActive = true;
            EditorSceneManager.RestoreSceneManagerSetup(setup);

        }

        /// <summary>Reloads the currently open scenes.</summary>
        public static void ReloadScenes()
        {
            if (EditorSceneManager.GetSceneManagerSetup().Length > 0)
                EditorSceneManager.RestoreSceneManagerSetup(EditorSceneManager.GetSceneManagerSetup());
        }

    }

}
