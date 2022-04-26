using BepInEx;
using HarmonyLib;
using Jotunn.Utils;
using RSClasses.Cards.Astronomer;
using UnityEngine;
using UnboundLib.Cards;

namespace RSClasses
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.classes.manager.reborn", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RSClasses : BaseUnityPlugin
    {
        private const string ModId = "com.rsmind.rounds.RSClasses";
        private const string ModName = "RSClasses";
        public const string Version = "1.1.0";
        public const string ModInitials = "RSC";
        public static RSClasses instance { get; private set; }

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        void Start()
        {
            instance = this;

            RSClasses.ArtAssets = AssetUtils.LoadAssetBundleFromResources("rsclassart", typeof(RSClasses).Assembly);

            if (RSClasses.ArtAssets == null)
            {
                UnityEngine.Debug.Log("Failed to load RSClasses art asset bundle");
            }

            CustomCard.BuildCard<Astronomer>((card) => Astronomer.Card = card);
            CustomCard.BuildCard<DarkHarvest>((card) => DarkHarvest.Card = card);
            CustomCard.BuildCard<DomainExtension>((card) => DomainExtension.Card = card);
            CustomCard.BuildCard<DualShields>((card) => DualShields.Card = card);
            CustomCard.BuildCard<FasterShields>((card) => FasterShields.Card = card);
            CustomCard.BuildCard<GravityWell>((card) => GravityWell.Card = card);
            CustomCard.BuildCard<Guardian>((card) => Guardian.Card = card);
            CustomCard.BuildCard<Harvester>((card) => Harvester.Card = card);
            CustomCard.BuildCard<PerfectGuard>((card) => PerfectGuard.Card = card);
            CustomCard.BuildCard<SharperScythes>((card) => SharperScythes.Card = card);
            CustomCard.BuildCard<TwinScythes>((card) => TwinScythes.Card = card);
        }

        public static bool Debug = false;
        internal static AssetBundle ArtAssets;
    }
}