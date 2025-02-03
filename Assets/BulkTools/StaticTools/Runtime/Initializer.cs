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
    internal static class Initializer
    {
        /// <summary>
        /// Hook all our custom subsystems
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HookIntoPlayerLoop()
        {
            AddToPlayerLoop("Update", 0, StaticUpdate.GetPlayerLoopSystem());
            AddToPlayerLoop("Update", 1, StaticCoroutines.GetPlayerLoopSystem());
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += StaticUpdate.EndPlayMode;
            EditorApplication.playModeStateChanged += StaticCoroutines.EndPlayMode;
#endif
        }

        /// <summary>
        /// Hooks a custom subsystem into the target Unity subsystem
        /// </summary>
        /// <param name="targetSubSystem"></param>
        /// <param name="targetIndex"></param>
        /// <param name="newSystem"></param>
        internal static void AddToPlayerLoop(string targetSubSystem, int targetIndex, PlayerLoopSystem newSystem)
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystem foundTargetSubSystem = new PlayerLoopSystem();
            int systemIndex = 0;
            bool foundSystem = false;
            for (int i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type.Name == targetSubSystem)
                {
                    foundTargetSubSystem = playerLoop.subSystemList[i];
                    foundSystem = true;
                    systemIndex = i;
                    break;
                }
            }
            if (!foundSystem)
            {
                Debug.LogError($"Could not find {targetSubSystem} subsystem, {newSystem.type.Name} will not be configured");
                return;
            }
            var tempList = new List<PlayerLoopSystem>(foundTargetSubSystem.subSystemList);

            tempList.Insert(Mathf.Clamp(targetIndex, 0, tempList.Count), newSystem);

            foundTargetSubSystem.subSystemList = tempList.ToArray();

            playerLoop.subSystemList[systemIndex] = foundTargetSubSystem;
            PlayerLoop.SetPlayerLoop(playerLoop);
        }
    }
}
