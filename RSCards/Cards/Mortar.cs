using RSCards.MonoBehaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.Cards;
using UnityEngine;

namespace RSCards.Cards
{
    class Mortar : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            gun.reloadTimeAdd = 0.5f;
            gun.gravity = 0f;
            cardInfo.allowMultiple = false;
            if (RSCards.Debug) { UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been setup."); }
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            List <ObjectsToSpawn> list = gun.objectsToSpawn.ToList();
            list.Add(new ObjectsToSpawn { AddToProjectile = new GameObject("Mortar_Mono", new Type[] { typeof(Mortar_Mono) }) });
            gun.objectsToSpawn = list.ToArray();
            if (RSCards.Debug) { UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            if (RSCards.Debug) { UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        protected override string GetTitle()
        {
            return "Mortar";
        }
        protected override string GetDescription()
        {
            return "Shooting redirects your bullets towards the cursor";
        }
        protected override GameObject GetCardArt()
        {
            return RSCards.ArtAssets.LoadAsset<GameObject>("C_Mortar");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullet gravity",
                    amount = "No",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload time",
                    amount = "+0.5s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.TechWhite;
        }
        public override string GetModName()
        {
            return RSCards.ModInitials;
        }
    }
}
