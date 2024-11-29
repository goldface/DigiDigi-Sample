using System;

namespace Scripts.Game
{
    public interface IDigi
    {
        void SetDigiMono(DigiMono aDigiMono);
        string UpdateAnimationName(string aAnimationName);
        void UpdateDigiHand();
        BaseDigi.eDragType CheckCatchAction(float aDiffYRange);
        void CatchReaction(Action aAction);
        void CatchFailReaction(Action aAction);
    }
}