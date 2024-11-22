using HarmonyLib;
using InControl;
using RSClasses.MonoBehaviours;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RSClasses.Extensions // This is Pykess's. I take 0 credit for this code
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

    public static class PlayerActionsExtension
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
        private static void Postfix(PlayerActions __instance)
        {
            __instance.GetAdditionalData().selfHarm = (PlayerAction)typeof(PlayerActions).InvokeMember("CreatePlayerAction",
                                    BindingFlags.Instance | BindingFlags.InvokeMethod |
                                    BindingFlags.NonPublic, null, __instance, new object[] { "Voidseer Hotkey" });

        }
    }

    [HarmonyPatch(typeof(PlayerActions), "CreateWithControllerBindings")]
    class PlayerActionsPatchCreateWithControllerBindings
    {
        private static void Postfix(ref PlayerActions __result)
        {
            __result.GetAdditionalData().selfHarm.AddDefaultBinding(InputControlType.DPadUp);
        }
    }

    [HarmonyPatch(typeof(PlayerActions), "CreateWithKeyboardBindings")]
    class PlayerActionsPatchCreateWithKeyboardBindings
    {
        private static void Postfix(ref PlayerActions __result)
        {
            __result.GetAdditionalData().selfHarm.AddDefaultBinding(Key.E);
        }
    }

    [HarmonyPatch(typeof(GeneralInput), "Update")]
    class GeneralInputPatchUpdate
    {
        private static void Postfix(GeneralInput __instance)
        {
            try
            {
                if (__instance.GetComponent<CharacterData>().playerActions.GetAdditionalData().selfHarm.WasPressed)
                {
                    UnityEngine.Debug.Log("selfHarm");
                    if (__instance.GetComponentInChildren<Voidseer_Mono>())
                    {
                        UnityEngine.Debug.Log("Attempted call");
                        __instance.GetComponentInChildren<Voidseer_Mono>().Trigger();
                    }
                }
            }
            catch { }
        }
    }
}