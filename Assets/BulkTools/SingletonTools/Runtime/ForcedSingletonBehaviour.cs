using UnityEngine;

namespace BTools.Singletons
{
    /// <summary>
    /// Any class inheriting from this class will be forced to exist at runtime as a "DontDestroyOnLoad" object.
    /// </summary>
    public class ForcedSingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        /// <summary>
        /// The instance of the singleton
        /// </summary>
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    //If there is not instance yet create it now
                    CreateInstance<T>();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Creates an instance of the singleton
        /// </summary>
        /// <typeparam name="C"></typeparam>
        private static void CreateInstance<C>() where C : MonoBehaviour
        {
            if (SingletonInitializer.ApplicationQuitting) { return; } //Prevent a new instance if we are shutting down
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

        /// <summary>
        /// Called durring awake only if the current instance is the main instance
        /// </summary>
        protected virtual void ValidInstanceAwake() { }
    }

    /// <summary>
    /// Initializes all forced singletons
    /// </summary>
    internal static class SingletonInitializer
    {
        //Used to determine if a new instance should be created when accessing an Instance variable
        internal static bool ApplicationQuitting { get; private set; }

        /// <summary>
        /// Some of the starting strings of assemblies I see often included in projects that should never include one of the targeted singletons
        /// </summary>
        private static string[] skippedAssemblies = new string[]
        {
            "UnityEngine",
            "UnityEditor",
            "System",
            "Bee.",
            "Unity.",
            "mscorlib",
            "netstandard",
            "Mono.",
            "unityplastic",
            "log4net",
            "JetBrains",
            "Anonymously Hosted",
            "PPv2URPConverters",
            "Microsoft",
            "nunit."
        };

        /// <summary>
        /// Finds all forced singletons and initializes them
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitForcedSingletons()
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                string assemblyName = assembly.GetName().ToString();
                if (CheckAssemblySkipped(assemblyName)) continue;
                foreach (var type in assembly.GetTypes())
                {
                    if (type.BaseType == null || !typeof(MonoBehaviour).IsAssignableFrom(type)) continue;
                    System.Type possibleGeneric = typeof(ForcedSingletonBehaviour<>).MakeGenericType(type);
                    if (possibleGeneric != type.BaseType) continue;
                    CreateInstance(type);
                }
            }
        }

        /// <summary>
        /// Checks if the assembly name matches the list of ignored assemblies
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool CheckAssemblySkipped(string name) 
        {
            foreach(string str in skippedAssemblies) 
            {
                if (name.StartsWith(str)) return true;
            }
            return false;
        }

        /// <summary>
        /// Creates an instance of the singleton type
        /// </summary>
        /// <param name="type"></param>
        private static void CreateInstance(System.Type type)
        {
            GameObject obj = new GameObject(type.Name);
            GameObject.DontDestroyOnLoad(obj);
            obj.AddComponent(type);
        }

        /// <summary>
        /// Subscribes callback for setting ApplicationQuitting
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void PreventInstanceInShutdownInit()
        {
            Application.quitting += () => { ApplicationQuitting = true; };
        }
    }
}