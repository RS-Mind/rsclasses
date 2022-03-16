using System;
using System.Collections.Generic;
using System.Linq;
using ModdingUtils.MonoBehaviours;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace RSCards.Cards
{
    class LastStand : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            //HealthBasedEffect effect = player.gameObject.AddComponent<HealthBasedEffect>();
            //effect.characterStatModifiersModifier.health_mult = 10f;
            //effect.SetPercThresholdMax(0.1f);
            //effect.SetColor(Color.green);
            HealthBasedEffect effect = player.gameObject.AddComponent<HealthBasedEffect>();
            effect.gunStatModifier.attackSpeedMultiplier_mult = 0.75f;
            effect.gunAmmoStatModifier.reloadTimeMultiplier_mult = 0.5f;
            effect.gunStatModifier.projectileSpeed_mult = 1.5f;
            effect.SetPercThresholdMax(0.5f);
            effect.SetColor(Color.red);
            UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }

        protected override string GetTitle()
        {
            return "Last Stand";
        }
        protected override string GetDescription()
        {
            return "+1000% HP when below 10% HP";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
        }
        public override string GetModName()
        {
            return RSCards.ModInitials;
        }
    }
}
