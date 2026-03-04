using BepInEx;
using HarmonyLib;
using Jotunn.Utils;
using Photon.Pun;
using RSClasses.Utilities;
using Sonigon;
using System.Collections.Generic;
using System.Reflection;
using ToggleCardsCategories;
using UnityEngine;

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
    [BepInDependency("com.aalund13.rounds.toggle_cards_categories", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RSClasses : BaseUnityPlugin
    {
        private const string ModId = "com.rsmind.rounds.RSClasses";
        private const string ModName = "RSClasses";
        public const string Version = "2.5.3";
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
            ToggleCardsCategoriesManager.instance.RegisterCategories(ModInitials);
            assets.LoadAsset<GameObject>("CardHolder").GetComponent<CardHolder>().RegisterCards();
        }

        internal static SoundEvent reflectSound;
        internal static SoundEvent shatterSound;
        internal static SoundEvent stardustSound;

        void Start()
        {
            instance = this;
            var plugins = (List<BaseUnityPlugin>)typeof(BepInEx.Bootstrap.Chainloader).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            if (plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.willuwontu.rounds.tabinfo"))
            {
                TabinfoInterface.Setup();
            }

            AudioClip reflectAudioClip = RSClasses.assets.LoadAsset<AudioClip>("reflect.ogg"); // Load reflection sound effect
            SoundContainer reflectSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            reflectSoundContainer.setting.volumeIntensityEnable = true;
            reflectSoundContainer.audioClip[0] = reflectAudioClip;
            reflectSound = ScriptableObject.CreateInstance<SoundEvent>();
            reflectSound.soundContainerArray[0] = reflectSoundContainer;

            AudioClip shatterAudioClip = RSClasses.assets.LoadAsset<AudioClip>("shatter.ogg"); // Load shatter sound effect
            SoundContainer shatterSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            shatterSoundContainer.setting.volumeIntensityEnable = true;
            shatterSoundContainer.audioClip[0] = reflectAudioClip;
            shatterSound = ScriptableObject.CreateInstance<SoundEvent>();
            shatterSound.soundContainerArray[0] = shatterSoundContainer;

            AudioClip stardustAudioClip = RSClasses.assets.LoadAsset<AudioClip>("stardustHit.ogg"); // Load sound effects
            SoundContainer stardustSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            stardustSoundContainer.setting.volumeIntensityEnable = true;
            stardustSoundContainer.audioClip[0] = stardustAudioClip;
            stardustSound = ScriptableObject.CreateInstance<SoundEvent>();
            stardustSound.soundContainerArray[0] = stardustSoundContainer;

            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            if (pool != null)
            {
                pool.ResourceCache.Add("Comet", assets.LoadAsset<GameObject>("Comet"));
                pool.ResourceCache.Add("Stardust", assets.LoadAsset<GameObject>("Stardust"));
            }

            // TODO
            // Card that causes your gravity to face the prism line? -- Probably a bad idea
            // Voidseer card that summons eldritch worms from fractures?
            // Stargazer card that pulls/pushes comets when block/shoot?
        }



        public bool pickPhase = false;
        public static bool Debug = false;
        internal static AssetBundle assets;
    }
}