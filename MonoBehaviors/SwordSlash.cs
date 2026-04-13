using ClassesManagerReborn;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.ParticleSystem;

namespace RSClasses
{
    public class SwordSlash : MonoBehaviour
    {
        static GameObject particles;

        private ProjectileHit bullet;
        private List<HitInfo> hits = new List<HitInfo>();
        private float range;
        private bool hitDone = false;

        void FixedUpdate()
        {
            if (hitDone) return;
            Vector3 shootPosition = bullet.ownWeapon.transform.position;
            Vector3 shootDirection = (bullet.ownWeapon.GetComponent<Gun>().shootPosition.position - bullet.ownWeapon.transform.position).normalized;
            if (bullet.view.IsMine)
            {
                List<RaycastHit2D> potentialHits = Physics2D.RaycastAll(bullet.ownWeapon.transform.position, -shootDirection, 1f, PlayerManager.instance.canSeePlayerMask).ToList();
                potentialHits = potentialHits.Concat(Physics2D.RaycastAll(bullet.ownWeapon.transform.position, shootDirection, range + 0.5f, PlayerManager.instance.canSeePlayerMask)).ToList();
                
                foreach (RaycastHit2D potentialHit in potentialHits)
                {
                    if (potentialHit.collider != null)
                    {
                        HitInfo hitInfo = GetHitInfo(potentialHit);
                        HealthHandler healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
                        if (healthHandler != null && healthHandler.GetComponent<Player>() == bullet.ownPlayer)
                            continue; // Don't kill yourself
                        foreach (HitInfo hit in hits)
                        {
                            if (hitInfo.transform == hit.transform)
                                continue; // element already in list
                        }
                        bullet.Hit(hitInfo);

                        if (!bullet.ownPlayer.data.currentCards.Contains(CardHolder.cards["Spectral Saber"]))
                        {
                            hitDone = true;
                            return;
                        }

                        hits.Append(hitInfo);
                    }
                }

            }
        }

        void Start()
        {
            if (particles == null)
            {
                particles = RSClasses.assets.LoadAsset<GameObject>("Sword_Slash_Particles");
            }
            bullet = this.GetComponent<ProjectileHit>();
            range = SwordLength_Mono.CalculateLength(bullet.ownWeapon.GetComponent<Gun>().projectileSpeed);
            Vector3 preShootPosition = bullet.ownWeapon.transform.position;
            Vector3 preShootDirection = (bullet.ownWeapon.GetComponent<Gun>().shootPosition.position - bullet.ownWeapon.transform.position).normalized;
            GameObject particle = Instantiate(particles, preShootPosition + (preShootDirection * (range + 0.5f)), bullet.ownWeapon.transform.rotation);
            particle.transform.localScale = new Vector3(range, range, range);
            GetComponentInChildren<CircleCollider2D>().enabled = false;
            RSClasses.instance.ExecuteAfterSeconds(0.1f, () =>
            {
                bullet.InvokeMethod("DestroyMe");
            });

            RayHitEffect effect = new NoDestroyRayHitEffect();
            bullet.effects = bullet.effects.Append(effect).ToList();
            bullet.destroyOnBlock = false;
        }

        private class NoDestroyRayHitEffect : RayHitEffect
        {
            public override HasToReturn DoHitEffect(HitInfo hit)
            {
                return HasToReturn.hasToReturn;
            }
        }

        internal static HitInfo GetHitInfo(RaycastHit2D raycastHit2D)
        {
            return new HitInfo
            {
                point = raycastHit2D.point,
                normal = raycastHit2D.normal,
                transform = raycastHit2D.transform,
                collider = raycastHit2D.collider,
                rigidbody = raycastHit2D.rigidbody
            };
        }
    }
}