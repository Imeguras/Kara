using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InstanceManager.Models;
using UnityEditor;

namespace InstanceManager.Utility
{

    /// <summary>Provides utility functions for sending 'events' to secondary instances.</summary>
    public static class CrossProcessEventUtility
    {

        static string path =>
            InstanceManager.isSecondaryInstance
                ? InstanceManager.instance.eventsFile
                : Paths.primaryEventsFile;

        #region Watcher

        /// <summary>Initializes the event listener.</summary>
        internal static void Initialize()
        {

            if (!File.Exists(path))
                File.Create(path);

            var file = new FileInfo(path);

            var lastUpdate = DateTime.Now;
            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            void Update()
            {

                if (DateTime.Now - lastUpdate < TimeSpan.FromSeconds(0.5))
                    return;
                lastUpdate = DateTime.Now;

                file.Refresh();
                if (file.Length > 0)
                    OnEvent();

            }

        }

        static void OnEvent()
        {

            if (!File.Exists(path) || new FileInfo(path).Length == 0)
                return;

            var str = File.ReadAllText(path);
            File.WriteAllText(path, null);

            var names = str.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in names.Distinct().ToArray())
                RaiseEvent(
                    name: name.Remove(name.IndexOf(Paths.InstanceSeparatorChar)),
                    param: name.Substring(name.IndexOf(Paths.InstanceSeparatorChar) + 1));

        }

        #endregion

        /// <summary>Sends an event to all open secondary instances.</summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="param">The parameter to send. Must be single line.</param>
        public static void Send(string name, string param = null)
        {
            foreach (var instance in InstanceManager.instances.ToArray())
                Send(instance, name, param);
        }

        /// <summary>Sends an event to the specified secondary instance.</summary>
        /// <param name="instance">The instance to send the event to.</param>
        /// <param name="name">The name of the event.</param>
        /// <param name="param">The parameter to send. Must be single line.</param>
        public static void Send(UnityInstance instance, string name, string param = null)
        {
            if (InstanceManager.isPrimaryInstance)
                Send(instance.eventsFile, name, param);
        }

        /// <summary>Sends an event to the primary instance.</summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="param">The parameter to send. Must be single line.</param>
        public static void SendToHost(string name, string param = null)
        {
            if (InstanceManager.isSecondaryInstance)
                Send(Paths.primaryEventsFile, name, param);
        }

        static void Send(string path, string name, string param = null)
        {

            if (param?.Contains(Environment.NewLine) ?? false)
                throw new ArgumentException("Param cannot be a multiline string.");

            if (!File.Exists(path))
                return;

            //Listeners can be added to primary instance
            RaiseEvent(name, param);
            using (var writer = File.AppendText(path))
                writer.WriteLine(name + Paths.InstanceSeparatorChar + param);

        }

        static readonly Dictionary<string, List<Action<string>>> listeners = new Dictionary<string, List<Action<string>>>();

        /// <summary>Adds a listener to the specified event.</summary>
        public static void On(string name, Action<string> action)
        {
            if (!listeners.ContainsKey(name))
                listeners.Add(name, new List<Action<string>>());
            listeners[name].Add(action);
        }

        /// <summary>Adds a listener to the specified event.</summary>
        public static void On(string name, Action action) =>
            On(name, (_) => action?.Invoke());

        static void RaiseEvent(string name, string param)
        {

            if (listeners.TryGetValue(name, out var list))
                foreach (var callback in list)
                    callback?.Invoke(param);

        }

    }

}
