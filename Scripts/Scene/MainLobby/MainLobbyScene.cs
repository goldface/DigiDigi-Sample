using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using DG.Tweening;
using JJFramework.Runtime.Attribute;
using JJFramework.Runtime.Extension;
using Scripts.Core;
using Scripts.UI;
using Scripts.Utility;
using TMPro;
using UniRx;
using UnityEngine.SceneManagement;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.Scene.MainLobby
{
    [DisallowMultipleComponent]
    public class MainLobbyScene : MonoBehaviour
    {
        public GameObject vButtonGroup;
        public Button vButtonStart;
        public Button vButtonCredit;
        public Toggle vToggleScound;
        public Button vButtonTutorial;
        public Button vButtonAchievement;
        public Button vButtonLeaderBoard;
        public TextMeshProUGUI vTextChance;
        public Button vButtonAdvertise;
        public Tutorial vTutorialCanvas;
        public Popup vPopupCanvas;
        public CanvasGroup vFade;
        public TextMeshProUGUI vTextRemainTime;
        public GameObject vImageRemainTime;

        private bool mIsStart;

        private void Awake()
        {
            App.Initialize();

            vFade.alpha = 1f;
            vFade.DOFade(0f, 1f);
            
            vPopupCanvas.SetActive(false);
            vButtonGroup.SetActive(false);
            
            var lIsSoundOff = PlayerPrefs.GetInt("SOUND_OFF") == 1;
            vToggleScound.isOn = lIsSoundOff;
            vToggleScound.onValueChanged.AddListener(_OnValueChangedToggleSound);
            vButtonStart.onClick.AddListener(_OnClickButtonStart);
            vButtonTutorial.OnClickAsObservable()
                .Subscribe(_ => vTutorialCanvas.OpenTutorial())
                .AddTo(this);

            vButtonAchievement.OnClickAsObservable()
                .Where(d => false == string.IsNullOrEmpty(App.Auth.pUserID))
                .Subscribe(_ =>
                {
                    Social.ShowAchievementsUI();
                })
                .AddTo(this);
            
            vButtonLeaderBoard.OnClickAsObservable()
                .Where(d => false == string.IsNullOrEmpty(App.Auth.pUserID))
                .Subscribe(_ =>
                {
                    Social.ShowLeaderboardUI();
                })
                .AddTo(this);
            
            vImageRemainTime.transform.parent.SetActive(false);
        }

        private IEnumerator Start()
        {
            while (false == App.Bundle.IsLoaded)
            {
                yield return null;
            }
            
            App.Sound.PlayBGM("BGM/main_bgm.mp3");
         
#if UNITY_EDITOR
            vButtonGroup.SetActive(true);
#else
            if (string.IsNullOrEmpty(App.Auth.pUserID))
            {
                App.Auth.Login((aIsSuccess, aMessage) =>
                {
                    if (false == aIsSuccess)
                    {
                        Debug.LogError($"Something has wrong! - {aMessage}");
                        vPopupCanvas.Initialize($"Something has wrong!\nMessage: {aMessage}", null, null, Application.Quit);
                        return;
                    }
                });

                while (string.IsNullOrEmpty(App.Auth.pUserID))
                {
                    yield return null;
                }

                yield return new WaitForSeconds(2f);
            }
#endif

            App.User.Initialize();
            while (false == App.User.IsInitialized)
            {
                yield return null;
            }
            
            vButtonGroup.SetActive(true);
            
            vImageRemainTime.transform.parent.SetActive(true);
            vImageRemainTime.SetActive(App.User.pCoin.Value < Const.cCoinMaxValue);
            
            Debug.Log($"UserInfo Init? {App.User.IsInitialized} / Coin? {App.User.pCoin != null}");
            Observable.Interval(TimeSpan.FromSeconds(1.0))
                .Where(d => App.User.IsInitialized && App.User.pCoin != null)
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

        private void _OnClickButtonStart()
        {
            if (mIsStart)
            {
                return;
            }

            if (App.User.UseCoin() == false)
            {
                vPopupCanvas.Initialize("Not enough coin!\nDo you want to charge a coin after shown advertisement?", _OnClickAdvertise, null, null);
                return;
            }
            
            ObscuredPrefs.SetBool("IS_CONTINUED", false);
            
            DOTween.Sequence()
                .Append(vFade.DOFade(1f, 1f))
                .AppendCallback(() => SceneManager.LoadScene("InGameScene"))
                .SetAutoKill(true);
            
            GameAnalyticsManager.Instance.TrackEvent("GameStart");

            mIsStart = true;
        }

        private void _OnValueChangedToggleSound(bool aToggle)
        {
            // aToggle = true = Sound Off
            Debug.Log(aToggle);
            PlayerPrefs.SetInt("SOUND_OFF", aToggle ? 1 : 0);
            App.Sound.SetBGMVolume(aToggle ? 0f : 1f);
            App.Sound.SetEffectVolume(aToggle ? 0f : 1f);
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