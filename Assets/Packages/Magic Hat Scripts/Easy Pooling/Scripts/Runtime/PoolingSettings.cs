using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManaSprite.EasyPooling
{
    public class PoolingSettings : ScriptableObject
    {
        public bool ShowPoolsInHierarchy = true;
        public int DefaultPoolSize = 10;

        [Header("Thinning")]
        [Tooltip("How often in seconds pools will delete objects to reach their defeault size if they have grown larger.")]
        public float ThinningCheckPeriod = 10f;
        [Tooltip("The max number of objects a pool can delete durring a single thinning cycle.")]
        public int MaxThinCount = 5;

        public static PoolingSettings Load()
        {
            return Resources.Load<PoolingSettings>("EasyPooling/PoolingSettings");
        }

        public void Reset()
        {
            ShowPoolsInHierarchy = true;
            DefaultPoolSize = 10;
            ThinningCheckPeriod = 10f;
            MaxThinCount = 5;
        }
    }
}
