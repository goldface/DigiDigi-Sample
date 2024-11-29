using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using Debug = JJFramework.Runtime.Extension.Debug;
using Loxodon.Framework.Bundles;
using Loxodon.Framework.Examples.Bundle;
using Loxodon.Framework.Security.Cryptography;
using UniRx;
using Object = UnityEngine.Object;

namespace Scripts.Core.IO
{
    public class AssetBundleManager
    {
        private IResources _resources;
        private BundleManifest _manifest;

        private IStreamDecryptor _cryptoGraph;
        
        public int BundleMaxCount { get; private set; }
        public int BundleLoadedCount { get; private set; }
        public bool IsLoaded
        {
            get { return BundleLoadedCount == BundleMaxCount; }
        }
        public bool HasError { get; private set; }
        private readonly Dictionary<int, float> _progressDic = new Dictionary<int, float>();
        
        public float CurrentProgress
        {
            get
            {
                var result = _progressDic.Sum(d => d.Value) / BundleMaxCount;
                return result;
            }
        }

        private const string ASSET_BUNDLE_PATH = "GameAsset";
        private const string IV = "5m1blU2EuZDqTYL3";
        private const string KEY = "RasIy8jSCTeaFzcD";
        
        public bool IsInitialize { get; private set; }

        // Start is called before the first frame update
        public void Initialize()
        {
            if (IsInitialize)
            {
                Debug.LogWarning("Already initialized!");
                return;
            }

            IsInitialize = true;
            
#if UNITY_EDITOR
            if (SimulationSetting.IsSimulationMode)
            {
                /* Create a BundleManifestLoader. */
                IBundleManifestLoader manifestLoader = new BundleManifestLoader();

                /* Loads BundleManifest. */
                _manifest = manifestLoader.Load(BundleUtil.GetReadOnlyDirectory() + BundleSetting.ManifestFilename);

/* Create a PathInfoParser. */
//IPathInfoParser pathInfoParser = new SimplePathInfoParser("@");
                IPathInfoParser pathInfoParser = new SimulationAutoMappingPathInfoParser();
/* Create a BundleManager */
                IBundleManager manager = new SimulationBundleManager();
/* Create a BundleResources */
                _resources = new BundleResources(pathInfoParser, manager);
            }
            else
            {
                return;
            }
#endif
            {
                /* Create a BundleManifestLoader. */
                IBundleManifestLoader manifestLoader = new BundleManifestLoader();

                /* Loads BundleManifest. */
                _manifest = manifestLoader.Load(BundleUtil.GetReadOnlyDirectory() + BundleSetting.ManifestFilename);

                /* Create a PathInfoParser. */
                IPathInfoParser pathInfoParser = new AutoMappingPathInfoParser(_manifest);

                IStreamDecryptor decryptor = CryptographUtil.GetDecryptor(Algorithm.AES128_CBC_PKCS7, Encoding.ASCII.GetBytes(KEY), Encoding.ASCII.GetBytes(IV));

                /* Use a BundleLoaderBuilder */
                ILoaderBuilder builder = new CustomBundleLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()), true, decryptor);

                /* Create a BundleManager */
                IBundleManager manager = new BundleManager(_manifest, builder);

                /* Create a BundleResources */
                _resources = new BundleResources(pathInfoParser, manager);
            }
        }

        public IEnumerator PreloadAllBundles()
        {
            HasError = false;
            _progressDic.Clear();
            
#if UNITY_EDITOR
            if (false == SimulationSetting.IsSimulationMode)
            {
                yield break;
            }
#endif

            var bundleList = _manifest.GetAll();
            BundleLoadedCount = 0;
            BundleMaxCount = bundleList.Length;
            for (int i = 0; i < BundleMaxCount; ++i)
            {
                var bundleInfo = bundleList[i];
                var result = _resources.LoadBundle(bundleInfo.FullName);
                var progressIndex = i;
                _progressDic.Add(progressIndex, 0f);

                result.Callbackable().OnProgressCallback(progress =>
                {
                    _progressDic[progressIndex] = progress;
                });
                result.Callbackable().OnCallback((r) =>
                {
                    try
                    {
                        if (r.Exception != null)
                        {
                            HasError = true;
                            throw r.Exception;
                        }

                        r.Result.AddTo(DOTween.instance.gameObject);
                        if (false == r.IsCancelled
                            && null != r.Result)
                        {
                            Debug.Log($"Loaded: {r.Result.Name}");
                            ++BundleLoadedCount;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Load failure.Error:{e}");
                    }
                });
            }

            while (BundleLoadedCount < BundleMaxCount)
            {
                if (HasError)
                {
                    break;
                }
                
                yield return null;
            }

            if (BundleLoadedCount != BundleMaxCount)
            {
                Debug.LogError("Failed to preload the AssetBundles");
            }
            else
            {
                Debug.Log("Completed to preload!");
            }
        }

        public T Load<T>(string bundleName, string assetName) where T : Object
        {
            var path = $"{ASSET_BUNDLE_PATH}/{bundleName}/{assetName}";
#if UNITY_EDITOR
            if (false == SimulationSetting.IsSimulationMode)
            {
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>($"Assets/{path}");
                Debug.Log($"Asset Load in Editor Mode: {asset != null} / {path}");
                return asset;
            }
#endif
            var result = _resources.LoadAsset<T>(path);
            return result;
        }

        public void Cleanup()
        {
            _progressDic.Clear();
            if (null != _manifest
                && null != _resources)
            {
                var activeList = _manifest.GetAllActivated();
                foreach (var bundle in activeList)
                {
                    var obj = _resources.GetBundle(bundle.Name);
                    obj?.Dispose();
                }
            }

            _manifest = null;
            _cryptoGraph = null;
            _resources = null;
        }
    }
}
