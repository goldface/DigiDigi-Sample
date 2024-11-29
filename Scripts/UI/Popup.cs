using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JJFramework.Runtime.Attribute;
using JJFramework.Runtime.Extension;
using TMPro;
using UniRx;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.UI
{
    [DisallowMultipleComponent]
    public class Popup : MonoBehaviour
    {
        [ComponentPath(true), SerializeField] private TextMeshProUGUI vTextMessage;
        [ComponentPath(true), SerializeField] private Button vButtonYes;
        [ComponentPath(true), SerializeField] private Button vButtonNo;
        [ComponentPath(true), SerializeField] private Button vButtonOK;

        private IDisposable mYesAction;
        private IDisposable mNoAction;
        private IDisposable mOKAction;

        public void Initialize(string aMessage, Action aYesAction, Action aNoAction, Action aOKAction)
        {
            vTextMessage.text = aMessage;
            
            vButtonYes.SetActive(aYesAction != null);
            mYesAction = vButtonYes.OnClickAsObservable()
                .Where(d => aYesAction != null)
                .Subscribe(_ =>
                {
                    aYesAction.Invoke();
                    _OnClose();
                })
                .AddTo(this);
            
            vButtonNo.SetActive(aNoAction != null);
            mNoAction = vButtonNo.OnClickAsObservable()
                .Where(d => aNoAction != null)
                .Subscribe(_ =>
                {
                    aNoAction.Invoke();
                    _OnClose();
                })
                .AddTo(this);

            vButtonOK.SetActive(aOKAction != null);
            mOKAction = vButtonOK.OnClickAsObservable()
                .Where(d => aOKAction != null)
                .Subscribe(_ =>
                {
                    aOKAction.Invoke();
                    _OnClose();
                })
                .AddTo(this);
            
            gameObject.SetActive(true);
        }

        private void _OnClose()
        {
            mYesAction.Dispose();
            mNoAction.Dispose();
            mOKAction.Dispose();
            
            gameObject.SetActive(false);
        }
    }
}
