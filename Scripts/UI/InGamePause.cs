using System;
using Scripts.Core;
using Scripts.Scene.InGame;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class InGamePause : MonoBehaviour
    {
        public Button vButtonPause;
        public Button vButtonResume;
        public Button vButtonEnd;
        public Toggle vToggleScound;
        public GameObject vPausePanel;
        
        private void Awake()
        {
            vPausePanel.SetActive(false);
            vButtonPause.onClick.AddListener(_OnClickButtonPause);
            vButtonResume.onClick.AddListener(_OnClickButtonResume);
            vButtonEnd.onClick.AddListener(_OnClickButtonEnd);
            vToggleScound.onValueChanged.AddListener(_OnValueChangedToggleSound);
        }

        private void _OnClickButtonPause()
        {
            GameAnalyticsManager.Instance.TrackEvent("GamePause");
            InGameScene.sMain.PauseGame();
            vPausePanel.SetActive(true);
            Time.timeScale = 0;
        }

        private void _OnClickButtonResume()
        {
            GameAnalyticsManager.Instance.TrackEvent("GameResume");
            Time.timeScale = 1;
            InGameScene.sMain.ResumeGame();
            vPausePanel.SetActive(false);
        }

        private void _OnClickButtonEnd()
        {
            GameAnalyticsManager.Instance.TrackEvent("GameEnd:Pause");
            Time.timeScale = 1;
            SceneManager.LoadScene("MainLobbyScene");
        }
        
        private void _OnValueChangedToggleSound(bool aToggle)
        {
            // aToggle = true = Sound Off
            Debug.Log(aToggle);
            PlayerPrefs.SetInt("SOUND_OFF", aToggle ? 1 : 0);
            App.Sound.SetBGMVolume(aToggle ? 0f : 1f);
            App.Sound.SetEffectVolume(aToggle ? 0f : 1f);
        }
    }
}