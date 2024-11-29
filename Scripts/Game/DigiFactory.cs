using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using UniRx;
using UnityEditor;
using Random = UnityEngine.Random;

namespace Scripts.Game
{
    public class DigiFactory
    {
        public static IDigi CreateRandomDigi(int aPhaseLevel)
        {
            BaseDigi.eDigiType lDigiType = _GenerateDigiType(aPhaseLevel);
            IDigi lDigi =  _CreateDigi(lDigiType);
            return lDigi;
        }

        private static BaseDigi.eDigiType _GenerateDigiType(int aPhaseLevel)
        {
            if (mDigiProbability == null)
            {
                mDigiProbability = new DigiProbability();
                mDigiProbability.Initialize();
            }

            return mDigiProbability.GenerateRandomDigi(App.db.Stage[aPhaseLevel].apper_prob);
            // Array lValuesArray = Enum.GetValues(typeof(BaseDigi.eDigiType));
            // return (BaseDigi.eDigiType) lValuesArray.GetValue(Random.Range(0, lValuesArray.Length));
        }

        private static IDigi _CreateDigi(BaseDigi.eDigiType aDigiType)
        {
            IDigi lDigi = null;
            switch (aDigiType)
            {
                case BaseDigi.eDigiType.Flower:
                    lDigi = new FlowerDigi();
                    break;
                case BaseDigi.eDigiType.Angry:
                    lDigi = new AngryDigi();
                    break;
                case BaseDigi.eDigiType.Safe:
                default:
                    lDigi = new SafeDigi();
                    break;
            }
            return lDigi;
        }
        
        public static void CreateFlyingDigi(Vector3 aCreatePosition)
        {
            if (mFlyingDigiPrefab == null)
            {
                var lFlyingDigiOrigin = App.Bundle.Load<GameObject>("InGameAsset", "FlyingDigi.prefab");
                mFlyingDigiPrefab = lFlyingDigiOrigin.GetComponent<FlyingDigi>();
            }

            GameObject.Instantiate(mFlyingDigiPrefab, aCreatePosition, Quaternion.identity);
        }

        public static void CreateFlower(Vector3 aCreatePosition)
        {
            if (mFlowerPrefab == null)
            {
                var lFlowerOrigin = App.Bundle.Load<GameObject>("InGameAsset", "Flower.prefab");
                mFlowerPrefab = lFlowerOrigin.GetComponent<Flower>();
            }

            GameObject.Instantiate(mFlowerPrefab, aCreatePosition, Quaternion.identity);
        }

        private static FlyingDigi mFlyingDigiPrefab;
        private static Flower mFlowerPrefab;
        private static DigiProbability mDigiProbability;
    }
}
