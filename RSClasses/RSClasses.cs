using BepInEx;
using HarmonyLib;
using Jotunn.Utils;
using RSClasses.Cards.Astronomer;
using RSClasses.Cards.MirrorMage;
using UnboundLib.Cards;
using UnityEngine;

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
        public const string Version = "1.2.5";
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

            // TODO
            // Make Mirror Mage function
            // Add "Shield Spikes" card for Guardian. On-block ability to deal damage using the barriers (pulse effect?)
            // Mirror Mage "Mirror Block" card? Would cause on-block effects to occur at the reflected position

            //CustomCard.BuildCard<MirrorMage>((card) => MirrorMage.Card = card);
            //CustomCard.BuildCard<MirrorMind>((card) => MirrorMind.Card = card);
            //CustomCard.BuildCard<PolishedMirror>((card) => PolishedMirror.Card = card);
            //CustomCard.BuildCard<Prism>((card) => Prism.Card = card);
            //CustomCard.BuildCard<ReflectionReplacement>((card) => ReflectionReplacement.Card = card);
            //CustomCard.BuildCard<Shatter>((card) => Shatter.Card = card);
        }

        public static bool Debug = true;
        internal static AssetBundle ArtAssets;
    }
}