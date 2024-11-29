using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Scripts.Core
{
    public class TimeManager
    {
        public DateTime GetCurrentTime(bool isKoreanTime = true)
        {
            var result = DateTime.UtcNow;

            if (isKoreanTime)
            {
                // NOTE(JJO): GMT+9
                result = result.AddHours(9.0);
            }

            return result;
        }
    }
}