using BepInEx;
using HarmonyLib;
using Jotunn.Utils;
using Photon.Pun;
using RSClasses.Utilities;
using Sonigon;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToggleCardsCategories;
using UnityEngine;

namespace RSClasses
{
    [BepInDependency("com.aalund13.rounds.toggle_cards_categories", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.CrazyCoders.Rounds.RarityBundle", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rsmind.rounds.fancycardbar", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rsmind.rounds.weaponsmanager", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.simulationChamber", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.tabinfo", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.playerjumppatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.cardtheme.lib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.classes.manager.reborn", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RSClasses : BaseUnityPlugin
    {
        private const string ModId = "com.rsmind.rounds.RSClasses";
        private const string ModName = "RSClasses";
        public const string Version = "3.0.1";
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
            cardHolder = assets.LoadAsset<GameObject>("CardHolder");
            cardHolder.GetComponent<CardHolder>().RegisterCards();
        }

        internal static SoundEvent reflectSound;
        internal static SoundEvent shatterSound;
        internal static SoundEvent stardustSound;
        internal static SoundEvent sovereignBladeChargeSound;
        internal static SoundEvent sovereignBladeStrikeSound;
        private static GameObject cardHolder;

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

            AudioClip stardustAudioClip = RSClasses.assets.LoadAsset<AudioClip>("stardustHit.ogg"); // Load stardust sound effect
            SoundContainer stardustSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            stardustSoundContainer.setting.volumeIntensityEnable = true;
            stardustSoundContainer.audioClip[0] = stardustAudioClip;
            stardustSound = ScriptableObject.CreateInstance<SoundEvent>();
            stardustSound.soundContainerArray[0] = stardustSoundContainer;

            AudioClip sovereignBladeChargeAudioClip = RSClasses.assets.LoadAsset<AudioClip>("SovereignBladeCharge.ogg"); // Load sovereign blade sound effects
            SoundContainer sovereignBladeChargeSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            sovereignBladeChargeSoundContainer.setting.volumeIntensityEnable = true;
            sovereignBladeChargeSoundContainer.audioClip[0] = sovereignBladeChargeAudioClip;
            sovereignBladeChargeSound = ScriptableObject.CreateInstance<SoundEvent>();
            sovereignBladeChargeSound.soundContainerArray[0] = sovereignBladeChargeSoundContainer;

            AudioClip sovereignBladeStrikeAudioClip = RSClasses.assets.LoadAsset<AudioClip>("SovereignBladeStrike.ogg");
            SoundContainer sovereignBladeStrikeSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            sovereignBladeStrikeSoundContainer.setting.volumeIntensityEnable = true;
            sovereignBladeStrikeSoundContainer.audioClip[0] = sovereignBladeStrikeAudioClip;
            sovereignBladeStrikeSound = ScriptableObject.CreateInstance<SoundEvent>();
            sovereignBladeStrikeSound.soundContainerArray[0] = sovereignBladeStrikeSoundContainer;

            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            if (pool != null)
            {
                pool.ResourceCache.Add("Comet", assets.LoadAsset<GameObject>("Comet"));
                pool.ResourceCache.Add("Stardust", assets.LoadAsset<GameObject>("Stardust"));
                pool.ResourceCache.Add("Sword_Slash", assets.LoadAsset<GameObject>("Sword_Slash"));
            }

            // TODO
            // Voidseer card that summons eldritch worms from fractures?
            // Stargazer card that pulls/pushes comets when block/shoot?
        }

        public static bool Debug = false;
        internal static AssetBundle assets;
    }
}