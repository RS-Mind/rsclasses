using ClassesManagerReborn.Util;
using HarmonyLib;
using InControl;
using ModdingUtils.Extensions;
using RSClasses.Extensions;
using RSClasses.MonoBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnityEngine;

namespace RSClasses.Cards.MirrorMage
{
    class Voidseer : CustomCard
    {

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = MirrorMageClass.nameVoidseer;
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            statModifiers.lifeSteal = 0.5f;

            cardInfo.allowMultiple = false;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been setup."); }
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            var voidseer = player.gameObject.GetOrAddComponent<VoidseerMono>();
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            var voidseer = player.gameObject.GetOrAddComponent<VoidseerMono>();
            Destroy(voidseer);
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        internal static CardInfo Card = null;
        protected override string GetTitle()
        {
            return "Voidseer";
        }
        protected override string GetDescription()
        {
            return "Peer through the cracks in the mirror and into the beyond\nPress 'E' or DPad-Up to deal damage to yourself";
        }
        protected override GameObject GetCardArt()
        {
            return RSClasses.ArtAssets.LoadAsset<GameObject>("C_Voidseer");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Lifesteal",
                    amount = "+25%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
        }
        public override string GetModName()
        {
            return RSClasses.ModInitials;
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
                                    BindingFlags.NonPublic, null, __instance, new object[] { "Harm Self" });

        }
    }

    // postfix PlayerActions to add controls for self damage
    [HarmonyPatch(typeof(PlayerActions), "CreateWithControllerBindings")]
    class PlayerActionsPatchCreateWithControllerBindings
    {
        private static void Postfix(ref PlayerActions __result)
        {
            __result.GetAdditionalData().selfHarm.AddDefaultBinding(InputControlType.DPadUp);
        }
    }
    // postfix PlayerActions to add controls for self damage
    [HarmonyPatch(typeof(PlayerActions), "CreateWithKeyboardBindings")]
    class PlayerActionsPatchCreateWithKeyboardBindings
    {
        private static void Postfix(ref PlayerActions __result)
        {
            __result.GetAdditionalData().selfHarm.AddDefaultBinding(Key.E);
        }
    }
    // postfix GeneralInput to add controls for self damage
    [HarmonyPatch(typeof(GeneralInput), "Update")]
    class GeneralInputPatchUpdate
    {
        private static void Postfix(GeneralInput __instance)
        {
            try
            {
                if (__instance.GetComponent<CharacterData>().playerActions.GetAdditionalData().selfHarm.WasPressed)
                {
                    if (__instance.GetComponent<VoidseerMono>())
                    {
                        __instance.GetComponent<VoidseerMono>().Trigger();
                    }
                }
            }
            catch { }
        }
    }
}
