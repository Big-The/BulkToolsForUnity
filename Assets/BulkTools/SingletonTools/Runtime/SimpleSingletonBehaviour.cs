using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTools.Singletons 
{
    /// <summary>
    /// Any class inheriting from this class will only be allowed to exist once, but not required to exist.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleSingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        /// <summary>
        /// The instance of the singleton
        /// </summary>
        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this as T;
            ValidInstanceAwake();
        }

        /// <summary>
        /// Called durring awake only if the current instance is the main instance
        /// </summary>
        protected virtual void ValidInstanceAwake() { }
    }
}
