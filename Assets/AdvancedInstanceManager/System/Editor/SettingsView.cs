using UnityEditor;
using UnityEngine;

namespace InstanceManager.Editor
{

    public partial class InstanceManagerWindow
    {

        /// <summary>This class is unused! It it kept here in case we need plugin settings in the future.</summary>
        class SettingsView : View
        {

            //string instancesPath;
            public override void OnEnable()
            {

            }

            public override void OnGUI()
            {

                EditorGUILayout.BeginVertical(Style.margin);
                Header();
                EditorGUILayout.EndVertical();

            }

            //Color grayedOutText = new Color(1, 1, 1, 0.5f);

            void Header()
            {

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(Content.back))
                {

                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.LabelField(Content.settingsText, Style.header);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

            }

        }

    }

}
