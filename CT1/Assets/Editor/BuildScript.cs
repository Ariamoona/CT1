using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildScript : MonoBehaviour
{
    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        Debug.Log("Starting build process for all platforms...");

      
        string buildsFolder = Path.Combine(Application.dataPath, "../Builds");
        if (!Directory.Exists(buildsFolder))
        {
            Directory.CreateDirectory(buildsFolder);
        }

        try
        {
           
            BuildPC(buildsFolder);

            BuildAndroid(buildsFolder);

            BuildWebGL(buildsFolder);

            Debug.Log("All builds completed successfully!");
            EditorUtility.DisplayDialog("Build Complete", "All builds finished successfully!", "OK");
        }
        catch (Exception e)
        {
            Debug.LogError($"Build failed: {e.Message}");
            EditorUtility.DisplayDialog("Build Failed", $"Build failed: {e.Message}", "OK");
        }
    }

    [MenuItem("Build/Build PC (Windows)")]
    public static void BuildPCOnly()
    {
        string buildsFolder = Path.Combine(Application.dataPath, "../Builds");
        BuildPC(buildsFolder);
    }

    [MenuItem("Build/Build Android")]
    public static void BuildAndroidOnly()
    {
        string buildsFolder = Path.Combine(Application.dataPath, "../Builds");
        BuildAndroid(buildsFolder);
    }

    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGLOnly()
    {
        string buildsFolder = Path.Combine(Application.dataPath, "../Builds");
        BuildWebGL(buildsFolder);
    }

    private static void BuildPC(string buildsFolder)
    {
        Debug.Log("Building for PC (Windows)...");

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        }

        string pcFolder = Path.Combine(buildsFolder, "PC");
        if (!Directory.Exists(pcFolder))
        {
            Directory.CreateDirectory(pcFolder);
        }

        string location = Path.Combine(pcFolder, "MyGame.exe");

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = location,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log($"PC build completed: {location}");
    }

    private static void BuildAndroid(string buildsFolder)
    {
        Debug.Log("Building for Android...");

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }

        string androidFolder = Path.Combine(buildsFolder, "Android");
        if (!Directory.Exists(androidFolder))
        {
            Directory.CreateDirectory(androidFolder);
        }

        string location = Path.Combine(androidFolder, "MyGame.apk");

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = location,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log($"Android build completed: {location}");
    }

    private static void BuildWebGL(string buildsFolder)
    {
        Debug.Log("Building for WebGL...");

        PlayerSettings.WebGL.template = "PROJECT:MyCustomTemplate";
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        }

        string webglFolder = Path.Combine(buildsFolder, "WebGL");
        if (!Directory.Exists(webglFolder))
        {
            Directory.CreateDirectory(webglFolder);
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = webglFolder,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log($"WebGL build completed: {webglFolder}");
    }

    private static string[] GetEnabledScenes()
    {
        var scenes = new System.Collections.Generic.List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenes.Add(scene.path);
            }
        }
        return scenes.ToArray();
    }
}