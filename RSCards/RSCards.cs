using BepInEx;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using HarmonyLib;
using RSCards.Cards;
using RSCards.Utilities;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using Jotunn.Utils;
using UnityEngine;
using System.Collections;

namespace RSCards
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.dk.rounds.plugins.zerogpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RSCards : BaseUnityPlugin
    {
        private const string ModId = "com.rsmind.rounds.RSCards";
        private const string ModName = "RSCards";
        public const string Version = "1.2.1";
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

            RSCards.ArtAssets = AssetUtils.LoadAssetBundleFromResources("rscardart", typeof(RSCards).Assembly);

            if (RSCards.ArtAssets == null)
            {
                UnityEngine.Debug.Log("Failed to load RSCards art asset bundle");
            }

            CustomCard.BuildCard<BounceAbsorption>();
            CustomCard.BuildCard<Changeup>();
            //CustomCard.BuildCard<HarmingField>((cardInfo) => { try { UnboundLib.Utils.CardManager.EnableCard(cardInfo); } catch { } });
            CustomCard.BuildCard<Hitscan>();
            CustomCard.BuildCard<Mortar>();
            CustomCard.BuildCard<OpenChamber>();
            CustomCard.BuildCard<RecklessAttack>();
            CustomCard.BuildCard<Repentance>();
            if (DateTools.WeekOf(new System.DateTime(System.DateTime.UtcNow.Year, 4, 1))) {
                CustomCard.BuildCard<Repentence>();
                CustomCard.BuildCard<Repen10ce>((cardInfo) => ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo));
            }
            CustomCard.BuildCard<Slug>();
            CustomCard.BuildCard<Split>();
            CustomCard.BuildCard<TwinScythe>();

            GameModeManager.AddHook(GameModeHooks.HookGameStart, GameStart);
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, PlayerPickStart);
        }

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

        public static bool Debug = false;
        internal static AssetBundle ArtAssets;
    }

    static class RSCardCategories
    {
        public static CardCategory BounceAbsorptionCategory = CustomCardCategories.instance.CardCategory("Bounce Absorption");
        public static CardCategory RepentanceCategory = CustomCardCategories.instance.CardCategory("Repentance");
    }
}