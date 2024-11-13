using RSClasses.Cards.MirrorMage;
using RSClasses.Extensions;
using Sonigon;
using Sonigon.Internal;
using System;
using System.Collections.Generic;
using UnboundLib;
using UnboundLib.Extensions;
using UnityEngine;

namespace RSClasses.MonoBehaviours
{
    internal class ShatterMono : MonoBehaviour
    {
        private void Awake()
        {
            player = gameObject.GetComponent<Player>();
        }

        private void Start()
        {
            AudioClip shatterAudioClip = RSClasses.ArtAssets.LoadAsset<AudioClip>("shatter.ogg");
            SoundContainer shatterSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            shatterSoundContainer.setting.volumeIntensityEnable = true;
            shatterSoundContainer.audioClip[0] = shatterAudioClip;
            shatterSound = ScriptableObject.CreateInstance<SoundEvent>();
            shatterSound.soundContainerArray[0] = shatterSoundContainer;

            AudioClip reflectAudioClip = RSClasses.ArtAssets.LoadAsset<AudioClip>("reflect.ogg");
            SoundContainer reflectSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            reflectSoundContainer.setting.volumeIntensityEnable = true;
            reflectSoundContainer.audioClip[0] = reflectAudioClip;
            reflectSound = ScriptableObject.CreateInstance<SoundEvent>();
            reflectSound.soundContainerArray[0] = reflectSoundContainer;

            if (player.data.view.IsMine)
            {
                reflection = GameObject.Instantiate(RSClasses.ArtAssets.LoadAsset<GameObject>("Reflection"), player.transform);
                reflection.SetActive(true);
                reflection.GetComponent<SpriteRenderer>().color = player.GetTeamColors().color;
            }
        }

        public void Update()
        {
            cooldownTimer = cooldownTimer - TimeHandler.deltaTime < 0f ? 0f : cooldownTimer - TimeHandler.deltaTime;
            if (player.data.view.IsMine)
            {
                reflection.transform.SetPositionAndRotation(new Vector3(-player.transform.position.x, player.transform.position.y, player.transform.position.z), player.transform.rotation);
                if (shatters.Count > 0)
                {
                    foreach (GameObject shatter in shatters)
                    {
                        var hits = Physics2D.OverlapCircleAll(shatter.transform.position, 166.66f * radius);
                        foreach (var hit in hits)
                        {
                            var healthHandler = hit.gameObject.GetComponent<HealthHandler>();
                            if (healthHandler)
                            {
                                Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                                if (hitPlayer.playerID != player.playerID) healthHandler.CallTakeDamage(((Vector2)hitPlayer.transform.position - (Vector2)shatter.transform.position).normalized * Time.deltaTime * 45,
                               (Vector2)this.transform.position, gameObject, player);
                            }
                        }
                    }
                }
            }
        }

        public void TriggerReflect()
        {
            if (cooldownTimer == 0)
            {
                soundParameterIntensity.intensity = (Optionshandler.vol_Sfx / 1.15f) * Optionshandler.vol_Master;
                SoundManager.Instance.PlayAtPosition(reflectSound, player.transform, player.transform, new SoundParameterBase[]
                {
                    soundParameterIntensity
                });

                player.GetComponent<PlayerCollision>().IgnoreWallForFrames(2);
                player.transform.SetPositionAndRotation(new Vector3(-player.transform.position.x, player.transform.position.y, player.transform.position.z), player.transform.rotation);
                if (player.data.view.IsMine) { player.data.block.RPCA_DoBlock(firstBlock: true); }
                cooldownTimer = cooldown;
                player.data.GetAdditionalData().invert = !player.data.GetAdditionalData().invert;
            }
        }

        public void TriggerShatter()
        {
            if (cooldownTimer == 0)
            {
                soundParameterIntensity.intensity = (Optionshandler.vol_Sfx / 1f) * Optionshandler.vol_Master;
                SoundManager.Instance.PlayAtPosition(shatterSound, player.transform, player.transform, new SoundParameterBase[]
                {
                    soundParameterIntensity
                });

                var shatter = Instantiate(RSClasses.ArtAssets.LoadAsset<GameObject>("Shatter"));
                shatter.transform.SetPositionAndRotation(player.transform.position, player.transform.rotation);
                shatter.transform.localScale = new Vector3(radius, radius, 1);
                shatter.transform.rotation = new Quaternion(0f, 0f, rand.Next() % 360, rand.Next() % 360);
                shatter.GetComponent<Canvas>().sortingLayerName = "MostFront";
                RSClasses.instance.ExecuteAfterSeconds(duration, () => DestroyShatter(shatter));
                shatters.Add(shatter);
            }
        }

        public void DestroyShatter(GameObject shatter)
        {
            Destroy(shatter);
            shatters.Remove(shatter);
        }

        private void OnDestroy()
        {
            while (shatters.Count > 0)
            {
                Destroy(shatters[0]);
                shatters.Remove(shatters[0]);
            }
        }

        GameObject reflection;
        System.Random rand = new System.Random(DateTime.Now.Millisecond);
        List<GameObject> shatters = new List<GameObject>();
        SoundEvent reflectSound;
        SoundEvent shatterSound;
        public float radius = 0.0225f;
        public float duration = 1f;
        public float cooldown = 1f;
        private float cooldownTimer = 0f;
        private Player player;
        private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);
    }

    internal class ShatterTrigger : WasDealtDamageTrigger
    {
        public ShatterMono mono;
        public bool shatter = false;
        public override void WasDealtDamage(Vector2 damage, bool selfDamage)
        {
            if (shatter) mono.TriggerShatter();
            mono.TriggerReflect();
        }

        private void OnDestroy()
        {
            Destroy(mono);
        }
    }
}