using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Game
{
    public class DigiHoleManager : MonoBehaviour
    {
        private readonly Vector3[] cDigiHolePositions =
        {
            new Vector3(0, 0.5f, 0),    // 0
            new Vector3(-6, -0.8f, 0),  // 1
            new Vector3(6, -0.8f, 0),   // 2
            new Vector3(-3, -4.3f, 0),  // 3
            new Vector3(3, -4.3f, 0)    // 4
        };

        public const int cDefaultHoleOrder = 10;
        
        public void Init()
        {
            var lDigiHoleOrigin = App.Bundle.Load<GameObject>("InGameAsset", "DigiHole.prefab");
            var lDigiHolePrefab = lDigiHoleOrigin.GetComponent<DigiHole>();
            
            mDigiHoles = new DigiHole[cDigiHolePositions.Length];
            for (int iHole = 0; iHole < cDigiHolePositions.Length; ++iHole)
            {
                mDigiHoles[iHole] = Instantiate<DigiHole>(lDigiHolePrefab, cDigiHolePositions[iHole], Quaternion.identity, transform);
                mDigiHoles[iHole].pNumber = iHole; 
                mDigiHoles[iHole].SetMaskSortingLayer(iHole + cDefaultHoleOrder, iHole + cDefaultHoleOrder + 1);
            }
        }

        public Vector3 GetHolePosition(int aHoleNumber)
        {
            return cDigiHolePositions[aHoleNumber];
        }

        public DigiHole GetDigiHole(int aHoleNumber)
        {
            return mDigiHoles[aHoleNumber];
        }

        public bool CheckDigiHoleIsFull()
        {
            for (int iHole = 0; iHole < mDigiHoles.Length; ++iHole)
            {
                if (mDigiHoles[iHole].pUse == false)
                    return false;
            }

            return true;
        }

        public int GetAppearHoleCount()
        {
            int lAppearCount = 0;
            for (int iHole = 0; iHole < mDigiHoles.Length; ++iHole)
            {
                if (mDigiHoles[iHole].pUse)
                    lAppearCount++;
            }

            return lAppearCount;
        }

        public DigiHole GenerateEmptyHole()
        {
            for (;;)
            {
                int lRandDigiIndex = Random.Range(0, mDigiHoles.Length);

                if (mDigiHoles[lRandDigiIndex].pUse)
                    continue;
                    
                return mDigiHoles[lRandDigiIndex];    
            }
        }
        
        private DigiHole[] mDigiHoles;
    }
}