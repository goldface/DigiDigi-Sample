using System;
using System.Collections;
using DG.Tweening;
using Scripts.Core;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.Scene
{
    [DisallowMultipleComponent]
    public class StoryScene : MonoBehaviour
    {
        public Animator[] vAnimStoryLayerArray;
        public CanvasGroup vFade;

        private void Awake()
        {
            DOTween.Init();
            App.Initialize();
         
            var lLength = vAnimStoryLayerArray.Length;
            for (int iAnim = 0; iAnim < lLength; ++iAnim)
                vAnimStoryLayerArray[iAnim].gameObject.SetActive(false);
        }

        private IEnumerator Start()
        {
            while (false == App.Bundle.IsLoaded)
            {
                yield return null;
            }
            
            App.Sound.PlayBGM("BGM/story_bgm.mp3");
            
            DOTween.Sequence()
                .AppendCallback(() => vAnimStoryLayerArray[0].gameObject.SetActive(true)) // Story1
                .AppendInterval(3f)
                .AppendCallback(() =>
                {
                    vAnimStoryLayerArray[0].gameObject.SetActive(false);
                    vAnimStoryLayerArray[1].gameObject.SetActive(true); // Story2
                })
                .AppendInterval(3f)
                .AppendCallback(() =>
                {
                    vAnimStoryLayerArray[1].gameObject.SetActive(false);
                    vAnimStoryLayerArray[2].gameObject.SetActive(true); // Story3
                })
                .AppendInterval(3f)
                .AppendCallback(() =>
                {
                    vAnimStoryLayerArray[2].gameObject.SetActive(false);
                    vAnimStoryLayerArray[3].gameObject.SetActive(true); // Story4
                })
                .AppendInterval(2f)
                .OnComplete(_LoadScene)
                .SetId(nameof(StoryScene))
                .SetAutoKill(true);

            Observable.EveryUpdate()
                .Where(d => Input.GetMouseButtonUp(0) && DOTween.IsTweening(nameof(StoryScene)))
                .Subscribe(_ => DOTween.Kill(nameof(StoryScene), true))
                .AddTo(this);
        }

        private void _LoadScene()
        {
            DOTween.Sequence()
                .Append(vFade.DOFade(1f, 1f))
                .OnComplete(() => SceneManager.LoadScene("MainLobbyScene"))
                .SetAutoKill(true);
        }
    }
}
