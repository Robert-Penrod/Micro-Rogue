using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HyperQuest.EasyPooling
{
    using Path = System.IO.Path;

    public static class PoolingSettingsAssetManager
    {
        private const string settingsFileName = "PoolingSettings";
        private const string subDirectoryName = "EasyPooling";


        public static string GetRootDirectory()
        {
            // Find the PoolingSettingsAssetManager script
            string[] guids = AssetDatabase.FindAssets("PoolingSettingsAssetManager");
            if (guids.Length == 0)
            {
                Debug.LogError("PoolingSettingsAssetManager script not found in the project.");
                return null;
            }

            // Get the path to the script
            string scriptPath = AssetDatabase.GUIDToAssetPath(guids.First());

            // Get the directory of the script
            string scriptDirectory = Path.GetDirectoryName(scriptPath);

            // Construct the root directory path of the Easy Pooling package
            string rootDirectory = Path.GetFullPath(Path.Combine(scriptDirectory, "..", "..")).Replace("\\", "/");
            rootDirectory = rootDirectory.Replace(Application.dataPath, "Assets"); // Make it relative to the Assets folder

            return rootDirectory; // This will be "Assets/path_to/Easy Pooling"
        }

        public static PoolingSettings GetOrCreateSettings()
        {
            // Find the root directory of the Easy Pooling package
            string rootDirectory = GetRootDirectory();
            string resourcesSubPath = Path.Combine("Resources", subDirectoryName);
            string relativeResourcesPath = Path.Combine(rootDirectory, resourcesSubPath);

            // Ensure the subdirectory within Resources exists
            string fullPathToResourcesSubDirectory = Path.Combine(Application.dataPath, relativeResourcesPath.Replace("Assets/", ""));
            if (!Directory.Exists(fullPathToResourcesSubDirectory))
            {
                Directory.CreateDirectory(fullPathToResourcesSubDirectory);
                AssetDatabase.Refresh();
            }

            // Attempt to load the settings, or create a new asset if it doesn't exist
            PoolingSettings settings = AssetDatabase.LoadAssetAtPath<PoolingSettings>(Path.Combine(relativeResourcesPath, settingsFileName + ".asset"))
                                    ?? CreateAndSavePoolingSettings(relativeResourcesPath);

            return settings;
        }

        private static PoolingSettings CreateAndSavePoolingSettings(string resourcesPath)
        {
            var settings = ScriptableObject.CreateInstance<PoolingSettings>();
            string assetPath = Path.Combine(resourcesPath, settingsFileName + ".asset");
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"PoolingSettings asset created at: {assetPath}");
            return settings;
        }
    }
}
