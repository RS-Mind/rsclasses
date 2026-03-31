using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using TabInfo.Extensions;
using UnboundLib;
using UnityEngine;
using UnityEngine.UI;

namespace RSClasses
{
    public class SwordLength_Mono : MonoBehaviour
    {
        private Gun gun;
        private Transform blade;
        void Start()
        {
            gun = GetComponent<Gun>();
            blade = transform.Find("Spring/Blade");
            RSClasses.instance.ExecuteAfterFrames(1, () =>
            {
                foreach (var image in this.GetComponentsInChildren<Image>()) if (image.name == "Color")
                        image.color = gun.player.GetTeamColors().color;
            });
        }

        void Update()
        {
            float length = CalculateLength(gun.projectileSpeed);
            blade.localScale = new Vector3(length, 0.15f, 1f);
            blade.localPosition = new Vector3(0, length / 2f, 0);
            gun.numberOfProjectiles = 1;
            gun.bursts = 1;
        }

        internal static float CalculateLength(float projectileSpeed)
        {
            return Mathf.Clamp(2 * ((float)Math.Log10(projectileSpeed) + 1f), 1f, 10f);
        }
    }
}