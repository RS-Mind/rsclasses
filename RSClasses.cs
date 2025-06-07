using BepInEx;
using HarmonyLib;
using Jotunn.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RSClasses.Utilities;

namespace RSClasses
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.simulationChamber", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.classes.manager.reborn", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.CrazyCoders.Rounds.RarityBundle", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.tabinfo", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rsmind.rounds.fancycardbar", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RSClasses : BaseUnityPlugin
    {
        private const string ModId = "com.rsmind.rounds.RSClasses";
        private const string ModName = "RSClasses";
        public const string Version = "2.4.1";
        public const string ModInitials = "RSC";
        internal static Harmony harmony;
        public static RSClasses instance { get; private set; }

        void Awake()
        {
            harmony = new Harmony(ModId);
            harmony.PatchAll();
            assets = AssetUtils.LoadAssetBundleFromResources("rsclassart", typeof(RSClasses).Assembly);
            if (assets == null)
            {
                UnityEngine.Debug.Log("Failed to load RSClasses asset bundle");
            }
            assets.LoadAsset<GameObject>("CardHolder").GetComponent<CardHolder>().RegisterCards();
        }

        void Start()
        {
            instance = this;
            var plugins = (List<BaseUnityPlugin>)typeof(BepInEx.Bootstrap.Chainloader).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            if (plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.willuwontu.rounds.tabinfo"))
            {
                TabinfoInterface.Setup();
            }

            // TODO
            // Card that causes your gravity to face the prism line? -- Probably a bad idea
            // Voidseer card that summons eldritch worms from fractures?
        }



        public bool pickPhase = false;
        public static bool Debug = false;
        internal static AssetBundle assets;
    }
}