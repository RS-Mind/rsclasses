using ClassesManagerReborn;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using UnboundLib;
using UnityEngine;

namespace RSClasses
{
    public class SwordSlash : MonoBehaviour
    {
        static GameObject particles;
        void Start()
        {
            if (particles == null)
            {
                particles = RSClasses.assets.LoadAsset<GameObject>("Sword_Slash_Particles");
            }
            ProjectileHit bullet = this.GetComponent<ProjectileHit>();
            float range = SwordLength_Mono.CalculateLength(bullet.ownWeapon.GetComponent<Gun>().projectileSpeed);
            Vector3 preShootPosition = bullet.ownWeapon.transform.position;
            Vector3 preShootDirection = (bullet.ownWeapon.GetComponent<Gun>().shootPosition.position - bullet.ownWeapon.transform.position).normalized;
            GameObject particle = Instantiate(particles, preShootPosition + (preShootDirection * (range + 0.5f)), bullet.ownWeapon.transform.rotation);
            RSClasses.instance.ExecuteAfterSeconds(0.1f, () =>
            {
                this.GetComponentInChildren<CircleCollider2D>().enabled = false;
                Vector3 shootPosition = bullet.ownWeapon.transform.position;
                Vector3 shootDirection = (bullet.ownWeapon.GetComponent<Gun>().shootPosition.position - bullet.ownWeapon.transform.position).normalized;
                particle.transform.localScale = new Vector3(range, range, range);
                if (bullet.view.IsMine)
                {
                    RaycastHit2D[] hits;
                    if (bullet.ownPlayer.data.currentCards.Contains(CardHolder.cards["Spectral Saber"])) // If the player has Spectral Saber, hit all valid targets, not just the first
                    {
                        hits = Physics2D.RaycastAll(bullet.ownWeapon.transform.position, shootDirection, range + 0.5f, PlayerManager.instance.canSeePlayerMask);
                        //Second backwards hit so you can hit players right next to you
                        hits = hits.Concat(Physics2D.RaycastAll(bullet.ownWeapon.transform.position, -shootDirection, 1f, PlayerManager.instance.canSeePlayerMask)).ToArray();
                    }
                    else
                    {
                        hits = new RaycastHit2D[2];
                        hits[0] = (Physics2D.Raycast(bullet.ownWeapon.transform.position, shootDirection, range + 0.5f, PlayerManager.instance.canSeePlayerMask));
                        //Second backwards hit so you can hit players right next to you
                        hits[1] = (Physics2D.Raycast(bullet.ownWeapon.transform.position, -shootDirection, 1f, PlayerManager.instance.canSeePlayerMask));
                    }
                    
                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.collider != null)
                        {
                            HitInfo hitInfo = GetHitInfo(hit);
                            HealthHandler healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
                            if (healthHandler != null && healthHandler.GetComponent<Player>() == bullet.ownPlayer)
                                continue; // Don't kill yourself
                            bullet.Hit(hitInfo);
                        }
                    }

                    this.GetComponent<ProjectileHit>().InvokeMethod("DestroyMe");
                }
            });
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