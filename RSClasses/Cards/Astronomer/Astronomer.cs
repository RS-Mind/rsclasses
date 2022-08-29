using ClassesManagerReborn.Util;
using RSClasses.MonoBehaviours;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace RSClasses.Cards.Astronomer
{
    class Astronomer : CustomCard
    {
        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>();
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            gun.damage = 0.75f;

            cardInfo.allowMultiple = false;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been setup."); }
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            var scythe = player.gameObject.GetOrAddComponent<ScytheMono>();
            var shield = player.gameObject.GetOrAddComponent<ShieldMono>();
            scythe.count += 2;
            shield.count += 2;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            var scythe = player.gameObject.GetOrAddComponent<ScytheMono>();
            var shield = player.gameObject.GetOrAddComponent<ShieldMono>();
            scythe.count -= 2;
            shield.count -= 2;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        internal static CardInfo Card = null;
        protected override string GetTitle()
        {
            return "Astronomer";
        }
        protected override string GetDescription()
        {
            return "Uses orbiting objects to fend off foes";
        }
        protected override GameObject GetCardArt()
        {
            return RSClasses.ArtAssets.LoadAsset<GameObject>("C_Astronomer");
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
                    stat = "Scythes",
                    amount = "+2",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Barriers",
                    amount = "+2",
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
            return CardThemeColor.CardThemeColorType.FirepowerYellow;
        }
        public override string GetModName()
        {
            return RSClasses.ModInitials;
        }
    }
}
