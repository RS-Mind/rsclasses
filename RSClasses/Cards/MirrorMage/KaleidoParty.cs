using ClassesManagerReborn.Util;
using RSClasses.MonoBehaviours;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnityEngine;

namespace RSClasses.Cards.MirrorMage
{
    class KaleidoParty : CustomCard
    {

        public override void Callback()
        {
            ClassNameMono className = gameObject.GetOrAddComponent<ClassNameMono>();
            className.className = MirrorMageClass.name;
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            gun.reloadTimeAdd = 0.25f;
            gun.damage = 0.75f;
            gun.reflects = 1;
            cardInfo.allowMultiple = false;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been setup."); }
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            var mirror = player.gameObject.GetOrAddComponent<KaleidoPartyMono>();
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            var mirror = player.gameObject.GetOrAddComponent<KaleidoPartyMono>();
            Destroy(mirror);
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        internal static CardInfo Card = null;
        protected override string GetTitle()
        {
            return "Kaleido Party";
        }
        protected override string GetDescription()
        {
            return "Your bullets get an extra bounce for each remaining ammo you have";
        }
        protected override GameObject GetCardArt()
        {
            return RSClasses.ArtAssets.LoadAsset<GameObject>("C_KaleidoParty");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullet bounce",
                    amount = "+1",
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
