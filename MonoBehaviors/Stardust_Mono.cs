using RSClasses.Extensions;
using Sonigon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnboundLib;
using UnboundLib.Extensions;
using UnboundLib.GameModes;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace RSClasses.MonoBehaviours
{
    class Stardust_Mono : MonoBehaviour
    {
        public Player player;
        public float rotationDirection = 90;
        private SoundEvent stardustSound;

        private void Start()
        {
            AudioClip stardustAudioClip = RSClasses.assets.LoadAsset<AudioClip>("stardustHit.ogg"); // Load sound effects
            SoundContainer stardustSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            stardustSoundContainer.setting.volumeIntensityEnable = true;
            stardustSoundContainer.audioClip[0] = stardustAudioClip;
            stardustSound = ScriptableObject.CreateInstance<SoundEvent>();
            stardustSound.soundContainerArray[0] = stardustSoundContainer;
        }

        private void Update()
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + (Time.deltaTime * rotationDirection));
        }

        private void FixedUpdate()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);

            if (player.data.view.IsMine)
            {
                foreach (var hit in hits)
                {
                    var damageable = hit.gameObject.GetComponent<Damagable>();
                    var healthHandler = hit.gameObject.GetComponent<HealthHandler>();
                    if (healthHandler)
                    {
                        Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                        if (hitPlayer == player) continue;
                        SoundManager.Instance.PlayAtPosition(stardustSound, this.transform, damageable.transform);
                        if (((Player)healthHandler.GetFieldValue("player")).GetComponent<Block>().blockedThisFrame) { continue; }
                    }
                    if (damageable)
                    {
                        damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)this.transform.position).normalized * (player.data.GetAdditionalData().cometDamage / 10),
                            (Vector2)this.transform.position, this.gameObject, player);
                        Destroy(this.gameObject);
                    }
                }
            }
        }
    }
}