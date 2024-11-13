using BepInEx;
using HarmonyLib;
using InControl;
using Jotunn.Utils;
using RSClasses.Cards.Astronomer;
using RSClasses.Cards.MirrorMage;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnityEngine;

namespace RSClasses
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.simulationChamber", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.classes.manager.reborn", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.CrazyCoders.Rounds.RarityBundle", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RSClasses : BaseUnityPlugin
    {
        private const string ModId = "com.rsmind.rounds.RSClasses";
        private const string ModName = "RSClasses";
        public const string Version = "2.0.0";
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

            CustomCard.BuildCard<Astronomer>((card) =>      Astronomer.Card = card);
            CustomCard.BuildCard<DarkHarvest>((card) =>     DarkHarvest.Card = card);
            CustomCard.BuildCard<DomainExtension>((card) => DomainExtension.Card = card);
            CustomCard.BuildCard<DualShields>((card) =>     DualShields.Card = card);
            CustomCard.BuildCard<FasterShields>((card) =>   FasterShields.Card = card);
            CustomCard.BuildCard<GravityWell>((card) =>     GravityWell.Card = card);
            CustomCard.BuildCard<Guardian>((card) =>        Guardian.Card = card);
            CustomCard.BuildCard<Harvester>((card) =>       Harvester.Card = card);
            CustomCard.BuildCard<HarvestSickle>((card) =>   HarvestSickle.Card = card);
            CustomCard.BuildCard<PerfectGuard>((card) =>    PerfectGuard.Card = card);
            CustomCard.BuildCard<SharperScythes>((card) =>  SharperScythes.Card = card);
            CustomCard.BuildCard<ShieldSpikes>((card) =>    ShieldSpikes.Card = card);
            CustomCard.BuildCard<TwinScythes>((card) =>     TwinScythes.Card = card);

            // TODO
            // Mirror Mage "Mirror Block" card? Would cause on-block effects to occur at the reflected position
            // Card that causes your gravity to face the prism line?
            // Voidseer card that summons eldritch worms from fractures?
            // Adjustable Voidseer hotkey

            CustomCard.BuildCard<EmeraldGlitter>((card) =>          EmeraldGlitter.Card = card);
            CustomCard.BuildCard<ForcedReflection>((card) =>        ForcedReflection.Card = card);
            CustomCard.BuildCard<ForcedRefraction>((card) =>        ForcedRefraction.Card = card);
            CustomCard.BuildCard<Fracture>((card) =>                Fracture.Card = card);
            CustomCard.BuildCard<KaleidoParty>((card) =>            KaleidoParty.Card = card);
            CustomCard.BuildCard<KaleidoWitch>((card) =>            KaleidoWitch.Card = card);
            CustomCard.BuildCard<MirrorMage>((card) =>              MirrorMage.Card = card);
            CustomCard.BuildCard<MirrorMind>((card) =>              MirrorMind.Card = card);
            CustomCard.BuildCard<PolishedMirror>((card) =>          PolishedMirror.Card = card);
            CustomCard.BuildCard<Prism>((card) =>                   Prism.Card = card);
            CustomCard.BuildCard<ReflectionReplacement>((card) =>   ReflectionReplacement.Card = card);
            CustomCard.BuildCard<RubyDust>((card) =>                RubyDust.Card = card);
            CustomCard.BuildCard<SapphireShards>((card) =>          SapphireShards.Card = card);
            CustomCard.BuildCard<Shatter>((card) =>                 Shatter.Card = card);
            CustomCard.BuildCard<Voidseer>((card) =>                Voidseer.Card = card);
            CustomCard.BuildCard<WeakenedMirror>((card) =>          WeakenedMirror.Card = card);

            GameModeManager.AddHook(GameModeHooks.HookPickStart, (gm) => Shatter.PickStart());
            GameModeManager.AddHook(GameModeHooks.HookPickEnd, (gm) => Shatter.PickEnd());
        }

        public bool pickPhase = false;
        public static bool Debug = true;
        internal static AssetBundle ArtAssets;
    }
}