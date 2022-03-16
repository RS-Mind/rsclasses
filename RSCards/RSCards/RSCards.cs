using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using RSCards.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using System.Collections.Generic;
using ModdingUtils.Extensions;

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

        public static CardCategory BounceAbsorptionCategory = CustomCardCategories.instance.CardCategory("Bounce Absorption");

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        void Start()
        {
            instance = this;

            foreach (var player in PlayerManager.instance.players)
            {
                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.RemoveAll((category) => category == CustomCardCategories.instance.CardCategory("Default"));
                if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(BounceAbsorptionCategory))
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(BounceAbsorptionCategory);
                }
            }

            CustomCard.BuildCard<BounceAbsorption>();
            CustomCard.BuildCard<Changeup>();
            CustomCard.BuildCard<OpenChamber>();
            CustomCard.BuildCard<RecklessAttack>();
            CustomCard.BuildCard<Repentence>();
            CustomCard.BuildCard<Slug>();
            CustomCard.BuildCard<Split>();
        }
    }
}
