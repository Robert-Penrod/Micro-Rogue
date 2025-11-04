using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperQuest.EasyPooling
{
    public class PoolingSettings : ScriptableObject
    {
        public bool ShowPoolsInHierarchy = true;
        public int DefaultPoolSize = 10;

        [Header("Thinning")]
        public float ThinningCheckRate = 5f;
        public int MaxThinCount = 5;

        public static PoolingSettings Load()
        {
            return Resources.Load<PoolingSettings>("EasyPooling/PoolingSettings");
        }

        public void Reset()
        {
            ShowPoolsInHierarchy = true;
            DefaultPoolSize = 10;
            ThinningCheckRate = 5f;
            MaxThinCount = 5;
        }
    }
}
