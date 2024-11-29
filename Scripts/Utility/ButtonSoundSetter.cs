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
    public class ButtonSoundSetter : MonoBehaviour
    {
        [SerializeField] protected string vSoundName;
        private Button mButton;

        protected virtual void Initialize()
        {
            if (string.IsNullOrEmpty(vSoundName))
            {
                Debug.LogError($"This object is EMPTY Sound Name - {name}");
                GameObject.Destroy(gameObject);
                return;
            }
            
            mButton = GetComponent<Button>();
            if (null == mButton)
            {
                Debug.LogWarning($"This object is not contain Button component - {name}");
                GameObject.Destroy(gameObject);
                return;
            }
            
            mButton.OnClickAsObservable()
                .Subscribe(_ => App.Sound.PlaySingle(vSoundName))
                .AddTo(this);
        }
        
        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            mButton = null;
        }
    }
}
