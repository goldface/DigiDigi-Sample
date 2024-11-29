using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace Scripts.Scene.InGame
{
    [DisallowMultipleComponent]
    public class BGAnimationController : MonoBehaviour
    {
        // Sheep Animation
        public Animator[] vSheepAnimators;
        public SpriteRenderer[] vSheepSpriteRenderers;
        
        public int vSheepAnimationRandomProbability = 5000;
        public int vSheepFlipRandomProbability = 4000;
        
        void Start()
        {
            mCoRandomSheepAnimation = StartCoroutine(_CoRandomSheepAnimation());
        }

        private void OnDisable()
        {
            StopCoroutine(mCoRandomSheepAnimation);
        }

        private IEnumerator _CoRandomSheepAnimation()
        {
            for (;;)
            {
                yield return mWaitFor1Seconds;

                for (int iSheep = 0; iSheep < vSheepAnimators.Length; ++iSheep)
                {
                    if (Random.Range(0, 10000) < vSheepAnimationRandomProbability)
                        vSheepAnimators[iSheep].Play("AnimSheep");

                    if (Random.Range(0, 10000) < vSheepFlipRandomProbability)
                        vSheepSpriteRenderers[iSheep].flipX = !vSheepSpriteRenderers[iSheep].flipX;
                }
            }
        }
        
        private WaitForSeconds mWaitFor1Seconds = new WaitForSeconds(1);
        private Coroutine mCoRandomSheepAnimation;
    }
}
