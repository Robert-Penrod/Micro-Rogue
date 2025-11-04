using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperQuest.EasyPooling
{
    public interface IPoolable 
    {
        /// <summary>
        /// Initializes the object when it is activated from the pool. 
        /// This method is intended to reset the object to a default state, preparing it for use as if newly instantiated.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Recycles the object when it is returned to the pool.
        /// This method is intended for cleaning up or resetting any state before the object is deactivated and stored in the pool.
        /// Implementations can optionally override this method to provide specific cleanup behavior.
        /// </summary>
        void Recycle() { }
    }
}