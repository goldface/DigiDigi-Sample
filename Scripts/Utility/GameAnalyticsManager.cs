using GameAnalyticsSDK;
using JJFramework.Runtime;
using UnityEngine;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.Utility
{
    public class GameAnalyticsManager : MonoSingleton<GameAnalyticsManager>
    {
        private const string cAdSdkName = "unityads";
        
        public bool pEnabled { private get; set; }
        private bool mIsInitialize;

        public void Initialize()
        {
            if (pEnabled == false)
            {
                Debug.LogWarning("GA is not enabled!");
                return;
            }

            if (mIsInitialize)
            {
                Debug.LogWarning("Already Initialized!");
                return;
            }

            GameAnalytics.SettingsGA.VerboseLogBuild =
#if __DEBUG__
                true;
#else
                false;
#endif

            GameAnalytics.Initialize();

            mIsInitialize = true;
        }

        public void SetCustomDimensions(string d1, string d2, string d3)
        {
            if (pEnabled == false)
            {
                Debug.LogWarning("GA is not enabled!");
                return;
            }

            if (mIsInitialize)
            {
                Debug.LogWarning("Already Initialized!");
                return;
            }

            if (string.IsNullOrEmpty(d1) == false)
            {
                GameAnalytics.SetCustomDimension01(d1);
            }

            if (string.IsNullOrEmpty(d2) == false)
            {
                GameAnalytics.SetCustomDimension02(d2);
            }

            if (string.IsNullOrEmpty(d3) == false)
            {
                GameAnalytics.SetCustomDimension03(d3);
            }
        }

        public void TrackLaunchEvent()
        {
            if (pEnabled == false)
            {
                Debug.LogWarning("GA is not enabled!");
                return;
            }

            if (mIsInitialize == false)
            {
                Debug.LogError("Initialize First!");
                return;
            }

            if (PlayerPrefs.GetInt("GA_LaunchEvent") == 1)
            {
                Debug.LogWarning("Already Tracked Event - LaunchEvent");
                return;
            }

            GameAnalytics.NewDesignEvent("LaunchEvent");

            PlayerPrefs.SetInt("GA_LaunchEvent", 1);
        }

        public void TrackEvent(string key)
        {
            if (pEnabled == false)
            {
                Debug.LogWarning("GA is not enabled!");
                return;
            }

            if (mIsInitialize == false)
            {
                Debug.LogError("Initialize First!");
                return;
            }

            GameAnalytics.NewDesignEvent(key);
            Debug.Log($"{key} tracked!");
        }

        public void TrackEvent(string key, float value)
        {
            if (pEnabled == false)
            {
                Debug.LogWarning("GA is not enabled!");
                return;
            }

            if (mIsInitialize == false)
            {
                Debug.LogError("Initialize First!");
                return;
            }

            GameAnalytics.NewDesignEvent(key, value);
            Debug.Log($"{key}/{value} tracked!");
        }

        public void TrackProgressEvent(string progress1, string progress2, string progress3)
        {
            if (pEnabled == false)
            {
                Debug.LogWarning("GA is not enabled!");
                return;
            }

            if (mIsInitialize == false)
            {
                Debug.LogError("Initialize First!");
                return;
            }

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, progress1, progress2,
                progress3);
            Debug.Log($"{progress1}/{progress2}/{progress3} tracked!");
        }

        public void TrackRevenueEvent(string currency, int amount, string itemType, string itemId, string cartType,
            string receipt, string signature, bool isFirstPurchase = false)
        {
            if (pEnabled == false)
            {
                Debug.LogWarning("GA is not enabled!");
                return;
            }

            if (mIsInitialize == false)
            {
                Debug.LogError("Initialize First!");
                return;
            }

            GameAnalytics.NewBusinessEvent(currency, amount, itemType, itemId, cartType);

            if (isFirstPurchase)
            {
                var eventName = $"FirstPurchased:{itemType}:{itemId}";
                GameAnalytics.NewDesignEvent(eventName, amount * 0.01f);
            }
        }

        public void TrackAdEvent(GAAdAction aAction, GAAdType aType, string aPlacementID)
        {
            if (pEnabled == false)
            {
                Debug.LogWarning("GA is not enabled!");
                return;
            }

            if (mIsInitialize == false)
            {
                Debug.LogError("Initialize First!");
                return;
            }

            GameAnalytics.NewAdEvent(aAction, aType, cAdSdkName, aPlacementID);
        }
    }
}
