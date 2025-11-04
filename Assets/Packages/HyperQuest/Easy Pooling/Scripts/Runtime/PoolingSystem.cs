using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HyperQuest.EasyPooling
{
    /// <summary>
    /// Manages a pool of reusable game objects, optimizing performance by reusing objects instead of instantiating new ones.
    /// </summary>
    public class PoolingSystem : MonoBehaviour
    {
        [SerializeField] private GameObject _template;
        [SerializeField] private List<PoolObject> _poolList = new List<PoolObject>();
        [SerializeField] private int _poolSize;
        [SerializeField] private int _averageInUseCount;

        private int _targetPoolSize;

        PoolingSettings _poolingSettings;

        private void Awake()
        {
            _poolingSettings = PoolingSettings.Load();
        }

        /// <summary>
        /// Initializes the pool and starts the thinning checks based on the predefined rate.
        /// </summary>
        private void Start()
        {
            InvokeRepeating("ThinningCheck", Random.Range(0f, 1f), _poolingSettings.ThinningCheckRate);
        }

        /// <summary>
        /// Creates a new Pooling System for a specific prefab.
        /// </summary>
        /// <param name="prefab">The prefab to pool.</param>
        /// <param name="name">Optional name for the new pool game object. If empty, prefab's name is used.</param>
        /// <returns>A new instance of a pooling system.</returns>
        public static PoolingSystem CreateNewPoolingSystem(GameObject prefab, string name = "")
        {
            var newName = string.IsNullOrEmpty(name) ? prefab.name : name;
            var newPoolingSystem = new GameObject(newName).AddComponent<PoolingSystem>();
            newPoolingSystem.InitializePool(prefab);
            return newPoolingSystem;
        }

        /// <summary>
        /// Periodically checks and thins the pool based on usage to optimize memory usage.
        /// </summary>
        void ThinningCheck()
        {
            int inUseCount = _poolSize - _poolList.Count;
            if (inUseCount < 0) inUseCount = 0;
            _averageInUseCount = (_averageInUseCount + inUseCount) / 2;

            int numToThin = (_poolSize - _averageInUseCount) / 2;
            numToThin = Mathf.Clamp(numToThin, 0, _poolingSettings.MaxThinCount);

            if (_poolList.Count > _targetPoolSize && _poolList.Count > 2f * _averageInUseCount)
            {
                ThinPool(numToThin);
            }
        }

        /// <summary>
        /// Retrieves a list of all game objects currently in the pool.
        /// </summary>
        /// <returns>A list of active game objects in the pool.</returns>
        public List<GameObject> GetPoolGameObjectList()
        {
            return _poolList.ConvertAll(poolObject => poolObject.gameObject);
        }

        /// <summary>
        /// Retrieves an object from the pool, or creates a new one if none are available.
        /// </summary>
        /// <returns>A game object ready for use.</returns>
        public GameObject GetGameObject()
        {
            if (_poolList.Count == 0)
            {
                CreateNewPoolObject();
            }

            var poolObject = GetPoolObjectFromPool();
            //poolObject.SetActive(true);
            return poolObject.gameObject;
        }

        public void SetTargetPoolSize(int targetSize)
        {
            _targetPoolSize = targetSize;
        }

        /// <summary>
        /// Initializes the pool with a specified number of objects based on the given template.
        /// </summary>
        /// <param name="template">The prefab template to use for creating pool objects.</param>
        /// <param name="initCount">Optional initial size of the pool. Uses a default value if not specified.</param>
        public void InitializePool(GameObject template, int? initCount = null, bool isInstant = false)
        {
            _targetPoolSize = initCount ?? _poolingSettings.DefaultPoolSize;

            _template = Instantiate(template, transform);
            _template.SetActive(false);

            ClearPool();
            CreateNewPoolObject();
            StartCoroutine(FillPool_Co(!isInstant));
        }

        IEnumerator FillPool_Co(bool doYield = true)
        {
            if(doYield) yield return null;
            for(int i = 0; _poolSize < _targetPoolSize; i++)
            {
                CreateNewPoolObject();
                if (doYield)
                {
                    yield return null;
                    yield return null;
                    yield return null;
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Clears the pool, destroying all objects.
        /// </summary>
        void ClearPool()
        {
            _poolList.ForEach(poolObject => Destroy(poolObject.gameObject));
            _poolList.Clear();
        }

        /// <summary>
        /// Creates and adds a new pool object based on the template.
        /// </summary>
        void CreateNewPoolObject()
        {
            PoolObject newPoolObject = Instantiate(_template, _template.transform.position, Quaternion.identity, this.transform).GetOrAddComponent<PoolObject>();
            //newPoolObject.gameObject.SetActive(true);
            newPoolObject.SetActive(false);
            newPoolObject.OnRecycle += (PoolObject poolObject) =>
            {
                if(!_poolList.Contains(poolObject)) _poolList.Add(poolObject);
                poolObject.transform.SetParent(this.transform);
            };
            newPoolObject.Recycle();
            _poolSize++;
        }

        void ThinPool(int count = 1)
        {
            for (int i = 0; i < count && _poolList.Count > 0; i++)
            {
                var poolObject = _poolList[0];
                _poolList.RemoveAt(0);
                Destroy(poolObject.gameObject);
                _poolSize--;
            }
        }

        PoolObject GetPoolObjectFromPool()
        {
            var poolObject = _poolList[0];
            _poolList.RemoveAt(0);
            poolObject.transform.SetParent(null);
            return poolObject;
        }
    }
}