using System.Collections.Generic;
using Scripts.Core;
using UniRx;
using UnityEngine;

namespace Scripts.Game
{
    public class DigiProbability
    {
        public class DigiProb
        {
            public int Group;
            public int CharIdx;
            public int Prob;
        }

        private const int cREFERENCE_PROBABILITY = 10000;
        private Dictionary<int, List<DigiProb>> mDigiProbDic = new Dictionary<int, List<DigiProb>>(128);
        
        public void Initialize()
        {
            _GenerateProbTable();
        }

        public BaseDigi.eDigiType GenerateRandomDigi(int aGroup)
        {
            BaseDigi.eDigiType lDigiType = BaseDigi.eDigiType.Angry;
            if (mDigiProbDic.ContainsKey(aGroup) == false)
            {
                // 그룹이 맞지 않으면 화난 두더지를 생성 예외적인 상황으로 취급함
                return lDigiType; // BaseDigi.eDigiType.Angry; 
            }

            int lDigiProb = 0;
            foreach (var lDigi in mDigiProbDic[aGroup])
            {
                lDigiProb += lDigi.Prob;
                int lRandProb = Random.Range(0, cREFERENCE_PROBABILITY);
                if (lDigiProb >= lRandProb)
                {
                    lDigiType = (BaseDigi.eDigiType)lDigi.CharIdx;
                    break;
                }
            }

            return lDigiType;
        }

        private void _GenerateProbTable()
        {
            foreach (var data in App.db.Prob.Values)
            {
                DigiProb lDigiProb = new DigiProb();
                lDigiProb.Group = data.group;
                lDigiProb.CharIdx = data.char_idx;
                lDigiProb.Prob = data.prob;
                if(mDigiProbDic.ContainsKey(data.group) == false)
                    mDigiProbDic.Add(data.group, new List<DigiProb>());
                
                mDigiProbDic[data.group].Add(lDigiProb);
            }
        }
    }
}
