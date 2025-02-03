using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BTools.StaticTools
{
    /// <summary>
    /// Enables update methods to be run without an owner
    /// </summary>
    public static class StaticUpdate
    {
        public delegate void UpdateDelegate();

        private static List<UpdateDelegate> updates = new List<UpdateDelegate>();

        internal static PlayerLoopSystem GetPlayerLoopSystem()
        {
            return new PlayerLoopSystem()
            {
                type = typeof(StaticUpdate),
                updateDelegate = Update
            };
        }

        /// <summary>
        /// Add a method to the static update loop
        /// </summary>
        /// <param name="update"></param>
        public static void AddUpdate(UpdateDelegate update)
        {
            if (!update.Method.IsStatic)//Force the method to be static for saftey and consistency 
            {
                Debug.LogError("StaticUpdate only supports running static methods.");
                return;
            }
            if (updates.Contains(update))
            {
                Debug.LogWarning($"{update.Method.Name} has already been added to StaticUpdate.");
                return;
            }
            updates.Add(update);
        }

        /// <summary>
        /// Remove a method from the static update loop
        /// </summary>
        /// <param name="update"></param>
        public static void RemoveUpdate(UpdateDelegate update)
        {
            updates.Remove(update);
        }

        //Runs the update delegates
        private static void Update()
        {
            if (!Application.isPlaying) { return; }
            foreach (var update in updates)
            {
                //try-catch each method so that an error doesn't prevent others from running
                try
                {
                    update.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

#if UNITY_EDITOR
        public static void EndPlayMode(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                updates.Clear();
            }
        }
#endif
    }
}