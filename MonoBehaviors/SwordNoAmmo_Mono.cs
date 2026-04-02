using System.Collections;
using System.Collections.Generic;
using UnboundLib;
using UnityEngine;

namespace RSClasses
{
    public class SwordNoAmmo_Mono : MonoBehaviour
    {
        public GunAmmo ammo;

        void Update()
        {
            ammo.SetFieldValue("currentAmmo", 2);
            ammo.maxAmmo = 2;
            ammo.reloadTime = 0.01f;
            ammo.reloadTimeAdd = 0;
        }
    }
}