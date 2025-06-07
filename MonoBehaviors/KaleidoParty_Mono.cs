using UnityEngine;
using Photon.Pun;
using SimulationChamber;
using System.Linq;
using UnboundLib;
using UnboundLib.Utils;
using UnboundLib.Extensions;
using UnboundLib.Cards;
using System.Reflection;

namespace RSClasses.MonoBehaviours
{
    public class KaleidoParty_Mono : MonoBehaviour // Kaleido Party's variable bonus bounces
    {
        Player player;
        Gun gun;

        public void Start()
        {
            player = GetComponentInParent<Player>(); // Get player and gun
            gun = player.data.weaponHandler.gun;
            gun.ShootPojectileAction += OnShootProjectileAction; // Add on shoot action to the gun
        }

        public void OnShootProjectileAction(GameObject obj)
        {
            if (obj)
            {
                var bounces = obj.GetOrAddComponent<RayHitReflect>(); // Get the bullet's bounces
                try
                {
                    GunAmmo gunAmmo = (GunAmmo) gun.GetFieldValue("gunAmmo"); // Get player's current ammo
                    int currentAmmo = (int) gunAmmo.GetFieldValue("currentAmmo");
                    bounces.reflects += currentAmmo - 1; // Add a bounce for each remaining ammo (-1 for the bullet we're currently shooting)
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