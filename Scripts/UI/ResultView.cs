using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using UniRx;

namespace Scripts.UI
{
    [DisallowMultipleComponent]
    public class ResultView : MonoBehaviour
    {
        public void AnimationEvent(string value)
        {
            App.Sound.PlaySingle(value);
        }
    }
}