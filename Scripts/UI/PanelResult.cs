using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using DG.Tweening;
using JJFramework.Runtime.Extension;
#if UNITY_ANDROID && !UNITY_EDITOR
using GooglePlayGames;
#endif
using Scripts.Core;
using Scripts.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scripts.Scene.InGame;
using Scripts.Utility;
using TMPro;
using UniRx;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.UI
{
    public class PanelResult : MonoBehaviour
    {
        private const string cHighScoreKey = "HighScore";

        public Image vImageFade;
        public GameObject vResultView;
        public GameObject vClearView;
        public GameObject vImageOTL;
        public GameObject vImageO_O;
        public Text vTextCurrentScore;
        public Text vTextHighScore;
        public Button vButtonReplay;
        public Button vButtonEnd;
        public GameObject vChancePlate;
        public TextMeshProUGUI vTextChance;
        public Button vButtonAdvertise;
        public Popup vPopupCanvas;
        public TextMeshProUGUI vTextRemainTime;
        public GameObject vImageRemainTime;
        public Image vImageGameOver;
        public GameObject vScoreBoard;

        public Button vClearButtonEnd;
        public Text vClearTextCurrentScore;
        public Text vClearTextHighScore;

        private readonly IntReactiveProperty mHighScoreFromSocial = new IntReactiveProperty(-1);

        private bool mIsEndingSequence = false;

        private void Awake()
        {
            App.Initialize();

            vTextHighScore.text = "0";
            vClearTextHighScore.text = "0";
            vButtonReplay.onClick.AddListener(_OnClickButtonReplay);
            vButtonEnd.onClick.AddListener(_OnClickButtonEnd);
            vClearButtonEnd.onClick.AddListener(_OnClickButtonEnd);

            mHighScoreFromSocial.SubscribeToText(vTextHighScore);
            mHighScoreFromSocial.SubscribeToText(vClearTextHighScore);
            
            // _ResetResult();
            // EndingSequence();
        }

        private IEnumerator Start()
        {
            while (false == App.Bundle.IsLoaded)
            {
                yield return null;
            }

            App.User.Initialize();
            while (false == App.User.IsInitialized)
            {
                yield return null;
            }

            while (false == mIsEndingSequence)
            {
                yield return null;
            }
            
            vImageRemainTime.SetActive(App.User.pCoin.Value < Const.cCoinMaxValue);
            Observable.Interval(TimeSpan.FromSeconds(1.0))
                .Subscribe(d =>
                {
                    if (App.User.pCoin.Value < Const.cCoinMaxValue)
                    {
                        var time = App.User.GetRemainTime();
                        vTextRemainTime.text = $"{time.Minutes:00}:{time.Seconds:00}";
                        vImageRemainTime.SetActive(true);
                    }
                    else
                    {
                        vImageRemainTime.SetActive(false);
                    }
                })
                .AddTo(this);
 
            App.User.pCoin
                .Subscribe(d => vTextChance.text = $"{App.User.pCoin} / {Const.cCoinMaxValue}")
                .AddTo(this);

            vButtonAdvertise.OnClickAsObservable()
                .Subscribe(_ => _OnClickAdvertise())
                .AddTo(this);
            
            App.Sound.PlayBGM(InGameScoreContext.pIsCleared ? "BGM/all_clear.ogg" : "BGM/game_over.mp3", false);
        }
        
        private void _OnClickAdvertise()
        {
            if (App.User.pCoin.Value < Const.cCoinMaxValue)
            {
                App.Advertise.ShowRewardAd(_OnAdvertiseCallback);
            }
            else
            {
                vPopupCanvas.Initialize("Coin is full", null, null, () => { });
            }
        }

        private void _OnClickButtonReplay()
        {
            if (App.User.UseCoin() == false)
            {
                vPopupCanvas.Initialize("Not enough coin!\nDo you want to charge a coin after shown advertisement?", _OnClickAdvertise, null, null);
                return;
            }
            
#if UNITY_ANDROID && !UNITY_EDITOR
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_revenge, 1, aIsSuccess => Debug.Log($"Achievement - revenge / {aIsSuccess}"));
#endif
            GameAnalyticsManager.Instance.TrackEvent("GameRetry");
            
            ObscuredPrefs.SetBool("IS_CONTINUED", false);

            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    vResultView.GetComponent<Animator>().enabled = false;
                    // 자연이 연출
                    vImageOTL.SetActive(false);
                    vImageO_O.SetActive(true);
                })
                .AppendInterval(0.5f)
                .AppendCallback(_FadeOut)
                .AppendInterval(1f)
                .OnComplete(() =>
                {
                    // 다시 게임 화면으로
                    SceneManager.LoadScene("InGameScene");
                })
                .SetAutoKill(true);
        }

        private void _OnClickButtonEnd()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!InGameScoreContext.pIsCleared)
            {
                PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement_coward);
            }
#endif
            GameAnalyticsManager.Instance.TrackEvent("GameEnd:Result");
       
            var lSequence = DOTween.Sequence()
                .AppendCallback(_FadeOut)
                .AppendInterval(1f)
                .OnComplete(() =>
                {
                    SceneManager.LoadScene("MainLobbyScene");
                })
                .SetAutoKill(true);

            lSequence.Play();
        }

        public IEnumerator UpdateGameScore(int aCurrentScore)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var leaderboardID = App.pIsDebug ? GPGSIds.leaderboard_highscore_old : GPGSIds.leaderboard_highscore;
            // NOTE(JJO): GPGS Leaderboard와 연동
            Social.LoadScores(leaderboardID, aScores =>
            {
                foreach (var lScore in aScores)
                {
                    if (lScore.userID == Social.localUser.id)
                    {
                        int.TryParse(lScore.formattedValue, out var result);
                        mHighScoreFromSocial.Value = result;
                        break;
                    }
                }
            });

            // FIXME(JJO): Observable으로 이쁘게 수정 가능하신 분 구함.
            var lLocalHighScore = ObscuredPrefs.GetInt(cHighScoreKey);
            var lCount = 0;
            while (mHighScoreFromSocial.Value == -1)
            {
                if (lCount++ >= 50)
                {
                    break;
                }
                
                yield return new WaitForSeconds(0.1f);
            }
            
            Social.ReportScore(aCurrentScore, leaderboardID, aResult => Debug.Log($"Report completed - {aCurrentScore}"));
            if (lLocalHighScore < aCurrentScore)
            {
                ObscuredPrefs.SetInt(cHighScoreKey, aCurrentScore);
                lLocalHighScore = aCurrentScore;
            }

            if (mHighScoreFromSocial.Value == -1)
            {
                Debug.Log($"Load Local High Score: {lLocalHighScore}");
                mHighScoreFromSocial.Value = lLocalHighScore;
            }

            vTextCurrentScore.text = aCurrentScore.ToString();
            vClearTextCurrentScore.text = aCurrentScore.ToString();
#else
            int lHighScore = PlayerPrefs.GetInt(cHighScoreKey, 0);
            if (aCurrentScore > lHighScore)
            {
                PlayerPrefs.SetInt(cHighScoreKey, aCurrentScore); // 최고 기록 갱신
                lHighScore = aCurrentScore;
            }

            vTextCurrentScore.text = aCurrentScore.ToString();
            vTextHighScore.text = lHighScore.ToString();

            vClearTextCurrentScore.text = aCurrentScore.ToString();
            vClearTextHighScore.text = lHighScore.ToString();
            
            yield return null;
#endif
        }

        public IEnumerator EndingSequence()
        {
            _ResetResult();
            
            yield return StartCoroutine(UpdateGameScore(InGameScoreContext.pScore));
            
            var lView = InGameScoreContext.pIsCleared ? vClearView : vResultView;
            lView.SetActive(true);
            var lAnimator = lView.GetComponent<Animator>();
            lAnimator.enabled = true;
            lAnimator.Play(InGameScoreContext.pIsCleared ? "AnimClear" : "AnimResult");

            mIsEndingSequence = true;
        }

        private void _FadeOut()
        {
            // 페이드 아웃
            vImageFade.gameObject.SetActive(true);
            Color lColor = vImageFade.color;
            lColor.a = 0;
            vImageFade.color = lColor;
            vImageFade.DOFade(1, 1f);
        }
        
        private void _ResetResult()
        {
            vClearView.SetActive(false);
            vResultView.SetActive(false);
            vImageO_O.SetActive(false);
            vImageFade.gameObject.SetActive(false);
        }
        
        private void _OnAdvertiseCallback(bool aIsSuccess, AdvertiseManager.Type_Result type)
        {
            Debug.Log($"{aIsSuccess} | {type}");
            if (false == aIsSuccess)
            {
                Debug.LogError($"Something has wrong! - {type}");
                vPopupCanvas.Initialize("Failed to show advertisement!\nPerhaps it is because of the AdBlock settings.",
                    null, null, () => { });
                return;
            }

            App.User.AddAdvertiseCoin();
        }
    }
}