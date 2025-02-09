using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Codice.Client.Common;
using System.Runtime.Remoting.Contexts;

namespace BTools.MagicEvents.EditorScripts
{
    public class MagicEventsEditorWindow : EditorWindow
    {
        private const int trackedEventsMaxLength = 50;
        private static List<MagicEventContext> trackedEvents = new List<MagicEventContext>();
        private static List<System.DateTime> timeStamps = new List<System.DateTime>();
#if DetailedMagicEventTracking
        private static List<InvokedMagicEventEditorInfo> trackedEventInfos = new List<InvokedMagicEventEditorInfo>();
#endif
        private static Vector2 scrollPos = Vector2.zero;
        private static MagicEventsEditorWindow windowInstance;
        private static GUIStyle failedCallbackStyle;
        private static int curTab = 0;

        [MenuItem("BulkTools/Magic Events/Magic Event Tracker")]
        public static void OpenWindow()
        {
            MagicEventsEditorWindow windowInstance = GetWindow<MagicEventsEditorWindow>();
            windowInstance.titleContent = new GUIContent("Magic Event Tracker");
        }

        private void OnEnable()
        {
            windowInstance = this;
#if DetailedMagicEventTracking
            MagicEvent.AddListener(MagicEvent.eventInfoLoggingEventName, OnInfoLogEvent);
#else
            MagicEvent.AddListener("", OnAnyEvent);
#endif
        }

        private void OnDisable()
        {
            windowInstance = null;
#if DetailedMagicEventTracking
            MagicEvent.RemoveListener(MagicEvent.eventInfoLoggingEventName, OnInfoLogEvent); 
#else
            MagicEvent.RemoveListener("", OnAnyEvent);
#endif
        }

        private void OnGUI()
        {
            if (failedCallbackStyle == null) 
            {
                failedCallbackStyle = new GUIStyle(EditorStyles.label);
                failedCallbackStyle.normal.textColor = Color.red;
            }
            DrawGUI();
        }

        private static void DrawGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

#if DetailedMagicEventTracking
            curTab = EditorGUILayout.Popup("Mode", curTab, new string[] { "Event Calls", "Active Event Listeners" });
#endif

            switch (curTab) 
            {
                case 0:
                default:
                    DrawEventCalls();
                    break;
#if DetailedMagicEventTracking
                case 1:
                    DrawEventCallbacks();
                    break;
#endif
            }

            EditorGUILayout.EndScrollView();
        }

#if DetailedMagicEventTracking
        private static void DrawEventCallbacks() 
        {
            var listeners = MagicEventEditorInfoTool.GetEventListeners();
            DrawLine();
            foreach (var listenerSet in listeners) 
            {
                if (listenerSet.eventName == MagicEvent.eventInfoLoggingEventName) { continue; }
                EditorGUILayout.LabelField(new GUIContent($"Event: {listenerSet.eventName}"));
                EditorGUI.indentLevel++;
                foreach (var callback in listenerSet.callbacks) 
                {
                    EditorGUILayout.LabelField(new GUIContent($"Listener: {callback.Target} : {callback.Method.Name}"));
                }
                EditorGUI.indentLevel--;
                DrawLine();
            }
        }
#endif
        private static void DrawEventCalls() 
        {
            DrawLine();
            for (int eventIndex = trackedEvents.Count - 1; eventIndex >= 0; eventIndex--)
            {
#if DetailedMagicEventTracking
                DrawEventCallsDetailed(trackedEvents[eventIndex], trackedEventInfos[eventIndex], timeStamps[eventIndex]);
#else
                DrawEventCallsBasic(trackedEvents[eventIndex], timeStamps[eventIndex]);
#endif
                DrawLine();
            }
        }



#if DetailedMagicEventTracking
        private static void DrawEventCallsDetailed(MagicEventContext context, InvokedMagicEventEditorInfo info, System.DateTime timeStamp)
        {
            EditorGUILayout.LabelField(new GUIContent($"{timeStamp.ToString("HH:mm:ss:fff")} : {context.eventName}"));

            EditorGUI.indentLevel++;

            //Data:
            var extractedContext = MagicEventEditorInfoTool.ExtractDataValues(context);
            if (extractedContext.Count != 0) 
            {
                EditorGUILayout.LabelField(new GUIContent("Included Data:"));
                EditorGUI.indentLevel++;
                foreach(var dataItem in extractedContext) 
                {
                    EditorGUILayout.LabelField(new GUIContent($"{dataItem.dataKey} : {dataItem.data}"));
                }
                EditorGUI.indentLevel--;
            }

            //Callbacks:
            EditorGUILayout.LabelField(new GUIContent("Triggered Callbacks"));
            EditorGUI.indentLevel++;
            for (int callbackIndex = 0; callbackIndex < info.callbackResults.Count; callbackIndex++) 
            {
                var callback = info.callbackResults[callbackIndex].callback;
                string infoStr = $"{callback.Target} : {callback.Method.Name}";
                if (info.callbackResults[callbackIndex].hadErrors) 
                {
                    infoStr += "(Exception)";
                }
                EditorGUILayout.LabelField(new GUIContent(infoStr), info.callbackResults[callbackIndex].hadErrors ? failedCallbackStyle : EditorStyles.label);
            }
            EditorGUI.indentLevel--;

            EditorGUI.indentLevel--;
        }

        private static void OnInfoLogEvent(MagicEventContext context) 
        {
            var info = context.GetData<InvokedMagicEventEditorInfo>("Info");


            trackedEvents.Add(info.context);
            trackedEventInfos.Add(info);
            timeStamps.Add(System.DateTime.Now);
            if (trackedEvents.Count > trackedEventsMaxLength)
            {
                trackedEvents.RemoveAt(0);
                trackedEventInfos.RemoveAt(0);
                timeStamps.RemoveAt(0);
            }
            windowInstance.Repaint();
        }
#endif
        private static void DrawEventCallsBasic(MagicEventContext context, System.DateTime timeStamp)
        {
            EditorGUILayout.LabelField(new GUIContent($"{timeStamp.ToString("HH:mm:ss:fff")} : {context.eventName}"));
        }

        private static void OnAnyEvent(MagicEventContext context) 
        {
            trackedEvents.Add(context);
            timeStamps.Add(System.DateTime.Now);
            if (trackedEvents.Count > trackedEventsMaxLength) 
            {
                trackedEvents.RemoveAt(0);
                timeStamps.RemoveAt(0);
            }
            windowInstance.Repaint();
        }



        /// <summary>
        /// Draws a 1 unit seprator line
        /// </summary>
        private static void DrawLine(bool horizontal = true)
        {
            if (horizontal)
            {
                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Height(1)), Color.grey);
            }
            else
            {
                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Width(1)), Color.grey);
            }
        }
    }
}
