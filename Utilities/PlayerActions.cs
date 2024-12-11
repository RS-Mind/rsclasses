using HarmonyLib;
using InControl;
using RSClasses.MonoBehaviours;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RSClasses.Extensions // Adds actions to players
{
    [Serializable]
    public class PlayerActionsAdditionalData
    {
        public PlayerAction selfHarm;


        public PlayerActionsAdditionalData()
        {
            selfHarm = null;
        }
    }

    public static class PlayerActionsExtension // Magic
    {
        public static readonly ConditionalWeakTable<PlayerActions, PlayerActionsAdditionalData> data =
            new ConditionalWeakTable<PlayerActions, PlayerActionsAdditionalData>();

        public static PlayerActionsAdditionalData GetAdditionalData(this PlayerActions playerActions)
        {
            return data.GetOrCreateValue(playerActions);
        }

        public static void AddData(this PlayerActions playerActions, PlayerActionsAdditionalData value)
        {
            try
            {
                data.Add(playerActions, value);
            }
            catch (Exception) { }
        }
    }

    [HarmonyPatch(typeof(PlayerActions))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] { })]
    class PlayerActionsPatchPlayerActions
    {
        private static void Postfix(PlayerActions __instance) // Voidseer hotkey
        {
            __instance.GetAdditionalData().selfHarm = (PlayerAction)typeof(PlayerActions).InvokeMember("CreatePlayerAction",
                                    BindingFlags.Instance | BindingFlags.InvokeMethod |
                                    BindingFlags.NonPublic, null, __instance, new object[] { "Voidseer Hotkey" });

        }
    }

    [HarmonyPatch(typeof(PlayerActions), "CreateWithControllerBindings")] // Voidseer for controller
    class PlayerActionsPatchCreateWithControllerBindings
    {
        private static void Postfix(ref PlayerActions __result)
        {
            __result.GetAdditionalData().selfHarm.AddDefaultBinding(InputControlType.DPadUp);
        }
    }
    
    [HarmonyPatch(typeof(PlayerActions), "CreateWithKeyboardBindings")] // Voidseer for keyboard
    class PlayerActionsPatchCreateWithKeyboardBindings
    {
        private static void Postfix(ref PlayerActions __result)
        {
            __result.GetAdditionalData().selfHarm.AddDefaultBinding(Key.E);
        }
    }

    [HarmonyPatch(typeof(GeneralInput), "Update")]
    class GeneralInputPatchUpdate // Check if the actions happened
    {
        private static void Postfix(GeneralInput __instance)
        {
            try
            {
                if (__instance.GetComponent<CharacterData>().playerActions.GetAdditionalData().selfHarm.WasPressed)
                {
                    if (__instance.GetComponentInChildren<Voidseer_Mono>()) // If they are a voidseer, trigger the effect
                    {
                        __instance.GetComponentInChildren<Voidseer_Mono>().Trigger();
                    }
                }
            }
            catch { }
        }
    }
}