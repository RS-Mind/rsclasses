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
    internal class ShatterEffect : MonoBehaviour // Reflection Replacement, Shatter, and related effects
    {
        GameObject reflection;
        System.Random rand = new System.Random(DateTime.Now.Millisecond);
        List<GameObject> shatters = new List<GameObject>();
        SoundEvent reflectSound;
        SoundEvent shatterSound;
        private float cooldownTimer = 0f;
        private Player player;
        private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);

        private void Awake()
        {
            player = gameObject.GetComponentInParent<Player>(); // get player
        }

        private void Start()
        {
            AudioClip shatterAudioClip = RSClasses.assets.LoadAsset<AudioClip>("shatter.ogg"); // Load sound effects
            SoundContainer shatterSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            shatterSoundContainer.setting.volumeIntensityEnable = true;
            shatterSoundContainer.audioClip[0] = shatterAudioClip;
            shatterSound = ScriptableObject.CreateInstance<SoundEvent>();
            shatterSound.soundContainerArray[0] = shatterSoundContainer;

            AudioClip reflectAudioClip = RSClasses.assets.LoadAsset<AudioClip>("reflect.ogg");
            SoundContainer reflectSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            reflectSoundContainer.setting.volumeIntensityEnable = true;
            reflectSoundContainer.audioClip[0] = reflectAudioClip;
            reflectSound = ScriptableObject.CreateInstance<SoundEvent>();
            reflectSound.soundContainerArray[0] = reflectSoundContainer;

            if (player.data.view.IsMine) // If your view, create your reflection
            {
                reflection = GameObject.Instantiate(RSClasses.assets.LoadAsset<GameObject>("Reflection"), player.transform);
                reflection.SetActive(true);
                reflection.GetComponent<SpriteRenderer>().color = player.GetTeamColors().color;
            }
        }

        public void Update()
        {
            if (player.data.view.IsMine) // Only run on card owner's client
            { // Update reflection's position
                reflection.transform.SetPositionAndRotation(new Vector3(-player.transform.position.x, player.transform.position.y, player.transform.position.z), player.transform.rotation);
            }
        }

        public void FixedUpdate()
        {
            cooldownTimer = cooldownTimer - TimeHandler.deltaTime < 0f ? 0f : cooldownTimer - TimeHandler.deltaTime; // Decrement cooldown. If it would be lower than 0, make it 0
            if (player.data.view.IsMine) // Only run on card owner's client
            {
                if (shatters.Count <= 0) // If there are no active shatters, return
                {
                    return;
                }
                foreach (GameObject shatter in shatters)
                {
                    var hits = Physics2D.OverlapCircleAll(shatter.transform.position, 166.66f * player.data.GetAdditionalData().fractureSize); // Get all collided targets
                    foreach (var hit in hits)
                    {
                        var healthHandler = hit.gameObject.GetComponent<HealthHandler>(); // If it's a player, damage them (DoT effect basically)
                        if (healthHandler)
                        {
                            Player hitPlayer = (Player)healthHandler.GetFieldValue("player");
                            if (hitPlayer.playerID != player.playerID) healthHandler.CallTakeDamage(((Vector2)hitPlayer.transform.position - (Vector2)shatter.transform.position).normalized * Time.deltaTime * player.data.weaponHandler.gun.damage,
                           (Vector2)this.transform.position, gameObject, player);
                        }
                    }
                }
            }
        }

        public void TriggerReflect() // Teleport the player across the screen
        {
            if (cooldownTimer == 0) // Only if cooldown is up
            {
                soundParameterIntensity.intensity = (Optionshandler.vol_Sfx / 1.15f) * Optionshandler.vol_Master; // Play sfx
                SoundManager.Instance.PlayAtPosition(reflectSound, player.transform, player.transform, new SoundParameterBase[]
                {
                    soundParameterIntensity
                });

                player.GetComponent<PlayerCollision>().IgnoreWallForFrames(2); // Disable collision during teleport
                player.transform.SetPositionAndRotation(new Vector3(-player.transform.position.x, player.transform.position.y, player.transform.position.z), player.transform.rotation); // Set new coords
                player.data.playerVel.SetFieldValue("velocity", Vector2.Scale(new Vector2(-1, 1), (Vector2)player.data.playerVel.GetFieldValue("velocity"))); // Reflect velocity
                if (player.data.view.IsMine) { player.data.block.RPCA_DoBlock(firstBlock: true); } // Trigger a block when teleporting
                cooldownTimer = player.data.GetAdditionalData().reflectionCooldown; // Apply cooldown
                player.data.GetAdditionalData().invert = !player.data.GetAdditionalData().invert; // Update Mirror Mind's inversion flag to prevent strange behavior
            }
        }

        public void TriggerFracture() // Create a Fracture
        {
            if (cooldownTimer == 0) // Only if cooldown is up
            {
                soundParameterIntensity.intensity = (Optionshandler.vol_Sfx / 1f) * Optionshandler.vol_Master; // Play sfx
                SoundManager.Instance.PlayAtPosition(shatterSound, player.transform, player.transform, new SoundParameterBase[]
                {
                    soundParameterIntensity
                });

                var shatter = Instantiate(RSClasses.assets.LoadAsset<GameObject>("Shatter")); // Load a shatter
                shatter.transform.SetPositionAndRotation(player.transform.position, player.transform.rotation); // Set the shatter to your position
                shatter.transform.localScale = new Vector3(player.data.GetAdditionalData().fractureSize, player.data.GetAdditionalData().fractureSize, 1); // Set scale
                shatter.transform.rotation = new Quaternion(0f, 0f, rand.Next() % 360, rand.Next() % 360); // Randomize rotation
                shatter.GetComponent<Canvas>().sortingLayerName = "MostFront"; // Make it render in front of everything
                RSClasses.instance.ExecuteAfterSeconds(player.data.GetAdditionalData().fractureDuration, () => DestroyShatter(shatter)); // Set to delete after duration is up
                shatters.Add(shatter); // Add to a list for tracking
            }
        }

        public void DestroyShatter(GameObject shatter)
        {
            Destroy(shatter); // Destroy the object
            shatters.Remove(shatter); // Remove it from the list
        }

        private void OnDestroy() // Delete all existing Fractures
        {
            while (shatters.Count > 0)
            {
                Destroy(shatters[0]);
                shatters.Remove(shatters[0]);
            }
        }
    }

    internal class Shatter_Mono : WasDealtDamageTrigger // Added to the player. Triggers the shatter effect
    {
        public ShatterEffect mono;
        private Player player;

        private void Start()
        {
            player = gameObject.GetComponentInParent<Player>(); // Get the player
            mono = player.gameObject.GetOrAddComponent<ShatterEffect>(); // Give them a shatter effect
        }
        public override void WasDealtDamage(Vector2 damage, bool selfDamage)
        {
            if (player.data.currentCards.Contains(CardHolder.cards["Fracture"])) mono.TriggerFracture(); // If they have Fracture, make one
            mono.TriggerReflect(); // Teleport the player
        }

        private void OnDestroy()
        {
            Destroy(mono); // Delete the shatter effect
        }
    }
}