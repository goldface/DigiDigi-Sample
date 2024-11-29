using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JJFramework.Runtime.Extension;
using UniRx;
// using Debug = GameRuntime.Util.Debug;

namespace Scripts.UI
{
    [DisallowMultipleComponent]
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private Button vButtonClose;
        [SerializeField] private Button vButtonPrev;
        [SerializeField] private Button vButtonNext;
        [SerializeField] private GameObject ImageSafeDigi;
        [SerializeField] private GameObject ImageFlowerDigi;
        [SerializeField] private GameObject ImageAngryDigi;
        [SerializeField] private Canvas vCanvas;
        
        private int mCurrentPage;
        private System.Action mCloseCallback;
 
        private void Start()
        {
            mCurrentPage = 0;
            SetTutorial();
            
            DOTween.Sequence()
                .Append(vButtonPrev.transform.DOScale(1.2f, 0.25f))
                .Append(vButtonPrev.transform.DOScale(1f, 0.25f))
                .SetLoops(-1)
                .SetAutoKill(true);
            DOTween.Sequence()
                .Append(vButtonNext.transform.DOScale(1.2f, 0.25f))
                .Append(vButtonNext.transform.DOScale(1f, 0.25f))
                .SetLoops(-1)
                .SetAutoKill(true);
            vButtonPrev.OnClickAsObservable()
                .Where(d => mCurrentPage > 0)
                .Subscribe(_ =>
                {
                    --mCurrentPage;
                    SetTutorial();
                })
                .AddTo(this);
            vButtonNext.OnClickAsObservable()
                .Where(d => mCurrentPage < 2)
                .Subscribe(_ =>
                {
                    ++mCurrentPage;
                    SetTutorial();
                })
                .AddTo(this);
            vButtonClose.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    mCloseCallback?.Invoke();
                    vCanvas.enabled = false;
                    // gameObject.SetActive(false);
                })
                .AddTo(this);
            
            vButtonPrev.SetActive(false);
            vButtonNext.SetActive(true);
            // gameObject.SetActive(false);
        }

        public void OpenTutorial(System.Action callback = null)
        {
            mCloseCallback = callback;
            vCanvas.enabled = true;
        }

        private void SetTutorial()
        {
            vButtonPrev.SetActive(mCurrentPage > 0);
            vButtonNext.SetActive(mCurrentPage < 2);
            
            ImageSafeDigi.SetActive(mCurrentPage == 0);
            ImageFlowerDigi.SetActive(mCurrentPage == 1);
            ImageAngryDigi.SetActive(mCurrentPage == 2);
        }
    }
}
