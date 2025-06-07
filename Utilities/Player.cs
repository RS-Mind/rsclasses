using System;
using System.Runtime.CompilerServices;

namespace RSClasses.Utilities // Adds additional variables to the player's stats
{
    [Serializable]
    public class PlayerAdditionalData
    {
        public bool invert;
        public int posMult;
        public int scytheCount;
        public int barrierCount;
        public int cometCount;
        public float scytheDamage;
        public float orbitalRadius;
        public float barrierSpeed;
        public float scytheSpeed;
        public float cometDamage;
        public float cometSpeed;
        public float fractureDuration;
        public float fractureSize;
        public float reflectionCooldown;

        public PlayerAdditionalData()
        {
            invert = false;
            posMult = 1;
            scytheCount = 0;
            barrierCount = 0;
            scytheDamage = 25f;
            orbitalRadius = 1f;
            scytheSpeed = 250f;
            cometSpeed = 2f;
            cometDamage = 180f;
            cometCount = 0;
            barrierSpeed = 100f;
            fractureDuration = 1f;
            fractureSize = 0.0225f;
            reflectionCooldown = 3f;
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