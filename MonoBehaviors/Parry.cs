using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponsManager;

namespace RSClasses
{
    public class Parry : MonoBehaviour
    {
        private Gun sword;
        void Start()
        {
            WeaponManager weaponManager = this.GetComponentInParent<WeaponManager>();
            sword = weaponManager.GetWeapon("RSC_Sword");

            // Find and activate the colliders
            Transform spring = sword.transform.GetChild(1);
            spring.GetChild(4).GetChild(1).gameObject.SetActive(true);
            Transform blade = spring.GetChild(5);
            blade.GetChild(3).gameObject.SetActive(true);
            blade.GetChild(1).GetChild(1).gameObject.SetActive(true);
            blade.GetChild(2).GetChild(1).gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            // Find and deactivate the colliders
            Transform spring = sword.transform.GetChild(1);
            spring.GetChild(4).GetChild(1).gameObject.SetActive(false);
            Transform blade = spring.GetChild(5);
            blade.GetChild(3).gameObject.SetActive(false);
            blade.GetChild(1).GetChild(1).gameObject.SetActive(false);
            blade.GetChild(2).GetChild(1).gameObject.SetActive(false);
        }
    }
}