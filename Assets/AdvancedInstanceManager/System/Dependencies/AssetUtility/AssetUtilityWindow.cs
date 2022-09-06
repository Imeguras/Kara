using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetUtility
{

    static class ViewUtility
    {

        public static View[] views { get; } = FindViews();

        public static View view { get; private set; }

        public static void Set(View view)
        {
            ViewUtility.view?.OnDisable();
            ViewUtility.view = view;
            view?.OnEnable();
        }

        static View[] FindViews() =>
            typeof(ViewUtility).Assembly.GetTypes().
                Where(t => t.BaseType == typeof(View) && (t.GetConstructor(Type.EmptyTypes)?.IsPublic ?? false)).
                Select(t => (View)Activator.CreateInstance(t)).
                Reverse().
                ToArray();

    }

    internal class AssetUtilityWindow : EditorWindow
    {

        //[MenuItem("Tools/Asset Utility")]
        static void Open()
        {
            var w = GetWindow<AssetUtilityWindow>();
            w.titleContent = new GUIContent("Asset Utility");
        }

        void OnEnable()
        {

            if (ViewUtility.views.Any())
            {
                if (selectedView < 0)
                    selectedView = 0;
                if (selectedView >= ViewUtility.views.Length)
                    selectedView = ViewUtility.views.Length - 1;
                ViewUtility.Set(ViewUtility.views.ElementAt(selectedView));
            }

            ViewUtility.view?.OnEnable();

        }

        void OnDisable()
        {
            ViewUtility.view?.OnDisable();
            selectedView = Array.IndexOf(ViewUtility.views, ViewUtility.view);
        }

        void OnFocus() => ViewUtility.view?.OnFocus();
        void OnLostFocus() => ViewUtility.view?.OnLostFocus();
        void Content() => ViewUtility.view?.OnGUI();

        Vector2 scrollHeader;
        Vector2 scrollContent;
        void OnGUI()
        {

            wantsMouseMove = true;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            scrollHeader = EditorGUILayout.BeginScrollView(scrollHeader, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width((headerWidth ?? 150) + 12));
            Header();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            if (!isHeaderScrollVisible && Event.current.type == EventType.Repaint)
            {
                var c = GUI.color;
                GUI.color = new Color(1, 1, 1, 0.1f);
                GUI.DrawTexture(new Rect(headerWidth ?? 150, 0, 1, position.height), EditorGUIUtility.whiteTexture);
                GUI.color = c;
            }

            EditorGUILayout.BeginVertical(new GUIStyle() { margin = new RectOffset(0, 16, 16, 16) });
            scrollContent = EditorGUILayout.BeginScrollView(scrollContent);
            Content();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            ViewUtility.view?.OnEnable();

            if (Event.current.type == EventType.MouseMove)
                Repaint();

            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
                Repaint();
            }

        }

        static float? headerWidth;
        GUIStyle style;

        static int selectedView
        {
            get => PlayerPrefs.GetInt("AssetUtility.SelectedView", 0);
            set => PlayerPrefs.SetInt("AssetUtility.SelectedView", value);
        }

        bool isHeaderScrollVisible;

        void Header()
        {

            if (style == null)
                style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = 14, padding = new RectOffset(left: 12, right: 12, 0, 0) };

            if (!headerWidth.HasValue)
                headerWidth = ViewUtility.views.Select(v => v.header).Select(h => new GUIContent(h)).Select(c => style.CalcSize(c).x).Max();

            foreach (var view in ViewUtility.views)
            {

                var rect = GUILayoutUtility.GetRect(headerWidth.Value, 42, GUILayout.ExpandWidth(false));

                var c = GUI.color;
                GUI.color = view == ViewUtility.view ? new Color(1, 1, 1, 0.1f) : Color.clear;

                if (rect.Contains(Event.current.mousePosition))
                    GUI.color = new Color(1, 1, 1, 0.25f);

                GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
                GUI.color = c;

                if (GUI.Button(rect, view.header, style))
                    ViewUtility.Set(view);

                isHeaderScrollVisible = rect.yMax > position.height;

            }

        }

    }

}
