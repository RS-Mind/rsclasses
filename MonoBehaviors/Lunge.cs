using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponsManager;

namespace RSClasses
{
    public class Lunge : MonoBehaviour
    {
        private Gun sword;
        private Player player;
        private float lastLungeTime = 0f;
        private static float lungeCooldown = 0.5f;
        void Start()
        {
            player = GetComponentInParent<Player>();
            WeaponManager weaponManager = this.GetComponentInParent<WeaponManager>();
            sword = weaponManager.GetWeapon("RSC_Sword");
            sword.ShootPojectileAction += OnShootProjectileAction;
        }

        private void OnDestroy()
        {
            sword.ShootPojectileAction -= OnShootProjectileAction;
        }

        private void OnShootProjectileAction(GameObject bullet)
        {
            if (Time.time - lastLungeTime < lungeCooldown)
            {
                return; // Exit if the cooldown hasn't passed
            }
            lastLungeTime = Time.time;
            Vector2 direction = player.data.aimDirection;
            player.data.healthHandler.TakeForce(direction * 7500f, ForceMode2D.Impulse, true, true);
        }
    }
}