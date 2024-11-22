using BepInEx;
using HarmonyLib;
using InControl;
using Jotunn.Utils;
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
        public const string Version = "2.2.0";
        public const string ModInitials = "RSC";
        public static RSClasses instance { get; private set; }

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
            RSClasses.assets = AssetUtils.LoadAssetBundleFromResources("rsclassart", typeof(RSClasses).Assembly);
            if (RSClasses.assets == null)
            {
                UnityEngine.Debug.Log("Failed to load RSClasses asset bundle");
            }
            assets.LoadAsset<GameObject>("CardHolder").GetComponent<CardHolder>().RegisterCards();
        }

        void Start()
        {
            instance = this;

            // TODO
            // Mirror Mage "Mirror Block" card? Would cause on-block effects to occur at the reflected position
            // Card that causes your gravity to face the prism line?
            // Voidseer card that summons eldritch worms from fractures?
            // Adjustable Voidseer hotkey
        }

        public bool pickPhase = false;
        public static bool Debug = true;
        internal static AssetBundle assets;
    }
}