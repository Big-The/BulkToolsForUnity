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
        #region Static Functionality
        public delegate void MagicEventCallback(MagicEventContext context);

        private static Dictionary<string, List<MagicEventCallback>> eventCallbacks = new Dictionary<string, List<MagicEventCallback>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName">Leave empty to catch all events</param>
        /// <param name="callback"></param>
        public static void AddListener(string eventName, MagicEventCallback callback)
        {
            if (eventCallbacks.TryGetValue(eventName, out List<MagicEventCallback> callbackSet))
            {
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
        MagicEventContext context;

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
            if (eventCallbacks.TryGetValue(name, out List<MagicEventCallback> callbackSet))
            {
                foreach (MagicEventCallback callback in callbackSet)
                {
                    try
                    {
                        callback.Invoke(context);
                    }
                    catch (System.Exception e)
                    {
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
                        callback.Invoke(context);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
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
                if (typeof(T).IsAssignableFrom(value.GetType()))
                {
                    return (T)value;
                }
            }

            return default(T);
        }
    }
}