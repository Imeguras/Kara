using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace InstanceManager.Utility
{

    /// <summary>Provides functions for interacting with SymLinker.exe.</summary>
    static class SymLinkUtility
    {

        /// <summary>These should not be linked, for use with use <see cref="string.EndsWith(string)"/>.</summary>
        static readonly string[] blacklist =
        {
            "-lock",
            "\\Search",
            "LastSceneManagerSetup.txt" ,
            "EditorInstance.json",
            "ArtifactDB",
            "SourceAssetDB",
            "\\Bee",
            ".instance",
            ".instance-events",
            "EditorSnapSettings.asset",
        };

        /// <summary>Creates a new secondary instance.</summary>
        public static Task Create(string projectPath, string targetPath, Action onComplete = null, Action afterCreateFolder = null, bool hideProgress = false) =>
           ProgressUtility.RunTask(
               displayName: "Creating instance",
               onComplete: (t) => onComplete?.Invoke(),
               hideProgress: hideProgress,
               task: new Task(async () =>
               {

                   if (Directory.Exists(targetPath))
                       Directory.Delete(targetPath, true);
                   Directory.CreateDirectory(targetPath);

                   if (afterCreateFolder != null)
                       EditorApplication.delayCall += afterCreateFolder.Invoke;

                   await Task.WhenAll(GenerateTasks().ToArray());

                   IEnumerable<Task> GenerateTasks()
                   {

                       //Link folders
                       yield return SymLinkRelative("Assets");
                       yield return SymLinkRelative("Packages");
                       yield return SymLinkRelative("ProjectSettings");
                       yield return SymLinkRelative("UserSettings");

                       //Link files
                       foreach (var file in Directory.GetFiles(projectPath, "*", SearchOption.TopDirectoryOnly))
                           if (!blacklist.Any(str => file.EndsWith(str)))
                               yield return SymLinkRelative(Path.GetFileName(file));

                       //Link all items in 'Library' folder, we need to do these individually since
                       //we need to avoid lock files and files causing conflicts
                       Directory.CreateDirectory(Path.Combine(targetPath, "Library"));
                       foreach (var file in Directory.GetFileSystemEntries(Path.Combine(projectPath, "Library"), "*", SearchOption.TopDirectoryOnly))
                       {

                           if (blacklist.Any(b => file.EndsWith(b)))
                               continue;

                           var path = Path.Combine(targetPath, "Library", Path.GetFileName(file));
                           yield return SymLink(file, path);

                       }

                       yield return Copy(Path.Combine(projectPath, "Library", "ArtifactDB"), Path.Combine(targetPath, "Library", "ArtifactDB"));
                       yield return Copy(Path.Combine(projectPath, "Library", "SourceAssetDB"), Path.Combine(targetPath, "Library", "SourceAssetDB"));

                       Task SymLinkRelative(string relativePath) =>
                           SymLink(
                               linkPath: Path.Combine(targetPath, relativePath),
                               path: Path.Combine(projectPath, relativePath));

                       Task SymLink(string path, string linkPath) =>
                           Task.Run(() =>
                           {

                               if (!File.Exists(path) && !Directory.Exists(path))
                                   return;

                               CommandUtility.RunCommand(
                                   windows: $"mklink {(Directory.Exists(path) ? "/j" : "/h")} {linkPath.ToWindowsPath().WithQuotes()} {path.ToWindowsPath().WithQuotes()}",
                                   linux: $"ln -s {path.WithQuotes()} {linkPath.WithQuotes()}");

                           });


                       Task Copy(string path, string destination) =>
                            Task.Run(() =>
                            {
                                if (File.Exists(path))
                                    File.Copy(path, destination);
                                else if (Directory.Exists(path))
                                    CopyDirectory(path, destination);
                            });

                   }

               }));

        static void CopyDirectory(string root, string dest)
        {

            foreach (var directory in Directory.GetDirectories(root))
            {
                var name = Path.GetFileName(directory);
                if (!Directory.Exists(Path.Combine(dest, name)))
                    Directory.CreateDirectory(Path.Combine(dest, name));
                CopyDirectory(directory, Path.Combine(dest, name));
            }

            foreach (var file in Directory.GetFiles(root))
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));

        }

        /// <summary>Deletes a secondary instance from Unity Hub. Only windows is currently supported.</summary>
        public static Task DeleteHubEntry(string instancePath, Action onComplete = null, bool hideProgress = false) =>
            ProgressUtility.RunTask(
               displayName: "Deleting hub entry",
               onComplete: (t) => onComplete?.Invoke(),
               hideProgress: hideProgress,
               task: new Task(() =>
               {
#if UNITY_EDITOR_WIN
                   using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Unity Technologies\Unity Editor 5.x", writable: true))
                       foreach (var name in key.GetValueNames().Where(n => n.StartsWith("RecentlyUsedProjectPaths")))
                       {
                           var value = Encoding.ASCII.GetString((byte[])key.GetValue(name));
                           if (value.StartsWith(instancePath.ToCrossPlatformPath()))
                               key.DeleteValue(name);
                       }

#elif UNITY_EDITOR_LINUX

                   var file = File.ReadAllText(@"/home/pc/.local/share/unity3d/prefs");
                   var lines = file.Split(new[] { Environment.NewLine.ToString() }, StringSplitOptions.None);
                   for (var i = 0; i < lines.Length; i++)
                   {
                       var line = lines[i];
                       if (line.Contains(@"<pref name=""RecentlyUsedProjectPaths"))
                       {

                           var base64 = line.Substring(line.IndexOf('>') + 1);
                           base64 = base64.Remove(base64.IndexOf("<"));

                           var value = Encoding.ASCII.GetString(Convert.FromBase64String(base64));
                           if (value.StartsWith(instancePath.ToCrossPlatformPath()))
                               line = line.Replace(base64, null);

                           lines[i] = line;

                       }
                   }

                   File.WriteAllText(@"/home/pc/.local/share/unity3d/prefs", string.Join(Environment.NewLine, lines));

#endif
               }));

        /// <summary>Deletes a secondary instance.</summary>
        public static Task Delete(string path, Action onComplete = null, bool hideProgress = false) =>
            ProgressUtility.RunTask(
               displayName: "Removing instance",
               onComplete: (t) => onComplete?.Invoke(),
               hideProgress: hideProgress,
               //Deleting with cmd, which prevents 'Directory not empty error', for Directory.Delete(path, recursive: true)
               task: new Task(() =>
                   CommandUtility.RunCommand(
                       windows: $"rmdir /s/q {path.ToWindowsPath().WithQuotes()}",
                       linux: $"rm -rf {path.WithQuotes()}")));

    }

}
