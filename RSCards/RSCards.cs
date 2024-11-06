using BepInEx;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using HarmonyLib;
using RSCards.Cards;
using RSCards.Utilities;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnboundLib.Utils;
using Jotunn.Utils;
using UnityEngine;
using System.Collections;
using UnboundLib;
using System.Linq;

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
        public const string Version = "1.3.3";
        public const string ModInitials = "RSC";
        public static RSCards instance { get; private set; }

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        void Start()
        {
            bool RSClasses = false;
            foreach (BaseUnityPlugin plugin in BepInEx.Bootstrap.Chainloader.Plugins)
            {
                if (plugin.Info.Metadata.GUID == "com.rsmind.rounds.RSClasses")
                {
                    RSClasses = true;
                    break;
                }
            }

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
            if (!RSClasses) CustomCard.BuildCard<TwinScythe>();

            GameModeManager.AddHook(GameModeHooks.HookGameStart, GameStart);
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, PlayerPickStart);

            this.ExecuteAfterSeconds(0.4f, () =>
            {
                CustomCardCategories.instance.MakeCardsExclusive(
                    CardManager.cards.Values.First(card => card.cardInfo.cardName == "Hitscan").cardInfo,
                    CardManager.cards.Values.First(card => card.cardInfo.cardName == "Mortar").cardInfo);

            });
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
                
                if (player.data.GetComponent<Holding>().holdable.GetComponent<Gun>().reflects >= 2)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Remove(RSCardCategories.BounceAbsorptionCategory);
                }
                else
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(RSCardCategories.BounceAbsorptionCategory);
                }

                if (player.GetComponent<CharacterStatModifiers>().lifeSteal >= 0.5f)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Remove(RSCardCategories.RepentanceCategory);
                }
                else
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