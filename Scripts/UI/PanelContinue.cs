using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using DG.Tweening;
using JJFramework.Runtime.Extension;
using Scripts.Core;
using Scripts.Scene.InGame;
using Scripts.Utility;
using TMPro;
using UniRx;
using UnityEngine.SceneManagement;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.UI
{
    [DisallowMultipleComponent]
    public class PanelContinue : MonoBehaviour
    {
        private const string cHighScoreKey = "HighScore";

        public Image vImageFade;
        public GameObject vResultView;
        public GameObject vImageOTL;
        public GameObject vImageO_O;
        public Button vButtonYes;
        public Button vButtonNo;
        public TextMeshProUGUI vTextContinue;
        public Image vImageAdBlock;

        public InGameScene vInGameScene;

        private void Awake()
        {
            App.Initialize();

            vButtonYes.onClick.AddListener(_OnClickButtonYes);
            vButtonNo.onClick.AddListener(_OnClickButtonNo);
        }

        private IEnumerator Start()
        {
            while (false == App.Bundle.IsLoaded)
            {
                yield return null;
            }

            yield return App.User.Initialize();
        }

        private void _OnClickButtonYes()
        {
            vImageAdBlock.SetActive(true);
            
            App.Advertise.ShowRewardAd((isSuccess, result) =>
            {
                if (false == isSuccess)
                {
                    return;
                }
                
                ObscuredPrefs.SetBool("IS_CONTINUED", true);

                DOTween.Sequence()
                    .AppendCallback(() =>
                    {
                        // 자연이 연출
                        vImageOTL.SetActive(false);
                        vImageO_O.SetActive(true);
                    })
                    .AppendInterval(0.5f)
                    .AppendCallback(_FadeOut)
                    .AppendInterval(1f)
                    .OnComplete(vInGameScene.ContinueGame)
                    .SetAutoKill(true);
            });
        }

        private void _OnClickButtonNo()
        {
            SceneManager.LoadScene("ResultScene");
        }

        public void EndingSequence()
        {
            _ResetResult();
            
            vImageAdBlock.SetActive(false);
            
            vResultView.SetActive(true);
            var animator = vResultView.GetComponent<Animator>();
            animator.enabled = true;
            
            var isContinued = ObscuredPrefs.GetBool("IS_CONTINUED");
            animator.Play(isContinued ? "AnimResult" : "AnimContinue");
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
            vResultView.SetActive(false);
            vImageO_O.SetActive(false);
            vImageFade.gameObject.SetActive(false);
        }
    }
}
