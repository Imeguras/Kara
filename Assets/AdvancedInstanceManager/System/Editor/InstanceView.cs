using System;
using System.Linq;
using InstanceManager.Utility;
using UnityEditor;
using UnityEngine;

namespace InstanceManager.Editor
{

    public partial class InstanceManagerWindow
    {

        class InstanceView : View
        {

            string[] layouts;
            (string path, SceneAsset asset, int index)[] addedScenes;
            (string path, SceneAsset asset, int index)[] scenes;
            public override Vector2? minSize => new Vector2(450, 82);

            const float textFieldWidth = 200;
            const float popupWidth = 204;

            static readonly string[] playModeOptions = { "When primary does", "Manually" };
            static readonly string[] editorOptions = { "Primary editor", "Secondary editor" };

            Color scenesSeparator = new Color32(100, 100, 100, 32);

            public override void OnEnable() =>
                RefreshLayouts();

            public override void OnFocus()
            {
                RefreshLayouts();
                RefreshScenes();
            }

            void RefreshLayouts() =>
                layouts = WindowLayoutUtility.availableLayouts.Select(l => l.name).ToArray();

            void RefreshScenes()
            {

                var allScenes = AssetDatabase.FindAssets("t:" + nameof(SceneAsset)).
                    Select(id => AssetDatabase.GUIDToAssetPath(id)).
                    Select(path => (
                        path,
                        asset: AssetDatabase.LoadAssetAtPath<SceneAsset>(path),
                        index: Array.IndexOf(instance.scenes, path)));

                addedScenes = allScenes.
                    Where(s => instance.scenes?.Contains(s.path) ?? false).
                    OrderBy(s => s.index).
                    ToArray();
                scenes = allScenes.Except(addedScenes).ToArray();

            }

            public override void OnGUI()
            {

                if (instance == null)
                    ClearInstance();

                if (layouts == null)
                    RefreshLayouts();

                EditorGUILayout.BeginVertical(Style.secondaryInstanceMargin);
                Header();
                DisplayName();
                Layout();
                AutoPlayMode();
                OpenPrimaryEditor();
                Scenes();
                EditorGUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                    instance.Save();

            }

            void Header()
            {

                GUILayout.BeginHorizontal();

                if (!InstanceManager.isSecondaryInstance && GUILayout.Button(Content.back))
                {
                    ClearInstance();
                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.LabelField(" ID: " + instance.id);

                GUILayout.FlexibleSpace();

                EditorGUI.BeginChangeCheck();

                var c = new GUIContent(Content.autoSync);
                var size = GUI.skin.label.CalcSize(c);

                EditorGUILayout.LabelField(c, GUILayout.Width(size.x));
                instance.autoSync = EditorGUILayout.ToggleLeft(Content.emptyString, instance.autoSync, GUILayout.Width(16));

                if (InstanceManager.isSecondaryInstance &&
                    GUILayout.Button(Content.reload, GUILayout.ExpandWidth(false)))
                {
                    InstanceManager.SyncWithPrimaryInstance();
                    Open();
                }

                GUILayout.EndHorizontal();

            }

            void DisplayName()
            {

                EditorGUILayout.BeginHorizontal(Style.elementMargin12);
                EditorGUILayout.LabelField("Display name:");
                instance.displayName = GUIExt.TextField(instance.displayName, Style.searchBox, instance.id, GUILayout.Width(textFieldWidth));
                EditorGUILayout.EndHorizontal();

            }

            void Layout()
            {

                if (!WindowLayoutUtility.isAvailable)
                    return;

                EditorGUILayout.BeginHorizontal(Style.elementMargin12);

                EditorGUILayout.LabelField(Content.preferredLayout);

                var i = Array.IndexOf(layouts, instance.preferredLayout ?? "Default");
                if (i == -1) i = 0;
                instance.preferredLayout = layouts[EditorGUILayout.Popup(i, layouts, GUILayout.Width(popupWidth))];

                EditorGUILayout.EndHorizontal();

                if (InstanceManager.isSecondaryInstance && GUILayout.Button(Content.apply, GUILayout.ExpandWidth(false)))
                {
                    WindowLayoutUtility.Find(instance.preferredLayout).Apply();
                    Open();
                }

            }

            void AutoPlayMode()
            {

                EditorGUILayout.BeginHorizontal(Style.elementMargin0);

                EditorGUILayout.LabelField(Content.autoPlayMode);

                var i = instance.enterPlayModeAutomatically ? 0 : 1;
                instance.enterPlayModeAutomatically = EditorGUILayout.Popup(i, playModeOptions, GUILayout.Width(popupWidth)) == 0;

                EditorGUILayout.EndHorizontal();

            }

            void OpenPrimaryEditor()
            {

                EditorGUILayout.BeginHorizontal(Style.elementMargin0);

                EditorGUILayout.LabelField(Content.openPrimaryEditor);

                var i = instance.openEditorInPrimaryEditor ? 0 : 1;
                instance.openEditorInPrimaryEditor = EditorGUILayout.Popup(i, editorOptions, GUILayout.Width(popupWidth)) == 0;

                EditorGUILayout.EndHorizontal();

            }

            Vector2 scroll;
            string q;
            void Scenes()
            {

                if (scenes == null)
                    RefreshScenes();

                EditorGUILayout.BeginVertical(Style.elementMargin12);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(Content.scenesToOpen);

                GUILayout.FlexibleSpace();
                GUILayout.FlexibleSpace();

                q = GUIExt.TextField(q, Style.searchBox, "Search:", GUILayout.Width(textFieldWidth));

                EditorGUILayout.EndHorizontal();

                scroll = EditorGUILayout.BeginScrollView(scroll, Style.scenesList, GUILayout.MaxHeight(float.MaxValue), GUILayout.MinWidth(window.position.width - 24));

                if (addedScenes?.Any() ?? false)
                {

                    foreach (var scene in addedScenes)
                        DrawScene(scene, canReorder: true);

                    if (scenes.Any())
                    {
                        var r = GUILayoutUtility.GetLastRect();
                        GUIExt.BeginColorScope(scenesSeparator);
                        GUI.Label(new Rect(r.x, r.yMax + 8, r.width, 2), Content.emptyString, Style.scenesSeparator);
                        GUIExt.EndColorScope();
                        EditorGUILayout.Space();
                    }

                }

                if (scenes != null)
                    foreach (var scene in scenes)
                    {

                        if (string.IsNullOrWhiteSpace(scene.path))
                            continue;

                        if (!string.IsNullOrWhiteSpace(q) && !scene.path.ToLower().Contains(q.ToLower()))
                            continue;

                        DrawScene(scene);

                    }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.FlexibleSpace();
                if (InstanceManager.isSecondaryInstance && addedScenes.Any() && GUILayout.Button(Content.openScenes))
                    SceneUtility.OpenScenes(addedScenes.Select(s => s.path).ToArray());

                EditorGUILayout.EndHorizontal();

            }

            void DrawScene((string path, SceneAsset asset, int index) scene, bool canReorder = false)
            {

                EditorGUILayout.BeginHorizontal(Style.secondaryInstanceMargin);

                var value = instance.scenes?.Contains(scene.path) ?? false;
                var newValue = EditorGUILayout.Toggle(value, GUILayout.Width(16), GUILayout.Height(22));
                if (value != newValue)
                {
                    instance.SetScene(scene.path, enabled: newValue);
                    RefreshScenes();
                }

                GUIExt.BeginEnabledScope(false);
                EditorGUILayout.ObjectField(scene.asset, typeof(SceneAsset), allowSceneObjects: false, GUILayout.Height(22));
                GUIExt.EndEnabledScope();

                if (canReorder)
                {

                    GUIExt.BeginEnabledScope(scene.index > 0);
                    if (GUILayout.Button(Content.up, Style.moveSceneButton, GUILayout.ExpandWidth(false)))
                    {
                        instance.SetScene(scene.path, index: scene.index - 1);
                        RefreshScenes();
                    }
                    GUIExt.EndEnabledScope();

                    GUIExt.BeginEnabledScope(scene.index < instance.scenes?.Length - 1);
                    if (GUILayout.Button(Content.down, Style.moveSceneButton, GUILayout.ExpandWidth(false)))
                    {
                        instance.SetScene(scene.path, index: scene.index + 1);
                        RefreshScenes();
                    }
                    GUIExt.EndEnabledScope();

                }

                EditorGUILayout.EndHorizontal();

            }

        }

    }

}
