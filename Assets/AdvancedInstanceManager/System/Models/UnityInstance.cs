using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InstanceManager.Utility;
using UnityEditor;
using UnityEngine;

namespace InstanceManager.Models
{

    /// <summary>Represents a secondary unity instance.</summary>
    [Serializable]
    public class UnityInstance : ISerializationCallbackReceiver
    {

        /// <summary>Create a new instance of <see cref="UnityInstance"/>.</summary>
        public UnityInstance()
        { }

        /// <summary>Create a new instance of <see cref="UnityInstance"/>.</summary>
        /// <param name="id">The id of this <see cref="UnityInstance"/>.</param>
        /// <param name="primaryID">The id of the primary instance that this <see cref="UnityInstance"/> is associated with.</param>
        public UnityInstance(string id, string primaryID)
        {
            m_ID = id;
            m_primaryID = primaryID;
            needsRepair = InstanceUtility.NeedsRepair(this);
        }

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {

            //Save instance process id
            if (InstanceProcess != null)
            {
                m_processID = InstanceProcess.Id;
                InstanceProcess.Exited -= InstanceProcess_Exited;
            }
            else
                m_processID = 0;

        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {

            //Find instance process
            try
            {
                if (m_processID > 0)
                {
                    InstanceProcess = Process.GetProcessById(m_processID);
                    InstanceProcess.Exited += InstanceProcess_Exited;
                }
            }
            catch
            { }

            m_processID = -1;

        }

        #endregion
        #region Properties

        [SerializeField] private string m_primaryID;
        [SerializeField] private string m_ID;
        [SerializeField] private int m_processID;
        [SerializeField] private string m_preferredLayout = "Default";
        [SerializeField] private bool m_autoSync = true;
        [SerializeField] private bool m_enterPlayModeAutomatically = true;
        [SerializeField] private string[] m_scenes;
        [SerializeField] private string m_displayName;
        [SerializeField] private bool m_openEditorInPrimaryEditor = false;

        /// <summary>The path to this instance file.</summary>
        internal string filePath => Paths.InstancePath(id) + "/" + InstanceUtility.instanceFileName;

        /// <summary>The path to the lock file.</summary>
        internal string lockPath => Paths.InstancePath(id) + "/" + InstanceUtility.instanceFileName + "-lock";

        /// <summary>The path to the event file.</summary>
        internal string eventsFile => Paths.InstancePath(id) + "/" + InstanceUtility.instanceFileName + "-events";

        /// <summary>Gets if this instance needs repairing.</summary>
        public bool needsRepair { get; }

        /// <summary>The display name of this instance.</summary>
        public string displayName
        {
            get => m_displayName;
            set => m_displayName = value;
        }

        /// <summary>Gets either <see cref="displayName"/>, or <see cref="id"/> depending on whatever <see cref="displayName"/> has value.</summary>
        public string effectiveDisplayName =>
            string.IsNullOrWhiteSpace(displayName)
            ? id
            : displayName;

        /// <summary>Gets or sets the window layout.</summary>
        public string preferredLayout
        {
            get => m_preferredLayout;
            set => m_preferredLayout = value;
        }

        /// <summary>Gets or sets whatever this instance should auto sync asset changes.</summary>
        public bool autoSync
        {
            get => m_autoSync;
            set => m_autoSync = value;
        }

        /// <summary>Gets or sets whatever scripts should open in the editor that is associated with the primary instance.</summary>
        public bool openEditorInPrimaryEditor
        {
            get => m_openEditorInPrimaryEditor;
            set => m_openEditorInPrimaryEditor = value;
        }

        /// <summary>Gets or sets whatever this instance should enter / exit play mode automatically when primary instance does.</summary>
        public bool enterPlayModeAutomatically
        {
            get => m_enterPlayModeAutomatically;
            set => m_enterPlayModeAutomatically = value;
        }

        /// <summary>Gets the scenes this instance should open when starting.</summary>
        public string[] scenes
        {
            get => m_scenes;
            set => m_scenes = value;
        }

        /// <summary>Gets whatever this instance is running.</summary>
        public bool isRunning
        {
            get
            {
                InstanceProcess?.Refresh();
                return InstanceProcess != null && !InstanceProcess.HasExited;
            }
        }

        /// <summary>Gets the id of this instance.</summary>
        public string id => m_ID;

        /// <summary>Gets the primary instance id that this instance is associated with.</summary>
        public string primaryID => m_primaryID;

        /// <summary>Gets the path of this instance.</summary>
        public string path => Paths.InstancePath(id);

        /// <summary>Gets if the instance is currently being set up.</summary>
        public bool isSettingUp => InstanceUtility.IsInstanceBeingSetUp(this);

        /// <summary>Gets the process of this instance, if it is running.</summary>
        public Process InstanceProcess { get; private set; }

        #endregion
        #region Methods

        /// <summary>Saves the instance settings to disk.</summary>
        public void Save() =>
            InstanceUtility.Save(this);

        /// <summary>Removes the instance from disk.</summary>
        public void Remove() =>
            InstanceUtility.Remove(this);

        /// <summary>Refreshes this <see cref="UnityInstance"/>.</summary>
        public void Refresh() =>
            InstanceUtility.Refresh(this);

        /// <summary>Set property of scene.</summary>
        /// <param name="enabled">Set whatever this scene is enabled or not.</param>
        /// <param name="index">Set the index of this scene.</param>
        public void SetScene(string path, bool? enabled = null, int? index = null)
        {

            if (m_scenes is null)
                m_scenes = Array.Empty<string>();

            if (enabled.HasValue)
            {
                if (enabled.Value && !m_scenes.Contains(path))
                    ArrayUtility.Add(ref m_scenes, path);
                else if (m_scenes.Contains(path))
                    ArrayUtility.Remove(ref m_scenes, path);
            }

            if (index.HasValue && m_scenes.Contains(path))
            {
                index = Mathf.Clamp(index.Value, 0, m_scenes.Length - 1);
                ArrayUtility.Remove(ref m_scenes, path);
                ArrayUtility.Insert(ref m_scenes, index.Value, path);
            }

        }

        /// <summary>Open if not running, othewise close.</summary>
        public void ToggleOpen()
        {
            if (isRunning)
                Close();
            else
                Open();
        }

        /// <summary>Open instance.</summary>
        public void Open()
        {

            if (InstanceManager.isSecondaryInstance || isRunning || needsRepair)
                return;

            InstanceUtility.UnlockInstance(this);
            SetupScenes();
            InstanceProcess = Process.Start(new ProcessStartInfo(
                fileName: EditorApplication.applicationPath,
                arguments: "-projectPath " + path.WithQuotes()));

            Save();
            InstanceProcess.EnableRaisingEvents = true;
            InstanceProcess.Exited += InstanceProcess_Exited;

        }

        /// <summary>Closes this instance.</summary>
        public void Close() =>
            Close(null);

        /// <summary>Closes this instance.</summary>
        /// <param name="onClosed">Callback when instance is fully closed, since closing happens async.</param>
        public void Close(Action onClosed = null)
        {

            if (InstanceManager.isSecondaryInstance || !isRunning || InstanceProcess is null)
                return;

            //Lets copy variable, since if we use property when killing process after 5
            //seconds we'll end up killing new instance process, if one started
            var process = InstanceProcess;

            process.Exited -= InstanceProcess_Exited;
            if (!process.HasExited)
            {

                //Send quit request since unity won't save settings unless EditorApplication.Exit() is called.
                //Process.Close() does nothing and Process.CloseMainWindow() closes, but does not save

                process.Exited += Exited;
                CrossProcessEventUtility.Send(this, "Quit");

                //In the off chance that the event was not registered in the secondary instance, lets kill process after 5 seconds
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    process?.Kill();
                });

                void Exited(object sender, EventArgs e)
                {
                    process.Exited -= Exited;
                    OnClosed();
                }

            }
            else
                OnClosed();

            void OnClosed()
            {

                if (InstanceProcess == process)
                    InstanceProcess = null;

                this.OnClosed();

                EditorApplication.delayCall +=
                    () => onClosed?.Invoke();

            }

        }

        void OnClosed()
        {
            SymLinkUtility.DeleteHubEntry(path);
            Save();
        }

        void InstanceProcess_Exited(object sender, EventArgs e) =>
            OnClosed();

        void SetupScenes()
        {

            var root = "sceneSetups:";
            var isFirstScene = true;

            string GetSceneString(string scenePath)
            {

                var str =
                    "- path: " + scenePath + Environment.NewLine +
                    "  isLoaded: 1" + Environment.NewLine +
                    "  isActive: " + (isFirstScene ? "1" : "0") + Environment.NewLine +
                    "  isSubScene: 0";

                isFirstScene = false;
                return str;

            }

            var path = Path.Combine(this.path, "Library", "LastSceneManagerSetup.txt");
            if (scenes.Any())
            {
                var yaml = root + Environment.NewLine + string.Join(Environment.NewLine, scenes?.Select(GetSceneString) ?? Array.Empty<string>());
                File.WriteAllText(path, yaml);
            }
            else if (File.Exists(path))
                File.Delete(path);

        }

        #endregion

    }

}
