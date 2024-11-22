using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSClasses.Extensions;
using HarmonyLib;

namespace RSClasses
{
    public class RSPlayerStats : MonoBehaviour
    {
        public int scythes = 0;
        public float scytheDamage = 1;
        public float scytheSpeed = 1;
        public int barriers = 0;
        public float barrierSpeed = 1;
        public float orbitalRadius = 1;
        public float fractureDuration = 0;
        public float fractureSize = 1;
        public float reflectionCooldown = 1;

        public void Apply(Player player)
        {
            player.data.GetAdditionalData().scytheCount += scythes;
            player.data.GetAdditionalData().barrierCount += barriers;
            player.data.GetAdditionalData().scytheDamage *= scytheDamage;
            player.data.GetAdditionalData().orbitalRadius *= orbitalRadius;
            player.data.GetAdditionalData().barrierSpeed *= barrierSpeed;
            player.data.GetAdditionalData().scytheSpeed *= scytheSpeed;
            player.data.GetAdditionalData().fractureDuration += fractureDuration;
            player.data.GetAdditionalData().fractureSize *= fractureSize;
            player.data.GetAdditionalData().reflectionCooldown *= reflectionCooldown;
        }
    }

    [HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
    public class ApplyPlayerStatsPatch
    {
        static void Postfix(ApplyCardStats __instance, Player ___playerToUpgrade)
        {
            if (__instance.GetComponent<RSPlayerStats>() is RSPlayerStats stats)
            {
                stats.Apply(___playerToUpgrade);
            }
        }
    }
}