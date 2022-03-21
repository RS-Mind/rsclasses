using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using RSCards.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using System.Collections.Generic;
using ModdingUtils.Extensions;
using System.Collections;
using UnboundLib.GameModes;
using Unity;
using UnityEngine;

namespace RSCards
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RSCards : BaseUnityPlugin
    {
        private const string ModId = "com.rsmind.rounds.RSCards";
        private const string ModName = "RSCards";
        public const string Version = "0.1.0";
        public const string ModInitials = "RSC";
        public static RSCards instance { get; private set; }

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        void Start()
        {
            instance = this;

            GameModeManager.AddHook(GameModeHooks.HookGameStart, GameStart);
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, PlayerPickStart);

            CustomCard.BuildCard<BounceAbsorption>();
            CustomCard.BuildCard<Changeup>();
            CustomCard.BuildCard<Hitscan>();
            CustomCard.BuildCard<OpenChamber>();
            CustomCard.BuildCard<RecklessAttack>();
            CustomCard.BuildCard<Repentance>();
            CustomCard.BuildCard<Slug>();
            CustomCard.BuildCard<Split>();
        }

        private static readonly AssetBundle Bundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("rscardart", typeof(RSCards).Assembly);

        public static GameObject ChangeupArt = Bundle.LoadAsset<GameObject>("C_Changeup");

        IEnumerator GameStart(IGameModeHandler gm)
        {
            // Runs at start of match
            yield break;
        }
        IEnumerator PlayerPickStart(IGameModeHandler gm)
        {
            // Runs at start of pick phase
            foreach (var player in PlayerManager.instance.players)
            {
                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.RemoveAll((category) => category == CustomCardCategories.instance.CardCategory("Default"));
                if (ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(RSCardCategories.BounceAbsorptionCategory) && player.data.GetComponent<Holding>().holdable.GetComponent<Gun>().reflects >= 2)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Remove(RSCardCategories.BounceAbsorptionCategory);
                }
                else if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(RSCardCategories.BounceAbsorptionCategory) && player.data.GetComponent<Holding>().holdable.GetComponent<Gun>().reflects < 2)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(RSCardCategories.BounceAbsorptionCategory);
                }

                if (ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(RSCardCategories.RepentanceCategory) && player.GetComponent<CharacterStatModifiers>().lifeSteal >= 0.5f)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Remove(RSCardCategories.RepentanceCategory);
                }
                else if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(RSCardCategories.RepentanceCategory) && player.GetComponent<CharacterStatModifiers>().lifeSteal < 0.5f)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(RSCardCategories.RepentanceCategory);
                }
            }

            yield break;
        }
    }
    static class RSCardCategories
    {
        public static CardCategory BounceAbsorptionCategory = CustomCardCategories.instance.CardCategory("Bounce Absorption");
        public static CardCategory RepentanceCategory = CustomCardCategories.instance.CardCategory("Repentance");
    }
}