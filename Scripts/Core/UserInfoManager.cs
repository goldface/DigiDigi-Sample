using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using Newtonsoft.Json;
using UniRx;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.Core
{
    public class UserInfo
    {
        [JsonProperty("isFirstLaunch")]
        public bool isFirstLaunch;
        [JsonProperty("coin")]
        public int coin;
        // NOTE(JJO): 완전 충전되는 데 필요한 시간.
        [JsonProperty("coinRefreshTime")]
        public string coinRefreshTime;
        
        public virtual string ToJson()
        {
            var result = JsonConvert.SerializeObject(this);
            return result;
        }

        public virtual UserInfo FromJson(string json)
        {
            var result = JsonConvert.DeserializeObject<UserInfo>(json);
            return result;
        }
    }
    
    public class UserInfoManager
    {
        // User Property
        private UserInfo mUserInfo;

        private DateTime mCoinChargeTicks;

        public bool IsInitialized { get; private set; }
        public bool IsPreInitialized { get; private set; }

        public ReactiveProperty<ObscuredInt> pCoin;

        public async Task PreInitialize(System.Action<bool> callback = null)
        {
            if (IsPreInitialized
                || mUserInfo != null)
            {
                return;
            }
            
            mUserInfo = await App.IO.Load<UserInfo>(nameof(UserInfo));
            IsPreInitialized = true;
            var lResult = mUserInfo != null;
            if (false == lResult)
            {
                mUserInfo = new UserInfo();
            }

            callback?.Invoke(lResult);
        }

        public async Task Initialize(bool aIsForceInit = false)
        {
            if (!aIsForceInit && IsInitialized)
            {
                return;
            }

            // NOTE(jjo): 서버로부터 받아와야 함.
            if (IsPreInitialized
                && null == mUserInfo)
            {
                mUserInfo = new UserInfo();
            }
            else
            {
                await PreInitialize();
            }

            if (!mUserInfo.isFirstLaunch)
            {
                Debug.Log("First Launch!");
                mUserInfo.coin = Const.cCoinMaxValue;
                mUserInfo.isFirstLaunch = true;
                UploadData();
            }

            long.TryParse(mUserInfo.coinRefreshTime, out var lTimeCheck);
            mCoinChargeTicks = new DateTime(lTimeCheck);
            
            pCoin = new ReactiveProperty<ObscuredInt>(mUserInfo.coin);
            pCoin
                .Where(d => mUserInfo != null)
                .Subscribe(d => mUserInfo.coin = d);
            
            // NOTE(JJO): 시간 업데이트.
            Observable.Interval(TimeSpan.FromSeconds(1.0), Scheduler.MainThreadIgnoreTimeScale)
                .Where(d => IsInitialized)
                .Subscribe(d =>
                {
                    _UpdateTimeChecker();
                })
                .AddTo(DOTween.instance);

            IsInitialized = true;
        }

        public bool UseCoin()
        {
            if (pCoin.Value > 0)
            {
                var currentTime = App.Time.GetCurrentTime();
                if (pCoin.Value == Const.cCoinMaxValue)
                {
                    mCoinChargeTicks = currentTime;
                }
                
                --pCoin.Value;

                if (mCoinChargeTicks < currentTime)
                {
                    mCoinChargeTicks = currentTime.AddSeconds(Const.cCoinChargeTime);
                }
                else
                {
                    mCoinChargeTicks = mCoinChargeTicks.AddSeconds(Const.cCoinChargeTime);
                }

                mUserInfo.coinRefreshTime = mCoinChargeTicks.Ticks.ToString();
                
                UploadData();

                return true;
            }

            Debug.LogWarning("코인이 부족하다!");
            
            return false;
        }

        public void AddAdvertiseCoin()
        {
            mCoinChargeTicks = mCoinChargeTicks.AddSeconds(-Const.cCoinChargeTime);
            mUserInfo.coinRefreshTime = mCoinChargeTicks.Ticks.ToString();
            
            ++pCoin.Value;
            if (pCoin.Value >= Const.cCoinMaxValue)
            {
                mCoinChargeTicks = App.Time.GetCurrentTime();
            }

            UploadData();
        }

        public TimeSpan GetRemainTime()
        {
            var currentTime = App.Time.GetCurrentTime();
            var time = mCoinChargeTicks - currentTime;
            while (time.TotalSeconds > Const.cCoinChargeTime)
            {
                time -= TimeSpan.FromSeconds(Const.cCoinChargeTime);
            }
            return time;
        }

        private void _UpdateTimeChecker()
        {
            // NOTE(JJO): 꽉 찬 경우는 패스
            if (pCoin.Value >= Const.cCoinMaxValue)
            {
                return;
            }

            var currentTime = App.Time.GetCurrentTime();
            // NOTE(JJO): 현재 시간이 더 많이 흘렀으면 꽉 채우고 끝냄
            if (mCoinChargeTicks <= currentTime)
            {
                pCoin.Value = Const.cCoinMaxValue;
                mCoinChargeTicks = currentTime;
                return;
            }

            // NOTE(JJO): 정의된 값만큼 지나면 코인이 충전되도록함.
            var time = mCoinChargeTicks - currentTime;
            var mod = (int)time.TotalSeconds % (int)Const.cCoinChargeTime;
            if (mod == 0)
            {
                ++pCoin.Value;
                if (pCoin.Value >= Const.cCoinMaxValue)
                {
                    mCoinChargeTicks = currentTime;
                }
            }
        }
        
        public void UploadData()
        {
            App.IO.Save<UserInfo>(nameof(UserInfo), mUserInfo);
        }

        public void ResetUserInfo()
        {
            mUserInfo = new UserInfo();
            UploadData();
        }

        public void Cleanup()
        {
            IsInitialized = false;
        }
    }
}
