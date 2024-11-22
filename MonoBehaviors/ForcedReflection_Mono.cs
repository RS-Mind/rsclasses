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
    public class ForcedReflection_Mono : MonoBehaviour
    {
        private void Start()
        {
            player = GetComponentInParent<Player>();
            Gun gun = player.data.weaponHandler.gun;
            gun.ShootPojectileAction += OnShootProjectileAction;
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            GameObject forcedReflect = new GameObject();
            forcedReflect.AddComponent<ForcedReflectionEffect>();
            forcedReflect.transform.SetParent(obj.transform);
        }

        Player player;
    }

    public class ForcedReflectionEffect : RayHitEffect
    {
        private void Start()
        {
            AudioClip reflectAudioClip = RSClasses.assets.LoadAsset<AudioClip>("reflect.ogg");
            SoundContainer reflectSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            reflectSoundContainer.setting.volumeIntensityEnable = true;
            reflectSoundContainer.audioClip[0] = reflectAudioClip;
            reflectSound = ScriptableObject.CreateInstance<SoundEvent>();
            reflectSound.soundContainerArray[0] = reflectSoundContainer; ;
        }
        public override HasToReturn DoHitEffect(HitInfo hit)
        {
            if (!hit.transform){ return HasToReturn.canContinue; }
            Player hitPlayer = hit.transform.GetComponentInParent<Player>();
            if (!hitPlayer) { return HasToReturn.canContinue; }

            if (hitPlayer.data.view.IsMine)
            {
                if (rand.Next() % 2 == 0)
                {
                    soundParameterIntensity.intensity = (Optionshandler.vol_Sfx / 1f) * Optionshandler.vol_Master;
                    SoundManager.Instance.PlayAtPosition(reflectSound, hit.transform, hit.transform, new SoundParameterBase[]
                    {
                soundParameterIntensity
                    });
                    hitPlayer.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
                    hitPlayer.transform.SetPositionAndRotation(new Vector3(-hitPlayer.transform.position.x, hitPlayer.transform.position.y, hitPlayer.transform.position.z), hitPlayer.transform.rotation);
                    hitPlayer.data.block.RPCA_DoBlock(firstBlock: false);
                }
            }
            return HasToReturn.canContinue;
        }

        private System.Random rand = new System.Random(DateTime.Now.Millisecond);
        private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);
        private SoundEvent reflectSound;
    }
}