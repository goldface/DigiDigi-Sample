using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Newtonsoft.Json;
using UniRx;
using Debug = JJFramework.Runtime.Extension.Debug;

namespace Scripts.Core.IO
{
    public class GoogleIOModule : BaseIOModule
    {
        private readonly Dictionary<string, object> mCachedDataDic = new Dictionary<string, object>(0);
        
        private bool mIsInitialized => App.Auth.pIsInitialized;

        public override void Save<T>(string aKey, T aValue)
        {
            if (mIsInitialized == false)
            {
                Debug.LogError("Initialize First!");
                return;
            }

            if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
            {
                Debug.LogError("Login First!");
                return;
            }

            var lSavedClient = PlayGamesPlatform.Instance.SavedGame;
            lSavedClient.OpenWithAutomaticConflictResolution(
                aKey,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                (aStatus, aData) =>
                {
                    if (aStatus == SavedGameRequestStatus.Success)
                    {
                        var lJson = JsonConvert.SerializeObject(aValue);
                        var lSaveData = Encoding.UTF8.GetBytes(lJson);
                        var lTime = App.Time.GetCurrentTime();
                        var lBuilder = new SavedGameMetadataUpdate.Builder();
                        lBuilder = lBuilder.WithUpdatedPlayedTime(lTime.TimeOfDay)
                            .WithUpdatedDescription($"Saved at {lTime}");

                        var lUpdatedData = lBuilder.Build();
                        lSavedClient.CommitUpdate(aData, lUpdatedData, lSaveData, _OnCommitUpdate);
                    }
                    else
                    {
                        Debug.LogError($"Failed to save - {aKey}");
                    }
                });
        }

        private void _OnCommitUpdate(SavedGameRequestStatus aStatus, ISavedGameMetadata aData)
        {
            Debug.Log($"Result: {aStatus}");
            Debug.Log($"Commit Info\nFile Name: {aData.Filename}\nDescription: {aData.Description}\nIsOpen: {aData.IsOpen}\nLast Modified: {aData.LastModifiedTimestamp}\nTotal Time Played: {aData.TotalTimePlayed}");
        }

        public override async Task<T> Load<T>(string aKey)
        {
            if (mIsInitialized == false)
            {
                Debug.LogError("Initialize First!");
                return default;
            }

            if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
            {
                Debug.LogError("Login First!");
                return default;
            }

            if (mCachedDictionary.ContainsKey(aKey))
            {
                Debug.LogWarning($"Already Loaded: {aKey}");
                return default;
            }

            if (mCachedDataDic.ContainsKey(aKey))
            {
                Debug.LogError($"Already request loading! - {aKey}");
                return default;
            }
            
            mCachedDataDic.Add(aKey, null);
            
            var lSavedClient = PlayGamesPlatform.Instance.SavedGame;
            lSavedClient.OpenWithAutomaticConflictResolution(
                aKey,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                (aStatus, aData) =>
                {
                    Debug.Log($"status: {aStatus}");
                    Debug.Log($"Data Name: {aData.Filename}");
                    if (aStatus == SavedGameRequestStatus.Success)
                    {
                        lSavedClient.ReadBinaryData(
                            aData,
                            (aStat, aDat) =>
                            {
                                Debug.Log($"status: {aStat} / data: {aDat != null}");
                                if (aStat == SavedGameRequestStatus.Success)
                                {
                                    Debug.Log($"binary data size: {aDat.Length}");
                                    var result = Encoding.Default.GetString(aDat);
                                    Debug.Log($"json data size: {result.Length}");
                                    Debug.Log(result);
                                    var lObj = JsonConvert.DeserializeObject<T>(result);
                                    Debug.Log($"obj: {lObj != null}");
                                    if (lObj == null)
                                    {
                                        mCachedDataDic.Remove(aKey);
                                    }
                                    else
                                    {
                                        mCachedDataDic[aKey] = lObj;
                                    }
                                }
                                else
                                {
                                    Debug.LogError($"Failed to read: {aStat}");
                                    mCachedDataDic.Remove(aKey);
                                }
                            });
                    }
                    else
                    {
                        Debug.LogError($"Failed to load: {aStatus}");
                        mCachedDataDic.Remove(aKey);
                    }
                });

            while (mCachedDataDic.ContainsKey(aKey) &&
                   mCachedDataDic[aKey] == null)
            {
                await Observable.NextFrame();
            }

            if (mCachedDataDic.ContainsKey(aKey))
            {
                var result = (T)mCachedDataDic[aKey];
                mCachedDataDic.Remove(aKey);
                return result;
            }

            return default;
        }
    }
}
