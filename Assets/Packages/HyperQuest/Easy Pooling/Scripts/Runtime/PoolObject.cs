using System;
using System.Collections.Generic;
using UnityEngine;

namespace HyperQuest.EasyPooling
{
    /// <summary>
    /// Manages the lifecycle of a pooled object, handling its initialization and recycling.
    /// </summary>
    public class PoolObject : MonoBehaviour
    {
        /// <summary>
        /// Event triggered when the object is initialized.
        /// </summary>
        public Action<PoolObject> OnInitialize;

        /// <summary>
        /// Event triggered when the object is recycled.
        /// </summary>
        public Action<PoolObject> OnRecycle;

        /// <summary>
        /// Indicates whether this object has been initialized since it was last recycled.
        /// </summary>
        private bool _hasBeenInitialized = false;

        /// <summary>
        /// A list of components implementing the IPoolable interface found on this object and its children.
        /// </summary>
        private List<IPoolable> _poolables = new List<IPoolable>();

        /// <summary>
        /// On Awake, retrieves all IPoolable components in this GameObject and its children.
        /// </summary>
        private void Awake()
        {
            _poolables = new List<IPoolable>(GetComponentsInChildren<IPoolable>(true));
        }

        /// <summary>
        /// On Enable, initializes the object if it has not been initialized.
        /// </summary>
        private void OnEnable()
        {
            if (!_hasBeenInitialized)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Initializes the pool object and all its IPoolable components. This method is idempotent and will not
        /// re-initialize an object if it's already been done since the last recycle.
        /// </summary>
        public void Initialize()
        {
            if (_hasBeenInitialized) return;

            _poolables.ForEach(x => x.Initialize());
            _hasBeenInitialized = true;
            OnInitialize?.Invoke(this);
        }

        /// <summary>
        /// Recycles the pool object, deactivating it and resetting its state via all IPoolable components.
        /// This method also ensures the object is deactivated and ready to be returned to the pool.
        /// </summary>
        public void Recycle()
        {
            _poolables.ForEach(x => x.Recycle());
            gameObject.SetActive(false);
            _hasBeenInitialized = false;
            OnRecycle?.Invoke(this);
        }

        public void SetActive(bool isActive)
        {
            this.gameObject.SetActive(isActive);
        }
    }
}
