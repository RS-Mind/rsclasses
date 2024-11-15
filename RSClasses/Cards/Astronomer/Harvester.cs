﻿using ClassesManagerReborn.Util;
using RarityLib.Utils;
using RSClasses.Extensions;
using RSClasses.MonoBehaviours;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace RSClasses.Cards.Astronomer
{
    class Harvester : CustomCard
    {

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = AstronomerClass.nameHarvester;
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            statModifiers.health = 0.75f;

            cardInfo.allowMultiple = false;
            gameObject.GetOrAddComponent<ClassNameMono>().className = AstronomerClass.name;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been setup."); }
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            var scythe = player.gameObject.GetOrAddComponent<ScytheMono>();
            player.data.GetAdditionalData().scytheCount += 2;
            scythe.speed += 150;
            player.data.GetAdditionalData().barrierCount -= 4;
            scythe.setColor(new Color(178f / 255f, 0f, 1f));
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            var scythe = player.gameObject.GetOrAddComponent<ScytheMono>();
            var shield = player.gameObject.GetOrAddComponent<ShieldMono>();
            player.data.GetAdditionalData().scytheCount -= 2;
            scythe.speed -= 150;
            player.data.GetAdditionalData().barrierCount += 4;
            scythe.setColor(new Color(0.6156863f, 0.227451f, 1f));
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        internal static CardInfo Card = null;
        protected override string GetTitle()
        {
            return "Harvester";
        }
        protected override string GetDescription()
        {
            return "Reapers of souls\nDeal more damage the more lifesteal you have";
        }
        protected override GameObject GetCardArt()
        {
            return RSClasses.ArtAssets.LoadAsset<GameObject>("C_Harvester");
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
                    stat = "Scythes",
                    amount = "+2",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Rotation speed",
                    amount = "+60%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Barriers",
                    amount = "-4",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return RSClasses.ModInitials;
        }
    }
}
