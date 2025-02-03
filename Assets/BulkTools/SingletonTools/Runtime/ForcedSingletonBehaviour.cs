using UnityEngine;

namespace BTools.Singletons
{
    /// <summary>
    /// Any class inheriting from this class will be forced to exist at runtime.
    /// </summary>
    public class ForcedSingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    CreateInstance<T>();
                }
                return _instance;
            }
        }

        private static void CreateInstance<C>() where C : MonoBehaviour
        {
            GameObject obj = new GameObject(typeof(C).Name);
            DontDestroyOnLoad(obj);
            obj.AddComponent<C>();
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

        protected virtual void ValidInstanceAwake() { }
    }

    public static class SingletonInitializer
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitForcedSingletons()
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                string assemblyName = assembly.GetName().ToString();
                if (!assemblyName.StartsWith("Assembly-CSharp")) continue;
                foreach (var type in assembly.GetTypes())
                {
                    if (type.BaseType == null || !typeof(MonoBehaviour).IsAssignableFrom(type)) continue;
                    System.Type possibleGeneric = typeof(ForcedSingletonBehaviour<>).MakeGenericType(type);
                    if (possibleGeneric != type.BaseType) continue;
                    CreateInstance(type);
                }
            }
        }

        private static void CreateInstance(System.Type type)
        {
            GameObject obj = new GameObject(type.Name);
            GameObject.DontDestroyOnLoad(obj);
            obj.AddComponent(type);
        }
    }
}