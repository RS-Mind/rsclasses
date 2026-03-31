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
            ammo.SetFieldValue("currentAmmo", 1);
        }
    }
}