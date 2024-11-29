#if UNITY_ANDROID && !UNITY_EDITOR
using GooglePlayGames;
#endif
using Scripts.Core;
using UnityEngine;
using Scripts.Game;
using Scripts.UI;
using Scripts.Utility;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.Scene
{
    [DisallowMultipleComponent]
    public class ResultScene : MonoBehaviour
    {
        public PanelResult vPanelResult;
        
        private void Awake()
        {
            GameAnalyticsManager.Instance.TrackEvent("Score:Total", InGameScoreContext.pScore);
            GameAnalyticsManager.Instance.TrackEvent("Score:SafeSuccess", InGameScoreContext.pSafeSuccessScore);
            GameAnalyticsManager.Instance.TrackEvent("Score:FlowerSuccess", InGameScoreContext.pFlowerSuccessScore);

            var safeSuccessCount = InGameScoreContext.pSafeSuccessScore / App.db.Char[(int)BaseDigi.eDigiType.Safe].score;
            var flowerSuccessCount = InGameScoreContext.pFlowerSuccessScore / App.db.Char[(int)BaseDigi.eDigiType.Flower].score;
            var totalCount = safeSuccessCount + flowerSuccessCount;

            GameAnalyticsManager.Instance.TrackEvent("Score:TotalCount", totalCount);
            GameAnalyticsManager.Instance.TrackEvent("Score:SafeSuccessCount", safeSuccessCount);
            GameAnalyticsManager.Instance.TrackEvent("Score:SafeFailCount", InGameScoreContext.pSafeFailCount);
            GameAnalyticsManager.Instance.TrackEvent("Score:FlowerSuccessCount", flowerSuccessCount);
            GameAnalyticsManager.Instance.TrackEvent("Score:FlowerFailCount", InGameScoreContext.pFlowerFailCount);
            GameAnalyticsManager.Instance.TrackEvent("Score:AngryFailCount", InGameScoreContext.pAngryFailCount);
            
#if UNITY_ANDROID && !UNITY_EDITOR
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_lucky_seven, totalCount, _OnCallbackAchievement);
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_dont_do_that, InGameScoreContext.pAngryFailCount, _OnCallbackAchievement);
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_helmet_collector, safeSuccessCount, _OnCallbackAchievement);
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_cant_pull_out, InGameScoreContext.pSafeFailCount, _OnCallbackAchievement);
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_flower_collector, InGameScoreContext.pFlowerFailCount, _OnCallbackAchievement);
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_go_back, flowerSuccessCount, _OnCallbackAchievement);
#endif
            Debug.Log($"Score: {InGameScoreContext.pScore}");
            Debug.Log($"S Suc: {InGameScoreContext.pSafeSuccessScore}");
            Debug.Log($"S Fai: {InGameScoreContext.pSafeFailCount}");
            Debug.Log($"F Suc: {InGameScoreContext.pFlowerSuccessScore}");
            Debug.Log($"F Fai: {InGameScoreContext.pFlowerFailCount}");
            Debug.Log($"Angry: {InGameScoreContext.pAngryFailCount}");
            
            // vPanelResult.UpdateGameScore(InGameScoreContext.pScore);
            StartCoroutine(vPanelResult.EndingSequence());
        }

        private void _OnCallbackAchievement(bool aIsSuccess)
        {
            Debug.Log($"Achievement Success? {aIsSuccess}");
        }
    }
}
