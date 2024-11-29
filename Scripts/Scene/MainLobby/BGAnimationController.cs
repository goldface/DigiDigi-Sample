using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace Scripts.Scene.MainLobby
{
    [DisallowMultipleComponent]
    public class BGAnimationController : MonoBehaviour
    {
        // Eye Animation
        public Animator vJayeonAnimator;
        public Animator vDigiAnimator;
        public int vEyeAnimationRandomProbability = 5000;
        
        // Logo Animation
        public Image[] vDigiLogo;
        public int vLogoAnimationRandomProbability = 5000;

        void Start()
        {
            mCoRandomEyeAnimation = StartCoroutine(_CoRandomEyeAnimation());
            mCoRandomLogoAnimation = StartCoroutine(_CoRandomLogoAnimation());
        }

        private void OnDisable()
        {
            StopCoroutine(mCoRandomEyeAnimation);
            StopCoroutine(mCoRandomLogoAnimation);
        }

        private IEnumerator _CoRandomEyeAnimation()
        {
            for (;;)
            {
                yield return mWaitFor1Seconds;

                if (Random.Range(0, 10000) < vEyeAnimationRandomProbability)
                    vJayeonAnimator.Play("AnimJayeonEye");

                if (Random.Range(0, 10000) < vEyeAnimationRandomProbability)
                    vDigiAnimator.Play("AnimDigiEye");    
            }
        }

        private IEnumerator _CoRandomLogoAnimation()
        {
            for (;;)
            {
                yield return mWaitFor1Seconds;

                for (int iLogo = 0; iLogo < vDigiLogo.Length; ++iLogo)
                {
                    if (Random.Range(0, 10000) < vLogoAnimationRandomProbability)
                    {
                        vDigiLogo[iLogo].rectTransform.DOShakeRotation(0.25f, 30f, 10, 0);
                    }    
                }
                
                if (Random.Range(0, 10000) < vLogoAnimationRandomProbability)
                    vDigiAnimator.Play("AnimDigiEye"); 
            }
        }

        private WaitForSeconds mWaitFor1Seconds = new WaitForSeconds(1);
        private Coroutine mCoRandomEyeAnimation;
        private Coroutine mCoRandomLogoAnimation;
    }
}
