using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JJFramework.Runtime.Resource;
using Scripts.Core.Auth;
using Scripts.Core.DB;
using Scripts.Core.IO;
using Scripts.UI;
using Scripts.Utility;
using UniRx;
using UnityEngine.SceneManagement;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.Core
{
    [DisallowMultipleComponent]
    public class App
    {
        public static bool pIsDebug
        {
            get
            {
#if __DEBUG__
                return true;
#else
                return false;
#endif
            }
        }
        private static bool mIsInitialize;

        public static readonly MasterDB db = new MasterDB();
        public static readonly AssetBundleManager Bundle = new AssetBundleManager();
        public static readonly SoundManager Sound = new SoundManager();
        public static readonly BaseLoginModule Auth = new GoogleLoginModule(); // NOTE(JJO): 추후 iOS쪽 진출할 경우 변경해야 함.
        public static readonly BaseIOModule IO
#if UNITY_EDITOR
            = new LocalIOModule();
#elif UNITY_ANDROID
            = new GoogleIOModule(); // NOTE(JJO): 추후 iOS쪽 진출할 경우 변경해야 함.
#else
            ;
#endif
        public static readonly TimeManager Time = new TimeManager();
        public static readonly UserInfoManager User = new UserInfoManager();
        public static readonly AdvertiseManager Advertise = new AdvertiseManager();

        public static async void Initialize()
        {
            if (mIsInitialize)
            {
                Debug.LogWarning($"Is Already Initialized");
                return;
            }
            
#if UNITY_ANDROID
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#elif UNITY_IOS
            iPhoneSettings.screenCanDarken = false;
#endif
            
#if __DEBUG__
            SRDebug.Init();
#endif
            
            GameAnalyticsManager.Instance.pEnabled = true;
            GameAnalyticsManager.Instance.Initialize();

            // NOTE(JJO): Bundle에서 Preload 시 DOTween의 Object를 사용하므로 미리 수동 초기화.
            DOTween.Init();
          
            Bundle.Initialize();
            // NOTE(JJO): 이걸 여기에 해야할지 고민인데 일단 둬봤음...
            await Bundle.PreloadAllBundles();
                        
            DataLoader.SetResourceLoader(Bundle.Load<TextAsset>, "dataasset");
            db.Init();

            Sound.Init((bundleName, soundName) =>
            {
                var lResult = Bundle.Load<AudioClip>(bundleName, soundName);
                return lResult;
            }, 20, "SoundAsset");

            // NOTE(JJO): 사전에 사운드 체크 여부 결정.
            var lIsSoundOff = PlayerPrefs.GetInt("SOUND_OFF") == 1;
            Sound.SetBGMVolume(lIsSoundOff ? 0f : 1f);
            Sound.SetEffectVolume(lIsSoundOff ? 0f : 1f);
            
            Auth.Initialize();
            
            // user.Initialize();
            Advertise.Initialize(
#if __DEBUG__
                true,
#else
                false,
#endif
                true);

            Observable.EveryUpdate()
                .Where(d => Input.GetKeyDown(KeyCode.Escape) &&
                            (SceneManager.GetActiveScene().name == "MainLobbyScene" ||
                             SceneManager.GetActiveScene().name == "ResultScene"))
                .Subscribe(_ =>
                {
                    var popup = GameObject.FindObjectOfType<Popup>(true);
                    popup?.Initialize("Quit the Game?", () => Application.Quit(0), () => { }, null);
                });

            mIsInitialize = true;
        }

        public static void Cleanup()
        {
            Sound.Cleanup();
            Bundle.Cleanup();

            mIsInitialize = false;
        }
    }
}
