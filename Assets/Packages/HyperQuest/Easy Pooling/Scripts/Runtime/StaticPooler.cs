using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManaSprite.EasyPooling
{
    public static class StaticPooler
    {
        public static Dictionary<GameObject, PoolingSystem> Pools = new Dictionary<GameObject, PoolingSystem>();
        public static Transform PoolHolder;
        static PoolingSettings _poolingSettings;

        static StaticPooler()
        {
            _poolingSettings = PoolingSettings.Load();

            SceneManager.sceneUnloaded += (Scene scene) =>
            {
                if (scene != SceneManager.GetActiveScene()) return;
                //ResetData();
            };

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += (PlayModeStateChange state) =>
            {
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    ResetData();
                }
            };
#endif
        }

        static void ResetData()
        {
            Debug.Log("Resetting Pool Data");
            Pools.Clear();
            PoolHolder = null;
        }

        public static PoolingSystem CreatePool(this GameObject sourceObject, int? initCount = null, bool isInstant = false)
        {
            if(sourceObject == null)
            {
                Debug.Log("Tried to create pool for null source object");
                return null;
            }

            //Debug.Log($"Creating Pool for {sourceObject.name}");
            if (initCount == null) initCount = _poolingSettings.DefaultPoolSize;

            if (!HasPool(sourceObject))
            {
                PoolingSystem newPoolingSystem = InstantiateNewPoolingSystem(sourceObject.name, sourceObject, initCount.Value, isInstant);
                if (Pools.ContainsKey(sourceObject)) Pools[sourceObject] = newPoolingSystem;
                else Pools.Add(sourceObject, newPoolingSystem);
                return newPoolingSystem;
            }
            else
            {
                if (initCount != null)
                {
                    Pools[sourceObject].SetTargetPoolSize(initCount.Value);
                }
                return Pools[sourceObject];
            }
        }

        public static GameObject PooledInstantiate(this GameObject sourceObject, Transform parent = null, Vector3? pos = null, int? targetCount = null)
        {
            if(sourceObject == null)
            {
                //Debug.Log("Tried to pooled instantiate null object...");
                return null;
            }

            bool hasPool = HasPool(sourceObject);

            GameObject go = hasPool ? Pools[sourceObject].GetGameObject() : CreatePool(sourceObject).GetGameObject();

            if(go == null)
            {
                //Debug.LogError("Instantiated null object?");
                //Debug.Log(sourceObject);
                return null;
            }

            if(targetCount != null && hasPool)
            {
                Pools[sourceObject].SetTargetPoolSize(targetCount.Value);
            }
            
            if(parent != null)
            {
                go.transform.SetParent(parent);
            }

            if(pos != null)
            {
                go.transform.position = pos.Value;
            }

            //go.SetActive(true);

            return go;
        }

        static bool HasPool(GameObject gameObject)
        {
            if (gameObject == null) return false;
            return Pools.ContainsKey(gameObject) && Pools[gameObject] != null;
        }

        static PoolingSystem InstantiateNewPoolingSystem(string name, GameObject source, int? initCount = null, bool isInstant = false)
        {
            if (initCount == null) initCount = _poolingSettings.DefaultPoolSize;

            if (PoolHolder == null) PoolHolder = new GameObject("[Pool Holder]").transform;
            //Object.DontDestroyOnLoad(PoolHolder);
            SetPoolsShown(_poolingSettings.ShowPoolsInHierarchy);

            GameObject newPoolingObject = new GameObject("Pool - " + name);
            newPoolingObject.transform.SetParent(PoolHolder.transform);
            PoolingSystem poolingSystem = newPoolingObject.AddComponent<PoolingSystem>();
            poolingSystem.InitializePool(source, initCount, isInstant);
            return poolingSystem;
        }

        public static void SetPoolsShown(bool showPoolsInHierarchy)
        {
            PoolHolder.hideFlags = showPoolsInHierarchy ? HideFlags.None : HideFlags.HideInHierarchy;
        }

        public static void DestroyOrRecycle(this GameObject o)
        {
            PoolObject poolObject = o.GetComponent<PoolObject>();
            if (poolObject != null)
            {
                poolObject.SendMessage("BeforeDestroy", SendMessageOptions.DontRequireReceiver);
                poolObject.Recycle();
            }
            else
            {
                if (Application.isPlaying)
                    Object.Destroy(o);
                else
                    Object.DestroyImmediate(o);
            }
        }

        public static void DestroyOrRecycleChildren(this GameObject o)
        {
            List<Transform> childTransforms = new List<Transform>(o.GetComponentsInChildren<Transform>(true));
            childTransforms.RemoveAt(0);
            childTransforms.ForEach(transform => transform.gameObject.DestroyOrRecycle());
        }

        public static void SetTargetPoolSize(this GameObject o, int targetPoolSize)
        {
            o.CreatePool(targetPoolSize);
        }
    }
}