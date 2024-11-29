using System;
using DG.Tweening;
using GameAnalyticsSDK;
using JJFramework.Runtime.Extension;
using Scripts.Utility;
using UniRx;
using UnityEngine.Advertisements;
#if UNITY_ANDROID && !UNITY_EDITOR
using GooglePlayGames;
#endif

namespace Scripts.Core
{
    public class AdvertiseManager : IUnityAdsListener
    {
        private bool _isInitialized => Advertisement.isInitialized;
        private bool _isTestMode;

        private IDisposable _timeChecker;
        private const int _timeOutSeconds = 30;
        private bool _isShowingAd;

        private bool _isSuccess;

        private const string _placementID = "rewardedVideo";

        private const string _gameID
#if UNITY_ANDROID
            = "3875723";
#elif UNITY_IOS
            = "3875722";
#else
            = "";
#endif
        
        private System.Action<bool, Type_Result> _resultAction;

        public enum Type_Result
        {
            NOT_INITIALIZED,
            
            SUCCESS_SHOW_AD,
            SUCCESS_LOAD_AD,
            
            SKIPPED,
            FAILED_TO_LOAD,
            FAILED_TO_SHOW,
            NOT_LOADED,
            
            FAILED_UNKNOWN,
            
            COUNT
        }

        private Type_Result _result = Type_Result.NOT_INITIALIZED;

        public bool IsReady
        {
            get
            {
                var result = _isInitialized
                             && Advertisement.IsReady(_placementID);

                return result;
            }
        }
        
        public void Initialize(bool isTestMode = false, bool isPreload = false)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[AdsManager::Initialize] Already Initialized!");
                
                return;
            }
            
            _isTestMode = isTestMode;
            
            Advertisement.AddListener(this);
            Advertisement.Initialize(_gameID, _isTestMode, true);

            if (isPreload)
            {
                RequestLoadRewardAd();
            }
        }

        public void Cleanup()
        {
            Advertisement.RemoveListener(this);
        }

        public async void RequestLoadRewardAd(System.Action<bool, Type_Result> resultAction = null)
        {
            if (!_isInitialized)
            {
                Debug.LogError($"Initialize First!");
                while (!_isInitialized)
                {
                    await Observable.NextFrame();
                }
                // resultAction?.Invoke(false, Type_Result.NOT_INITIALIZED);
                // return false;
            }

            if (Advertisement.IsReady(_placementID))
            {
                Debug.LogWarning("[AdsManager::RequestLoadRewardAd] Already Loaded");
                // resultAction?.Invoke(true, Type_Result.SUCCESS_LOAD_AD);
                // return true;
            }
            
            _resultAction = resultAction;

            GameAnalyticsManager.Instance.TrackAdEvent(GAAdAction.Request, GAAdType.RewardedVideo, _placementID);
            Advertisement.Load(_placementID);
            
            Debug.Log("Request Completed");
        }

        public bool ShowRewardAd(System.Action<bool, Type_Result> resultAction = null)
        {
            if (!IsReady)
            {
                Debug.Log("[AdsManager::ShowRewardAd] Is NOT ready!");
                GameAnalyticsManager.Instance.TrackAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, _placementID);
                resultAction?.Invoke(false, Type_Result.NOT_LOADED);
                
                return false;
            }
            
            _resultAction = resultAction;
            Advertisement.Show(_placementID);
            
            _isShowingAd = true;
            _isSuccess = false;

            _SetTimeOut();

            return true;
        }

        [System.Diagnostics.Conditional("ADMOB_TEST")]
        private void _SetTimeOut()
        {
            if (null != _resultAction)
            {
                _timeChecker = Observable.Timer(TimeSpan.FromSeconds(_timeOutSeconds))
                    .Subscribe(_ =>
                    {
                        // NOTE(JJO): 타임아웃! 오류 프로세스 진행.
                        Debug.LogError("[AdsManager::SetTimeOut] Timeout!");
                        
                        _resultAction?.Invoke(false, Type_Result.FAILED_TO_SHOW);
                        _resultAction = null;

                        _timeChecker = null;
                    });
            }
        }
        
#region Unity Ads Callback
        private void _ExecuteAdClosed()
        {
            _resultAction?.Invoke(_isSuccess, _result);
            _resultAction = null;
                    
            // NOTE(JJO): 광고가 닫히면 즉시 다음 광고를 준비하도록 함.
            RequestLoadRewardAd();
        }

        public void OnUnityAdsReady(string placementId)
        {
            Debug.Log($"{placementId} is ready!");
            GameAnalyticsManager.Instance.TrackAdEvent(GAAdAction.Loaded, GAAdType.RewardedVideo, _placementID);
            _resultAction?.Invoke(false, Type_Result.SUCCESS_LOAD_AD);
            _resultAction = null;
        }

        public void OnUnityAdsDidError(string message)
        {
            Debug.LogError($"{message}");
            GameAnalyticsManager.Instance.TrackAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, _placementID);
            _resultAction?.Invoke(false, Type_Result.FAILED_TO_SHOW);
            _resultAction = null;
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            Debug.Log($"{placementId} did start!");
            GameAnalyticsManager.Instance.TrackAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, _placementID);
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            if (showResult == ShowResult.Failed)
            {
                Debug.LogError($"Failed to show ({placementId}) - {showResult}");
                GameAnalyticsManager.Instance.TrackAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, _placementID);
                _result = Type_Result.FAILED_TO_SHOW;
            }
            else if (showResult == ShowResult.Skipped)
            {
                Debug.Log($"Skipped ads ({placementId})");
                GameAnalyticsManager.Instance.TrackAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, _placementID);
                _result = Type_Result.SKIPPED;
            }
            else
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_thank_you_for_watching, 1, aIsSuccess => Debug.Log($"Achievement - watching / {aIsSuccess}"));
#endif
                GameAnalyticsManager.Instance.TrackAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, _placementID);
                _result = Type_Result.SUCCESS_SHOW_AD;
                _isSuccess = true;
            }
            
            _ExecuteAdClosed();

        }
#endregion
    }
}

