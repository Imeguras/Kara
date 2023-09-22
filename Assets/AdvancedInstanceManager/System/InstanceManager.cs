using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InstanceManager.Models;
using InstanceManager.Utility;
using UnityEditor;
using UnityEngine;

namespace InstanceManager
{

    /// <summary>The main class of Instance Manager.</summary>
    public static class InstanceManager
    {

        /// <summary>The secondary instances that have been to this project.</summary>
        public static IEnumerable<UnityInstance> instances => InstanceUtility.Enumerate();

        /// <summary>The current instance. <see langword="null"/> if primary.</summary>
        public static UnityInstance instance { get; } =
            InstanceUtility.LocalInstance();

        /// <summary>Gets if the current instance is the primary instance.</summary>
        public static bool isPrimaryInstance => instance == null;

        /// <summary>Gets if the current instance is a secondary instance.</summary>
        public static bool isSecondaryInstance => instance != null;

        static string m_ID;
        /// <summary>Gets the id of the current instance.</summary>
        public static string id =>
            isPrimaryInstance
            ? m_ID
            : instance?.id;

        #region Events

        /// <summary>Occurs during startup if current instance is secondary.</summary>
        public static event Action OnSecondInstanceStarted;

        /// <summary>Occurs when primary instance is paused.</summary>
        public static event Action OnPrimaryPause;

        /// <summary>Occurs when primary instance is unpaused.</summary>
        public static event Action OnPrimaryUnpause;

        /// <summary>Occurs when primary instance enters play mode.</summary>
        public static event Action OnPrimaryEnterPlayMode;

        /// <summary>Occurs when primary instance exiting play mode.</summary>
        public static event Action OnPrimaryExitPlayMode;

        /// <summary>Occurs when primary instance has had its assets changed.</summary>
        public static event Action OnPrimaryAssetsChanged;

        class AssetsChangedCallback : AssetPostprocessor
        {

            static void OnPostprocessAllAssets(string[] _, string[] __, string[] ___, string[] ____)
            {
                if (isPrimaryInstance)
                    CrossProcessEventUtility.Send(nameof(OnPrimaryAssetsChanged));
            }

        }

        static void SetupCrossProcessEvents(bool isPrimary)
        {

            CrossProcessEventUtility.Initialize();

            if (isPrimary)
            {

                EditorApplication.playModeStateChanged += (state) =>
                {

                    if (state == PlayModeStateChange.ExitingEditMode)
                        CrossProcessEventUtility.Send(nameof(OnPrimaryEnterPlayMode));
                    else if (state == PlayModeStateChange.ExitingPlayMode)
                        CrossProcessEventUtility.Send(nameof(OnPrimaryExitPlayMode));
                };

                EditorApplication.pauseStateChanged += (state) =>
                {
                    if (state == PauseState.Paused)
                        CrossProcessEventUtility.Send(nameof(OnPrimaryPause));
                    else if (state == PauseState.Unpaused)
                        CrossProcessEventUtility.Send(nameof(OnPrimaryUnpause));
                };

            }
            else
            {

                CrossProcessEventUtility.On(nameof(OnPrimaryEnterPlayMode), () => OnPrimaryEnterPlayMode?.Invoke());
                CrossProcessEventUtility.On(nameof(OnPrimaryExitPlayMode), () => OnPrimaryExitPlayMode?.Invoke());
                CrossProcessEventUtility.On(nameof(OnPrimaryPause), () => OnPrimaryPause?.Invoke());
                CrossProcessEventUtility.On(nameof(OnPrimaryUnpause), () => OnPrimaryUnpause?.Invoke());
                CrossProcessEventUtility.On(nameof(OnPrimaryAssetsChanged), () => OnPrimaryAssetsChanged?.Invoke());

                OnPrimaryEnterPlayMode += () => { if (instance.enterPlayModeAutomatically) EditorApplication.isPlaying = true; };
                OnPrimaryExitPlayMode += () => { if (instance.enterPlayModeAutomatically) EditorApplication.isPlaying = false; };
                OnPrimaryPause += () => { if (instance.enterPlayModeAutomatically) EditorApplication.isPaused = true; };
                OnPrimaryUnpause += () => { if (instance.enterPlayModeAutomatically) EditorApplication.isPaused = false; };
                OnPrimaryAssetsChanged += () => { if (instance.autoSync) SyncWithPrimaryInstance(); };

                CrossProcessEventUtility.On("Quit", () => EditorApplication.Exit(0));

            }

        }

        #endregion

        [InitializeOnLoadMethod]
        static void OnLoad()
        {

#if UNITY_EDITOR_WIN
            //Prevents taskbar from sometimes flashing window
            WindowUtility.Initialize();
#endif

            if (isPrimaryInstance)
                InitializePrimaryInstance();
            else
                InitializeSecondInstance();

            SetupCrossProcessEvents(isPrimaryInstance);

        }

        static void InitializePrimaryInstance()
        {

            m_ID = File.Exists(InstanceUtility.instanceFileName) ? File.ReadAllText(InstanceUtility.instanceFileName) : null;
            if (string.IsNullOrWhiteSpace(m_ID))
                File.WriteAllText(InstanceUtility.instanceFileName, m_ID = IDUtility.Generate());

            EditorApplication.wantsToQuit += () =>
            {
                foreach (var instance in instances)
                    instance?.Close();
                return true;
            };

        }

        static async void InitializeSecondInstance()
        {

            if (!InstanceUtility.IsLocked())
            {
                await Task.Delay(100);
                WindowLayoutUtility.Find(instance.preferredLayout).Apply();
                InstanceUtility.SetLocked(true);
            }

            EditorApplication.quitting += () =>
                InstanceUtility.SetLocked(false);

#if UNITY_EDITOR_WIN
            EditorApplication.playModeStateChanged += (state) =>
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                    WindowUtility.StopTaskbarFromFlashing();
            };
#endif
            OnSecondInstanceStarted?.Invoke();

        }

        /// <summary>Sync this instance with the primary instance, does nothing if current instance is primary.</summary>
        public static void SyncWithPrimaryInstance()
        {

            if (isPrimaryInstance || Application.isPlaying)
                return;

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            SceneUtility.ReloadScenes();
#if !UNITY_2018
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
#endif

        }

    }

}
