using System.IO;
using UnityEditor;
using UnityEngine;

namespace HyperQuest.EasyPooling
{
    using Path = System.IO.Path;

    public class PoolingSettingsWindow : EditorWindow
    {
        private PoolingSettings settings;

        [MenuItem("Tools/Easy Pooling/Settings")]
        private static void Open()
        {
            string title = "Easy Pooling Settings";
            var window = GetWindow<PoolingSettingsWindow>(title);
            window.Show();

            string iconPath = Path.GetFullPath(Path.Combine(PoolingSettingsAssetManager.GetRootDirectory(), "Scripts", "Editor", "icon.png")).Replace("\\", "/").Replace(Application.dataPath, "Assets");
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if(icon != null)
            {
                window.titleContent = new GUIContent(title, icon);
            }
        }

        private void OnEnable()
        {
            settings = PoolingSettingsAssetManager.GetOrCreateSettings();
        }

        /*
        private void OnGUI()
        {
            if (settings == null)
            {
                settings = PoolingSettingsAssetManager.GetOrCreateSettings();
            }

            EditorGUI.BeginChangeCheck();
            float margin = 10;
            EditorGUIUtility.labelWidth = 180;
            GUILayout.Space(margin);


            bool oldShowPoolsInHierarchy = settings.ShowPoolsInHierarchy;
            GUILayout.BeginHorizontal();
            GUILayout.Space(margin);
            settings.ShowPoolsInHierarchy = EditorGUILayout.Toggle(new GUIContent("Show Pools In Hierarchy", "Toggle this to show or hide pools in the Unity hierarchy."), settings.ShowPoolsInHierarchy);
            GUILayout.Space(margin);
            GUILayout.EndHorizontal();
            if(settings.ShowPoolsInHierarchy != oldShowPoolsInHierarchy)
            {
                if (EditorApplication.isPlaying)
                {
                    StaticPooler.SetPoolsShown(settings.ShowPoolsInHierarchy);
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(margin);
            settings.DefaultPoolSize = EditorGUILayout.IntField(new GUIContent("Default Pool Size", "The default size of each pool created."), settings.DefaultPoolSize);
            GUILayout.Space(margin);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(margin);
            settings.ThinningCheckRate = EditorGUILayout.FloatField(new GUIContent("Thinning Check Rate (seconds)", "Time interval in seconds at which the pool will check and adjust its size to free memory."), settings.ThinningCheckRate);
            GUILayout.Space(margin);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(margin);
            settings.MaxThinCount = EditorGUILayout.IntField(new GUIContent("Max Thin Count", "The amount of objects that can be destroyed at once to free memory (larger values may cause lag spikes)"), settings.MaxThinCount);
            GUILayout.Space(margin);
            GUILayout.EndHorizontal();

            GUILayout.Space(margin);
            GUILayout.BeginHorizontal();
            GUILayout.Space(margin);
            if (GUILayout.Button("Reset to Default", GUILayout.Width(100)))
            {
                ResetSettings();
            }
            GUILayout.Space(margin);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }
        }

        void ResetSettings()
        {
            settings.Reset();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            StaticPooler.SetPoolsShown(settings.ShowPoolsInHierarchy);
            Repaint();
        }
        */
    }
}
