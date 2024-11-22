using UnityEngine;
using Photon.Pun;
using SimulationChamber;
using System.Linq;
using RSClasses.Extensions;
using UnboundLib;
using UnboundLib.Utils;
using UnboundLib.Extensions;
using UnboundLib.Cards;
using System.Reflection;

namespace RSClasses.MonoBehaviours
{
    public class KaleidoParty_Mono : MonoBehaviour
    {
        Player player;
        Gun gun;

        public void Start()
        {
            // Get Player
            player = GetComponentInParent<Player>();
            // Get Gun
            gun = player.data.weaponHandler.gun;
            // Add action
            gun.ShootPojectileAction += OnShootProjectileAction;
        }

        public void OnShootProjectileAction(GameObject obj)
        {
            if (obj)
            {
                var bounces = obj.GetOrAddComponent<RayHitReflect>();
                try
                {
                    GunAmmo gunAmmo = (GunAmmo) gun.GetFieldValue("gunAmmo");
                    int currentAmmo = (int) gunAmmo.GetFieldValue("currentAmmo");
                    bounces.reflects += currentAmmo - 1;
                }
                catch { }
            }
        }

        public void OnDestroy()
        {
            // Remove action when the mono is removed
            gun.ShootPojectileAction -= OnShootProjectileAction;
        }
    }
}