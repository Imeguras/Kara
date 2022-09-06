using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditorInternal;

namespace InstanceManager.Utility
{

#if UNITY_EDITOR_WIN

    /// <summary>Provides utility functions for working with windows.</summary>
    static class WindowUtility
    {

        #region Pinvoke

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [Flags]
        public enum FlashMode : uint
        {
            /// 
            /// Stop flashing. The system restores the window to its original state.
            /// 
            FLASHW_STOP = 0,
            /// 
            /// Flash the window caption.
            /// 
            FLASHW_CAPTION = 1,
            /// 
            /// Flash the taskbar button.
            /// 
            FLASHW_TRAY = 2,
            /// 
            /// Flash both the window caption and taskbar button.
            /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
            /// 
            FLASHW_ALL = 3,
            /// 
            /// Flash continuously, until the FLASHW_STOP flag is set.
            /// 
            FLASHW_TIMER = 4,
            /// 
            /// Flash continuously until the window comes to the foreground.
            /// 
            FLASHW_TIMERNOFG = 12
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        #endregion

        static IntPtr mainWindowHandle
        {
            get => (IntPtr)EditorPrefs.GetInt("InstanceManager.MainWindowHandle:" + InstanceManager.id, 0);
            set => EditorPrefs.SetInt("InstanceManager.MainWindowHandle:" + InstanceManager.id, (int)value);
        }

        public static void Initialize()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        static void Update()
        {

            if (InternalEditorUtility.isApplicationActive)
            {
                EditorApplication.update -= Update;
                mainWindowHandle = GetActiveWindow();
            }

        }

        /// <summary>Stops the taskbar icon from flashing.</summary>
        public static void StopTaskbarFromFlashing()
        {

            var fInfo = new FLASHWINFO
            {
                hwnd = mainWindowHandle,
                dwFlags = (uint)FlashMode.FLASHW_STOP,
                uCount = 0,
                dwTimeout = 0,
            };

            fInfo.cbSize = (uint)Marshal.SizeOf(fInfo);
            _ = FlashWindowEx(ref fInfo);

        }

    }
    
#endif

}
