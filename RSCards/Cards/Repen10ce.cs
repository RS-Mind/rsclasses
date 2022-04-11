using CardChoiceSpawnUniqueCardPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace RSCards.Cards
{
    class Repen10ce : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            statModifiers.health = 1 / 50f;
            gun.damage = 10f;
            if (RSCards.Debug) { UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been setup."); }
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            if (RSCards.Debug) { UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            if (RSCards.Debug) { UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        protected override string GetTitle()
        {
            return "Repen10ce";
        }
        protected override string GetDescription()
        {
            return "Ever notice the misspelling?";
        }
        protected override GameObject GetCardArt()
        {
            return RSCards.ArtAssets.LoadAsset<GameObject>("C_Repen10ce");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Damage",
                    amount = "+900%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "HP",
                    amount = "-5000%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.NatureBrown;
        }
        public override string GetModName()
        {
            return RSCards.ModInitials;
        }
        public override bool GetEnabled()
        {
            return false;
        }
    }
}
