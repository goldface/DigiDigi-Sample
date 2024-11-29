using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using JJFramework.Runtime.Extension;
using UniRx.Toolkit;
using UnityEngine;

namespace Scripts.Game
{
    public class CountPool : ObjectPool<CountObject>
    {
        private GameObject mPrefab;
        private RectTransform mParent;

        public CountPool(GameObject aPrefab, RectTransform aParent)
        {
            mPrefab = aPrefab;
            mParent = aParent;
        }

        protected override void OnBeforeRent(CountObject instance)
        {
            instance.transform.localScale = Vector3.one;
            instance.SetActive(true);
        }

        protected override CountObject CreateInstance()
        {
            var lObj = GameObject.Instantiate(mPrefab, mParent, true);
            lObj.transform.localScale = Vector3.one;

            return lObj.GetComponent<CountObject>();
        }
    }
}
