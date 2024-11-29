#if UNITY_ANDROID
using System;
using System.Collections;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Scripts.Core.Auth;
using UniRx;
using UnityEngine;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.Core.Auth
{
    public class GoogleLoginModule : BaseLoginModule
    {
        public override string pUserID => PlayGamesPlatform.Instance.GetIdToken();

        public override void Initialize()
        {
            if (pIsInitialized)
            {
                Debug.LogWarning("Already Init!");
                return;
            }
            
#if UNITY_EDITOR
            pIsInitialized = true;
            pIsEditor = true;
            return;
#endif
            
            var lConfig = new PlayGamesClientConfiguration.Builder()
                .RequestServerAuthCode(false)
                .RequestIdToken()
                .EnableSavedGames()
                .Build();

            PlayGamesPlatform.InitializeInstance(lConfig);
            // FIXME(JJO): RELEASE BUILD에서는 Debug Log가 안나오도록 해야함.
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            pIsInitialized = true;
        }

        public override void Login(Action<bool, string> aCallback)
        {
            if (pIsInitialized == false)
            {
                Debug.LogError("Initialize First!");
                return;
            }
            
#if UNITY_EDITOR
            aCallback?.Invoke(true, string.Empty);
            return;
#endif
            PlayGamesPlatform.Instance.Authenticate((aIsSuccess, aErrorMessage) =>
            {
                Debug.Log($"isSuccess => {aIsSuccess} ");
                if (aIsSuccess == true)
                {
                    Debug.Log("Google Login Success!");
                }
                else
                {
                    Debug.Log("Google Login Fail" + aErrorMessage);
                }

                // 권한 거부 등등 로그인 실패
                if (string.IsNullOrEmpty(aErrorMessage) == false)
                {
                    Debug.Log($"=== Google Signin Fail === {aErrorMessage}");
                }

                Observable.Timer(TimeSpan.FromSeconds(1.0))
                    .Subscribe(x => _LoginCallback(aIsSuccess, aErrorMessage, aCallback));
            }, false);
        }

        private void _LoginCallback(bool aIsAuthenticateSuccess, string aMessage, Action<bool, string> aCallback)
        {
            Debug.Log($"isAuthenticateSuccess => {aIsAuthenticateSuccess} ");
            
            if (aIsAuthenticateSuccess)
            {
                Debug.Log($"ID: {Social.localUser.id}");
                Debug.Log($"Token: {((PlayGamesLocalUser)Social.localUser).GetIdToken()}");
                
                //var id = Social.localUser.id;
                var lToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();

                // NOTE(JJO): 토큰이 없다면 정상 종료한게 아니다. 저장해놨던 토큰을 보내자. 만료되면 _idToken이 다시 온다.
                if (string.IsNullOrEmpty(lToken))
                {
                    lToken = PlayerPrefs.GetString($"{nameof(GoogleLoginModule)}_TOKEN");
                    Debug.Log($"saved Google token : {lToken} ");
                }
                else
                {
                    PlayerPrefs.SetString($"{nameof(GoogleLoginModule)}_TOKEN", lToken);
                    PlayerPrefs.Save();
                }
            }
            
            aCallback?.Invoke(aIsAuthenticateSuccess, aMessage);
        }

        public override void Logout(Action<bool, string> aCallback)
        {
            if (pIsInitialized == false)
            {
                aCallback?.Invoke(false, "Init First");
                return;
            }
            
#if UNITY_EDITOR
            aCallback?.Invoke(true, string.Empty);
            return;
#endif
            
            if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
            {
                aCallback?.Invoke(false, "Login First");
                return;
            }
            
            PlayGamesPlatform.Instance.SignOut();
            
            aCallback?.Invoke(true, string.Empty);
        }
    }
}
#endif
