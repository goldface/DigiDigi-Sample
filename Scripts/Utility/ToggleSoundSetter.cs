using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using UniRx;

namespace Scripts.Utility
{
    [DisallowMultipleComponent]
    public class ToggleSoundSetter : ButtonSoundSetter
    {
        private Toggle mToggle;
        
        protected override void Initialize()
        {
            if (string.IsNullOrEmpty(vSoundName))
            {
                Debug.LogError($"This object is EMPTY Sound Name - {name}");
                GameObject.Destroy(gameObject);
                return;
            }
            
            mToggle = GetComponent<Toggle>();
            if (null == mToggle)
            {
                Debug.LogWarning($"This object is not contain Button component - {name}");
                GameObject.Destroy(gameObject);
                return;
            }

            mToggle.onValueChanged.AddListener(value => App.Sound.PlaySingle(vSoundName));
        }

        private void OnDestroy()
        {
            mToggle.onValueChanged.RemoveAllListeners();
            mToggle = null;
        }
    }
}
