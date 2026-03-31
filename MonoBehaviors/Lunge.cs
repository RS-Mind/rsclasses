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
            Vector2 direction = player.data.aimDirection;
            player.data.healthHandler.TakeForce(direction * 15000f * Mathf.Min(sword.attackSpeed * sword.attackSpeedMultiplier, 2f), ForceMode2D.Impulse, true, true);
        }
    }
}