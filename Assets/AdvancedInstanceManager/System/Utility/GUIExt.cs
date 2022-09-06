using System;
using UnityEditor;
using UnityEngine;

namespace InstanceManager.Editor
{

    /// <summary>Contains a few extra gui functions.</summary>
    public static class GUIExt
    {

        //Called by ColorScope and EnabledScope
        static void Update()
        {
            prevEnabled = null;
            prevColor = null;
        }

        #region ColorScope

        static Color? prevColor;

        /// <summary>
        /// <para>Begins a color scope, this sets <see cref="GUI.color"/> and saves previous value, allowing it to be restored using <see cref="EndColorScope"/>.</para>
        /// <para>See also <see cref="EndColorScope"/></para>
        /// </summary>
        public static void BeginColorScope(Color color)
        {

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            if (prevColor.HasValue)
                throw new Exception($"Cannot use {nameof(BeginColorScope)} before ending existing scope with {nameof(EndColorScope)}.");

            prevColor = GUI.color;
            GUI.color = color;

        }

        /// <summary>Ends the color scope, that was started with <see cref="BeginColorScope(Color)"/>.</summary>
        public static void EndColorScope()
        {
            if (prevColor.HasValue)
                GUI.color = prevColor.Value;
            prevColor = null;
        }

        #endregion
        #region EnabledScope

        static bool? prevEnabled;

        /// <summary>
        /// <para>Begins an enabled scope, this sets <see cref="GUI.enabled"/> and saves previous value, allowing it to be restored using <see cref="EndEnabledScope"/>.</para>
        /// <para>See also <see cref="EndColorScope"/></para>
        /// </summary>
        public static void BeginEnabledScope(bool enabled, bool overrideWhenAlreadyFalse = false)
        {

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            if (!GUI.enabled && !overrideWhenAlreadyFalse)
                return;

            if (prevEnabled.HasValue)
                throw new Exception($"Cannot use {nameof(BeginEnabledScope)} before ending existing scope with {nameof(EndEnabledScope)}.");

            prevEnabled = GUI.enabled;
            GUI.enabled = enabled;

        }

        /// <summary>Ends the enabled scope, that was started with <see cref="BeginEnabledScope(bool)"/>.</summary>
        public static void EndEnabledScope()
        {
            if (prevEnabled.HasValue)
                GUI.enabled = prevEnabled.Value;
            prevEnabled = null;
        }

        #endregion
        #region GenericMenu item

        /// <summary>Adds an item to this <see cref="GenericMenu"/>.</summary>
        /// <param name="menu">The <see cref="GenericMenu"/>.</param>
        /// <param name="content">The content of this item.</param>
        /// <param name="action">The action to perform when click, if <paramref name="enabled"/>.</param>
        /// <param name="enabled">Sets whatever this item is enabled.</param>
        /// <param name="isChecked">Sets if checked.</param>
        /// <param name="offContent">The content to display when item disabled, defaults to <paramref name="content"/> if <see langword="false"/>.</param>
        public static void AddItem(this GenericMenu menu, GUIContent content, Action action, bool enabled = true, bool isChecked = false, GUIContent offContent = null) =>
            AddItem(menu, content, _ => action.Invoke(), isChecked, enabled, offContent);

        /// <summary>Adds an item to this <see cref="GenericMenu"/>.</summary>
        /// <param name="menu">The <see cref="GenericMenu"/>.</param>
        /// <param name="content">The content of this item.</param>
        /// <param name="action">The action to perform when click, if <paramref name="enabled"/>.</param>
        /// <param name="isChecked">Sets if checked.</param>
        /// <param name="enabled">Sets whatever this item is enabled.</param>
        /// <param name="offContent">The content to display when item disabled, defaults to <paramref name="content"/> if <see langword="false"/>.</param>
        public static void AddItem(this GenericMenu menu, GUIContent content, Action<bool> action, bool isChecked, bool enabled = true, GUIContent offContent = null)
        {
            if (enabled)
                menu.AddItem(content, isChecked, () => action.Invoke(!isChecked));
            else
                menu.AddDisabledItem(offContent ?? content, isChecked);
        }

        #endregion
        #region WatermarkTextField

        //A style for grayed out text, we're using this for watermark instead of ColorScope because labels do not use GUI.color in Unity 2018?
        static GUIStyle grayedOut;

        /// <summary>Creates a <see cref="EditorGUILayout.TextField(GUIContent, string, GUILayoutOption[])"/>, but with a watermark.</summary>
        public static string TextField(string text, GUIStyle style, string watermark = null, params GUILayoutOption[] options)
        {

            if (grayedOut == null)
                grayedOut = new GUIStyle() { normal = new GUIStyleState() { textColor = Color.gray } };

            text = EditorGUILayout.TextField(text, style, options);

            if (string.IsNullOrEmpty(text) && watermark != null)
            {
                var r = GUILayoutUtility.GetLastRect();
                EditorGUI.LabelField(new Rect(r.x + 3, r.y + 1.5f, r.width, r.height), watermark, grayedOut);
            }

            return text;

        }

        #endregion

        /// <summary>
        /// <para>Unfocuses elements when blank area of <see cref="UnityEditor.EditorWindow"/> clicked.</para>
        /// <para>Returns true if element was unfocused, you may want to <see cref="UnityEditor.EditorWindow.Repaint"/> then.</para>
        /// </summary>
        public static bool UnfocusOnClick()
        {
            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
                return true;
            }
            return false;
        }

    }

}
