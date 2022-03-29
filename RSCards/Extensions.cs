using System;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace RSCards.Extensions
{
    [Serializable]
    public class BlockAdditionalData
    {
        public float harmingFieldRange;
        public float timeOfLastSuccessfulBlock;


        public BlockAdditionalData()
        {
            harmingFieldRange = 0f;
            timeOfLastSuccessfulBlock = -100f;
        }
    }

    public static class BlockExtension
    {
        public static readonly ConditionalWeakTable<Block, BlockAdditionalData> data =
            new ConditionalWeakTable<Block, BlockAdditionalData>();

        public static BlockAdditionalData GetAdditionalData(this Block block)
        {
            return data.GetOrCreateValue(block);
        }

        public static void AddData(this Block block, BlockAdditionalData value)
        {
            try
            {
                data.Add(block, value);
            }
            catch (Exception) { }
        }
    }

    [HarmonyPatch(typeof(Block), "ResetStats")]
    class BlockPatchResetStats
    {
        private static void Prefix(Block __instance)
        {

            __instance.GetAdditionalData().harmingFieldRange = 0f;
            __instance.GetAdditionalData().timeOfLastSuccessfulBlock = -100f;


        }
    }
}
