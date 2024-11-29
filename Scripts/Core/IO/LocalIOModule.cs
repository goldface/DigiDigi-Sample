using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UniRx;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.Core.IO
{
    public class LocalIOModule : BaseIOModule
    {
        public override void Save<T>(string aKey, T aValue)
        {
            var lJson = JsonConvert.SerializeObject(aValue);
            Debug.Log($"json data size: {lJson.Length}");
            Debug.Log(lJson);
            PlayerPrefs.SetString(aKey, lJson);
        }

        public override async Task<T> Load<T>(string aKey)
        {
            // NOTE(JJO): Warning 제거를 위한 추가 삽입.
            await Observable.NextFrame();
            
            var lValue = PlayerPrefs.GetString(aKey, "{}");
            var lResult = JsonConvert.DeserializeObject<T>(lValue);
            Debug.Log($"json data size: {lValue.Length}");
            Debug.Log(lValue);
            return lResult;
        }
    }
}