﻿using UnityEngine;
using HarmonyLib;

namespace RSClasses.Utilities
{
    public class RSPlayerStats : MonoBehaviour // Used to apply custom stats to unity cards
    {
        public int scythes = 0;
        public float scytheDamage = 1;
        public float scytheSpeed = 1;
        public int barriers = 0;
        public float barrierSpeed = 1;
        public float orbitalRadius = 1;
        public int comets = 0;
        public float cometDamage = 1;
        public float cometSpeed = 1;
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
            player.data.GetAdditionalData().cometCount += comets;
            player.data.GetAdditionalData().cometDamage *= cometDamage;
            player.data.GetAdditionalData().scytheSpeed *= scytheSpeed;
            player.data.GetAdditionalData().cometSpeed *= cometSpeed;
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