using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class InGameScore : MonoBehaviour
    {
        // public Text vScoreText;
        public TextMeshProUGUI vScoreText;

        public void UpdateScore(int aScore)
        {
            vScoreText.text = aScore.ToString();
        }
    }
}