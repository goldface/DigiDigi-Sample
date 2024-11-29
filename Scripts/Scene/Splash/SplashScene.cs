using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using JJFramework.Runtime.Extension;
using Scripts.Core;
using Scripts.UI;
using Scripts.Utility;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.Scene.Splash
{
    [DisallowMultipleComponent]
    public class SplashScene : MonoBehaviour
    {
        // 기준해상도의 가로 사이즈
        // 기존 제작 480x320에서 4배 확대한 사이즈 (480*4)
        // 현재 UICanvas의 기준 해상도를 480x320의 4배로 잡고있다.
        private const float cReferenceResolutionWidth = 1920f;
        
        public Image vLogoGamePocket;
        public CanvasGroup vLogoBoysStudio;
        public RectTransform vAnimGroupTransform;

        public Popup vPopupCanvas;
        
        private void Awake()
        {
            App.Initialize();

            mScreenWidthHalfSize = Screen.width * 0.5f;
            
            Color lChangeColor = vLogoGamePocket.color.SetAlpha(0);
            vLogoGamePocket.color = lChangeColor;
            vLogoBoysStudio.alpha = 0;
            vLogoGamePocket.SetActive(false);
            vLogoBoysStudio.SetActive(false);
            
            vPopupCanvas.SetActive(false);
            
            Vector3 lAnimGroupPosition = vAnimGroupTransform.position;
            lAnimGroupPosition.Set(Screen.width + mScreenWidthHalfSize, lAnimGroupPosition.y, lAnimGroupPosition.z);;
        }

        private IEnumerator Start()
        {
            GameAnalyticsManager.Instance.TrackLaunchEvent();
            
            using (var web = UnityWebRequest.Get("https://gamepocket.team/digi_version.txt"))
            {
                yield return web.SendWebRequest();

                if (web.isHttpError
                    || false == string.IsNullOrEmpty(web.error))
                {
                    _GoToStore();
                    
                    yield break;
                }

                var rawVersion = web.downloadHandler.text;
                if (string.IsNullOrEmpty(rawVersion))
                {
                    _GoToStore();
                    
                    yield break;
                }

                var version = rawVersion.Split('\n');
                Debug.Log($"App Version: {Application.version} / Current Version: {version[0]} / Review Version: {version[1]}");
                // NOTE(JJO): Review
                if (string.IsNullOrEmpty(version[1])
                    || version[1] != Application.version)
                {
                    // NOTE(JJO): Store
                    if (string.IsNullOrEmpty(version[0])
                        || version[0] != Application.version)
                    {
                        GameAnalyticsManager.Instance.TrackEvent("GoToStore");
                        
                        _GoToStore();
                    
                        yield break;
                    }
                }
            }
            
            yield return new WaitForEndOfFrame();
            
            yield return StartCoroutine(_CoSplash());
            SceneManager.LoadScene("StoryScene");
        }

        private void _GoToStore()
        {
            vPopupCanvas.Initialize("Need to Update!", null, null, () =>
            {
                Application.OpenURL("https://play.google.com/store/apps/details?id=team.gamepocket.digi");

                Application.Quit();
            });
        }

        private IEnumerator _CoSplash()
        {
            float lDuration = 5f;
            float lDurationRatio = Screen.width / cReferenceResolutionWidth;
            
            var lSequence = DOTween.Sequence()
                .AppendInterval(1f)
                .AppendCallback(() =>
                {
                    vLogoGamePocket.SetActive(true);
                    App.Sound.PlaySingle("Logo/eff_logo_2.wav", false, 0.3f);
                })
                .Append(vLogoGamePocket.DOFade(1, 0.5f))
                .AppendInterval(1f)
                .Append(vLogoGamePocket.DOFade(0, 0.5f))
                .AppendInterval(1f)
                .AppendCallback(() => vLogoBoysStudio.SetActive(true))
                .Append(vLogoBoysStudio.DOFade(1, 0.5f))
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    vAnimGroupTransform.DOLocalMoveX(-Screen.width - mScreenWidthHalfSize, lDuration * lDurationRatio);
                })
                .AppendInterval(3f)
                .Append(vLogoBoysStudio.DOFade(0, 0.5f))
                .SetAutoKill(true);
            yield return lSequence.WaitForCompletion();
        }

        private float mScreenWidthHalfSize;
    }
}
