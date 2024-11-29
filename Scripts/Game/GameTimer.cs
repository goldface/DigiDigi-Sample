using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Scripts.Core;
using Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Scripts.Game
{
    [DisallowMultipleComponent]
    public class GameTimer : MonoBehaviour
    {
        private const int cTimerInterval = 1000;

        public InGameTimer vInGameTimer;
        
        public ObscuredFloat pElapseTime { get; private set; }

        private void Update()
        {
            if (mRun == false)
                return;

            float lDeltaTime = Time.deltaTime;

            pElapseTime += lDeltaTime; 
            mCount1Second += lDeltaTime;
            if (mCount1Second >= 1f)
            {
                mCount1Second = (1f - mCount1Second);
                mGamePlayTime -= 1;
                if (mGamePlayTime <= 10 && mIsWarningSoundPlay == false)
                {
                    mIsWarningSoundPlay = true;
                    App.Sound.PlaySingle("Sounds/S-PVP_END_2.ogg",false, 0.3f);
                }

                if (mGamePlayTime <= 10) // 10초 이하 일 때 시계 흔들림
                {
                    vInGameTimer.StartTimerShake();
                }
                else
                    vInGameTimer.StopTimerShake();
                    
                
                vInGameTimer.UpdateTimerText(mGamePlayTime);
                if (mGamePlayTime <= 0)
                {
                    StopGameTimer();
                    mGamePlayTimeFinishEvent.Invoke();
                }
            }
        }

        public void Init()
        {
            mRun = false;
            mGamePlayTime = 0;
            mGamePlayTimeFinishEvent = new UnityEvent();
            mIsWarningSoundPlay = false;
        }

        public void Destroy()
        {
            mGamePlayTimeFinishEvent = null;
            mIsWarningSoundPlay = false;
        }

        public void AddGamePlayTimeFinishEvent(UnityAction aAction)
        {
            mGamePlayTimeFinishEvent.AddListener(aAction);
        }

        public void SetGameAdjustTimeAction(System.Action<int> aAction)
        {
            mGameAdjustTimeAction = aAction;
        }

        public void SetGameTime(int aGamePlayTimeValue)
        {
            mGamePlayTime = aGamePlayTimeValue;
            vInGameTimer.UpdateTimerText(mGamePlayTime);
        }

        public void AdjustGameTime(int aTimeValue)
        {
            if (aTimeValue == 0)
            {
                return;
            }
            
            mGamePlayTime += aTimeValue;
            vInGameTimer.UpdateTimerText(mGamePlayTime);
            mGameAdjustTimeAction?.Invoke(aTimeValue);
        }

        public void StartGameTimer()
        {
            mRun = true;
        }

        public void StopGameTimer()
        {
            mRun = false;
        }

        public bool pIsStopTimer => mGamePlayTime <= 0;

        private ObscuredBool mRun;
        private ObscuredInt mGamePlayTime;
        private ObscuredFloat mCount1Second;

        private UnityEvent mGamePlayTimeFinishEvent;
        private System.Action<int> mGameAdjustTimeAction;
        
        private bool mIsWarningSoundPlay;
    }
}