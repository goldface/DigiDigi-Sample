using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Scripts.Core;
using UniRx;
using Random = UnityEngine.Random;

namespace Scripts.Game
{
    [DisallowMultipleComponent]
    public class Flower : MonoBehaviour
    {
        public SpriteRenderer vSpriteRenderer;

        void Awake()
        {
            Vector3 lStartPosition = transform.position;
            float lRandPosX = Random.Range(lStartPosition.x + 2f, lStartPosition.x - 2f);
            Vector3 lRandomMove = new Vector3(lRandPosX, 10f, 0);

            transform.DOLocalRotate(cRotateVector, 0.25f)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear)
                .SetRelative();
            transform.DOMove(lRandomMove, 1f).OnComplete(OnFlyingComplete);
        }

        private void OnFlyingComplete()
        {
            App.Sound.PlaySingle($"Sounds/skyline2.mp3", false, 0.01f);

            float lRandPosX = Random.Range(2f, -2f);
            Vector3 lRandomMove = new Vector3(lRandPosX, -5f, 0);

            var lSequence = DOTween.Sequence()
                //.Append(transform.DOScale(cFalldownScale, 0f))
                .Append(transform.DOMove(lRandomMove, 2f).SetRelative())
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
    }
}