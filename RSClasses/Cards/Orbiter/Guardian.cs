using ClassesManagerReborn.Util;
using RSClasses.MonoBehaviors;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace RSClasses.Cards.Astronomer
{
    class Guardian : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            statModifiers.health = 1.5f;

            cardInfo.allowMultiple = false;
            gameObject.GetOrAddComponent<ClassNameMono>().className = AstronomerClass.name;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been setup."); }
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            var scythe = player.gameObject.GetOrAddComponent<ScytheMono>();
            var shield = player.gameObject.GetOrAddComponent<ShieldMono>();
            scythe.count -= 4;
            shield.count += 4;
            shield.radius += 2f;
            shield.setColor(new Color(0.4f, 1f, 1f));
            RSClasses.instance.ExecuteAfterSeconds(0.5f, () => shield.UpdateStats());
            RSClasses.instance.ExecuteAfterSeconds(0.5f, () => scythe.UpdateStats());
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            var scythe = player.gameObject.GetOrAddComponent<ScytheMono>();
            var shield = player.gameObject.GetOrAddComponent<ShieldMono>();
            scythe.count += 4;
            shield.count -= 4;
            shield.radius -= 2f;
            shield.setColor(new Color(1f, 1f, 0.7411765f));
            RSClasses.instance.ExecuteAfterSeconds(0.5f, () => scythe.UpdateStats());
            RSClasses.instance.ExecuteAfterSeconds(0.5f, () => shield.UpdateStats());
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        internal static CardInfo Card = null;
        protected override string GetTitle()
        {
            return "Guardian";
        }
        protected override string GetDescription()
        {
            return "Stalwart Defenders. Their shields defend them from all bullets";
        }
        protected override GameObject GetCardArt()
        {
            return RSClasses.ArtAssets.LoadAsset<GameObject>("C_Guardian");
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
                    stat = "Shields",
                    amount = "+4",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Health",
                    amount = "+50%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Shield size",
                    amount = "+4",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Scythes",
                    amount = "-4",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.ColdBlue;
        }
        public override string GetModName()
        {
            return RSClasses.ModInitials;
        }
    }
}
