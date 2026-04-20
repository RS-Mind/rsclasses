using HarmonyLib;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using WeaponsManager;

namespace RSClasses
{
    public class AbsoluteKill : MonoBehaviour
    {
        internal Gun sword;
        private ObjectsToSpawn effect;
        private void Start()
        {
            WeaponManager weaponManager = this.GetComponentInParent<WeaponManager>();
            sword = weaponManager.GetWeapon("RSC_Sword");
            effect = new ObjectsToSpawn();
            effect.AddToProjectile = new GameObject("AbsoluteKillEffect", typeof(AbsoluteKillHitEffect));
            sword.objectsToSpawn = sword.objectsToSpawn.AddItem(effect).ToArray();
        }

        private void OnDestroy()
        {
            sword.objectsToSpawn = sword.objectsToSpawn.Where(x => x.effect.GetComponent<AbsoluteKillHitEffect>() == null).ToArray();
        }

        public class AbsoluteKillHitEffect : RayHitEffect
        {
            public bool active = true;
            public override HasToReturn DoHitEffect(HitInfo hit)
            {
                if (!hit.transform || !active)
                {
                    return HasToReturn.canContinue;
                }
                if (hit.transform.GetComponent<Player>() is Player damagedPlayer && damagedPlayer != null)
                {
                    damagedPlayer.data.stats.remainingRespawns = 0;
                    if (damagedPlayer.data.view.IsMine)
                        damagedPlayer.data.view.RPC("RPCA_Die", RpcTarget.All, new object[] { hit.normal });
                }
                return HasToReturn.canContinue;
            }
        }
    }
}