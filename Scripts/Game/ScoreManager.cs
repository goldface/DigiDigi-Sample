using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using Scripts.UI;

namespace Scripts.Game
{
    [DisallowMultipleComponent]
    public class ScoreManager : MonoBehaviour
    {
        public InGameScore vInGameScore;
        public bool pIsDecreaseScore { get; private set; }
        
        private void Awake()
        {
            mScore = 0;
            pIsDecreaseScore = true;
            _UpdateScore();
        }

        public void IncreaseSafeScore(int aScore)
        {
            mSafeSuccessScore += aScore;
            _IncreaseScore(aScore);
        }

        public void IncreaseFlowerScore(int aScore)
        {
            mFlowerSuccessScore += aScore;
            _IncreaseScore(aScore);
        }

        private void _IncreaseScore(int aScore)
        {
            if (aScore == 0)
            {
                return;
            }
            
            pIsDecreaseScore = false;
            mScore += aScore;
            _UpdateScore();
            mScoreAction?.Invoke(aScore);
        }

        public void FailedSafeDigi(int aScore)
        {
            mSafeFailScore += aScore;
        }

        public void FailedFlowerDigi(int aScore)
        {
            mFlowerFailScore += aScore;
        }

        public void FailedAngryDigi(int aScore)
        {
            mAngryFailScore += aScore;
        }

        private void _DecreaseScore(int aScore)
        {
            if (aScore == 0)
            {
                return;
            }
            
            pIsDecreaseScore = true;
            // NOTE(JJO): 현재까지 줄어든 스코어 누계
            mMinusScore += aScore;
            
            mScore -= aScore;
            
            if (mScore < 0)
                mScore = 0;
            
            _UpdateScore();
            mScoreAction?.Invoke(aScore);
        }

        /// <summary>
        /// 화면에 표시되는 최종 스코어
        /// </summary>
        /// <returns>모든것이 합산된 최종 스코어</returns>
        public int GetScore()
        {
            return mScore;
        }

        public int GetSafeSuccess()
        {
            return mSafeSuccessScore;
        }

        public int GetSafeFail()
        {
            return mSafeFailScore;
        }

        public int GetFlowerSuccess()
        {
            return mFlowerSuccessScore;
        }

        public int GetFlowerFail()
        {
            return mFlowerFailScore;
        }

        public int GetAngryFail()
        {
            return mAngryFailScore;
        }

        public int GetMinusScore()
        {
            return mMinusScore;
        }

        private void _UpdateScore()
        {
            vInGameScore.UpdateScore(mScore);
        }

        public void SetScoreAction(System.Action<int> aAction)
        {
            mScoreAction = aAction;
        }

        private ObscuredInt mScore;
        private ObscuredInt mSafeSuccessScore;
        private ObscuredInt mSafeFailScore;
        private ObscuredInt mFlowerSuccessScore;
        private ObscuredInt mFlowerFailScore;
        private ObscuredInt mAngryFailScore;
        private ObscuredInt mMinusScore;

        private System.Action<int> mScoreAction;
    }
}