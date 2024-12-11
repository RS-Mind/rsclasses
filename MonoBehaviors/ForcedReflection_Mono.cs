using UnityEngine;
using Photon.Pun;
using SimulationChamber;
using System.Linq;
using RSClasses.Extensions;
using ModdingUtils.RoundsEffects;
using System;
using Sonigon;
using Photon.Realtime;
using Sonigon.Internal;
using System.Collections.Generic;
using UnboundLib;

namespace RSClasses.MonoBehaviours
{
    public class ForcedReflection_Mono : MonoBehaviour // Holds the Forced Reflection effect on the player
    {
        private void Start()
        {
            player = GetComponentInParent<Player>(); // Get player and gun
            Gun gun = player.data.weaponHandler.gun;
            gun.ShootPojectileAction += OnShootProjectileAction; // Add the on shoot projectile action
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            GameObject forcedReflect = new GameObject();
            forcedReflect.AddComponent<ForcedReflectionEffect>();
            forcedReflect.transform.SetParent(obj.transform); // Add effect to the bullet
        }

        Player player;
    }

    public class ForcedReflectionEffect : RayHitEffect
    {
        private System.Random rand = new System.Random(DateTime.Now.Millisecond);
        private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);
        private SoundEvent reflectSound;

        private void Start()
        {
            AudioClip reflectAudioClip = RSClasses.assets.LoadAsset<AudioClip>("reflect.ogg"); // Load reflection sound effect
            SoundContainer reflectSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            reflectSoundContainer.setting.volumeIntensityEnable = true;
            reflectSoundContainer.audioClip[0] = reflectAudioClip;
            reflectSound = ScriptableObject.CreateInstance<SoundEvent>();
            reflectSound.soundContainerArray[0] = reflectSoundContainer; ;
        }
        public override HasToReturn DoHitEffect(HitInfo hit)
        {
            if (!hit.transform) { return HasToReturn.canContinue; } // End if no actual hit object
            Player hitPlayer = hit.transform.GetComponentInParent<Player>();
            if (!hitPlayer) { return HasToReturn.canContinue; } // End if no player

            if (hitPlayer.data.view.IsMine) // Run on target's client only
            {
                if (rand.Next() % 2 == 0) // 50% chance to trigger
                {
                    soundParameterIntensity.intensity = (Optionshandler.vol_Sfx / 1f) * Optionshandler.vol_Master; // Play sfx
                    SoundManager.Instance.PlayAtPosition(reflectSound, hit.transform, hit.transform, new SoundParameterBase[]
                    {
                soundParameterIntensity
                    });
                    hitPlayer.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2); // Disable collision while teleportating
                    hitPlayer.transform.SetPositionAndRotation(new Vector3(-hitPlayer.transform.position.x, hitPlayer.transform.position.y, hitPlayer.transform.position.z), hitPlayer.transform.rotation); // Flip player's X coord
                    hitPlayer.data.block.RPCA_DoBlock(firstBlock: false); // Cause the player to block
                }
            }
            return HasToReturn.canContinue;
        }
    }
}