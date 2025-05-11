using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTools.MagicEvents
{
    /// <summary>
    /// MagicEvents provide a simple way to connect systems together without any direct references
    /// </summary>
    public class MagicEvent
    {
#if DetailedMagicEventTracking && UNITY_EDITOR
        public const string eventInfoLoggingEventName = "__INTERNAL_EVENT_LOG__";
#endif

        #region Static Functionality
        public delegate void MagicEventCallback(MagicEventContext context);

        internal static Dictionary<string, List<MagicEventCallback>> eventCallbacks = new Dictionary<string, List<MagicEventCallback>>();

        private static Queue<MagicEvent> eventQueue = new Queue<MagicEvent>();
        private static bool waitingOnInvoke = false;

        /// <summary>
        /// Subscribes a callback to the target eventName
        /// </summary>
        /// <param name="eventName">Leave empty to catch all events</param>
        /// <param name="callback"></param>
        public static void AddListener(string eventName, MagicEventCallback callback)
        {
            if (callback == null) { return; }
            if (eventCallbacks.TryGetValue(eventName, out List<MagicEventCallback> callbackSet))
            {
                if (callbackSet.Contains(callback)) { return; }
                callbackSet.Add(callback);
            }
            else
            {
                eventCallbacks.Add(eventName, new List<MagicEventCallback>() { callback });
            }
        }

        /// <summary>
        /// Unsubscribes a callback from the target eventName
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callback"></param>
        public static void RemoveListener(string eventName, MagicEventCallback callback)
        {
            if(callback == null) { return; }
            if (eventCallbacks.TryGetValue(eventName, out List<MagicEventCallback> callbackSet))
            {
                callbackSet.Remove(callback);
                if (callbackSet.Count == 0)
                {
                    eventCallbacks.Remove(eventName);
                }
            }
        }

#endregion



        #region Instance Functionality
        private string name = string.Empty;
        private MagicEventContext context;

        /// <summary>
        /// Creates a new MagicEvent call to be prepared and invoked. Call .Invoke() on this object to finalize the event.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="MissingNameException"></exception>
        public MagicEvent(string name)
        {
            if (name == "" || name == null)
            {
                throw new MissingNameException();
            }
            this.name = name;
            context = new MagicEventContext()
            {
                eventName = name
            };
        }

        /// <summary>
        /// Attaches a data object to the event.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="DataAlreadyExistsWithIDException"></exception>
        public MagicEvent AddData(string id, object value)
        {
            if (context.data == null)
            {
                context.data = new Dictionary<string, object>();
            }
            if (!context.data.ContainsKey(id))
            {
                context.data.Add(id, value);
            }
            else
            {
                throw new DataAlreadyExistsWithIDException(id, value);
            }
            return this;
        }

        /// <summary>
        /// Adds the event to the event queue, triggering it when it is reached. The event queue enables any previous events to finish invoking before starting a new one.
        /// </summary>
        public void Invoke()
        {
            if (waitingOnInvoke) 
            {
                eventQueue.Enqueue(this);
                return;
            }

            waitingOnInvoke = true;
#if DetailedMagicEventTracking && UNITY_EDITOR
            InvokedMagicEventEditorInfo invokeInfo = new InvokedMagicEventEditorInfo()
            {
                eventName = name,
                context = context,
                callbackResults = new List<(MagicEventCallback callback, bool hadErrors)>()
            };
#endif
            if (eventCallbacks.TryGetValue(name, out List<MagicEventCallback> callbackSet))
            {
                foreach (MagicEventCallback callback in callbackSet)
                {
                    try
                    {
#if DetailedMagicEventTracking && UNITY_EDITOR
                        callback.Invoke(context);
                        invokeInfo.callbackResults.Add((callback, false));
                    }
                    catch (System.Exception e)
                    {
                        invokeInfo.callbackResults.Add((callback, true));
#else
                    callback.Invoke(context);
                    }
                    catch (System.Exception e)
                    {
#endif
                        Debug.LogException(e);
                    }
                }
            }
            //This enables a catch all event
            if (eventCallbacks.TryGetValue("", out List<MagicEventCallback> callbackSetNoName))
            {
                foreach (MagicEventCallback callback in callbackSetNoName)
                {
                    try
                    {
#if DetailedMagicEventTracking && UNITY_EDITOR
                        callback.Invoke(context);
                        invokeInfo.callbackResults.Add((callback, false));
                    }
                    catch (System.Exception e)
                    {
                        invokeInfo.callbackResults.Add((callback, true));
#else
                        callback.Invoke(context);
                    }
                    catch (System.Exception e)
                    {
#endif
                        Debug.LogException(e);
                    }
                }
            }
            waitingOnInvoke = false;

#if DetailedMagicEventTracking && UNITY_EDITOR
            if (name != eventInfoLoggingEventName) 
            {
                new MagicEvent(eventInfoLoggingEventName)
                    .AddData("Info", invokeInfo)
                    .Invoke();
            }
#endif
            if(eventQueue.Count > 0) 
            {
                eventQueue.Dequeue().Invoke();
            }
        }
#endregion

        /// <summary>
        /// Thrown when a data object is added to an event under the same id as a previously added piece of data
        /// </summary>
        public class DataAlreadyExistsWithIDException : System.Exception
        {
            public DataAlreadyExistsWithIDException(string id, object value) : base($"Can not add data({value}) with ID:({id}) as the id is already in use.") { }
        }


        /// <summary>
        /// Thrown when a MagicEvent is created with a blank or null name
        /// </summary>
        public class MissingNameException : System.Exception
        {
            public MissingNameException() : base("Can not create an event with no name.") { }
        }
    }

    /// <summary>
    /// Contains the data added to the event when it was prepared.
    /// </summary>
    public struct MagicEventContext
    {
        public string eventName { get; internal set; } //switched from basic variable to a property, kept camelCase for backwards compatabilty.
        internal Dictionary<string, object> data;

        /// <summary>
        /// Retrieves the data stored at the target id as T. If there is no data at the ID or it's type does not match T then it will return the default value of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public T GetData<T>(string dataID)
        {
            if (data == null) { return default(T); }

            if (data.TryGetValue(dataID, out object value))
            {
                if (value != null && typeof(T).IsAssignableFrom(value.GetType()))
                {
                    return (T)value;
                }
            }

            return default(T);
        }
    }

#if DetailedMagicEventTracking && UNITY_EDITOR
    public struct InvokedMagicEventEditorInfo 
    {
        public string eventName;
        public MagicEventContext context;
        public List<(MagicEvent.MagicEventCallback callback, bool hadErrors)> callbackResults;
    }

    public static class MagicEventEditorInfoTool
    {
        public static List<(string dataKey, object data)> ExtractDataValues(MagicEventContext context) 
        {
            List<(string dataKey, object data)> values = new List<(string dataKey, object data)>();
            if(context.data == null) { return values; }
            foreach (var data in context.data) 
            {
                values.Add((data.Key, data.Value));
            }
            return values;
        }

        public static List<(string eventName, List<MagicEvent.MagicEventCallback> callbacks)> GetEventListeners() 
        {
            List<(string eventName, List<MagicEvent.MagicEventCallback> callbacks)> values = new List<(string eventName, List<MagicEvent.MagicEventCallback> callbacks)>();

            foreach (var callbackSet in MagicEvent.eventCallbacks) 
            {
                values.Add((callbackSet.Key, new List<MagicEvent.MagicEventCallback>(callbackSet.Value)));
            }

            return values;
        }
    }
#endif
}