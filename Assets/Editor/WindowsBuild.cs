#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEditor.Build.Reporting;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class WindowsBuild
{
    private const string BuildProfilePath = "Assets/Settings/Build Profiles/Main.asset";
    private const string BuildPath = "Builds/Windows/PNTD.exe";

    public static void PerformBuild()
    {
        BuildAddressables();

        var buildProfile = AssetDatabase.LoadAssetAtPath<BuildProfile>(BuildProfilePath);

        if (buildProfile == null)
        {
            throw new Exception($"Build Profile not found: {BuildProfilePath}");
        }

        var options = new BuildPlayerWithProfileOptions
        {
            buildProfile = buildProfile,
            locationPathName = BuildPath
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Build failed: {report.summary.result}");
        }

        Console.WriteLine("Build succeeded");
    }

    private static void BuildAddressables()
    {
        AddressableAssetSettings settings =
            AddressableAssetSettingsDefaultObject.Settings;

        if (settings == null)
        {
            throw new Exception("AddressableAssetSettings not found.");
        }

        Console.WriteLine("Cleaning Addressables content...");
        AddressableAssetSettings.CleanPlayerContent();

        Console.WriteLine("Building Addressables content...");
        AddressableAssetSettings.BuildPlayerContent();

        Console.WriteLine("Addressables build succeeded");
    }
}

#endif