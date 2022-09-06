using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InstanceManager.Models;
using UnityEditor;
using UnityEngine;

namespace InstanceManager.Utility
{

    /// <summary>Provides utility functions for working with secondary instances.</summary>
    public static class InstanceUtility
    {

        /// <summary>Occurs when an instance is changed.</summary>
        public static event Action onInstancesChanged;

        /// <summary>The name of the instance settings file.</summary>
        public const string instanceFileName = ".instance";

        static string instanceLockFile => Paths.project + "/" + instanceFileName + "-lock";

        /// <summary>Gets if instance is 'locked'.</summary>
        internal static bool IsLocked() =>
            File.Exists(instanceLockFile);

        /// <summary>'Locks' this instance.</summary>
        internal static void SetLocked(bool isLocked)
        {
            if (isLocked && InstanceManager.isSecondaryInstance)
                File.WriteAllText(instanceLockFile, null);
            else if (!isLocked && IsLocked())
                File.Delete(instanceLockFile);
        }

        /// <summary>'Unlocks' the instance.</summary>
        internal static void UnlockInstance(UnityInstance instance)
        {
            var path = instance.filePath + "-lock";
            if (File.Exists(path))
                File.Delete(path);
        }

        /// <summary>Loads local instance file. Returns <see langword="null"/> if none exists or instance is primary.</summary>
        public static UnityInstance LocalInstance() =>
            Load(Paths.project + "/" + instanceFileName);

        /// <summary>Finds the secondary instance with the specified id.</summary>
        public static UnityInstance Find(string id) =>
            Load(Paths.InstancePath(id) + "/" + instanceFileName);

        /// <summary>Finds the secondary instance using the specified predicate.</summary>
        public static UnityInstance Find(Func<UnityInstance, bool> predicate) =>
            Enumerate().FirstOrDefault(predicate);

        /// <summary>Enumerates all secondary instances for this project.</summary>
        public static IEnumerable<UnityInstance> Enumerate() =>
            Directory.GetDirectories(Paths.aboveProject).
            Select(f => ActionUtility.Try(() => Load(Path.Combine(f, instanceFileName)), hideError: true)).
            OfType<UnityInstance>().
            Where(instance => instance.primaryID == InstanceManager.id);

        /// <summary>Create a new secondary instance. Returns null if current instance is secondary.</summary>
        public static UnityInstance Create()
        {

            if (InstanceManager.isSecondaryInstance)
                return null;

            var id = IDUtility.Generate(validate: _id => !Directory.Exists(Paths.InstancePath(_id)));
            var path = Paths.InstancePath(id);
            var instance = new UnityInstance(id, InstanceManager.id);
            settingUp.Add(instance.id);

            SymLinkUtility.Create(Paths.project, path,
                 afterCreateFolder: instance.Save,
                 onComplete: () =>
                 {
                     settingUp.Remove(instance.id);
                     onInstancesChanged?.Invoke();
                 });

            return instance;

        }

        /// <summary>Repairs the instance. No effect if current instance is secondary.</summary>
        public static Task Repair(UnityInstance instance, string path) =>
            ProgressUtility.RunTask(
               displayName: "Repairing instance",
               canRun: InstanceManager.isPrimaryInstance,
               task: new Task(async () =>
               {

                   settingUp.Add(instance.id);

                   await SymLinkUtility.Delete(path, hideProgress: true);
                   Directory.CreateDirectory(path);
                   await SymLinkUtility.Create(Paths.project, path, hideProgress: true);

                   settingUp.Remove(instance.id);
                   onInstancesChanged?.Invoke();

               }));

        /// <summary>Gets if the instance needs to be repaired.</summary>
        public static bool NeedsRepair(UnityInstance instance) =>
            !Directory.Exists(Paths.InstancePath(instance.id)) ||
            !Directory.EnumerateFileSystemEntries(Paths.InstancePath(instance.id), "*.*", SearchOption.AllDirectories).Any();

        internal static void Refresh(UnityInstance instance)
        {

            if (Path.GetFileName(instance.filePath) != instanceFileName)
                return;

            if (!File.Exists(instance.filePath))
                return;

            var json = File.ReadAllText(instance.filePath);
            JsonUtility.FromJsonOverwrite(json, instance);

        }

        static UnityInstance Load(string path)
        {

            if (path.StartsWith(Paths.PrimaryInstancePath() + "/"))
                return null;

            if (Path.GetFileName(path) != instanceFileName)
                return null;

            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return JsonUtility.FromJson<UnityInstance>(json);

        }

        internal static void Save(UnityInstance instance)
        {
            var json = JsonUtility.ToJson(instance);
            File.WriteAllLines(instance.filePath.ToCrossPlatformPath(), new[] { json });
            RaiseOnInstancesChanged();
        }

        internal static void Remove(UnityInstance instance)
        {

            if (instance.isRunning)
                throw new Exception("Cannot remove instance while running!");

            settingUp.Add(instance.id);
            SymLinkUtility.Delete(instance.path, RaiseOnInstancesChanged);

        }

        static void RaiseOnInstancesChanged()
        {
            EditorApplication.delayCall += () => onInstancesChanged?.Invoke();
            EditorApplication.QueuePlayerLoopUpdate();
        }

        static readonly List<string> settingUp = new List<string>();

        /// <summary>Gets if the <see cref="UnityInstance"/> is being set up, this would be when its being created, or when being removed.</summary>
        public static bool IsInstanceBeingSetUp(UnityInstance instance) =>
            settingUp.Contains(instance.id);

    }

}
