using CodiceApp.EventTracking.Plastic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTools.MagicEvents
{
    /// <summary>
    /// Built for convenience not speed
    /// </summary>
    public class MagicEvent
    {
#if DetailedMagicEventTracking && UNITY_EDITOR
        public const string eventInfoLoggingEventName = "__INTERNAL_EVENT_LOG__";
#endif

        #region Static Functionality
        public delegate void MagicEventCallback(MagicEventContext context);

        internal static Dictionary<string, List<MagicEventCallback>> eventCallbacks = new Dictionary<string, List<MagicEventCallback>>();

        /// <summary>
        /// 
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
        /// 
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

        public MagicEvent(string name)
        {
            if (name == "")
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

        public void Invoke()
        {
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
#if DetailedMagicEventTracking && UNITY_EDITOR
            if (name != eventInfoLoggingEventName) 
            {
                new MagicEvent(eventInfoLoggingEventName)
                    .AddData("Info", invokeInfo)
                    .Invoke();
            }
#endif
        }
#endregion

        public class DataAlreadyExistsWithIDException : System.Exception
        {
            public DataAlreadyExistsWithIDException(string id, object value) : base($"Can not add data({value}) with ID:({id}) as the id is already in use.") { }
        }

        public class MissingNameException : System.Exception
        {
            public MissingNameException() : base("Can not create an event with no name.") { }
        }


    }

    public struct MagicEventContext
    {
        public string eventName;
        internal Dictionary<string, object> data;
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