﻿using HarmonyLib;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RSClasses.Extensions // This is Pykess's. I take 0 credit for this code
{
    // ADD FIELDS TO GUN
    [Serializable]
    public class GunAdditionalData
    {
        public bool allowStop;

        public GunAdditionalData()
        {
            allowStop = false;
        }
    }
    public static class GunExtension
    {
        public static readonly ConditionalWeakTable<Gun, GunAdditionalData> data =
            new ConditionalWeakTable<Gun, GunAdditionalData>();

        public static GunAdditionalData GetAdditionalData(this Gun gun)
        {
            return data.GetOrCreateValue(gun);
        }

        public static void AddData(this Gun gun, GunAdditionalData value)
        {
            try
            {
                data.Add(gun, value);
            }
            catch (Exception) { }
        }
    }
    // apply additional projectile stats
    [HarmonyPatch(typeof(Gun), "ApplyProjectileStats")]
    class GunPatchApplyProjectileStats
    {
        private static void Prefix(Gun __instance, GameObject obj, int numOfProj = 1, float damageM = 1f, float randomSeed = 0f)
        {
            MoveTransform component3 = obj.GetComponent<MoveTransform>();
            component3.allowStop = __instance.GetAdditionalData().allowStop;
        }
    }
    // reset extra gun attributes when resetstats is called
    [HarmonyPatch(typeof(Gun), "ResetStats")]
    class GunPatchResetStats
    {
        private static void Prefix(Gun __instance)
        {
            __instance.GetAdditionalData().allowStop = false;
        }
    }
}