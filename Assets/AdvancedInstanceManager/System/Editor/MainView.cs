using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InstanceManager.Models;
using InstanceManager.Utility;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InstanceManager.Editor
{

    public partial class InstanceManagerWindow
    {

        class MainView : View
        {

            public override void OnEnable() => EditorApplication.update += Update;
            public override void OnDisable() => EditorApplication.update -= Update;

            void Update() => UpdatePromo();

            public override Vector2? minSize => new Vector2(450, 350);

            Color background => EditorGUIUtility.isProSkin
                ? new Color32(60, 60, 60, 255)
                : new Color32(194, 194, 194, 255);

            Color listBackground = new Color32(40, 40, 40, 255);
            Color line1 = new Color32(35, 35, 35, 0);
            Color line2 = new Color32(35, 35, 35, 255);

            bool isScrollbarInitialized;
            Vector2 scrollPos;
            Vector2 maxScroll;

            float normalizedYScroll =>
                isScrollbarInitialized && maxScroll.y != 0
                ? scrollPos.y / maxScroll.y
                : 0;

            void BeginScrollbar()
            {

                if (window.hasResized)
                    isScrollbarInitialized = false;

                //Workaround for not having a way to get normalized scroll pos
                //Just set scroll pos to float.MaxValue and let unity clamp to max and save value,
                //then reset
                if (!isScrollbarInitialized && Event.current.type == EventType.Repaint)
                {
                    var scroll = scrollPos;
                    scrollPos = new Vector2(0, float.MaxValue);
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUIStyle.none, GUI.skin.verticalScrollbar);
                    maxScroll = scrollPos;
                    scrollPos = scroll;
                    isScrollbarInitialized = true;
                }
                else
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUIStyle.none, GUI.skin.verticalScrollbar);

            }

            public override void OnGUI()
            {

                GUIExt.BeginColorScope(listBackground);
                GUI.DrawTexture(new Rect(0, 0, position.width, position.height), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
                GUIExt.EndColorScope();

                DrawInstanceRow("Name:", Content.status, isHeader: true);

                BeginScrollbar();

                foreach (var instance in InstanceManager.instances.OfType<UnityInstance>().ToArray())
                    DrawInstanceRow(instance);

                if (!InstanceManager.instances.Any())
                    EditorGUILayout.LabelField(Content.noInstances, Style.noItemsText, GUILayout.Height(42));

                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();
                DrawFooter();

            }

            void DrawInstanceRow(UnityInstance instance) =>
                DrawInstanceRow(
                    name: instance.effectiveDisplayName,
                    status: instance.isRunning ? Content.running : Content.notRunning,
                    openButtonValue: instance.isRunning,
                    isEnabled: !instance.isSettingUp,
                    instance);

            readonly List<float> statusLabelPos = new List<float>();
            void DrawInstanceRow(string name, GUIContent status, bool? openButtonValue = null, bool isEnabled = true, UnityInstance instance = null, bool isHeader = false)
            {

                var r = GUILayoutUtility.GetRect(Screen.width, 0);

                GUIExt.BeginEnabledScope(isEnabled);

                EditorGUILayout.BeginHorizontal(Style.row);

                GUIExt.BeginColorScope(background);
                GUI.DrawTexture(new Rect(0, r.yMin, window.maxSize.x - 1, Style.row.fixedHeight), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
                GUIExt.EndColorScope();

                EditorGUILayout.LabelField(name);
                if (isHeader)
                {

                    var pos = !statusLabelPos.Any()
                        ? position.width - 150
                        : statusLabelPos.Min();

                    EditorGUILayout.LabelField(GUIContent.none);
                    r = GUILayoutUtility.GetLastRect();
                    var width = GUI.skin.label.CalcSize(status).x;
                    GUI.Label(new Rect(pos, r.y, width, r.height), status);
                    statusLabelPos.Clear();

                }
                else
                {

                    GUILayout.Label(status, GUILayout.ExpandWidth(false));

                    if (Event.current.type == EventType.Repaint)
                        statusLabelPos.Add(GUILayoutUtility.GetLastRect().x);

                }

                if (!openButtonValue.HasValue)
                    GUILayout.Label(Content.emptyString, GUILayout.ExpandWidth(false));
                else if (instance.needsRepair)
                {
                    if (GUILayout.Button(Content.repair, GUILayout.ExpandWidth(false)))
                        InstanceUtility.Repair(instance, instance.path);
                }
                else if (GUILayout.Button(instance.isRunning ? Content.close : Content.open, GUILayout.ExpandWidth(false)))
                    instance.ToggleOpen();

                menuButtonPressed = openButtonValue.HasValue && GUILayout.Button(Content.menu, Style.menu, GUILayout.ExpandWidth(false));

                GUILayout.Space(6);

                EditorGUILayout.EndHorizontal();
                if (instance != null)
                    ContextMenu_Item(instance);

                GUIExt.EndEnabledScope();

                if (!isHeader)
                {
                    GUIExt.BeginColorScope(listBackground);
                    GUI.DrawTexture(new Rect(0, r.yMax, window.maxSize.x, 1), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
                    GUIExt.EndColorScope();
                }

            }

            bool menuButtonPressed;
            void ContextMenu_Item(UnityInstance instance)
            {

                var rect = new Rect(0, GUILayoutUtility.GetLastRect().y + 73, window.position.width, 40);
                var pos = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y + 73);

                if (menuButtonPressed || (Event.current.type == EventType.ContextClick && rect.Contains(pos)))
                {

                    var menu = new GenericMenu();

                    if (!instance.isRunning)
                        menu.AddItem(Content.open, instance.Open, enabled: !instance.needsRepair);
                    else
                        menu.AddItem(Content.close, instance.Close, enabled: !instance.needsRepair);

                    menu.AddSeparator(string.Empty);
                    menu.AddItem(Content.showInExplorer, false, () => Process.Start(instance.path));

                    menu.AddItem(Content.options, () => SetInstance(instance.id), enabled: !instance.isRunning);
                    menu.AddSeparator(string.Empty);
                    menu.AddItem(Content.delete, instance.Remove, enabled: !instance.isRunning);

                    menu.ShowAsContext();

                    Repaint();

                }

            }

            const float footerHeight = 42;
            void DrawFooter()
            {

                var r = GUILayoutUtility.GetRect(Screen.width, 1);

                GUIExt.BeginColorScope(background);
                GUI.DrawTexture(new Rect(0, r.yMin, window.maxSize.x, window.maxSize.y), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
                GUIExt.EndColorScope();

                GUIExt.BeginColorScope(normalizedYScroll == 1 ? line1 : line2);
                GUI.DrawTexture(r, EditorGUIUtility.whiteTexture);
                GUIExt.EndColorScope();

                EditorGUILayout.BeginHorizontal(Style.margin, GUILayout.Height(footerHeight));

                GUILayout.FlexibleSpace();

                var promoPos = GUILayoutUtility.GetLastRect();
                promoPos.height = footerHeight;

                DrawPromo(promoPos);

                if (GUILayout.Button(Content.createNewInstance, Style.createButton))
                {
                    InstanceUtility.Create();
                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.EndHorizontal();

            }

            #region Promo

            Texture2D promoBackground;
            Texture2D[] promoSwaps;
            int promoSwap;
            const float promoSwapDelay = 4;
            double lastPromoSwapDelayTime;

            void UpdatePromo()
            {

                if (promoSwaps == null)
                    return;

                if (EditorApplication.timeSinceStartup - lastPromoSwapDelayTime > promoSwapDelay)
                {
                    lastPromoSwapDelayTime = EditorApplication.timeSinceStartup;
                    promoSwap += 1;
                    if (promoSwap > promoSwaps.GetUpperBound(0))
                        promoSwap = 0;
                    Repaint();
                }

            }

            bool isMouseDownPromo;
            void DrawPromo(Rect position)
            {

                if (!promoBackground)
                    promoBackground = Resources.Load<Texture2D>("InstanceManager/Promotion/background");

                if (promoSwaps == null)
                    promoSwaps = Resources.LoadAll<Texture2D>("InstanceManager/Promotion/swap");

                var r3 = new Rect(position);
                r3.yMin += (position.height / 2) - (promoBackground.height / 2);
                r3.xMin -= 6;
                r3.width = promoBackground.width;
                r3.height = promoBackground.height;
                GUI.DrawTexture(r3, promoBackground);
                GUI.DrawTexture(r3, promoSwaps[promoSwap]);

                EditorGUIUtility.AddCursorRect(r3, MouseCursor.Link);

                var isMouseOver = r3.Contains(Event.current.mousePosition);

                if (Event.current.isMouse)
                    if (!isMouseOver)
                        isMouseDownPromo = false;
                    else if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        isMouseDownPromo = true;
                    else if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && isMouseDownPromo)
                        Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/advanced-scene-manager-174152");

            }

            #endregion Promo

        }

    }

}
