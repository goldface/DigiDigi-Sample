using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JJFramework.Runtime.Attribute;
using TMPro;
using UniRx;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.UI
{
    [DisallowMultipleComponent]
    public class InGamePhase : MonoBehaviour
    {
        private static readonly Color cDeactive = new Color(0.2f, 0.2f, 0.2f);
        
        [SerializeField, ComponentPath("imgPhase", true)]
        protected List<Image> phaseList;

        public void SetPhase(int aValue)
        {
            var count = phaseList.Count;
            for (int i = 0; i < count; ++i)
            {
                if (i < aValue)
                {
                    if (phaseList[i].color.r < 1f)
                    {
                        DOTween.Sequence()
                            .Append(phaseList[i].rectTransform.DOScale(1.5f, 0.25f))
                            .Join(phaseList[i].rectTransform.DOLocalRotate(new Vector3(0f, 0f, 360f), 0.25f).SetRelative(true))
                            .Append(phaseList[i].rectTransform.DOScale(1f, 0.25f))
                            .Join(phaseList[i].rectTransform.DOLocalRotate(new Vector3(0f, 0f, 360f), 0.25f).SetRelative(true))
                            .SetAutoKill(true);
                        
                        phaseList[i].DOColor(Color.white, 0.5f).SetAutoKill(true);
                    }
                }
                else
                {
                    phaseList[i].color = cDeactive;
                }
            }
        }
    }
}
