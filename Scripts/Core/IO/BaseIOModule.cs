using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;

namespace Scripts.Core.IO
{
    public abstract class BaseIOModule
    {
        public abstract void Save<T>(string aKey, T aValue);

        public abstract Task<T> Load<T>(string aKey);

        protected Dictionary<string, string> mCachedDictionary = new Dictionary<string, string>();

        public bool IsLoaded(string aKey)
        {
            return mCachedDictionary.ContainsKey(aKey);
        }

        public void PrintAllData()
        {
            foreach (var lItem in mCachedDictionary)
            {
                Debug.Log($"Key: {lItem.Key} / Value: {lItem.Value}");
            }
        }

        public int GetInt(string aKey)
        {
            if (mCachedDictionary.ContainsKey(aKey) == false)
            {
                return 0;
            }

            var lValue = mCachedDictionary[aKey];
            var lResult = int.Parse(lValue);
            return lResult;
        }

        public bool GetBool(string aKey)
        {
            if (mCachedDictionary.ContainsKey(aKey) == false)
            {
                return false;
            }

            var lValue = mCachedDictionary[aKey];
            var lResult = bool.Parse(lValue);
            return lResult;
        }
        
        public string GetString(string aKey)
        {
            if (mCachedDictionary.ContainsKey(aKey) == false)
            {
                return null;
            }

            var lValue = mCachedDictionary[aKey];
            return lValue;
        }
    }
}
