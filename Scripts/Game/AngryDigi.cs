using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Core;
using UnityEngine;

namespace Scripts.Game
{
    public class AngryDigi : BaseDigi
    {
        public override string UpdateAnimationName(string aAnimationName)
        {
            return $"AnimAngryDigi{aAnimationName}";
        }

        public override void UpdateDigiHand()
        {
            mDigiMono.vAnimator.Play("DigiHandAngry", mDigiMono.pHandLayer);
        }
        
        public override eDragType CheckCatchAction(float aDiffYRange)
        {
            return eDragType.Fail;
        }

        public override void CatchReaction(Action aAction)
        {
            aAction.Invoke();
        }

        public override void CatchFailReaction(Action aAction)
        {
            App.Sound.PlaySingle("Sounds/angry_touch.mp3");

            var lSequence = DOTween.Sequence()
                .AppendInterval(4f)
                .Append(mDigiMono.vDigiBodyTransform.DOMoveY(-DigiMono.cDigiHeight, 0.5f).SetRelative())
                .OnComplete(aAction.Invoke)
                .SetAutoKill(true);
            lSequence.Play();
        }
    }
}