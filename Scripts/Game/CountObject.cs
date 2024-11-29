using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Game
{
    public class CountObject : MonoBehaviour
    {
        private CanvasGroup mCanvasGroup;
        private RectTransform mRectTransform;
        
        private TextMeshProUGUI mText;

        private void Awake()
        {
            mRectTransform = transform as RectTransform;
            mCanvasGroup = GetComponent<CanvasGroup>();
            mText = GetComponent<TextMeshProUGUI>();
        }

        public void Initialize(int aValue, System.Action<CountObject> aReturnAction)
        {
            mText.color = aValue > 0 ? Color.green : Color.red;
            mText.text = $"{(aValue > 0 ? "+" : string.Empty)}{aValue}";

            mRectTransform.anchoredPosition = Vector2.zero;
            mCanvasGroup.alpha = 0f;
            
            DOTween.Sequence()
                .Append(mCanvasGroup.DOFade(1f, 0.25f))
                .AppendInterval(0.25f)
                .Append(mRectTransform.DOAnchorPosY(50f, 0.25f).SetRelative(true))
                .Join(mCanvasGroup.DOFade(0f, 0.25f))
                .OnComplete(() => aReturnAction.Invoke(this))
                .SetAutoKill(true);
        }
    }
}
