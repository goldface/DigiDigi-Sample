using Path = System.IO.Path;
using System.Collections.Generic;
using System.Diagnostics;
using GooglePlayServices;
using Loxodon.Framework.Bundles;
using Loxodon.Framework.Bundles.Editors;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor.Batch
{
    [DisallowMultipleComponent]
    public class Builder : MonoBehaviour
    {
        [MenuItem("@GameEditor/Export Table")]
        public static void ExportTable()
        {
            var startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = $"{Application.dataPath}/../../../ExternalTools";
#if UNITY_EDITOR_WIN
            startInfo.FileName = $"{Application.dataPath}/../../../ExternalTools/UpdateTable.bat";
#else
            startInfo.FileName = $"{Application.dataPath}/../../../ExternalTools/UpdateTable.sh";
#endif
            startInfo.UseShellExecute = true;
            var process = Process.Start(startInfo);
            process.WaitForExit();
            
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        [MenuItem("@GameEditor/Build Android")]
        public static void BuildAndroid()
        {
            var keystorePathName =
                Path.Combine(Application.dataPath, "..", "..", "..", "Keystores", "digi_google.keystore");
            PlayerSettings.Android.keystoreName = keystorePathName;
            PlayerSettings.Android.keystorePass = "voidSetup(0);";
            PlayerSettings.Android.keyaliasName = "digi_google";
            PlayerSettings.Android.keyaliasPass = "voidSetup(0);";

            var scenes = EditorBuildSettings.scenes;
            List<string> sceneList = new List<string>();

            foreach (var scene in scenes)
            {
                if (scene.enabled)
                    sceneList.Add(scene.path);
            }

#if UNITY_ANDROID
            if (PlayServicesResolver.ResolveSync(true) == false)
            {
                EditorUtility.DisplayDialog("Error", "Build Failed!", "OK");
                return;
            }
#endif

            var sceneArray = sceneList.ToArray();
            BuildPipeline.BuildPlayer(sceneArray, "UnityAndroid.apk", BuildTarget.Android, BuildOptions.None);
        }

        [MenuItem("@GameEditor/Build Android With Bundles")]
        public static void BuildAndroidWithBundles()
        {
            BuildAndroidAssetBundles();

            BuildAndroid();
        }

        [MenuItem("@GameEditor/Build Android AssetBundles")]
        public static void BuildAndroidAssetBundles()
        {
            var buildVM = new BuildVM();
            buildVM.Algorithm = Algorithm.AES128_CBC_PKCS7;
            buildVM.Compression = CompressOptions.ChunkBasedCompression;
            buildVM.BuildTarget = BuildTarget.Android;
            buildVM.DataVersion = Application.version;
            buildVM.IV = "5m1blU2EuZDqTYL3";
            buildVM.KEY = "RasIy8jSCTeaFzcD";
            buildVM.FilterType = EncryptionFilterType.All;
            buildVM.Encryption = true;
            buildVM.CopyToStreaming = true;
            buildVM.UseHashFilename = true;
            buildVM.OutputPath = "AssetBundles";
            buildVM.UsePlayerSettingVersion = true;
            buildVM.Build(false);
        }
    }
}
