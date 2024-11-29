using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class InGameTimer : MonoBehaviour
    {
        private const string cTimerShake = "TimerShake";
        
        public Image vImgTimer;
        public Slider vTimerSlider;
        public Text vTimerText;

        public void UpdateTimerText(int aTimerValue)
        {
            vTimerText.text = aTimerValue.ToString();
        }

        public void StartTimerShake()
        {
            if (!DOTween.IsTweening(cTimerShake))
            {
                vImgTimer.rectTransform
                    .DOShakeScale(0.5f, 0.5f)
                    .SetLoops(-1)
                    .SetAutoKill(true)
                    .SetId(cTimerShake);
            }
        }

        public void StopTimerShake()
        {
            if (DOTween.IsTweening(cTimerShake))
            {
                DOTween.Kill(cTimerShake, true);
            }
        }
    }
}