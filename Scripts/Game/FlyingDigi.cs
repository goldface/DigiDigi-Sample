using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Core;
using UnityEngine;
using Debug = JJFramework.Runtime.Extension.Debug;
using Random = UnityEngine.Random;

namespace Scripts.Game
{
    public class FlyingDigi : MonoBehaviour
    {
        public SpriteRenderer vSpriteRenderer;

        private void Awake()
        {
            Vector3 lStartPosition = transform.position;
            float lRandPosX = Random.Range(lStartPosition.x + 5f, lStartPosition.x - 5f);
            Vector3 lRandomMove = new Vector3(lRandPosX, 10f, 0);

            transform.DOLocalRotate(cRotateVector, 0.25f)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear)
                .SetRelative();
            transform.DOMove(lRandomMove, 1f).OnComplete(OnFlyingComplete);
        }

        private void OnFlyingComplete()
        {
            App.Sound.PlaySingle($"Sounds/skyline.mp3", false,0.01f);
            
            float lRandPosX = Random.Range(5f, -5f);
            Vector3 lRandomMove = new Vector3(lRandPosX, -5f, 0);

            var lSequence = DOTween.Sequence()
                .Append(transform.DOScale(cFalldownScale, 0f))
                .Join(transform.DOMove(lRandomMove, 2f).SetRelative())
                .Join(vSpriteRenderer.DOFade(0f, 3f))
                .SetAutoKill(true)
                .OnComplete(_OnCompleteCallback);
            lSequence.Play();
        }

        private void _OnCompleteCallback()
        {
            GameObject.Destroy(gameObject);
        }

        private readonly Vector3 cRotateVector = new Vector3(0, 0, 360);
        private readonly Vector3 cFalldownScale = new Vector3(0.1f, 0.1f, 1);
    }
}