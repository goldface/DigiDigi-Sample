using System;
using DG.Tweening;
using Scripts.Core;
using UnityEngine;

namespace Scripts.Game
{
    public class SafeDigi : BaseDigi, IDigi
    {
        private int mLaughSoundIndex;
        
        public override string UpdateAnimationName(string aAnimationName)
        {
            return $"AnimSafeDigi{aAnimationName}";
        }

        public override void UpdateDigiHand()
        {
            mDigiMono.vAnimator.Play("DigiHandSafe", mDigiMono.pHandLayer);
        }

        public override eDragType CheckCatchAction(float aDiffYRange)
        {
            if (aDiffYRange > 40f)
            {
                return eDragType.Success;
            }
            else if (aDiffYRange < -40f)
            {
                return eDragType.Fail;
            }

            return eDragType.None;
        }

        public override void CatchReaction(Action aAction)
        {
            DOTween.Kill(mDigiMono.GetTweenId(), true); // Tween도중에 다른 Tween이 동작하는 것에 대한 방지
            
            App.Sound.PlaySingle("Sounds/safe_up.wav");
            
            Debug.Log("SafeDigi CatchReaction");
            mDigiMono.UpdateAnimation("Fly");
            DigiFactory.CreateFlyingDigi(mDigiMono.vDigiBodyTransform.position);
            mDigiMono.SetVisible(false);
            aAction.Invoke();
        }

        public override void CatchFailReaction(Action aAction)
        {
            DOTween.Kill(mDigiMono.GetTweenId(), true); // Tween도중에 다른 Tween이 동작하는 것에 대한 방지
            
            App.Sound.PlaySingle("Sounds/safe_fail.wav");
            
            Transform lDigiTransform = mDigiMono.vDigiBodyTransform;
            
            var lSequence = DOTween.Sequence()
                .Append(lDigiTransform.DOMoveY(-(DigiMono.cDigiHeight * 0.5f), 0.5f).SetRelative())
                .AppendInterval(1f)
                .AppendCallback(() =>
                {
                    mLaughSoundIndex = App.Sound.PlaySingle("Sounds/laugh.mp3");
                    mDigiMono.UpdateAnimation("Smile");
                    lDigiTransform.DOMoveY((DigiMono.cDigiHeight * 0.5f), 0.5f).SetRelative();
                })
                .AppendInterval(1f)
                .Append(lDigiTransform.DOMoveY(-DigiMono.cDigiHeight, 0.5f).SetRelative())
                .OnComplete(() =>
                {
                    App.Sound.StopEffect(mLaughSoundIndex);
                    //mDigiMono.Reset();
                    aAction.Invoke();
                })
                .SetAutoKill(true);

            lSequence.Play();
        }
    }
}