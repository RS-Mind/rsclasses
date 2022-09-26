using HarmonyLib;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RSClasses.Extensions // This is Pykess's. I take 0 credit for this code
{
    // ADD FIELDS TO GUN
    [Serializable]
    public class PlayerAdditionalData
    {
        public bool invert;
        public bool prism;
        public int posMult;
        public int scytheCount;
        public int barrierCount;

        public PlayerAdditionalData()
        {
            invert = false;
            posMult = 1;
            prism = false;
            scytheCount = 0;
            barrierCount = 0;
        }
    }
    public static class PlayerExtension
    {
        public static readonly ConditionalWeakTable<CharacterData, PlayerAdditionalData> data =
            new ConditionalWeakTable<CharacterData, PlayerAdditionalData>();

        public static PlayerAdditionalData GetAdditionalData(this CharacterData chara)
        {
            return data.GetOrCreateValue(chara);
        }

        public static void AddData(this CharacterData chara, PlayerAdditionalData value)
        {
            try
            {
                data.Add(chara, value);
            }
            catch (Exception) { }
        }
    }
}