using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Game
{
    public static class InGameScoreContext
    {
        public static bool pIsCleared { get; set; }
        public static int pScore { get; set; }
        public static int pSafeSuccessScore { get; set; }
        public static int pSafeFailCount { get; set; }
        public static int pFlowerSuccessScore { get; set; }
        public static int pFlowerFailCount { get; set; }
        public static int pAngryFailCount { get; set; }
        public static int pMinusScore { get; set; }
    }
}