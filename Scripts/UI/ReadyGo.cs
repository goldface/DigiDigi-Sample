using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class ReadyGo : MonoBehaviour
    {
        public Image vImageReady;
        public Image vImageGo;

        public void StartSequence(Action aActionCompleteCallback)
        {
            Debug.Log("ReadyGo StartSequence");
            var lSequence = DOTween.Sequence()
                .AppendInterval(1f)
                .AppendCallback(() =>
                {
                    vImageReady.gameObject.SetActive(true);
                    App.Sound.PlaySingle("Sounds/ready.mp3");
                })
                .Append(vImageReady.rectTransform.DOAnchorPosY(0f, 0.5f))
                .Join(vImageReady.DOFade(1f, 0.5f))
                .AppendInterval(0.75f)
                .Append(vImageReady.DOFade(0f, 0.25f))
                .AppendCallback(() =>
                {
                    vImageGo.gameObject.SetActive(true);
                    App.Sound.PlaySingle("Sounds/go.mp3");
                })
                .Append(vImageGo.transform.DOScale(2f, 0.5f))
                .Join(vImageGo.DOFade(0f, 0.5f))
                .OnComplete(aActionCompleteCallback.Invoke)
                .SetAutoKill(true);

            lSequence.Play();
        }
    }
}