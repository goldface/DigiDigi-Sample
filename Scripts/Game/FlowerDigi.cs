using System;
using DG.Tweening;
using Scripts.Core;
using UnityEngine;

namespace Scripts.Game
{
    public class FlowerDigi : BaseDigi, IDigi
    {
        public override string UpdateAnimationName(string aAnimationName)
        {
            return $"AnimFlowerDigi{aAnimationName}";
        }

        public override void UpdateDigiHand()
        {
            mDigiMono.vAnimator.Play("DigiHandFlower", mDigiMono.pHandLayer);
        }

        public override eDragType CheckCatchAction(float aDiffYRange)
        {
            if (aDiffYRange < -40f)
            {
                return eDragType.Success;
            }
            else if (aDiffYRange > 40f)
            {
                return eDragType.Fail;
            }

            return eDragType.None;
        }

        public override void CatchReaction(Action aAction)
        {
            DOTween.Kill(mDigiMono.GetTweenId(), true); // Tween도중에 다른 Tween이 동작하는 것에 대한 방지
            
            App.Sound.PlaySingle("Sounds/flower_down.wav");
            
            mDigiMono.UpdateAnimation("Down");
            Transform lDigiTransform = mDigiMono.vDigiBodyTransform;
            lDigiTransform.DOShakePosition(0.5f, cShakeStrength)
                .SetLoops(-1);
            lDigiTransform.DOMoveY(lDigiTransform.position.y - DigiMono.cDigiHeight, 0.5f)
                .OnComplete(() =>
                {
                    //mDigiMono.Reset();
                    lDigiTransform.DOKill();
                    aAction.Invoke();
                })
                .SetAutoKill(true);
        }

        public override void CatchFailReaction(Action aAction)
        {
            DOTween.Kill(mDigiMono.GetTweenId(), true); // Tween도중에 다른 Tween이 동작하는 것에 대한 방지
            Debug.Log("FlowerDigi CatchFailReaction");

            App.Sound.PlaySingle("Sounds/flower_fail.wav");

            mDigiMono.UpdateAnimation("End");
            Vector3 lDigiPosition = mDigiMono.vDigiBodyTransform.position;
            lDigiPosition.x -= 0.55f;
            lDigiPosition.y += 2.1f;
            DigiFactory.CreateFlower(lDigiPosition);
            Transform lDigiTransform = mDigiMono.vDigiBodyTransform;
            var lSequence = DOTween.Sequence()
                .AppendInterval(2f)
                .Append(lDigiTransform.DOMoveY(-DigiMono.cDigiHeight, 0.5f).SetRelative()
                    .OnComplete(() =>
                    {
                        //mDigiMono.Reset();
                        lDigiTransform.DOKill();
                        aAction.Invoke();
                    }))
                .SetAutoKill(true);

            lSequence.Play();
        }

        private readonly Vector2 cShakeStrength = new Vector2(0.3f, 0.1f);
    }
}