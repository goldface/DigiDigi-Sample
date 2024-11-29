using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Scripts.Game
{
    public class DigiHole : MonoBehaviour
    {
        public SpriteMask vSpriteMask;
        
        public ObscuredBool pUse { get; set; }
        public ObscuredInt pNumber { get; set; }
        
        private void Awake()
        {
            pUse = false;
        }
        
        public void SetMaskSortingLayer(int aBackSortingOrder, int aFrontSortingOrder)
        {
            vSpriteMask.isCustomRangeActive = true;
            vSpriteMask.frontSortingLayerID = SortingLayer.NameToID("Default");
            vSpriteMask.frontSortingOrder = aFrontSortingOrder;
            vSpriteMask.backSortingLayerID = SortingLayer.NameToID("Default");
            vSpriteMask.backSortingOrder = aBackSortingOrder;
        }
    }
}