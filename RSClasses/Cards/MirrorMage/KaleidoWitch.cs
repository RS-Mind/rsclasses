using ClassesManagerReborn.Util;
using RarityLib.Utils;
using RSClasses.Extensions;
using RSClasses.MonoBehaviours;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace RSClasses.Cards.MirrorMage
{
    class KaleidoWitch : CustomCard
    {
        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = MirrorMageClass.name;
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            gun.damage = 0.75f;
            gun.gravity = 0f;

            cardInfo.allowMultiple = false;
            gameObject.GetOrAddComponent<ClassNameMono>().className = MirrorMageClass.name;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been setup."); }
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            player.data.GetAdditionalData().kaleido = true;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            player.data.GetAdditionalData().kaleido = false;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        internal static CardInfo Card = null;
        protected override string GetTitle()
        {
            return "Kaleido Witch";
        }
        protected override string GetDescription()
        {
            return "Witness the kaleidoscope";
        }
        protected override GameObject GetCardArt()
        {
            return RSClasses.ArtAssets.LoadAsset<GameObject>("C_KaleidoWitch");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return RarityUtils.GetRarity("Exotic");
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullet gravity",
                    amount = "-100%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Damage",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.MagicPink;
        }
        public override string GetModName()
        {
            return RSClasses.ModInitials;
        }
    }
}
