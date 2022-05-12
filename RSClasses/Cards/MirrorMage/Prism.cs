using ClassesManagerReborn.Util;
using RSClasses.Extensions;
using RSClasses.MonoBehaviours;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace RSClasses.Cards.MirrorMage
{
    class Prism : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            gun.damage = 0.75f;

            cardInfo.allowMultiple = false;
            gameObject.GetOrAddComponent<ClassNameMono>().className = MirrorMageClass.name;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been setup."); }
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            player.data.GetAdditionalData().prism = true;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            player.data.GetAdditionalData().prism = false;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        internal static CardInfo Card = null;
        protected override string GetTitle()
        {
            return "Prism";
        }
        protected override string GetDescription()
        {
            return "Gain the ability to perceive the prism";
        }
        protected override GameObject GetCardArt()
        {
            return RSClasses.ArtAssets.LoadAsset<GameObject>("C_Prism");
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
                    positive = false,
                    stat = "Damage",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.PoisonGreen;
        }
        public override string GetModName()
        {
            return RSClasses.ModInitials;
        }
    }
}
