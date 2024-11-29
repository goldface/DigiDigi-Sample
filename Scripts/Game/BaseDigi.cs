using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Game
{
    public abstract class BaseDigi : IDigi
    {
        public enum eDigiType
        {
            Safe = 101,
            Flower = 102,
            Angry = 103,
            None = 0
        }

        public enum eDragType
        {
            Success,
            Fail,
            None
        }

        public void SetDigiMono(DigiMono aDigiMono)
        {
            mDigiMono = aDigiMono;
        }

        public abstract string UpdateAnimationName(string aAnimationName);
        public abstract void UpdateDigiHand();

        public abstract eDragType CheckCatchAction(float aDiffYRange);
        public abstract void CatchReaction(Action aAction);
        public abstract void CatchFailReaction(Action aAction);

        protected DigiMono mDigiMono;
    }
}
